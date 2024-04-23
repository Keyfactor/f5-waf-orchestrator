using System.Security.Cryptography.X509Certificates;
using Keyfactor.Extensions.Orchestrator.F5WafOrchestrator.TLS;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Org.BouncyCastle.Pkcs;

namespace F5WafOrchestrator.Tests;

public class F5WafTls
{
    public F5WafTls()
    {
        ConfigureLogging();
    }
    
    [Fact]
    public void F5WafOrchestrator_TLS_Inventory_IntegrationTest_ReturnSuccess()
    {
        var config = new InventoryJobConfiguration
        {
            CertificateStoreDetails = new CertificateStore
            {
                ClientMachine = "keyfactor.console.ves.volterra.io",
                StorePath = "tls-sgdemonamespace",
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
    public void F5WafOrchestrator_TLS_ManagementAdd_IntegrationTest_ReturnSuccess()
    {
        string pfxBase64 = "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";
        string password = "H7zn4Y5cWbTQ";

        var config = new ManagementJobConfiguration
        
        {
            CertificateStoreDetails = new CertificateStore
            {
                ClientMachine = "keyfactor.console.ves.volterra.io",
                StorePath = "tls-sgdemonamespace",
                StorePassword = "",
                Type = 0
            },
            ServerPassword = "Q8pcZGz89EqhcUfzWsuFVE/L0Ps=",
            OperationType = CertStoreOperationType.Add,
            Overwrite = true,
            JobCertificate = new ManagementJobCertificate
            {
                Thumbprint = null,
                Contents = pfxBase64,
                Alias = "unittest333",
                PrivateKeyPassword = password
            }
        };
        
        var management = new Management();
        
        var result = management.ProcessJob(config);
        
        // Assert
        Assert.Equal(OrchestratorJobStatusJobResult.Success, result.Result);
    }
    
    
    [Fact]
    public void F5WafOrchestrator_TLS_ManagementRemove_IntegrationTest_ReturnSuccess()
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
                Type = 0
            },
            ServerPassword = "Q8pcZGz89EqhcUfzWsuFVE/L0Ps=",
            OperationType = CertStoreOperationType.Remove,
            Overwrite = false,
            JobCertificate = new ManagementJobCertificate
            {
                Thumbprint = null,
                Contents = base64EncodedPfx,
                Alias = "test111",
                PrivateKeyPassword = "jzYBiWjRBEtV"
            }
        };

        var management = new Management();

        var result = management.ProcessJob(config);
        
        // Assert
        Assert.Equal(OrchestratorJobStatusJobResult.Success, result.Result);
    }
    
    [Fact]
    public void F5WafOrchestrator_TLS_Discovery_IntegrationTest_ReturnSuccess()
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
    
    // [Fact]
    // public void F5WafOrchestrator_TLS_ManagementAddChain_IntegrationTest_ReturnSuccess()
    // {
    //     byte[] pemFileBytes = File.ReadAllBytes("C:\\Users\\asmith\\Downloads\\EJBCAEnroll_xunit.pem");
    //
    //     string base64EncodedPem = Convert.ToBase64String(pemFileBytes);
    //
    //     var config = new ManagementJobConfiguration
    //     
    //     {
    //         CertificateStoreDetails = new CertificateStore
    //         {
    //             ClientMachine = "keyfactor.console.ves.volterra.io",
    //             StorePath = "sgdemonamespace",
    //             StorePassword = "",
    //             Properties =
    //                 "{\"ServerUsername\":\"aspen.smith@keyfactor.com\",\"ServerPassword\":\"8i/rSASoUEfu/0yru2nQ4ly7Tdk=\",\"ServerUseSsl\":\"true\"}",
    //             Type = 0
    //         },
    //         ServerPassword = "Q8pcZGz89EqhcUfzWsuFVE/L0Ps=",
    //         OperationType = CertStoreOperationType.Add,
    //         Overwrite = true,
    //         JobCertificate = new ManagementJobCertificate
    //         {
    //             Thumbprint = null,
    //             Contents = base64EncodedPem,
    //             Alias = "unittest12",
    //             PrivateKeyPassword = null
    //         }
    //     };
    //
    //     var management = new Management();
    //
    //     var result = management.ProcessJob(config);
    //     
    //     // Assert
    //     Assert.Equal(OrchestratorJobStatusJobResult.Success, result.Result);
    // }

    // [Fact]
    // public void F5WafOrchestrator_GetChain_ManagementJobReturnTest_ReturnTrue()
    // {
    //     bool result = true;
    //     // PFX enrolled ManagementJobCertificate.RawData contents
    //     string base64EncodedPfx = File.ReadAllText("C:\\Users\\asmith\\OneDrive - Keyfactor\\Desktop\\ManagementJobCertificateContentsRawData.txt");
    //     string password = "6fuErUrnnQxM";
    //     
    //     // convert the base64 string to a byte array
    //     byte[] pfxData = Convert.FromBase64String(base64EncodedPfx);
    //     
    //     // load the PFX certificate contents 
    //     var certificate = new X509Certificate2(pfxData, password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
    //
    //     // init a certificate chain
    //     var chain = new X509Chain();
    //
    //     // disable chain policy checks for the sake of testing
    //     chain.ChainPolicy = new X509ChainPolicy
    //     {
    //         RevocationMode = X509RevocationMode.NoCheck, // Disable revocation check
    //         VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority // Allow certificates from unknown authorities
    //     };
    //
    //     // build chain from the certificate contents
    //     bool chainBuilt = chain.Build(certificate);
    //     if (!chainBuilt)
    //     { 
    //         Console.WriteLine($"Error creating chain.");
    //         result = false;
    //     }
    //
    //     int count = 0;
    //     // enumerate and print chain elements
    //     Console.WriteLine("Certificate Chain:");
    //     foreach (X509ChainElement element in chain.ChainElements)
    //     {
    //         count += 1;
    //     }
    //
    //     if (count > 1)
    //     {
    //         result = true;
    //     }
    //     
    //     Assert.True(result);
    // }

}
