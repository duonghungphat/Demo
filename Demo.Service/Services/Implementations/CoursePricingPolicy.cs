using Demo.Service.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Demo.Service.Services.Implementations
{
    public class CoursePricingPolicy : ICoursePricingPolicy
    {
        public decimal MaxDiscountPercent { get; }

        public CoursePricingPolicy(IConfiguration configuration)
        {
            MaxDiscountPercent = configuration.GetValue<decimal?>("CoursePricing:MaximumDiscountPercent") ?? 50m;
        }

        public bool IsDiscountAllowed(decimal discountPercent)
        {
            return discountPercent >= 0 && discountPercent <= MaxDiscountPercent;
        }
    }
}