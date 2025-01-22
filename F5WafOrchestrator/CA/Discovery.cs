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

public class Discovery : Job, IDiscoveryJobExtension
{
    ILogger _logger = LogHandler.GetClassLogger<Discovery>();

    public JobResult ProcessJob(DiscoveryJobConfiguration config, SubmitDiscoveryUpdate cb)
    {
        _logger.LogDebug($"Begin {config.Capability} for job id {config.JobId}...");
        _logger.LogDebug($"Server: {config.ClientMachine}");

        List<string> namespaces;

        try
        {
            F5Client = new F5WafClient(config.ClientMachine, config.ServerPassword);

            namespaces = F5Client.DiscoverNamespacesforStoreType("ca-");
            _logger.LogDebug($"Found {namespaces.Count()} namespaces in {config.ClientMachine}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception for {config.Capability}: {F5WAFException.FlattenExceptionMessages(ex, string.Empty)} for job id {config.JobId}");
            return new JobResult() { Result = OrchestratorJobStatusJobResult.Failure, JobHistoryId = config.JobHistoryId, FailureMessage = F5WAFException.FlattenExceptionMessages(ex, $"Server {config.ClientMachine}:") };
        }

        cb.Invoke(namespaces);

        _logger.LogDebug($"...End {config.Capability} job for job id {config.JobId}");
        return new JobResult() { Result = OrchestratorJobStatusJobResult.Success, JobHistoryId = config.JobHistoryId };
    }
}