using System;
using System.Collections.Generic;
using System.Linq;
using RemodelingProposalSystem.Core.LLM;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RemodelingProposalSystem.Infrastructure.LLM
{
    public interface ILLMServiceFactory
    {
        ILLMService GetService(string modelName = null);
        IEnumerable<string> GetAvailableModels();
    }

    public class LLMServiceFactory : ILLMServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LLMServiceFactory> _logger;
        private readonly Dictionary<string, Type> _serviceTypes;

        public LLMServiceFactory(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILogger<LLMServiceFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;

            _serviceTypes = new Dictionary<string, Type>
            {
                { "gpt-4", typeof(OpenAIService) },
                { "gpt-3.5-turbo", typeof(OpenAIService) },
                { "gemma-7b", typeof(GemmaService) },
                { "gemma-2b", typeof(GemmaService) },
                { "llama-2-7b", typeof(LlamaService) },
                { "llama-2-13b", typeof(LlamaService) },
                { "llama-2-70b", typeof(LlamaService) }
            };
        }

        public ILLMService GetService(string modelName = null)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                modelName = _configuration["DefaultLLMModel"] ?? "gpt-4";
            }

            if (!_serviceTypes.TryGetValue(modelName, out var serviceType))
            {
                throw new ArgumentException($"Unsupported model: {modelName}");
            }

            try
            {
                var service = (ILLMService)_serviceProvider.GetRequiredService(serviceType);
                if (!service.IsAvailable())
                {
                    throw new InvalidOperationException($"Service for model {modelName} is not available");
                }
                return service;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service for model {ModelName}", modelName);
                throw;
            }
        }

        public IEnumerable<string> GetAvailableModels()
        {
            return _serviceTypes.Keys.Where(model =>
            {
                try
                {
                    var service = GetService(model);
                    return service.IsAvailable();
                }
                catch
                {
                    return false;
                }
            });
        }
    }
} 