using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesOrder.Data;
using SalesOrder.Models;
using SalesOrder.ViewModels;
using X.PagedList;

namespace SalesOrder.Controllers
{
    public class CustomerController : Controller
    {
        private readonly SalesOrderContext _context;

        public CustomerController(SalesOrderContext context)
        {
            _context = context;
        }

        // GET: OrderDatas
        public IActionResult Index(int page)
        {
            // Getting initial list
            var listCustomer = from cst in _context.Customer
                               select cst;

            // Paging logic
            int pageSize = 4;
            int pageNumber = page > 0 ? page : 1;

            // Create the list of OrderDataViewModel
            IList<CustomerViewModel> items = listCustomer
                .Select(cst => new CustomerViewModel
                {
                    Id = cst.Id,
                    Name = cst.Name,
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Getting total count for paging
            int totalCount = listCustomer.Count();

            // Creating the paged list
            IPagedList<CustomerViewModel> pagedListData = new StaticPagedList<CustomerViewModel>(items, pageNumber, pageSize, totalCount);

            // Returning the view with paged data
            return View("Index", pagedListData);
        }

        // POST: OrderDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel cust)
        {
            if (ModelState.IsValid)
            {
                Customer customer = new()
                {
                    Name = cust.Name
                };
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
