namespace RemodelingProposalSystem.Core.Models;

public class SeasonalPricing
{
    public string Id { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public decimal Multiplier { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
} 