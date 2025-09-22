using RemodelingProposalSystem.Core.Interfaces;
using RemodelingProposalSystem.Core.Models;

namespace RemodelingProposalSystem.Core.Services;

public class AIService : IAIService
{
    private readonly IPricingService _pricingService;
    private readonly Dictionary<string, List<Service>> _serviceCatalog = new()
    {
        {
            "Residential", new List<Service>
            {
                new() { Name = "Kitchen Remodel", BasePrice = 25000m, LaborCost = 15000m, Category = "Kitchen", RequiresPermit = true, ComplexityLevel = "Advanced" },
                new() { Name = "Bathroom Renovation", BasePrice = 15000m, LaborCost = 10000m, Category = "Bathroom", RequiresPermit = true, ComplexityLevel = "Intermediate" },
                new() { Name = "Flooring Installation", BasePrice = 8000m, LaborCost = 5000m, Category = "Flooring", RequiresPermit = false, ComplexityLevel = "Basic" },
                new() { Name = "Interior Painting", BasePrice = 3000m, LaborCost = 2000m, Category = "Painting", RequiresPermit = false, ComplexityLevel = "Basic" },
                new() { Name = "Carpentry Work", BasePrice = 5000m, LaborCost = 3500m, Category = "Carpentry", RequiresPermit = false, ComplexityLevel = "Intermediate" }
            }
        },
        {
            "Commercial", new List<Service>
            {
                new() { Name = "Office Space Renovation", BasePrice = 50000m, LaborCost = 30000m, Category = "Office", RequiresPermit = true, ComplexityLevel = "Advanced" },
                new() { Name = "Retail Space Remodel", BasePrice = 75000m, LaborCost = 45000m, Category = "Retail", RequiresPermit = true, ComplexityLevel = "Advanced" },
                new() { Name = "Restaurant Renovation", BasePrice = 100000m, LaborCost = 60000m, Category = "Restaurant", RequiresPermit = true, ComplexityLevel = "Advanced" },
                new() { Name = "Commercial Flooring", BasePrice = 15000m, LaborCost = 10000m, Category = "Flooring", RequiresPermit = false, ComplexityLevel = "Intermediate" }
            }
        },
        {
            "Industrial", new List<Service>
            {
                new() { Name = "Warehouse Renovation", BasePrice = 100000m, LaborCost = 60000m, Category = "Industrial", RequiresPermit = true, ComplexityLevel = "Advanced" },
                new() { Name = "Facility Upgrades", BasePrice = 75000m, LaborCost = 45000m, Category = "Industrial", RequiresPermit = true, ComplexityLevel = "Advanced" },
                new() { Name = "Industrial Flooring", BasePrice = 25000m, LaborCost = 15000m, Category = "Flooring", RequiresPermit = false, ComplexityLevel = "Intermediate" }
            }
        }
    };

    public AIService(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public async Task<Proposal> GenerateProposalDetailsAsync(
        string propertyType,
        decimal propertySize,
        string region,
        decimal budget,
        List<string> requestedServices)
    {
        var proposal = new Proposal
        {
            Id = Guid.NewGuid(),
            PropertyType = propertyType,
            PropertySize = propertySize,
            Region = region,
            Budget = budget,
            CreatedAt = DateTime.UtcNow,
            ValidUntil = DateTime.UtcNow.AddDays(30),
            Status = "Draft"
        };

        // Get available services for the property type
        var availableServices = _serviceCatalog.GetValueOrDefault(propertyType, new List<Service>());
        
        // Filter requested services
        var selectedServices = availableServices
            .Where(s => requestedServices.Contains(s.Name))
            .ToList();

        // Calculate costs for each service
        foreach (var service in selectedServices)
        {
            // Calculate service cost with regional adjustments
            var serviceCost = await _pricingService.CalculateServiceCostAsync(service);
            var regionalMultiplier = await _pricingService.GetRegionalPriceMultiplierAsync(region);
            var propertyTypeMultiplier = await _pricingService.GetPropertyTypeMultiplierAsync(propertyType);
            var seasonalMultiplier = await _pricingService.GetSeasonalMultiplierAsync(DateTime.UtcNow);

            service.TotalCost = serviceCost * regionalMultiplier * propertyTypeMultiplier * seasonalMultiplier;
            proposal.Services.Add(service);
        }

        // Calculate total costs
        proposal.TotalCost = proposal.Services.Sum(s => s.TotalCost);
        proposal.LaborCost = proposal.Services.Sum(s => s.LaborCost);
        proposal.MaterialCost = proposal.Services.Sum(s => s.MaterialCost);

        return proposal;
    }

    public async Task<string> GenerateProposalDescriptionAsync(Proposal proposal)
    {
        var description = new System.Text.StringBuilder();
        
        description.AppendLine($"Landscaping Proposal for {proposal.PropertyType} Property");
        description.AppendLine($"Property Size: {proposal.PropertySize} square feet");
        description.AppendLine($"Region: {proposal.Region}");
        description.AppendLine($"Budget: ${proposal.Budget:F2}");
        description.AppendLine();
        description.AppendLine("Services Included:");
        
        foreach (var service in proposal.Services)
        {
            description.AppendLine($"- {service.Name}: ${service.TotalCost:F2}");
        }
        
        description.AppendLine();
        description.AppendLine($"Total Cost: ${proposal.TotalCost:F2}");
        description.AppendLine($"Labor Cost: ${proposal.LaborCost:F2}");
        description.AppendLine($"Material Cost: ${proposal.MaterialCost:F2}");
        description.AppendLine();
        description.AppendLine($"Proposal Valid Until: {proposal.ValidUntil:MM/dd/yyyy}");
        
        return description.ToString();
    }

    public async Task<List<Service>> SuggestServicesAsync(
        string propertyType,
        decimal propertySize,
        string region,
        decimal budget)
    {
        var availableServices = _serviceCatalog.GetValueOrDefault(propertyType, new List<Service>());
        var suggestedServices = new List<Service>();
        
        // Get regional and property type multipliers
        var regionalMultiplier = await _pricingService.GetRegionalPriceMultiplierAsync(region);
        var propertyTypeMultiplier = await _pricingService.GetPropertyTypeMultiplierAsync(propertyType);
        var seasonalMultiplier = await _pricingService.GetSeasonalMultiplierAsync(DateTime.UtcNow);
        
        // Calculate total multiplier
        var totalMultiplier = regionalMultiplier * propertyTypeMultiplier * seasonalMultiplier;
        
        // Filter services that fit within budget
        foreach (var service in availableServices)
        {
            var estimatedCost = (service.BasePrice + service.LaborCost + service.MaterialCost) * totalMultiplier;
            
            if (estimatedCost <= budget)
            {
                suggestedServices.Add(service);
            }
        }
        
        // Sort by estimated value (cost per square foot)
        return suggestedServices
            .OrderByDescending(s => (s.BasePrice + s.LaborCost + s.MaterialCost) / propertySize)
            .ToList();
    }
} 