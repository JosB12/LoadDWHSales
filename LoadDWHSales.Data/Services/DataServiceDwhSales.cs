using LoadDWHSales.Data.Context;
using LoadDWHSales.Data.Entities.DWHSales;
using LoadDWHSales.Data.Entities.Northwind;
using LoadDWHSales.Data.Interfaces;
using LoadDWHSales.Data.Result;
using Microsoft.EntityFrameworkCore;


namespace LoadDWHSales.Data.Services
{
    public class DataServiceDWHSales : IDataServiceDWHSales
    {
        private readonly NorwindContext _norwindContext;
        private readonly DWHSalesContext _salesContext;

        public DataServiceDWHSales(NorwindContext norwindContext,
                                   DWHSalesContext salesContext)
        {
            _norwindContext = norwindContext;
            _salesContext = salesContext;
        }

        public async Task<OperactionResult> LoadDHW()
        {
            OperactionResult result = new OperactionResult();
            try
            {// Limpiar las tablas antes de cargar los datos
                await ClearTablesAsync();
                
                //Cargar las tablas

                 await LoadDimEmployee();
                 await LoadDimProductCategory();
                 await LoadDimCustomers();
                 await LoadDimShippers();
                //await LoadFactSales();
                //await LoadFactCustomerServed();
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando el DWH Ventas. {ex.Message}";
            }

            return result;
        }
        private async Task ClearTablesAsync()
        {
            try
            {
                // Limpiar las tablas de dimensiones
                _salesContext.DimEmployees.RemoveRange(_salesContext.DimEmployees);
                _salesContext.DimProductCategories.RemoveRange(_salesContext.DimProductCategories);
                _salesContext.DimCustomers.RemoveRange(_salesContext.DimCustomers);
                _salesContext.DimShippers.RemoveRange(_salesContext.DimShippers);

                // Guardar los cambios para aplicar la limpieza
                await _salesContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al limpiar las tablas: {ex.Message}");
            }
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

                var existeCustomerId = await _salesContext.DimCustomers
            .Where(c => customers.Select(cust => cust.CustomerID).Contains(c.CustomerID))
            .Select(c => c.CustomerID)
            .ToListAsync();

                var newCustomers = customers
                    .Where(c => !existeCustomerId.Contains(c.CustomerID))
                    .ToList();

                if (newCustomers.Any())
                {
                    await _salesContext.DimCustomers.AddRangeAsync(newCustomers);
                    await _salesContext.SaveChangesAsync();
                }
                // Carga dimension de cliente.
               

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

        private async Task<OperactionResult> LoadFactSales()
        {
            OperactionResult result = new OperactionResult();

            try
            {
                var ventas = await _norwindContext.VwVwventas.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando el fact de ventas {ex.Message} ";
            }

            return result;
        }

        private async Task<OperactionResult> LoadFactCustomerServed()
        {
            OperactionResult result = new OperactionResult() { Success = true };

            try
            {
                var customerServed = await _norwindContext.VwServedCustomers.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando el fact de clientes atendidos {ex.Message} ";
            }
            return result;
        }
    }
}
