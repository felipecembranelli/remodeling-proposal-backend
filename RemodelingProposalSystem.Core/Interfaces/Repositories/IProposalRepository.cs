using RemodelingProposalSystem.Core.Models;

namespace RemodelingProposalSystem.Core.Interfaces.Repositories;

public interface IProposalRepository
{
    Task<IEnumerable<Proposal>> GetAllAsync();
    Task<Proposal?> GetByIdAsync(string id);
    Task<Proposal> AddAsync(Proposal proposal);
    Task<Proposal> UpdateAsync(Proposal proposal);
    Task DeleteAsync(string id);
    
}