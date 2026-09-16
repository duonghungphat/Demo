namespace Demo.Service.Services.Interfaces
{
    public interface ICoursePriceCalculator
    {
        Guid InstanceId { get; }
        decimal CalculateFinalPrice(decimal price, decimal discountPercent);
    }
}
