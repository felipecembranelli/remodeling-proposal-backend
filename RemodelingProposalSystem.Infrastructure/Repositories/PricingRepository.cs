using RemodelingProposalSystem.Core.Interfaces.Repositories;
using RemodelingProposalSystem.Core.Models;
using RemodelingProposalSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RemodelingProposalSystem.Infrastructure.Repositories;

public class PricingRepository : IPricingRepository
{
    private readonly ApplicationDbContext _context;

    public PricingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Regional Pricing
    public async Task<IEnumerable<RegionalPricing>> GetAllRegionalPricingAsync()
    {
        return await _context.RegionalPricing.ToListAsync();
    }

    public async Task<RegionalPricing?> GetRegionalPricingAsync(string region)
    {
        return await _context.RegionalPricing.FirstOrDefaultAsync(p => p.Region == region);
    }

    public async Task<RegionalPricing> AddRegionalPricingAsync(RegionalPricing pricing)
    {
        _context.RegionalPricing.Add(pricing);
        await _context.SaveChangesAsync();
        return pricing;
    }

    public async Task<RegionalPricing> UpdateRegionalPricingAsync(RegionalPricing pricing)
    {
        var existing = await _context.RegionalPricing.FirstOrDefaultAsync(p => p.Region == pricing.Region);
        if (existing == null)
            throw new KeyNotFoundException($"Regional pricing for region {pricing.Region} not found.");

        existing.Multiplier = pricing.Multiplier;
        existing.Description = pricing.Description;
        existing.LastUpdated = DateTime.UtcNow;
        existing.UpdatedBy = pricing.UpdatedBy;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteRegionalPricingAsync(string region)
    {
        var pricing = await _context.RegionalPricing.FirstOrDefaultAsync(p => p.Region == region);
        if (pricing == null)
            throw new KeyNotFoundException($"Regional pricing for region {region} not found.");

        _context.RegionalPricing.Remove(pricing);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetRegionalMultiplierAsync(string region)
    {
        var pricing = await _context.RegionalPricing.FirstOrDefaultAsync(p => p.Region == region);
        return pricing?.Multiplier ?? 1.0m;
    }

    // Property Type Pricing
    public async Task<IEnumerable<PropertyTypePricing>> GetAllPropertyTypePricingAsync()
    {
        return await _context.PropertyTypePricing.ToListAsync();
    }

    public async Task<PropertyTypePricing?> GetPropertyTypePricingAsync(string propertyType)
    {
        return await _context.PropertyTypePricing.FirstOrDefaultAsync(p => p.PropertyType == propertyType);
    }

    public async Task<PropertyTypePricing> AddPropertyTypePricingAsync(PropertyTypePricing pricing)
    {
        _context.PropertyTypePricing.Add(pricing);
        await _context.SaveChangesAsync();
        return pricing;
    }

    public async Task<PropertyTypePricing> UpdatePropertyTypePricingAsync(PropertyTypePricing pricing)
    {
        var existing = await _context.PropertyTypePricing.FirstOrDefaultAsync(p => p.PropertyType == pricing.PropertyType);
        if (existing == null)
            throw new KeyNotFoundException($"Property type pricing for type {pricing.PropertyType} not found.");

        existing.Multiplier = pricing.Multiplier;
        existing.Description = pricing.Description;
        existing.LastUpdated = DateTime.UtcNow;
        existing.UpdatedBy = pricing.UpdatedBy;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeletePropertyTypePricingAsync(string propertyType)
    {
        var pricing = await _context.PropertyTypePricing.FirstOrDefaultAsync(p => p.PropertyType == propertyType);
        if (pricing == null)
            throw new KeyNotFoundException($"Property type pricing for type {propertyType} not found.");

        _context.PropertyTypePricing.Remove(pricing);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetPropertyTypeMultiplierAsync(string propertyType)
    {
        var pricing = await _context.PropertyTypePricing.FirstOrDefaultAsync(p => p.PropertyType == propertyType);
        return pricing?.Multiplier ?? 1.0m;
    }

    // Seasonal Pricing
    public async Task<IEnumerable<SeasonalPricing>> GetAllSeasonalPricingAsync()
    {
        return await _context.SeasonalPricing.ToListAsync();
    }

    public async Task<SeasonalPricing?> GetSeasonalPricingAsync(string season)
    {
        return await _context.SeasonalPricing.FirstOrDefaultAsync(p => p.Season == season);
    }

    public async Task<SeasonalPricing> AddSeasonalPricingAsync(SeasonalPricing pricing)
    {
        _context.SeasonalPricing.Add(pricing);
        await _context.SaveChangesAsync();
        return pricing;
    }

    public async Task<SeasonalPricing> UpdateSeasonalPricingAsync(SeasonalPricing pricing)
    {
        var existing = await _context.SeasonalPricing.FirstOrDefaultAsync(p => p.Season == pricing.Season);
        if (existing == null)
            throw new KeyNotFoundException($"Seasonal pricing for season {pricing.Season} not found.");

        existing.Multiplier = pricing.Multiplier;
        existing.Description = pricing.Description;
        existing.LastUpdated = DateTime.UtcNow;
        existing.UpdatedBy = pricing.UpdatedBy;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteSeasonalPricingAsync(string season)
    {
        var pricing = await _context.SeasonalPricing.FirstOrDefaultAsync(p => p.Season == season);
        if (pricing == null)
            throw new KeyNotFoundException($"Seasonal pricing for season {season} not found.");

        _context.SeasonalPricing.Remove(pricing);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetSeasonalMultiplierAsync(string season)
    {
        var pricing = await _context.SeasonalPricing.FirstOrDefaultAsync(p => p.Season == season);
        return pricing?.Multiplier ?? 1.0m;
    }

    // Labor Pricing
    public async Task<IEnumerable<LaborPricing>> GetAllLaborPricingAsync()
    {
        return await _context.LaborPricing.ToListAsync();
    }

    public async Task<LaborPricing?> GetLaborPricingAsync(string serviceType)
    {
        return await _context.LaborPricing.FirstOrDefaultAsync(p => p.ServiceType == serviceType);
    }

    public async Task<LaborPricing> AddLaborPricingAsync(LaborPricing pricing)
    {
        _context.LaborPricing.Add(pricing);
        await _context.SaveChangesAsync();
        return pricing;
    }

    public async Task<LaborPricing> UpdateLaborPricingAsync(LaborPricing pricing)
    {
        var existing = await _context.LaborPricing.FirstOrDefaultAsync(p => p.ServiceType == pricing.ServiceType);
        if (existing == null)
            throw new KeyNotFoundException($"Labor pricing for service type {pricing.ServiceType} not found.");

        existing.BaseRate = pricing.BaseRate;
        existing.Description = pricing.Description;
        existing.LastUpdated = DateTime.UtcNow;
        existing.UpdatedBy = pricing.UpdatedBy;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteLaborPricingAsync(string serviceType)
    {
        var pricing = await _context.LaborPricing.FirstOrDefaultAsync(p => p.ServiceType == serviceType);
        if (pricing == null)
            throw new KeyNotFoundException($"Labor pricing for service type {serviceType} not found.");

        _context.LaborPricing.Remove(pricing);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetLaborRateAsync(string serviceType)
    {
        var pricing = await _context.LaborPricing.FirstOrDefaultAsync(p => p.ServiceType == serviceType);
        return pricing?.BaseRate ?? 0m;
    }

    // Material Base Pricing
    public async Task<IEnumerable<MaterialBasePricing>> GetAllMaterialBasePricingAsync()
    {
        return await _context.MaterialBasePricing.ToListAsync();
    }

    public async Task<MaterialBasePricing?> GetMaterialBasePricingAsync(string materialId)
    {
        return await _context.MaterialBasePricing.FirstOrDefaultAsync(p => p.MaterialId == materialId);
    }

    public async Task<MaterialBasePricing> AddMaterialBasePricingAsync(MaterialBasePricing pricing)
    {
        _context.MaterialBasePricing.Add(pricing);
        await _context.SaveChangesAsync();
        return pricing;
    }

    public async Task<MaterialBasePricing> UpdateMaterialBasePricingAsync(MaterialBasePricing pricing)
    {
        var existing = await _context.MaterialBasePricing.FirstOrDefaultAsync(p => p.MaterialId == pricing.MaterialId);
        if (existing == null)
            throw new KeyNotFoundException($"Material base pricing for material {pricing.MaterialId} not found.");

        existing.BasePrice = pricing.BasePrice;
        existing.UnitOfMeasure = pricing.UnitOfMeasure;
        existing.Description = pricing.Description;
        existing.LastUpdated = DateTime.UtcNow;
        existing.UpdatedBy = pricing.UpdatedBy;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteMaterialBasePricingAsync(string materialId)
    {
        var pricing = await _context.MaterialBasePricing.FirstOrDefaultAsync(p => p.MaterialId == materialId);
        if (pricing == null)
            throw new KeyNotFoundException($"Material base pricing for material {materialId} not found.");

        _context.MaterialBasePricing.Remove(pricing);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetMaterialBasePriceAsync(string materialId)
    {
        var pricing = await _context.MaterialBasePricing.FirstOrDefaultAsync(p => p.MaterialId == materialId);
        return pricing?.BasePrice ?? 0m;
    }

    // Service Base Pricing
    public async Task<IEnumerable<ServiceBasePricing>> GetAllServiceBasePricingAsync()
    {
        return await _context.ServiceBasePricing.ToListAsync();
    }

    public async Task<ServiceBasePricing?> GetServiceBasePricingAsync(string serviceId)
    {
        return await _context.ServiceBasePricing.FirstOrDefaultAsync(p => p.ServiceId == serviceId);
    }

    public async Task<ServiceBasePricing> AddServiceBasePricingAsync(ServiceBasePricing pricing)
    {
        _context.ServiceBasePricing.Add(pricing);
        await _context.SaveChangesAsync();
        return pricing;
    }

    public async Task<ServiceBasePricing> UpdateServiceBasePricingAsync(ServiceBasePricing pricing)
    {
        var existing = await _context.ServiceBasePricing.FirstOrDefaultAsync(p => p.ServiceId == pricing.ServiceId);
        if (existing == null)
            throw new KeyNotFoundException($"Service base pricing for service {pricing.ServiceId} not found.");

        existing.BasePrice = pricing.BasePrice;
        existing.UnitOfMeasure = pricing.UnitOfMeasure;
        existing.Description = pricing.Description;
        existing.LastUpdated = DateTime.UtcNow;
        existing.UpdatedBy = pricing.UpdatedBy;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteServiceBasePricingAsync(string serviceId)
    {
        var pricing = await _context.ServiceBasePricing.FirstOrDefaultAsync(p => p.ServiceId == serviceId);
        if (pricing == null)
            throw new KeyNotFoundException($"Service base pricing for service {serviceId} not found.");

        _context.ServiceBasePricing.Remove(pricing);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetServiceBasePriceAsync(string serviceId)
    {
        var pricing = await _context.ServiceBasePricing.FirstOrDefaultAsync(p => p.ServiceId == serviceId);
        return pricing?.BasePrice ?? 0m;
    }

    // Combined Pricing Calculations
    public async Task<decimal> CalculateTotalPriceAsync(
        string region,
        string propertyType,
        string season,
        string serviceType,
        string materialId,
        decimal quantity)
    {
        var regionalMultiplier = await GetRegionalMultiplierAsync(region);
        var propertyTypeMultiplier = await GetPropertyTypeMultiplierAsync(propertyType);
        var seasonalMultiplier = await GetSeasonalMultiplierAsync(season);
        var laborRate = await GetLaborRateAsync(serviceType);
        var materialBasePrice = await GetMaterialBasePriceAsync(materialId);

        var totalMultiplier = regionalMultiplier * propertyTypeMultiplier * seasonalMultiplier;
        var laborCost = laborRate * quantity * totalMultiplier;
        var materialCost = materialBasePrice * quantity * totalMultiplier;

        return laborCost + materialCost;
    }

    public async Task<decimal> CalculateLaborPriceAsync(
        string region,
        string propertyType,
        string season,
        string serviceType,
        decimal quantity)
    {
        var regionalMultiplier = await GetRegionalMultiplierAsync(region);
        var propertyTypeMultiplier = await GetPropertyTypeMultiplierAsync(propertyType);
        var seasonalMultiplier = await GetSeasonalMultiplierAsync(season);
        var laborRate = await GetLaborRateAsync(serviceType);

        var totalMultiplier = regionalMultiplier * propertyTypeMultiplier * seasonalMultiplier;
        return laborRate * quantity * totalMultiplier;
    }

    public async Task<decimal> CalculateMaterialPriceAsync(
        string region,
        string propertyType,
        string season,
        string materialId,
        decimal quantity)
    {
        var regionalMultiplier = await GetRegionalMultiplierAsync(region);
        var propertyTypeMultiplier = await GetPropertyTypeMultiplierAsync(propertyType);
        var seasonalMultiplier = await GetSeasonalMultiplierAsync(season);
        var materialBasePrice = await GetMaterialBasePriceAsync(materialId);

        var totalMultiplier = regionalMultiplier * propertyTypeMultiplier * seasonalMultiplier;
        return materialBasePrice * quantity * totalMultiplier;
    }

    public async Task<decimal> CalculateServicePriceAsync(
        string region,
        string propertyType,
        string season,
        string serviceId,
        decimal quantity)
    {
        var regionalMultiplier = await GetRegionalMultiplierAsync(region);
        var propertyTypeMultiplier = await GetPropertyTypeMultiplierAsync(propertyType);
        var seasonalMultiplier = await GetSeasonalMultiplierAsync(season);
        var serviceBasePrice = await GetServiceBasePriceAsync(serviceId);

        var totalMultiplier = regionalMultiplier * propertyTypeMultiplier * seasonalMultiplier;
        return serviceBasePrice * quantity * totalMultiplier;
    }

    // Bulk Operations
    public async Task UpdateAllRegionalMultipliersAsync(Dictionary<string, decimal> multipliers)
    {
        foreach (var multiplier in multipliers)
        {
            var pricing = await GetRegionalPricingAsync(multiplier.Key);
            if (pricing != null)
            {
                pricing.Multiplier = multiplier.Value;
                pricing.LastUpdated = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAllPropertyTypeMultipliersAsync(Dictionary<string, decimal> multipliers)
    {
        foreach (var multiplier in multipliers)
        {
            var pricing = await GetPropertyTypePricingAsync(multiplier.Key);
            if (pricing != null)
            {
                pricing.Multiplier = multiplier.Value;
                pricing.LastUpdated = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAllSeasonalMultipliersAsync(Dictionary<string, decimal> multipliers)
    {
        foreach (var multiplier in multipliers)
        {
            var pricing = await GetSeasonalPricingAsync(multiplier.Key);
            if (pricing != null)
            {
                pricing.Multiplier = multiplier.Value;
                pricing.LastUpdated = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAllLaborRatesAsync(Dictionary<string, decimal> rates)
    {
        foreach (var rate in rates)
        {
            var pricing = await GetLaborPricingAsync(rate.Key);
            if (pricing != null)
            {
                pricing.BaseRate = rate.Value;
                pricing.LastUpdated = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAllMaterialBasePricesAsync(Dictionary<string, decimal> prices)
    {
        foreach (var price in prices)
        {
            var pricing = await GetMaterialBasePricingAsync(price.Key);
            if (pricing != null)
            {
                pricing.BasePrice = price.Value;
                pricing.LastUpdated = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAllServiceBasePricesAsync(Dictionary<string, decimal> prices)
    {
        foreach (var price in prices)
        {
            var pricing = await GetServiceBasePricingAsync(price.Key);
            if (pricing != null)
            {
                pricing.BasePrice = price.Value;
                pricing.LastUpdated = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
    }

    // Price History
    public async Task<IEnumerable<PriceHistory>> GetPriceHistoryAsync(string itemId, DateTime startDate, DateTime endDate)
    {
        return await _context.PriceHistory
            .Where(h => h.ItemId == itemId && h.ChangeDate >= startDate && h.ChangeDate <= endDate)
            .OrderByDescending(h => h.ChangeDate)
            .ToListAsync();
    }

    public async Task<PriceHistory> AddPriceHistoryAsync(PriceHistory history)
    {
        _context.PriceHistory.Add(history);
        await _context.SaveChangesAsync();
        return history;
    }

    public async Task<IEnumerable<PriceHistory>> GetPriceHistoryByTypeAsync(string itemType, DateTime startDate, DateTime endDate)
    {
        return await _context.PriceHistory
            .Where(h => h.ItemType == itemType && h.ChangeDate >= startDate && h.ChangeDate <= endDate)
            .OrderByDescending(h => h.ChangeDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PriceHistory>> GetPriceHistoryByRegionAsync(string region, DateTime startDate, DateTime endDate)
    {
        return await _context.PriceHistory
            .Where(h => h.Region == region && h.ChangeDate >= startDate && h.ChangeDate <= endDate)
            .OrderByDescending(h => h.ChangeDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PriceHistory>> GetPriceHistoryByPropertyTypeAsync(string propertyType, DateTime startDate, DateTime endDate)
    {
        return await _context.PriceHistory
            .Where(h => h.PropertyType == propertyType && h.ChangeDate >= startDate && h.ChangeDate <= endDate)
            .OrderByDescending(h => h.ChangeDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PriceHistory>> GetPriceHistoryBySeasonAsync(string season, DateTime startDate, DateTime endDate)
    {
        return await _context.PriceHistory
            .Where(h => h.Season == season && h.ChangeDate >= startDate && h.ChangeDate <= endDate)
            .OrderByDescending(h => h.ChangeDate)
            .ToListAsync();
    }
} 