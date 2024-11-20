using LoadDWHSales.Data.Entities.Northwind;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHSales.Data.Context
{
    public class NorwindContext : DbContext
    {
        public NorwindContext(DbContextOptions<NorwindContext> options) : base(options) 
        { 

        }
        #region "Db Sets"
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        #endregion
    }
}
