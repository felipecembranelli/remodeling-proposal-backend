using RemodelingProposalSystem.Core.Models;

namespace RemodelingProposalSystem.Core.Interfaces.Repositories;

public interface IPricingRepository
{
    // Regional Pricing
    Task<IEnumerable<RegionalPricing>> GetAllRegionalPricingAsync();
    Task<RegionalPricing?> GetRegionalPricingAsync(string region);
    Task<RegionalPricing> AddRegionalPricingAsync(RegionalPricing pricing);
    Task<RegionalPricing> UpdateRegionalPricingAsync(RegionalPricing pricing);
    Task DeleteRegionalPricingAsync(string region);
    Task<decimal> GetRegionalMultiplierAsync(string region);

    // Property Type Pricing
    Task<IEnumerable<PropertyTypePricing>> GetAllPropertyTypePricingAsync();
    Task<PropertyTypePricing?> GetPropertyTypePricingAsync(string propertyType);
    Task<PropertyTypePricing> AddPropertyTypePricingAsync(PropertyTypePricing pricing);
    Task<PropertyTypePricing> UpdatePropertyTypePricingAsync(PropertyTypePricing pricing);
    Task DeletePropertyTypePricingAsync(string propertyType);
    Task<decimal> GetPropertyTypeMultiplierAsync(string propertyType);

    // Seasonal Pricing
    Task<IEnumerable<SeasonalPricing>> GetAllSeasonalPricingAsync();
    Task<SeasonalPricing?> GetSeasonalPricingAsync(string season);
    Task<SeasonalPricing> AddSeasonalPricingAsync(SeasonalPricing pricing);
    Task<SeasonalPricing> UpdateSeasonalPricingAsync(SeasonalPricing pricing);
    Task DeleteSeasonalPricingAsync(string season);
    Task<decimal> GetSeasonalMultiplierAsync(string season);

    // Labor Pricing
    Task<IEnumerable<LaborPricing>> GetAllLaborPricingAsync();
    Task<LaborPricing?> GetLaborPricingAsync(string serviceType);
    Task<LaborPricing> AddLaborPricingAsync(LaborPricing pricing);
    Task<LaborPricing> UpdateLaborPricingAsync(LaborPricing pricing);
    Task DeleteLaborPricingAsync(string serviceType);
    Task<decimal> GetLaborRateAsync(string serviceType);

    // Material Base Pricing
    Task<IEnumerable<MaterialBasePricing>> GetAllMaterialBasePricingAsync();
    Task<MaterialBasePricing?> GetMaterialBasePricingAsync(string materialId);
    Task<MaterialBasePricing> AddMaterialBasePricingAsync(MaterialBasePricing pricing);
    Task<MaterialBasePricing> UpdateMaterialBasePricingAsync(MaterialBasePricing pricing);
    Task DeleteMaterialBasePricingAsync(string materialId);
    Task<decimal> GetMaterialBasePriceAsync(string materialId);

    // Service Base Pricing
    Task<IEnumerable<ServiceBasePricing>> GetAllServiceBasePricingAsync();
    Task<ServiceBasePricing?> GetServiceBasePricingAsync(string serviceId);
    Task<ServiceBasePricing> AddServiceBasePricingAsync(ServiceBasePricing pricing);
    Task<ServiceBasePricing> UpdateServiceBasePricingAsync(ServiceBasePricing pricing);
    Task DeleteServiceBasePricingAsync(string serviceId);
    Task<decimal> GetServiceBasePriceAsync(string serviceId);

    // Combined Pricing Calculations
    Task<decimal> CalculateTotalPriceAsync(
        string region,
        string propertyType,
        string season,
        string serviceType,
        string materialId,
        decimal quantity);

    Task<decimal> CalculateLaborPriceAsync(
        string region,
        string propertyType,
        string season,
        string serviceType,
        decimal quantity);

    Task<decimal> CalculateMaterialPriceAsync(
        string region,
        string propertyType,
        string season,
        string materialId,
        decimal quantity);

    Task<decimal> CalculateServicePriceAsync(
        string region,
        string propertyType,
        string season,
        string serviceId,
        decimal quantity);

    // Bulk Operations
    Task UpdateAllRegionalMultipliersAsync(Dictionary<string, decimal> multipliers);
    Task UpdateAllPropertyTypeMultipliersAsync(Dictionary<string, decimal> multipliers);
    Task UpdateAllSeasonalMultipliersAsync(Dictionary<string, decimal> multipliers);
    Task UpdateAllLaborRatesAsync(Dictionary<string, decimal> rates);
    Task UpdateAllMaterialBasePricesAsync(Dictionary<string, decimal> prices);
    Task UpdateAllServiceBasePricesAsync(Dictionary<string, decimal> prices);

    // Price History
    Task<IEnumerable<PriceHistory>> GetPriceHistoryAsync(string itemId, DateTime startDate, DateTime endDate);
    Task<PriceHistory> AddPriceHistoryAsync(PriceHistory history);
    Task<IEnumerable<PriceHistory>> GetPriceHistoryByTypeAsync(string itemType, DateTime startDate, DateTime endDate);
    Task<IEnumerable<PriceHistory>> GetPriceHistoryByRegionAsync(string region, DateTime startDate, DateTime endDate);
    Task<IEnumerable<PriceHistory>> GetPriceHistoryByPropertyTypeAsync(string propertyType, DateTime startDate, DateTime endDate);
    Task<IEnumerable<PriceHistory>> GetPriceHistoryBySeasonAsync(string season, DateTime startDate, DateTime endDate);
    
    
}