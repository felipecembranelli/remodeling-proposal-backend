using System;
using System.Collections.Generic;

namespace RemodelingProposalSystem.Core.Models;

public class Material
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; // Cabinets, Countertops, Flooring, etc.
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal TotalCost { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty; // Square feet, linear feet, each, etc.
    public List<string> PropertyTypes { get; set; } = new(); // Residential, Commercial, Industrial
    public List<string> Regions { get; set; } = new();
    public decimal EstimatedDuration { get; set; } // Days for installation
    public string Category { get; set; } = string.Empty; // Flooring, Cabinetry, Plumbing, etc.
    public string Grade { get; set; } = string.Empty; // Economy, Standard, Premium, Luxury
    public string Brand { get; set; } = string.Empty;
    public bool IsEcoFriendly { get; set; } = false;
    public string Warranty { get; set; } = string.Empty;
} 