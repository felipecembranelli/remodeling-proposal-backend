namespace RemodelingProposalSystem.Core.Models;

public class MaterialBasePricing
{
    public string Id { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
} 