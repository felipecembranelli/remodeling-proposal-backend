using RemodelingProposalSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemodelingProposalSystem.Core.Interfaces.Services
{
    public interface IProposalService
    {
        Task<IEnumerable<Proposal>> GetAllProposalsAsync();
        Task<Proposal> GetProposalByIdAsync(Guid id);
        Task<Proposal> AddProposalAsync(Proposal rawProposal);
        Task<Proposal> UpdateProposalAsync(Proposal rawProposal);
        Task DeleteRawProposalAsync(Guid id);

        Task<Proposal> GenerateProposalAsync(
            string propertyType,
            decimal propertySize,
            string region,
            decimal budget,
            List<string> requestedServices,
            string clientName,
            string clientPhone,
            string clientEmail,
            string siteAnalysis);
    }
}
