using RemodelingProposalSystem.Core.Interfaces;
using RemodelingProposalSystem.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;

namespace RemodelingProposalSystem.Infrastructure.Services;

public class MaterialService : IMaterialService
{
    private readonly IConfiguration _configuration;

    public MaterialService(IConfiguration configuration)
    {
        _configuration = configuration;
        
    }

    public Task<List<Material>> GetMaterialsByServicesAsync(List<Service> requestedServices)
    {
        throw new NotImplementedException();
    }
} 