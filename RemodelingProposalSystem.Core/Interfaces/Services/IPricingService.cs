using RemodelingProposalSystem.Core.Models;

namespace RemodelingProposalSystem.Core.Interfaces;

public interface IPricingService
{
    Task<decimal> CalculateServiceCostAsync(Service service);
    Task<decimal> CalculateMaterialCostAsync(Material material, decimal quantity);
    Task<decimal> CalculateLaborCostAsync(Service service, string region);
    Task<decimal> GetRegionalPriceMultiplierAsync(string region);
    Task<decimal> GetPropertyTypeMultiplierAsync(string propertyType);
    Task<decimal> GetSeasonalMultiplierAsync(DateTime date);
} 