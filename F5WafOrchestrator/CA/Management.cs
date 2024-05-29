// Copyright 2023 Keyfactor
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Keyfactor.Extensions.Orchestrator.F5WafOrchestrator.Client;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;

namespace Keyfactor.Extensions.Orchestrator.F5WafOrchestrator.CA;

public class Management : Job, IManagementJobExtension
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
                F5Client = new F5WafClient(config.CertificateStoreDetails.ClientMachine, config.ServerPassword);
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not connect to F5 Client" + ex.Message);
                return result;
            }

            try
            {
                // check if the string starts with "ca-" and remove it if present
                if (config.CertificateStoreDetails.StorePath.StartsWith("ca-"))
                {
                    config.CertificateStoreDetails.StorePath = config.CertificateStoreDetails.StorePath.Substring(3);  
                }
                
                switch (config.OperationType)
                {
                    case CertStoreOperationType.Add:
                        _logger.LogDebug("Adding certificate to F5 Cloud");
                        
                        PerformCaCertAddition(config, result);

                        _logger.LogDebug("Add operation complete.");
                        
                        result.Result = OrchestratorJobStatusJobResult.Success;
                        break;
                    case CertStoreOperationType.Remove:
                        _logger.LogDebug("Removing certificate from F5 Cloud");
                        
                        F5Client.RemoveCaCertificate(config.CertificateStoreDetails.StorePath, config.JobCertificate.Alias);

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
                return result;
            }

            return result;
        }
        
        private void PerformCaCertAddition(ManagementJobConfiguration config, JobResult result)
        {
            // ensure that an alias is provided
            if (string.IsNullOrWhiteSpace(config.JobCertificate.Alias))
            {
                throw new Exception("Certificate alias is required.");
            }
            // ensure that an alias is provided
            if (!string.IsNullOrWhiteSpace(config.JobCertificate.PrivateKeyPassword))
            {
                throw new Exception("Certificate passed is not a CA.");
            }
            
            if (F5Client.CertificateExistsInF5(config.CertificateStoreDetails.StorePath, config.JobCertificate.Alias) && !config.Overwrite)
            {
                string message =
                    $"Certificate with alias \"{config.JobCertificate.Alias}\" already exists in F5, and job was not configured to overwrite.";
                _logger.LogDebug(message);
                throw new Exception(message);
            }
            
            F5WafClient.CaPostRoot reqBody = F5Client.FormatCaCertificateRequest(config.JobCertificate);
            if (F5Client.CertificateExistsInF5(config.CertificateStoreDetails.StorePath, config.JobCertificate.Alias) &&
                config.Overwrite)
            {
                _logger.LogDebug("Overwrite is enabled, replacing certificate in F5 called \"{0}\"",
                    config.JobCertificate.Alias);
                F5Client.ReplaceCaCertificateInF5(config.CertificateStoreDetails.StorePath, reqBody);
            }
            else
            {
                _logger.LogDebug("Adding certificate to F5 Cloud");
                F5Client.AddCaCertificate(config.CertificateStoreDetails.StorePath, reqBody);
            }
        }
}
