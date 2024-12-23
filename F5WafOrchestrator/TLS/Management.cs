// Copyright 2024 Keyfactor
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

using System;

namespace Keyfactor.Extensions.Orchestrator.F5WafOrchestrator.TLS;

public class Management : Job, IManagementJobExtension
{
    ILogger _logger = LogHandler.GetClassLogger<Management>();

    public JobResult ProcessJob(ManagementJobConfiguration config)
    {
        _logger.LogDebug($"Begin {config.Capability} for job id {config.JobId}...");
        _logger.LogDebug($"Server: {config.CertificateStoreDetails.ClientMachine}");
        _logger.LogDebug($"Store Path: {config.CertificateStoreDetails.StorePath}");

        try
        {
            F5Client = new F5WafClient(config.CertificateStoreDetails.ClientMachine, config.ServerPassword);

            // check if the string starts with "tls-" and remove it if present.  This may occur
            //   if store was created in Command via a Discovery job
            if (config.CertificateStoreDetails.StorePath.StartsWith("tls-"))
            {
                config.CertificateStoreDetails.StorePath = config.CertificateStoreDetails.StorePath.Substring(4);
            }

            switch (config.OperationType)
            {
                case CertStoreOperationType.Add:
                    _logger.LogDebug($"BEGIN add Operation for {config.CertificateStoreDetails.StorePath} on {config.CertificateStoreDetails.ClientMachine}.");
                    PerformTlsCertAddition(config);
                    _logger.LogDebug($"END Add Operation for {config.CertificateStoreDetails.StorePath} on {config.CertificateStoreDetails.ClientMachine}.");
                    break;
                case CertStoreOperationType.Remove:
                    _logger.LogDebug($"BEGIN Delete Operation for {config.CertificateStoreDetails.StorePath} on {config.CertificateStoreDetails.ClientMachine}.");
                    PerformTlsCertRemove(config);
                    _logger.LogDebug($"END Delete Operation for {config.CertificateStoreDetails.StorePath} on {config.CertificateStoreDetails.ClientMachine}.");
                    break;
                default:
                    return new JobResult() { Result = OrchestratorJobStatusJobResult.Failure, JobHistoryId = config.JobHistoryId, FailureMessage = $"Site {config.CertificateStoreDetails.StorePath} on server {config.CertificateStoreDetails.ClientMachine}: Unsupported operation: {config.OperationType.ToString()}" };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception for {config.Capability}: {F5WAFException.FlattenExceptionMessages(ex, string.Empty)} for job id {config.JobId}");
            return new JobResult() { Result = OrchestratorJobStatusJobResult.Failure, JobHistoryId = config.JobHistoryId, FailureMessage = F5WAFException.FlattenExceptionMessages(ex, $"Site {config.CertificateStoreDetails.StorePath} on server {config.CertificateStoreDetails.ClientMachine}:") };
        }

        _logger.LogDebug($"...End {config.Capability} job for job id {config.JobId}");
        return new JobResult() { Result = OrchestratorJobStatusJobResult.Success, JobHistoryId = config.JobHistoryId };
    }

    private void PerformTlsCertRemove(ManagementJobConfiguration config)
    {
        _logger.MethodEntry(LogLevel.Debug);

        if (F5Client.JobCertIsAttachedToHttpLoadBalancer(config.CertificateStoreDetails.StorePath,
                config.JobCertificate.Alias))
        {
            throw new Exception(
                "The job cert is bound to an http load balancer. Must unbind before performing management job.");
        }

        F5Client.RemoveCaOrTlsCertificate(config.CertificateStoreDetails.StorePath, config.JobCertificate.Alias, true);

        _logger.MethodExit(LogLevel.Debug);
    }

    private void PerformTlsCertAddition(ManagementJobConfiguration config)
    {
        _logger.MethodEntry(LogLevel.Debug);

        bool certificateExists = F5Client.CertificateExistsInF5(config.CertificateStoreDetails.StorePath, config.JobCertificate.Alias, true);

        if (certificateExists && !config.Overwrite)
        {
            string message =
                $"Certificate with alias \"{config.JobCertificate.Alias}\" already exists in F5, and job was not configured to overwrite.";
            throw new Exception(message);
        }

        F5WafClient.PostRoot reqBody = F5Client.FormatTlsCertificateRequest(config.JobCertificate); 
        if (certificateExists && config.Overwrite)
        {
            _logger.LogDebug("Overwrite is enabled, replacing certificate in F5 called \"{0}\"",
                config.JobCertificate.Alias);
            F5Client.ReplaceCaOrTlsCertificate(config.CertificateStoreDetails.StorePath, reqBody, null, true);
        }
        else
        {
            _logger.LogDebug("Adding certificate to F5 Cloud");
            F5Client.AddCaOrTlsCertificate(config.CertificateStoreDetails.StorePath, reqBody, null, true);
        }

        _logger.MethodExit(LogLevel.Debug);
    }
}
