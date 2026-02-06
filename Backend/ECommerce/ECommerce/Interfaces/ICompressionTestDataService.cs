namespace ECommerce.Interfaces
{
    public interface ICompressionTestDataService
    {
        List<dynamic> GetLargeData1MB();
        List<dynamic> GetLargeData5MB();
        List<dynamic> GetLargeData10MB();
    }
}
