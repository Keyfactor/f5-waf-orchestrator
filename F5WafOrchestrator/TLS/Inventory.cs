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

namespace Keyfactor.Extensions.Orchestrator.F5WafOrchestrator.TLS;

[Job("Inventory")]
public class Inventory : Job<Inventory>, IInventoryJobExtension
{
    private readonly ILogger _logger = LogHandler.GetClassLogger<Inventory>();

    public JobResult ProcessJob(InventoryJobConfiguration config, SubmitInventoryUpdate cb)
    {
        _logger.LogDebug("Beginning F5 Distributed Cloud Inventory Job");
        
        var result = new JobResult
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

        List<CurrentInventoryItem> inventoryItems;
        string storePath = config.CertificateStoreDetails.StorePath;
        try
        {
            // check if the string starts with "ca-" and remove it if present
            if (config.CertificateStoreDetails.StorePath.StartsWith("tls-"))
            {
                storePath = config.CertificateStoreDetails.StorePath.Substring(4);  // Skip the first 3 characters ("ca-")
            }
            
            var (names, certs) = F5Client.TlsCertificateRetrievalProcess(storePath);
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
            _logger.LogError(ex, $"Error getting F5 Certificate from namespace {storePath}:\n" + ex.Message);
            result.FailureMessage = $"Error getting F5 Certificates from namespace {storePath}:\n" + ex.Message;
            return result;
        }
        
        cb.Invoke(inventoryItems);

        result.Result = OrchestratorJobStatusJobResult.Success;
        return result;
    }
}