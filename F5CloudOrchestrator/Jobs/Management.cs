using Keyfactor.Extensions.Orchestrator.F5CloudOrchestrator.Client;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Orchestrator.F5CloudOrchestrator.Jobs;

[Job("Management")]
public class Management : F5CloudJob<Management>, IManagementJobExtension
{
    ILogger _logger = LogHandler.GetClassLogger<Management>();
        
        public JobResult ProcessJob(ManagementJobConfiguration config)
        {
            _logger.LogDebug("Beginning F5 Cloud Management Job");

            JobResult result = new JobResult
            {
                Result = OrchestratorJobStatusJobResult.Failure,
                JobHistoryId = config.JobHistoryId
            };
            
            try
            {
                F5Client = new F5CloudClient(config.CertificateStoreDetails.ClientMachine, config.ServerPassword);
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not connect to F5 Client" + ex.Message);
                return result;
            }

            try
            {
                switch (config.OperationType)
                {
                    case CertStoreOperationType.Add:
                        _logger.LogDebug("Adding certificate to F5 Cloud");
                        
                        PerformAddition(config, result);
                        
                        _logger.LogDebug("Add operation complete.");
                        
                        result.Result = OrchestratorJobStatusJobResult.Success;
                        break;
                    case CertStoreOperationType.Remove:
                        _logger.LogDebug("Removing certificate from F5 Cloud");
                        
                        PerformRemove(config, result);
                        
                        _logger.LogDebug("Remove operation complete.");
                        
                        result.Result = OrchestratorJobStatusJobResult.Success;
                        break;
                    default:
                        _logger.LogDebug("Invalid management operation type: {0}", config.OperationType);
                        throw new ArgumentOutOfRangeException();
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing job:\n {0}", ex.Message);
                result.FailureMessage = ex.Message;
            }

            return result;
        }

        private void PerformAddition(ManagementJobConfiguration config, JobResult result)
        {
            // Ensure that the certificate is in PKCS#12 format
            if (string.IsNullOrWhiteSpace(config.JobCertificate.PrivateKeyPassword))
            {
                throw new Exception("Certificate must be in PKCS#12 format.");
            }
            // Ensure that an alias is provided
            if (string.IsNullOrWhiteSpace(config.JobCertificate.Alias))
            {
                throw new Exception("Certificate alias is required.");
            }
            
            if (F5Client.CertificateExistsInF5(config.CertificateStoreDetails.StorePath, config.JobCertificate.Alias) && !config.Overwrite)
            {
                string message =
                    $"Certificate with alias \"{config.JobCertificate.Alias}\" already exists in F5, and job was not configured to overwrite.";
                _logger.LogDebug(message);
                throw new Exception(message);
            }
            
            try
            {
                F5CloudClient.PostRoot reqBody = F5Client.FormatCertificateRequest(config.JobCertificate);
                if (F5Client.CertificateExistsInF5(config.CertificateStoreDetails.StorePath, config.JobCertificate.Alias) &&
                    config.Overwrite)
                {
                    _logger.LogDebug("Overwrite is enabled, replacing certificate in F5 called \"{0}\"",
                        config.JobCertificate.Alias);
                    F5Client.ReplaceCertificateInF5(config.CertificateStoreDetails.StorePath, reqBody);
                }
                else
                {
                    _logger.LogDebug("Adding certificate to F5 Cloud");
                    F5Client.CreateCertificateInF5(config.CertificateStoreDetails.StorePath, reqBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"An error occured while adding {config.JobCertificate.Alias} to {config.CertificateStoreDetails.StorePath}: " + ex.Message);
                result.FailureMessage = $"An error occured while adding {config.JobCertificate.Alias} to {config.CertificateStoreDetails.StorePath}: " + ex.Message;
            }
        }

        private void PerformRemove(ManagementJobConfiguration config, JobResult result)
        {
            try
            {
                F5Client.RemoveCertificate(config.CertificateStoreDetails.StorePath, config.JobCertificate.Alias);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"An error occured while removing {config.JobCertificate.Alias} to {config.CertificateStoreDetails.StorePath}: " + ex.Message);
                result.FailureMessage = $"An error occured while removing {config.JobCertificate.Alias} to {config.CertificateStoreDetails.StorePath}: " + ex.Message;
            }
        }
}
