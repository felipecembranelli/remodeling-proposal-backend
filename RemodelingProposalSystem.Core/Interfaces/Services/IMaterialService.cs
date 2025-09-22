using RemodelingProposalSystem.Core.Models;

namespace RemodelingProposalSystem.Core.Interfaces;

public interface IMaterialService
{
    Task<List<Material>> GetMaterialsByServicesAsync(List<Service> requestedServices);
} 