using System.Security.Cryptography.X509Certificates;
using Keyfactor.Extensions.Orchestrator.F5WafOrchestrator.CA;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace F5WafOrchestrator.Tests;

public class F5WafCa
{
    public F5WafCa()
    {
        ConfigureLogging();
    }
    
    [Fact]
    public void F5WafOrchestrator_CA_Inventory_IntegrationTest_ReturnSuccess()
    {
        var config = new InventoryJobConfiguration
        {
            CertificateStoreDetails = new CertificateStore
            {
                ClientMachine = "keyfactor.console.ves.volterra.io",
                StorePath = "ca-sgdemonamespace",
                StorePassword = "",
                Type = 0
            },
            ServerPassword = "Q8pcZGz89EqhcUfzWsuFVE/L0Ps="
        };

        var inventory = new Inventory();

        var result = inventory.ProcessJob(config, (inventoryItems) =>
        {
            // Assert
            Assert.NotEmpty(inventoryItems);
            return true;
        });
        
        // Assert
        Assert.Equal(OrchestratorJobStatusJobResult.Success, result.Result);
    }
    
    static void ConfigureLogging()
    {
        var config = new NLog.Config.LoggingConfiguration();

        // Targets where to log to: File and Console
        var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
        logconsole.Layout = @"${date:format=HH\:mm\:ss} ${logger} [${level}] - ${message}";

        // Rules for mapping loggers to targets            
        config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logconsole);

        // Apply config           
        NLog.LogManager.Configuration = config;

        LogHandler.Factory = LoggerFactory.Create(builder =>
        {
            builder.AddNLog();
        });
    }

    [Fact]
    public void F5WafOrchestrator_CA_ManagementAdd_IntegrationTest_ReturnSuccess()
    {
        string base64EncodedPfx = "MIIDeTCCAmGgAwIBAgIQSVQNM9+tTo9Dd52qg4MI1DANBgkqhkiG9w0BAQsFADBPMRMwEQYKCZImiZPyLGQBGRYDbGFiMRkwFwYKCZImiZPyLGQBGRYJa2V5ZmFjdG9yMR0wGwYDVQQDExRrZXlmYWN0b3ItS0ZUUkFJTi1DQTAeFw0xOTA1MTAwMzMyMzJaFw0yNDA1MTAwMzQyMzFaME8xEzARBgoJkiaJk/IsZAEZFgNsYWIxGTAXBgoJkiaJk/IsZAEZFglrZXlmYWN0b3IxHTAbBgNVBAMTFGtleWZhY3Rvci1LRlRSQUlOLUNBMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmqN1+RED9SuRsLnIF4AB7uFkaismnxhGXc9LWAVBPc8bt8McchMlHmJqVN1DPR0ZT8tVT8jqIODBULrcWZVo6ox15BTrFqzrFUiIuuq16NDW+WYu2rljoMBaOTegkmWs7ZoME+w/MHqFFqPBBvg7uDSZW/w+1VKyn7aRA2Bywy6o5UHpladsokVKwNhyMQvfJnJQ2xJio8mhXV1AM15FCp8hQZ8dXj/cAPKQxk31M1thIP7M8yx779QbxIs6PKLNxarmY+D73r8Q3t8scO+GVQUwSvbDZiF+kzpl/5YTkeD6gLqfQsQr86YiK5nV5xCb2PL8KwnmMCocVImX2fm3vQIDAQABo1EwTzALBgNVHQ8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAdBgNVHQ4EFgQUcBUzPW7ZQuqUMP3RFTCbDU1hTGUwEAYJKwYBBAGCNxUBBAMCAQAwDQYJKoZIhvcNAQELBQADggEBAIYye4+Gd8piML1BXzkMNgt6aNOu7hS4h3sYfojtpV40OdJ64/Pt9pC5NecMt8B0ikiZvfu9c+xO20VB3uFDGNWVLqfoaZi+cvMAYH9gMrK8KiNe21jekbG1uTuIPZ0oJtEDnn7aJ+rXzVTEe6QHZ/gjVcZoPy1/rdCnzMRdH0NS6xpn0HqWpy/IxjnJP0Ux6ZPNzrEmhsUGruVJwF8u5+FTlD9pF55eHqI4COtEqJ8YEMb25s8xCCJVL0al+LbydR0neG4Ic/zA0QEwB7ixFsuytaBUOXv4QVpsu7R4mtWQHdSoJz3I+g117tHDlJfGEoQpsc/gHBwMptPQCobpI30=";
    
        var config = new ManagementJobConfiguration
        
        {
            CertificateStoreDetails = new CertificateStore
            {
                ClientMachine = "keyfactor.console.ves.volterra.io",
                StorePath = "sgdemonamespace",
                StorePassword = "",
                Type = 0
            },
            ServerPassword = "Q8pcZGz89EqhcUfzWsuFVE/L0Ps=",
            OperationType = CertStoreOperationType.Add,
            Overwrite = true,
            JobCertificate = new ManagementJobCertificate
            {
                Thumbprint = null,
                Contents = base64EncodedPfx,
                Alias = "test1001",
                PrivateKeyPassword = null
            }
        };
    
        var management = new Management();
    
        var result = management.ProcessJob(config);
        
        // Assert
        Assert.Equal(OrchestratorJobStatusJobResult.Success, result.Result);
    }
    
    
    [Fact]
    public void F5WafOrchestrator_CA_ManagementRemove_IntegrationTest_ReturnSuccess()
    {
        var config = new ManagementJobConfiguration
        
        {
            CertificateStoreDetails = new CertificateStore
            {
                ClientMachine = "keyfactor.console.ves.volterra.io",
                StorePath = "sgdemonamespace",
                StorePassword = "",
                Type = 0
            },
            ServerPassword = "Q8pcZGz89EqhcUfzWsuFVE/L0Ps=",
            OperationType = CertStoreOperationType.Remove,
            Overwrite = false,
            JobCertificate = new ManagementJobCertificate
            {
                Thumbprint = null,
                Contents = "",
                Alias = "teeeesssttt555"
            }
        };
    
        var management = new Management();
    
        var result = management.ProcessJob(config);
        
        // Assert
        Assert.Equal(OrchestratorJobStatusJobResult.Success, result.Result);
    }
    
    [Fact]
    public void F5WafOrchestrator_CA_Discovery_IntegrationTest_ReturnSuccess()
    {
        var config = new DiscoveryJobConfiguration
        {
            JobCancelled = false,
            ServerError = null,
            JobHistoryId = 0,
            RequestStatus = 0,
            ServerUsername = "aspen.smith@keyfactor.com",
            ServerPassword = "Q8pcZGz89EqhcUfzWsuFVE/L0Ps=",
            UseSSL = false,
            JobProperties = null,
            JobTypeId = default,
            JobId = default,
            Capability = null,
            ClientMachine = "keyfactor.console.ves.volterra.io"
        };
    
        var discovery = new Discovery();
        
        var result = discovery.ProcessJob(config, (discoveryItems) =>
        {
            // Assert
            Assert.NotEmpty(discoveryItems);
            return true;
        });
        
        // Assert
        Assert.Equal(OrchestratorJobStatusJobResult.Success, result.Result);
    }
}
