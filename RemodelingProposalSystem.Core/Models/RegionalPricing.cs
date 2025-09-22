namespace RemodelingProposalSystem.Core.Models;

public class RegionalPricing
{
    public string Id { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public decimal Multiplier { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
} 