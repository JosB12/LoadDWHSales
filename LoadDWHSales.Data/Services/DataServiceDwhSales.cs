using LoadDWHSales.Data.Context;
using LoadDWHSales.Data.Entities.DWHSales;
using LoadDWHSales.Data.Entities.Northwind;
using LoadDWHSales.Data.Interfaces;
using LoadDWHSales.Data.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Cryptography;


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

                //   //Cargar las tablas

                await LoadDimEmployee();
                await LoadDimProductCategory();
                await LoadDimCustomers();
                await LoadDimShippers();
                await LoadDimDate();
                await LoadFactOrders();
                await LoadFactServedCustomers();
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
                await _salesContext.FactOrders
                   .Where(fo => _salesContext.DimCustomers.Any(dc => dc.CustomerID == fo.CustomerId))
                   .ExecuteDeleteAsync();

                // Eliminar las filas en FactServedCustomers si es necesario
                await _salesContext.FactServedCustomers
                 .Where(fo => _salesContext.DimEmployees.Any(dc => dc.EmployeeID == fo.EmployeeId))
                 .ExecuteDeleteAsync();

                // Luego limpiar las tablas de dimensiones (las tablas "padre")
                _salesContext.DimCustomers.RemoveRange(_salesContext.DimCustomers);
                _salesContext.DimEmployees.RemoveRange(_salesContext.DimEmployees);
                _salesContext.DimProductCategories.RemoveRange(_salesContext.DimProductCategories);
                _salesContext.DimShippers.RemoveRange(_salesContext.DimShippers);
                _salesContext.DimDates.RemoveRange(_salesContext.DimDates);

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

        private async Task<OperactionResult> LoadDimDate()
        {
            OperactionResult result = new OperactionResult() { Success = true };

            try
            {
                // Obtener todas las fechas únicas de VwDates   
                var vwDates = await _norwindContext.VwDates
                    .AsNoTracking()
                    .Where(v => v.DateKey.HasValue && v.FullDate.HasValue)
                    .Select(v => new
                    {
                        DateKey = v.DateKey.Value,
                        FullDate = v.FullDate.Value.ToDateTime(TimeOnly.MinValue),
                        Year = v.Year.Value,
                        Month = v.Month.Value,
                    })
                    .Distinct()
                    .ToListAsync();

                var existingDates = await _salesContext.DimDates
                    .Select(d => d.FullDate)
                    .ToListAsync();

                var newDates = vwDates
                    .Where(d => !existingDates.Contains(d.FullDate))
                    .Select(d => new DimDate
                    {
                        DateID = d.DateKey,
                        FullDate = d.FullDate,
                        Year = d.Year,
                        Month = d.Month,
                    })
                    .ToList();

                if (newDates.Any())
                {
                    await _salesContext.DimDates.AddRangeAsync(newDates);
                    await _salesContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error cargando la dimensión de fechas: {ex.Message}";
            }

            return result;
        }


        private async Task<OperactionResult> LoadFactOrders()
        {
            OperactionResult result = new OperactionResult();

            try
            {
                var ventas = await _norwindContext.VwVwventas.AsNoTracking().ToListAsync();


                int[] ordersId = await _salesContext.FactOrders.Select(cd => cd.OrderId).ToArrayAsync();

                if (ordersId.Any())
                {
                    await _salesContext.FactOrders.Where(cd => ordersId.Contains(cd.OrderId))
                                                  .AsNoTracking()
                                                  .ExecuteDeleteAsync();
                }

                foreach (var venta in ventas)
                {
                    var customer = await _salesContext.DimCustomers.SingleOrDefaultAsync(cust => cust.CustomerID == venta.CustomerId);
                    var employee = await _salesContext.DimEmployees.SingleOrDefaultAsync(emp => emp.EmployeeID == venta.EmployeeId);
                    var shipper = await _salesContext.DimShippers.SingleOrDefaultAsync(ship => ship.ShipperID == venta.ShipperId);
                    var product = await _salesContext.DimProductCategories.SingleOrDefaultAsync(pro => pro.ProductID == venta.ProductId);
                  
                    FactOrders factOrder = new FactOrders()
                    {
                        Quantity = venta.Cantidad.Value,
                        Country = venta.Country,
                        CustomerId = customer.CustomerID,
                        EmployeeId = employee.EmployeeID,
                        DateId = venta.DateKey.Value,
                        ProductId = product.ProductID,
                        ShipperId = shipper.ShipperID,
                        TotalSales = Convert.ToDecimal(venta.TotalVentas)
                    };

                    await _salesContext.FactOrders.AddAsync(factOrder);

                    await _salesContext.SaveChangesAsync();
                }



                result.Success = true;
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando el fact de ventas {ex.Message} ";
            }

            return result;
        }
         private async Task<OperactionResult> LoadFactServedCustomers()
        {
            OperactionResult result = new OperactionResult() { Success = true };

            try
            {
                var customerServeds = await _norwindContext.VwServedCustomers.AsNoTracking().ToListAsync();

                int[] customerIds = _salesContext.FactServedCustomers.Select(cli => cli.ClienteAtendidoId).ToArray();

                //Limpiamos la tabla de facts //

                if (customerIds.Any())
                {
                    await _salesContext.FactServedCustomers.Where(fact => customerIds.Contains(fact.ClienteAtendidoId))
                                                            .AsNoTracking()
                                                            .ExecuteDeleteAsync();
                }

                //Carga el fact de clientes atendidos. //
                foreach (var customer in customerServeds)
                {
                    var employee = await _salesContext.DimEmployees
                                                      .SingleOrDefaultAsync(emp => emp.EmployeeID ==
                                                                               customer.EmployeeId);


                    FactServedCustomers factClienteAtendido = new FactServedCustomers()
                    {
                        EmployeeId = employee.EmployeeID,
                        TotalClientes = customer.TotalCustomersServed
                    };


                    await _salesContext.FactServedCustomers.AddAsync(factClienteAtendido);

                    await _salesContext.SaveChangesAsync();
                }

                result.Success = true;

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
