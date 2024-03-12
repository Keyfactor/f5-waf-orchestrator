using Keyfactor.Extensions.Orchestrator.F5CloudOrchestrator.Client;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Orchestrator.F5CloudOrchestrator.Jobs;

[Job("Management")]
public class Discovery : F5CloudJob<Discovery>, IDiscoveryJobExtension
{
    ILogger _logger = LogHandler.GetClassLogger<Management>();
        
        public JobResult ProcessJob(DiscoveryJobConfiguration config, SubmitDiscoveryUpdate cb)
        {
            _logger.LogDebug("Beginning F5 Distributed Cloud Discovery Job");
        
            var result = new JobResult
            {
                Result = OrchestratorJobStatusJobResult.Failure,
                JobHistoryId = config.JobHistoryId
            };
        
            try
            {
                F5Client = new F5CloudClient(config.ClientMachine, config.ServerPassword);
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not connect to F5 Client" + ex.Message);
                return result;
            }
        
        
            List<string> namespaces;
            // CertificateStore certificateStore = new CertificateStore() { ClientMachine = config.ClientMachine };
            
            try
            {
                namespaces = F5Client.DiscoverNamespaces();
                _logger.LogDebug($"Found {namespaces.Count()} namespaces in {config.ClientMachine}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting F5 namespaces from {config.ClientMachine}:\n" + ex.Message);
                result.FailureMessage = $"Error getting F5 namespaces from {config.ClientMachine}:\n" + ex.Message;
                return result;
            }
            
            cb.Invoke(namespaces);

            result.Result = OrchestratorJobStatusJobResult.Success;
            return result;
        }
}