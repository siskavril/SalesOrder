using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesOrder.Models;

namespace SalesOrder.Data
{
    public class SalesOrderContext : DbContext
    {
        public SalesOrderContext (DbContextOptions<SalesOrderContext> options)
            : base(options)
        {
        }

        public DbSet<OrderData> OrderData { get; set; } = default!;
        public DbSet<Customer> Customer { get; set; } = default!;
        public DbSet<OrderDataDetail> OrderDataDetail { get; set; } = default!;
        public DbSet<Temporary> Temporary { get; set; } = default!;
    }
}
