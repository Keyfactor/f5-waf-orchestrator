using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Keyfactor.Extensions.Orchestrator.F5CloudOrchestrator.Jobs;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace F5CloudOrchestrator.Tests;

public class F5Cloud
{
    public F5Cloud()
    {
        ConfigureLogging();
    }
    
    [Fact]
    public void F5CloudOrchestrator_Inventory_IntegrationTest_ReturnSuccess()
    {
        var config = new InventoryJobConfiguration
        {
            CertificateStoreDetails = new CertificateStore
            {
                ClientMachine = "keyfactor.console.ves.volterra.io",
                StorePath = "sgdemonamespace",
                StorePassword = "",
                Properties = "{\"ServerUsername\":\"aspen.smith@keyfactor.com\",\"ServerPassword\":\"Synchro1!keyfactor\",\"ServerUseSsl\":\"true\",\"Authorization\":\"8i/rSASoUEfu/0yru2nQ4ly7Tdk=\"}",
                Type = 0
            },
            ServerPassword = "8i/rSASoUEfu/0yru2nQ4ly7Tdk="
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
    public void F5CloudOrchestrator_ManagementAdd_IntegrationTest_ReturnSuccess()
    {
        byte[] pfxFileBytes = File.ReadAllBytes("C:\\Users\\asmith\\Downloads\\test1234.pfx");

        string base64EncodedPfx = Convert.ToBase64String(pfxFileBytes);

        var config = new ManagementJobConfiguration
        
        {
            CertificateStoreDetails = new CertificateStore
            {
                ClientMachine = "keyfactor.console.ves.volterra.io",
                StorePath = "sgdemonamespace",
                StorePassword = "",
                Properties =
                    "{\"ServerUsername\":\"aspen.smith@keyfactor.com\",\"ServerPassword\":\"Synchro1!keyfactor\",\"ServerUseSsl\":\"true\",\"Authorization\":\"8i/rSASoUEfu/0yru2nQ4ly7Tdk=\"}",
                Type = 0
            },
            ServerPassword = "8i/rSASoUEfu/0yru2nQ4ly7Tdk=",
            OperationType = CertStoreOperationType.Add,
            Overwrite = true,
            JobCertificate = new ManagementJobCertificate
            {
                Thumbprint = null,
                Contents = base64EncodedPfx,
                Alias = "commonname",
                PrivateKeyPassword = "jzYBiWjRBEtV"
            }
        };

        var management = new Management();

        var result = management.ProcessJob(config);
        
        // Assert
        Assert.Equal(OrchestratorJobStatusJobResult.Success, result.Result);
    }
    
    
    [Fact]
    public void F5CloudOrchestrator_ManagementRemove_IntegrationTest_ReturnSuccess()
    {
        byte[] pfxFileBytes = File.ReadAllBytes("C:\\Users\\asmith\\Downloads\\test1234.pfx");

        string base64EncodedPfx = Convert.ToBase64String(pfxFileBytes);

        var config = new ManagementJobConfiguration
        
        {
            CertificateStoreDetails = new CertificateStore
            {
                ClientMachine = "keyfactor.console.ves.volterra.io",
                StorePath = "sgdemonamespace",
                StorePassword = "",
                Properties =
                    "{\"ServerUsername\":\"aspen.smith@keyfactor.com\",\"ServerPassword\":\"Synchro1!keyfactor\",\"ServerUseSsl\":\"true\",\"Authorization\":\"8i/rSASoUEfu/0yru2nQ4ly7Tdk=\"}",
                Type = 0
            },
            ServerPassword = "8i/rSASoUEfu/0yru2nQ4ly7Tdk=",
            OperationType = CertStoreOperationType.Remove,
            Overwrite = false,
            JobCertificate = new ManagementJobCertificate
            {
                Thumbprint = null,
                Contents = base64EncodedPfx,
                Alias = "test1234",
                PrivateKeyPassword = "jzYBiWjRBEtV"
            }
        };

        var management = new Management();

        var result = management.ProcessJob(config);
        
        // Assert
        Assert.Equal(OrchestratorJobStatusJobResult.Success, result.Result);
    }
    
    [Fact]
    public void F5CloudOrchestrator_Discovery_IntegrationTest_ReturnSuccess()
    {
        var config = new DiscoveryJobConfiguration
        {
            JobCancelled = false,
            ServerError = null,
            JobHistoryId = 0,
            RequestStatus = 0,
            ServerUsername = "aspen.smith@keyfactor.com",
            ServerPassword = "{\"Authorization\":\"8i/rSASoUEfu/0yru2nQ4ly7Tdk=\"}",
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