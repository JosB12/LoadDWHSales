using LoadDWHSales.Data.Result;


namespace LoadDWHSales.Data.Interfaces
{
    public interface IDataServiceDWHSales
    {
        Task<OperactionResult> LoadDHW();
    }
}
