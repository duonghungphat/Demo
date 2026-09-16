using Demo.Service.Services.Interfaces;

namespace Demo.Service.Services.Implementations
{
    public class CoursePriceCalculator : ICoursePriceCalculator
    {
        public Guid InstanceId { get; } = Guid.NewGuid();   
        public decimal CalculateFinalPrice(decimal price, decimal discountPercent)
        {
            var discountAmount = price * (discountPercent / 100);
            return price - discountAmount;
        }
    }
}
