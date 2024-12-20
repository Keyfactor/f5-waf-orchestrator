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
using System.Collections.Generic;
using System.Linq;

namespace Keyfactor.Extensions.Orchestrator.F5WafOrchestrator.CA;

public class Inventory : Job, IInventoryJobExtension
{
    private readonly ILogger _logger = LogHandler.GetClassLogger<Inventory>();

    public JobResult ProcessJob(InventoryJobConfiguration config, SubmitInventoryUpdate cb)
    {
        _logger.LogDebug($"Begin {config.Capability} for job id {config.JobId}...");
        _logger.LogDebug($"Server: {config.CertificateStoreDetails.ClientMachine}");
        _logger.LogDebug($"Store Path: {config.CertificateStoreDetails.StorePath}");

        List<CurrentInventoryItem> inventoryItems;

        try
        {
            F5Client = new F5WafClient(config.CertificateStoreDetails.ClientMachine, config.ServerPassword);
        
            string storePath = config.CertificateStoreDetails.StorePath;

            // check if the string starts with "ca-" and remove it if present.  This may occur
            //   if store was created in Command via a Discovery job
            if (config.CertificateStoreDetails.StorePath.StartsWith("ca-"))
            {
                storePath = config.CertificateStoreDetails.StorePath.Substring(3);  // Skip the first 3 characters ("ca-")
            }
            
            var (names, certs) = F5Client.CaCertificateRetrievalProcess(storePath);
            inventoryItems = certs.Zip(names, (certificate, name) => new CurrentInventoryItem
            {
                Alias = name,
                Certificates = new List<string> { certificate },
                PrivateKeyEntry = false,
                UseChainLevel = false
            }).ToList();
            _logger.LogDebug($"Found {inventoryItems.Count} certificates in namespace {storePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception for {config.Capability}: {F5WAFException.FlattenExceptionMessages(ex, string.Empty)} for job id {config.JobId}");
            return new JobResult() { Result = OrchestratorJobStatusJobResult.Failure, JobHistoryId = config.JobHistoryId, FailureMessage = F5WAFException.FlattenExceptionMessages(ex, $"Site {config.CertificateStoreDetails.StorePath} on server {config.CertificateStoreDetails.ClientMachine}:") };
        }

        try
        {
            cb.Invoke(inventoryItems);
            _logger.LogDebug($"...End {config.Capability} job for job id {config.JobId}");
            return new JobResult() { Result = OrchestratorJobStatusJobResult.Success, JobHistoryId = config.JobHistoryId };
        }
        catch (Exception ex)
        {
            string errorMessage = F5WAFException.FlattenExceptionMessages(ex, string.Empty);
            _logger.LogError($"Exception returning certificates for {config.Capability}: {errorMessage} for job id {config.JobId}");
            return new JobResult() { Result = OrchestratorJobStatusJobResult.Failure, JobHistoryId = config.JobHistoryId, FailureMessage = F5WAFException.FlattenExceptionMessages(ex, $"Site {config.CertificateStoreDetails.StorePath} on server {config.CertificateStoreDetails.ClientMachine}:") };
        }
    }
}