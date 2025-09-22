namespace RemodelingProposalSystem.Core.Models;

public class LaborPricing
{
    public string Id { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public decimal BaseRate { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
} 