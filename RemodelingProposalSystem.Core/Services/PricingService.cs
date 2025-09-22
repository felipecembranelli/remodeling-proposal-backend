using RemodelingProposalSystem.Core.Interfaces;
using RemodelingProposalSystem.Core.Models;
using Microsoft.Extensions.Configuration;

namespace RemodelingProposalSystem.Infrastructure.Services;

public class PricingService : IPricingService
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, decimal> _regionalMultipliers;
    private readonly Dictionary<string, decimal> _propertyTypeMultipliers;
    private readonly Dictionary<string, decimal> _seasonalMultipliers;

    public PricingService(IConfiguration configuration)
    {
        _configuration = configuration;
        _regionalMultipliers = new Dictionary<string, decimal>
        {
            { "north", 1.2m },
            { "south", 0.9m },
            { "east", 1.1m },
            { "west", 1.0m }
        };

        _propertyTypeMultipliers = new Dictionary<string, decimal>
        {
            { "residential", 1.0m },
            { "commercial", 1.3m },
            { "industrial", 1.5m }
        };

        _seasonalMultipliers = new Dictionary<string, decimal>
        {
            { "spring", 1.1m },
            { "summer", 1.0m },
            { "fall", 0.9m },
            { "winter", 0.8m }
        };
    }

    public async Task<decimal> CalculateServiceCostAsync(Service service)
    {
        var baseCost = service.BasePrice * service.Quantity;
        var laborCost = await CalculateLaborCostAsync(service, service.Category);
        var materialCost = service.RequiredMaterials.Sum(m => m.TotalCost);

        return baseCost + laborCost + materialCost;
    }

    public async Task<decimal> CalculateMaterialCostAsync(Material material, decimal quantity)
    {
        var baseCost = material.UnitPrice * quantity;
        var regionalMultiplier = await GetRegionalPriceMultiplierAsync(material.Id);
        var seasonalMultiplier = await GetSeasonalMultiplierAsync(DateTime.UtcNow);

        return baseCost * regionalMultiplier * seasonalMultiplier;
    }

    public async Task<decimal> CalculateLaborCostAsync(Service service, string region)
    {
        var baseLaborCost = service.LaborCost * service.Quantity;
        var regionalMultiplier = await GetRegionalPriceMultiplierAsync(region);
        //var propertyTypeMultiplier = await GetPropertyTypeMultiplierAsync(service.propertyType);
        var propertyTypeMultiplier = 1;
        var seasonalMultiplier = await GetSeasonalMultiplierAsync(DateTime.UtcNow);

        return baseLaborCost * regionalMultiplier * propertyTypeMultiplier * seasonalMultiplier;
    }

    public async Task<decimal> GetRegionalPriceMultiplierAsync(string region)
    {
        return _regionalMultipliers.TryGetValue(region.ToLower(), out var multiplier)
            ? multiplier
            : 1.0m;
    }

    public async Task<decimal> GetPropertyTypeMultiplierAsync(string propertyType)
    {
        return _propertyTypeMultipliers.TryGetValue(propertyType.ToLower(), out var multiplier)
            ? multiplier
            : 1.0m;
    }

    public async Task<decimal> GetSeasonalMultiplierAsync(DateTime date)
    {
        var month = date.Month;
        var season = month switch
        {
            >= 3 and <= 5 => "spring",
            >= 6 and <= 8 => "summer",
            >= 9 and <= 11 => "fall",
            _ => "winter"
        };

        return _seasonalMultipliers.TryGetValue(season, out var multiplier)
            ? multiplier
            : 1.0m;
    }
} 