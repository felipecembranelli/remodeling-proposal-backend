namespace RemodelingProposalSystem.Core.Models;

public class PriceHistory
{
    public string Id { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public string Region { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public DateTime ChangeDate { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
} 