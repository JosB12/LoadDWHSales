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
        public DbSet<VwServedCustomer> VwServedCustomers { get; set; }

        public DbSet<VwVwventa> VwVwventas { get; set; }
        public DbSet<VwDate> VwDates { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VwServedCustomer>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("VW_ServedCustomers", "DWH");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
                entity.Property(e => e.EmployeeName)
                    .IsRequired()
                    .HasMaxLength(31);
            });

            modelBuilder.Entity<VwVwventa>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("VW_VWVentas", "DWH");

                entity.Property(e => e.Country).HasMaxLength(15);
                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(40);
                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsFixedLength()
                    .HasColumnName("CustomerID");
                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(40);
                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
                entity.Property(e => e.EmployeeName)
                    .IsRequired()
                    .HasMaxLength(31);
                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(40);
                entity.Property(e => e.ShipperId).HasColumnName("ShipperID");
            });
            modelBuilder.Entity<Product>()
               .Property(p => p.UnitPrice)
               .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<VwDate>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("VwDates");
            });

        }

    }
}
