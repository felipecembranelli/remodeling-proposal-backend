using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemodelingProposalSystem.Core.Models
{
    //[Table("RawProposals")]
    public class Proposal
    {
        public Guid Id { get; set; }
        public string PropertyType { get; set; } = string.Empty; // Residential, Commercial, Industrial
        public decimal PropertySize { get; set; } // Square footage
        public string Region { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public List<string> RequestedServices { get; set; } = new(); // Kitchen remodel, bathroom renovation, etc.
        public string ProposalRawBody { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string Status { get; set; } = "Draft";
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string? SiteAnalysis { get; set; } = string.Empty; // Current condition assessment
        public string? ProjectScope { get; set; } = string.Empty; // Detailed scope of work
        public decimal? EstimatedDuration { get; set; } // Days/weeks
        public List<string?> RequiredPermits { get; set; } = new(); // Building permits needed
    }
}
