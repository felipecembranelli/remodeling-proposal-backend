using System;
using System.Collections.Generic;

namespace RemodelingProposalSystem.Core.Models;

public class Service
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; // Kitchen Remodel, Bathroom Renovation, etc.
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal MaterialCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal TotalCost { get; set; }
    public List<string> PropertyTypes { get; set; } = new(); // Residential, Commercial, Industrial
    public List<string> Regions { get; set; } = new();
    public List<Material> RequiredMaterials { get; set; } = new();
    public string UnitOfMeasure { get; set; } = string.Empty; // Square feet, linear feet, each, etc.
    public decimal EstimatedDuration { get; set; } // Days
    public string Category { get; set; } = string.Empty; // Kitchen, Bathroom, Flooring, etc.
    public List<string> RequiredSkills { get; set; } = new(); // Plumbing, Electrical, Carpentry, etc.
    public bool RequiresPermit { get; set; } = false;
    public string ComplexityLevel { get; set; } = string.Empty; // Basic, Intermediate, Advanced
} 