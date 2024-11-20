using LoadDWHSales.Data.Context;
using LoadDWHSales.Data.Entities.DWHSales;
using LoadDWHSales.Data.Entities.Northwind;
using LoadDWHSales.Data.Interfaces;
using LoadDWHSales.Data.Result;
using Microsoft.EntityFrameworkCore;


namespace LoadDWHSales.Data.Services
{
    public class DataServiceDwhSales : IDataServiceDWHSales
    {
        private readonly NorwindContext _norwindContext;
        private readonly DWHSalesContext _salesContext;

        public DataServiceDwhSales(NorwindContext norwindContext,
                                   DWHSalesContext salesContext)
        {
            _norwindContext = norwindContext;
            _salesContext = salesContext;
        }

        public async Task<OperactionResult> LoadDHW()
        {
            OperactionResult result = new OperactionResult();
            try
            {
                // await LoadDimEmployee();
                // await LoadDimProductCategory();
                // await LoadDimCustomers();
                // await LoadDimShippers();
          
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando el DWH Ventas. {ex.Message}";
            }

            return result;
        }

        private async Task<OperactionResult> LoadDimEmployee()
        {
            OperactionResult result = new OperactionResult();

            try
            {
                //Obtener los empleados de la base de datos de norwind.
                var employees = await _norwindContext.Employees.AsNoTracking().Select(emp => new DimEmployee()
                {
                    EmployeeID = emp.EmployeeId,
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                    Title = emp.Title,
                    

                }).ToListAsync();

                // Carga la dimension de empleados.

                _salesContext.DimEmployees.RemoveRange(employees);
                await _salesContext.DimEmployees.AddRangeAsync(employees);
                await _salesContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando la dimension de empleado {ex.Message}";
            }


            return result;
        }

        private async Task<OperactionResult> LoadDimProductCategory()
        {
            OperactionResult result = new OperactionResult();
            try
            {
                // Obtener las products categories de norwind //

                var productCategories = await (from product in _norwindContext.Products
                                               join category in _norwindContext.Categories on product.CategoryId equals category.CategoryId
                                               select new DimProductCategory()
                                               {
                                                   CategoryId = category.CategoryId,
                                                   ProductName = product.ProductName,
                                                   CategoryName = category.CategoryName,
                                                   ProductID = product.ProductId
                                               }).AsNoTracking().ToListAsync();


                // Carga la dimension de Products Categories.
                _salesContext.DimProductCategories.RemoveRange(productCategories);
                await _salesContext.DimProductCategories.AddRangeAsync(productCategories);
                await _salesContext.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error cargando la dimension de producto y categoria. {ex.Message}";
            }
            return result;
        }

        private async Task<OperactionResult> LoadDimCustomers()
        {
            OperactionResult operaction = new OperactionResult() { Success = false };


            try
            {
                // Obtener clientes de norwind

                var customers = await _norwindContext.Customers.Select(cust => new DimCustomer()
                {
                    CustomerID = cust.CustomerId,
                    CustomerName = cust.CompanyName,
                    City = cust.City,
                    Country = cust.Country,
                    Phone = cust.Phone

                }).AsNoTracking()
                  .ToListAsync();

                // Carga dimension de cliente.
                _salesContext.DimCustomers.RemoveRange(customers);
                await _salesContext.DimCustomers.AddRangeAsync(customers);
                await _salesContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                operaction.Success = false;
                operaction.Message = $"Error: {ex.Message} cargando la dimension de clientes.";
            }
            return operaction;
        }

        private async Task<OperactionResult> LoadDimShippers()
        {
            OperactionResult result = new OperactionResult();

            try
            {
                var shippers = await _norwindContext.Shippers.Select(ship => new DimShipper()
                {
                    ShipperID = ship.ShipperID,
                    ShipperName = ship.CompanyName
                }).ToListAsync();

                _salesContext.DimShippers.RemoveRange(shippers);
                await _salesContext.DimShippers.AddRangeAsync(shippers);
                await _salesContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando la dimension de shippers {ex.Message} ";
            }
            return result;
        }

    }
}
