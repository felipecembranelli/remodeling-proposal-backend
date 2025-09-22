using RemodelingProposalSystem.Core.Models;

namespace RemodelingProposalSystem.Core.Interfaces.Repositories;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAllAsync();
    Task<Service?> GetByIdAsync(string id);
    Task<Service?> GetByNameAsync(string name);
    Task<Service> AddAsync(Service service);
    Task UpdateAsync(Service service);
    Task DeleteAsync(string id);
    Task<IEnumerable<Service>> GetRequestedServicesDetailsAsync(List<string> requestedServices);
}