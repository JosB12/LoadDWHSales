using LoadDWHSales.Data.Context;
using LoadDWHSales.Data.Interfaces;
using LoadDWHSales.Data.Services;
using LoadDWHSales.WorkerService;
using Microsoft.EntityFrameworkCore;

namespace LoadDWHSales.WorkerService
{
    public class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) => {

                services.AddDbContextPool<NorwindContext>(options =>
                                                          options.UseSqlServer(hostContext.Configuration.GetConnectionString("DbNorwind")));

                services.AddDbContextPool<DWHSalesContext>(options =>
                                                          options.UseSqlServer(hostContext.Configuration.GetConnectionString("DWHSales")));


                services.AddScoped<IDataServiceDWHSales, DataServiceDWHSales>();

                services.AddHostedService<Worker>();
            });
    }
}