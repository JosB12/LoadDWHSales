using LoadDWHSales.Data.Entities.DWHSales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHSales.Data.Context
{
    public class DWHSalesContext : DbContext
    {
        public DWHSalesContext(DbContextOptions<DWHSalesContext> options) : base(options) 
        { 
        
        }
        #region DbSets
        public DbSet<DimEmployee> DimEmployees { get; set; }
        public DbSet<DimProductCategory> DimProductCategories { get; set; }
        public DbSet<DimCustomer> DimCustomers { get; set; }
        public DbSet<DimShipper> DimShippers { get; set; }
        public DbSet<DimDate> DimDates { get; set; }
        public DbSet<FactOrders> FactOrders { get; set; }
        public DbSet<FactServedCustomers> FactServedCustomers { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ajusta la precisión y escala según sea necesario

            modelBuilder.Entity<FactOrders>()
            .Property(f => f.TotalSales)
            .HasColumnType("decimal(18, 2)");
        }
    }
}
