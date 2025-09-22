using RemodelingProposalSystem.Core.Models;
//using RemodelingProposalSystem.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemodelingProposalSystem.Core.LLM
{
    public interface ILLMService
    {
        Task<Proposal> GenerateProposalAsync(
            string clientName,
            string propertyType,
            decimal propertySize,
            string region,
            decimal budget,
            List<string> requestedServices,
            string siteAnalysis);
        string GetModelName();
        bool IsAvailable();
    }
} 