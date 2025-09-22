using RemodelingProposalSystem.Core.Models;

namespace RemodelingProposalSystem.Core.Interfaces.Repositories;

public interface IMaterialRepository
{
    Task<IEnumerable<Material>> GetAllAsync();
    Task<Material?> GetByIdAsync(string id);
    Task<Material?> GetByNameAsync(string name);
    Task<Material> AddAsync(Material material);
    Task<Material> UpdateAsync(Material material);
    Task DeleteAsync(string id);
    Task<IEnumerable<Material>> GetMaterialsByServicesAsync(List<string> requestedServices);
}