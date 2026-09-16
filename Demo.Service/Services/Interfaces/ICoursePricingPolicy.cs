namespace Demo.Service.Services.Interfaces
{
    public interface ICoursePricingPolicy
    {
        decimal MaxDiscountPercent { get; }
        bool IsDiscountAllowed(decimal discountPercent);
    }
}
