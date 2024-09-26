using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NuGet.Common;
using SalesOrder.Data;
using SalesOrder.Models;
using SalesOrder.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Net.Http.Headers;
using System.Text;
using X.PagedList;

namespace SalesOrder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SalesOrderContext _context;

        public HomeController(ILogger<HomeController> logger, SalesOrderContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int page, string sortOrder, DateTime orderDate, string keyword)
        {
            ViewData["CurrentKeyword"] = keyword;
            ViewData["CurrentOrderDate"] = orderDate.ToString("yyyy-MM-dd");

            // Getting initial list
            var listOrderData = from od in _context.OrderData
                                join cst in _context.Customer on od.IdCustomer equals cst.Id
                                select new
                                {
                                    od.Id,
                                    od.SalesOrder,
                                    od.OrderDate,
                                    cst.Name
                                };

            // Searching with keyword and order date
            if (!string.IsNullOrEmpty(keyword) || orderDate != DateTime.MinValue)
            {
                // Filter by order date and keyword if both are provided
                if (!string.IsNullOrEmpty(keyword) && orderDate != DateTime.MinValue)
                {
                    listOrderData = listOrderData.Where(i => i.OrderDate.Date == orderDate && (i.SalesOrder.Contains(keyword) || i.Name.Contains(keyword)));
                }
                else
                {
                    if (orderDate != DateTime.MinValue)
                    {
                        listOrderData = listOrderData.Where(i => i.OrderDate.Date == orderDate);
                    }

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        listOrderData = listOrderData.Where(i => i.SalesOrder.Contains(keyword) || i.Name.Contains(keyword));
                    }
                }

            }

            // Paging logic
            int pageSize = 4;
            int pageNumber = page > 0 ? page : 1;

            // Create the list of OrderDataViewModel
            IList<OrderDataViewModel> items = listOrderData
                .Select(od => new OrderDataViewModel
                {
                    Id = od.Id,
                    SalesOrder = od.SalesOrder,
                    OrderDate = od.OrderDate,
                    CustomerName = od.Name
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Getting total count for paging
            int totalCount = listOrderData.Count();

            // Creating the paged list
            IPagedList<OrderDataViewModel> pagedListData = new StaticPagedList<OrderDataViewModel>(items, pageNumber, pageSize, totalCount);

            // Returning the view with paged data
            return View("Index", pagedListData);
        }

        // GET: Create
        public IActionResult Create()
        {
            // Retrieve the list of customers from the database
            var customers = _context.Customer.ToList();

            // Create a list of SelectListItems from the customer data
            var customerList = customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(), // Assuming Id is of a type that can be converted to string
                Text = c.Name // Assuming Name is the display text
            }).ToList();

            // Pass the customer list to the View
            ViewData["ListCustomer"] = new SelectList(customerList, "Value", "Text");

            return View();
        }

        //POST: Create
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDataViewModel orderData)
        {
            if (ModelState.IsValid)
            {
                OrderData order = new()
                {
                    SalesOrder = orderData.SalesOrder,
                    OrderDate = orderData.OrderDate,
                    IdCustomer = orderData.IdCustomer,
                    Address = orderData.Address,
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                if (!orderData.ListOrderDataDetail.IsNullOrEmpty())
                {
                    foreach (var item in orderData.ListOrderDataDetail!)
                    {
                        OrderDataDetail detail = new()
                        {
                            IdOrderData = order.Id,
                            ItemName = item.ItemName,
                            Price = item.Price,
                            Qty = item.Qty,
                            Total = item.Qty * item.Price // Calculate total based on Qty and Price
                        };
                        _context.Add(detail); // Change this to add the detail, not the order
                    }
                    await _context.SaveChangesAsync(); // Save all details after the loop
                }

                TempData[SD.Success] = "Data Saved";
                return RedirectToAction(nameof(Index));
            }
            TempData[SD.Error] = "Data not save, please check again!";
            return View(orderData);
        }

        // GET: Edit/5
        public async Task<IActionResult> Edit(OrderDataViewModel vm)
        {
            if (vm.Id == 0)
            {
                ViewData[SD.Error] = "Data Not Found!";
                return RedirectToAction(nameof(Index));
            }

            var orderData = await _context.OrderData.FindAsync(vm.Id);
            if (orderData == null)
            {
                ViewData[SD.Error] = "Data Not Found!";
                return NotFound();
            }

            var order = (from od in _context.OrderData
                         join cst in _context.Customer on od.IdCustomer equals cst.Id
                         where od.Id == vm.Id
                         select new
                         {
                             od.SalesOrder,
                             od.OrderDate,
                             cst.Id,
                             cst.Name,
                             od.Address
                         }).Single();

            vm.SalesOrder = order.SalesOrder;
            vm.OrderDate = order.OrderDate;
            vm.Address = order.Address;
            vm.IdCustomer = order.Id;
            vm.CustomerName = order.Name;

            // Retrieve the list of customers from the database
            var customers = _context.Customer.ToList();

            // Create a list of SelectListItems from the customer data
            var customerList = customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(), // Assuming Id is of a type that can be converted to string
                Text = c.Name // Assuming Name is the display text
            }).ToList();

            // Pass the customer list to the View
            ViewData["ListCustomer"] = new SelectList(customerList, "Value", "Text", order.Id);

            var orderDetail = _context.OrderDataDetail.Where(i => i.IdOrderData == vm.Id);
            if (orderDetail.Any())
            {
                int totalItem = 0;
                double totalAmount = 0;
                List<OrderDataDetailViewModel> listDetail = new();
                foreach (var item in orderDetail.ToList())
                {
                    OrderDataDetailViewModel detail = new()
                    {
                        Id = item.Id,
                        ItemName = item.ItemName,
                        Qty = item.Qty,
                        Price = item.Price,
                        Total = item.Total
                    };
                    listDetail.Add(detail);

                    totalItem = totalItem + item.Qty;
                    totalAmount = totalAmount + item.Price;
                }
                vm.ListOrderDataDetail = listDetail;

                ViewData["TotalItem"] = totalItem;
                ViewData["TotalAmount"] = totalAmount;
            }

            return View(vm);
        }

        // POST: Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var orderData = await _context.OrderData.FindAsync(id);
            if (orderData != null)
            {
                _context.OrderData.Remove(orderData);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: OrderDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(OrderDataViewModel orderData)
        {
            var order = _context.OrderData.Where(i => i.Id == orderData.Id).Single();
            order.SalesOrder = orderData.SalesOrder;
            order.OrderDate = orderData.OrderDate;
            order.IdCustomer = orderData.IdCustomer;
            order.Address = orderData.Address;
            _context.Update(order);
            await _context.SaveChangesAsync();

            if (orderData.ListOrderDataDetail != null)
            {
                // Update existing details
                foreach (var detail in orderData.ListOrderDataDetail!)
                {
                    if (detail.Id > 0) // Existing items
                    {
                        var existingDetail = await _context.OrderDataDetail.FindAsync(detail.Id);
                        if (existingDetail != null)
                        {
                            existingDetail.ItemName = detail.ItemName;
                            existingDetail.Qty = detail.Qty;
                            existingDetail.Price = detail.Price;
                            existingDetail.Total = detail.Total;
                        }
                    }
                    else // New items
                    {
                        OrderDataDetail newDetail = new()
                        {
                            IdOrderData = order.Id,
                            ItemName = detail.ItemName,
                            Qty = detail.Qty,
                            Price = detail.Price,
                            Total = detail.Qty * detail.Price
                        };
                        _context.OrderDataDetail.Add(newDetail);
                    }
                }
            }

            // Handle deleted items
            if (orderData.DeletedItems != null)
            {
                foreach (var itemId in orderData.DeletedItems)
                {
                    var detailToDelete = await _context.OrderDataDetail.FindAsync(itemId);
                    if (detailToDelete != null)
                    {
                        _context.OrderDataDetail.Remove(detailToDelete);
                    }
                }
            }

            // Save changes
            await _context.SaveChangesAsync();
            TempData[SD.Success] = "Data saved successfully";
            return RedirectToAction(nameof(Edit), new { orderData.Id });
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            // Retrieve data to export
            var listOrderData = from od in _context.OrderData
                                join cst in _context.Customer on od.Id equals cst.Id
                                select new
                                {
                                    od.Id,
                                    od.SalesOrder,
                                    od.OrderDate,
                                    CustomerName = cst.Name // Adjusting property for clarity
                                };

            // Create a new workbook and a worksheet
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sales Orders");

            // Define header
            string[] headers = { "ID", "Sales Order", "Order Date", "Customer Name" };
            IRow headerRow = sheet.CreateRow(0);

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.CreateCell(i).SetCellValue(headers[i]);
            }

            // Loop through data and fill the worksheet
            int rowIndex = 1; // Start from the second row (index 1)
            foreach (var order in listOrderData)
            {
                IRow row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(order.Id);
                row.CreateCell(1).SetCellValue(order.SalesOrder);
                row.CreateCell(2).SetCellValue(order.OrderDate.ToString("yyyy-MM-dd")); // Format date if necessary
                row.CreateCell(3).SetCellValue(order.CustomerName);

                rowIndex++;
            }

            // Autofit columns (optional)
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            // Convert to file
            using (var stream = new MemoryStream())
            {
                workbook.Write(stream);
                var fileName = "SalesOrders.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


        private bool OrderDataExists(int id)
        {
            return _context.OrderData.Any(e => e.Id == id);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
