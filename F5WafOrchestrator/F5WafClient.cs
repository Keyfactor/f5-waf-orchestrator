using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Keyfactor.Orchestrators.Extensions;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Pkcs;


namespace Keyfactor.Extensions.Orchestrator.F5WafOrchestrator.Client;

public class F5WafClient
{
    public class Spec
    {
        public string? certificate_url { get; set; }
    }
    
    public class RootObject
    {
        public Spec? spec { get; set; }
    }
    
    public struct PostRoot
    {
        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }
    
        [JsonPropertyName("spec")]
        public PostSpec Spec { get; set; }
    }

    public struct Metadata
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("disable")]
        public bool Disable { get; set; }
    }

    public struct PostSpec
    {
        [JsonPropertyName("certificate_url")]
        public string certificate_url { get; set; }

        [JsonPropertyName("private_key")]
        public PrivateKey PrivateKey { get; set; }

        [JsonPropertyName("disable_ocsp_stapling")]
        public DisableOcspStapling DisableOcspStapling { get; set; }
    }

    public struct PrivateKey
    {
        [JsonPropertyName("clear_secret_info")]
        public ClearSecretInfo ClearSecretInfo { get; set; }
    }

    public struct ClearSecretInfo
    {
        [JsonPropertyName("url")]
        public string Location { get; set; }
    }
    
    public struct DisableOcspStapling
    {
    }
    
    public struct PostChainRoot
    {
        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }
    
        [JsonPropertyName("spec")]
        public PostChainSpec Spec { get; set; }
    }
    
    public struct PostChainSpec
    {
        [JsonPropertyName("certificate_url")]
        public string certificate_url { get; set; }
    }
    
    public struct CaPostRoot
    {
        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }
    
        [JsonPropertyName("spec")]
        public CaSpec Spec { get; set; }
    }
    
    public class CaSpec
    {
        public string? trusted_ca_url { get; set; }
    }
    
    public class CaRootObject
    {
        public CaSpec? spec { get; set; }
    }

    public F5WafClient(string hostname, string apiToken)
    {
        Log = LogHandler.GetClassLogger<F5WafClient>();
        Log.LogDebug("Initializing F5 Distributed Cloud client");
        
        var f5ClientHandler = new HttpClientHandler();
        
        F5Client = new HttpClient(f5ClientHandler);
        F5Client.BaseAddress = new Uri("https://" + hostname);
        
        var auth = $"APIToken {apiToken}";
        F5Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", auth);
    }

    private ILogger Log { get; }
    private HttpClient F5Client { get; }

    public string GetTlsCertificatesFromF5(string f5Namespace)
    {
        var response = F5Client.GetAsync($"/api/config/namespaces/{f5Namespace}/certificates");
        response.Wait();
        var stringResponse = response.Result.Content.ReadAsStringAsync();
        stringResponse.Wait();
        
        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            throw new Exception(stringResponse.ToString());
        }
        
        return stringResponse.Result;
    }
    
    public string GetCaCertificatesFromF5(string f5Namespace)
    {
        var response = F5Client.GetAsync($"/api/config/namespaces/{f5Namespace}/trusted_ca_lists");
        response.Wait();
        var stringResponse = response.Result.Content.ReadAsStringAsync();
        stringResponse.Wait();
        
        // parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            throw new Exception(stringResponse.ToString());
        }
        
        return stringResponse.Result;
    }
    
    public string GetHttpLoadBalancersFromF5(string f5Namespace)
    {
        var response = F5Client.GetAsync($"/api/config/namespaces/{f5Namespace}/http_loadbalancers");
        response.Wait();
        var stringResponse = response.Result.Content.ReadAsStringAsync();
        stringResponse.Wait();
        
        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            throw new Exception(stringResponse.ToString());
        }
        
        return stringResponse.Result;
    }
    
    public string GetHttpLoadBalancerFromF5(string f5Namespace, string certAlias)
    {
        var response = F5Client.GetAsync($"/api/config/namespaces/{f5Namespace}/http_loadbalancers/{certAlias}?response_format=GET_RSP_FORMAT_DEFAULT");
        response.Wait();
        var stringResponse = response.Result.Content.ReadAsStringAsync();
        stringResponse.Wait();
        
        // parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            throw new Exception(stringResponse.ToString());
        }
        
        return stringResponse.Result;
    }
    
    public string? GetTlsCertificateContentsFromF5(string f5Namespace, string certName)
    {
        var response = F5Client.GetAsync($"/api/config/namespaces/{f5Namespace}/certificates/{certName}?response_format=GET_RSP_FORMAT_DEFAULT");
        response.Wait();
        var resp = response.Result.Content.ReadAsStringAsync();
        resp.Wait();

        // parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            throw new Exception($"Error retrieving F5 certificate contents: {resp}");
        }
        
        RootObject rootObject = JsonSerializer.Deserialize<RootObject>(resp.Result)
                                 ?? throw new InvalidOperationException("Deserialized RootObject is null.");
        
        return rootObject.spec?.certificate_url;
    }
    
    public string? GetCaCertificateContentsFromF5(string f5Namespace, string certName)
    {
        var response = F5Client.GetAsync($"/api/config/namespaces/{f5Namespace}/trusted_ca_lists/{certName}?response_format=GET_RSP_FORMAT_DEFAULT");
        response.Wait();
        var resp = response.Result.Content.ReadAsStringAsync();
        resp.Wait();

        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            throw new Exception($"Error retrieving F5 certificate contents: {resp}");
        }
        
        CaRootObject rootObject = JsonSerializer.Deserialize<CaRootObject>(resp.Result)
                                ?? throw new InvalidOperationException("Deserialized RootObject is null.");
        
        return rootObject.spec?.trusted_ca_url;
    }
    
    public (IEnumerable<string>, IEnumerable<string>) TlsCertificateRetrievalProcess(string f5Namespace)
    {
        List<string> encodedCerts = new List<string>();
        List<string> certNames = new List<string>();
        try
        {
            var certsJson = GetTlsCertificatesFromF5(f5Namespace);
            var certs = JsonDocument.Parse(certsJson);
            
            var items = certs.RootElement.GetProperty("items").EnumerateArray();
            
            // Iterate through each cert in "items" JSON object
            foreach (var item in items)
            {
                if (item.TryGetProperty("name", out JsonElement nameElement))
                {
                    string? name = nameElement.GetString();
                    if (name != null)
                    {
                        string? certString = GetTlsCertificateContentsFromF5(f5Namespace, name);
                        if (certString?.StartsWith("string:///") ?? false)
                        {
                            // remove the "string:///" part
                            string encodedCert = certString.Substring("string:///".Length);
                            byte[] decodedBytes = Convert.FromBase64String(encodedCert);
                            string decodedCert = Encoding.UTF8.GetString(decodedBytes);
                            encodedCerts.Add(decodedCert);
                            certNames.Add(name);
                        }
                        else
                        {
                            Log.LogDebug($"No certificates found in F5.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.LogDebug($"Error retrieving F5 certificates: {ex.Message}");
            throw new Exception($"Error retrieving F5 certificates: {ex.Message}");
        }
        
        return (certNames, encodedCerts);
    }
    
    public (IEnumerable<string>, IEnumerable<string>) CaCertificateRetrievalProcess(string f5Namespace)
    {
        List<string> encodedCerts = new List<string>();
        List<string> certNames = new List<string>();
        try
        {
            var certsJson = GetCaCertificatesFromF5(f5Namespace);
            var certs = JsonDocument.Parse(certsJson);
            
            var items = certs.RootElement.GetProperty("items").EnumerateArray();
            
            // ierate through each cert in "items" JSON object
            foreach (var item in items)
            {
                if (item.TryGetProperty("name", out JsonElement nameElement))
                {
                    string? name = nameElement.GetString();
                    if (name != null)
                    {
                        string? certString = GetCaCertificateContentsFromF5(f5Namespace, name);
                        if (certString?.StartsWith("string:///") ?? false)
                        {
                            // remove the "string:///" part
                            string encodedCert = certString.Substring("string:///".Length);
                            byte[] decodedBytes = Convert.FromBase64String(encodedCert);
                            string decodedCert = Encoding.UTF8.GetString(decodedBytes);
                            encodedCerts.Add(decodedCert);
                            certNames.Add(name);
                        }
                        else
                        {
                            Log.LogDebug($"No certificates found in F5.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.LogDebug($"Error retrieving F5 certificates: {ex.Message}");
            throw new Exception($"Error retrieving F5 certificates: {ex.Message}");
        }
        
        return (certNames, encodedCerts);
    }

    static string ConvertCertToPemFormat(string base64EncodedCertificate)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("-----BEGIN CERTIFICATE-----\n");

        // split base64 string into 64-character lines
        for (int i = 0; i < base64EncodedCertificate.Length; i += 64)
        {
            int lineLength = Math.Min(64, base64EncodedCertificate.Length - i);
            string line = base64EncodedCertificate.Substring(i, lineLength);
            builder.Append(line + "\n"); 
        }

        builder.Append("-----END CERTIFICATE-----\n");

        return builder.ToString();
    }
    
    static string ConvertKeyToPemFormat(string base64EncodedCertificate)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("-----BEGIN RSA PRIVATE KEY-----\n");

        // split base64 string into 64-character lines
        for (int i = 0; i < base64EncodedCertificate.Length; i += 64)
        {
            int lineLength = Math.Min(64, base64EncodedCertificate.Length - i);
            string line = base64EncodedCertificate.Substring(i, lineLength);
            builder.Append(line + "\n"); 
        }

        builder.Append("-----END RSA PRIVATE KEY-----\n");

        return builder.ToString();
    }

    public string ExtractEndEntityandCertChain(string pfxData, string password)
    {
        string endEntityandChain = "";
        
        byte[] pfxBytes = Convert.FromBase64String(pfxData);

        Pkcs12StoreBuilder storeBuilder = new Pkcs12StoreBuilder();
        Pkcs12Store store = storeBuilder.Build();
        store.Load(new MemoryStream(pfxBytes), password.ToCharArray());

        foreach (string alias in store.Aliases)
        {
            if (store.IsKeyEntry(alias))
            {
                X509CertificateEntry[] chain = store.GetCertificateChain(alias);
                if (chain == null)
                {
                    throw new Exception("No certificate chain found or no key entry exists.");
                }
                string[] pemCertificates = new string[chain.Length];
                for (int i = 0; i < chain.Length; i++)
                {
                    pemCertificates[i] = ConvertCertToPemFormat(Convert.ToBase64String(chain[i].Certificate.GetEncoded()));
                    endEntityandChain += pemCertificates[i];
                }
            }
        }
        return endEntityandChain;
    }

    public void AddTlsCertificate(string f5Namespace, PostRoot reqBody)
    {
        var jsonReqBody = JsonSerializer.Serialize(reqBody);
        var stringReqBody = new StringContent(jsonReqBody, Encoding.UTF8, "application/json");

        var response = F5Client.PostAsync($"/api/config/namespaces/{f5Namespace}/certificates", stringReqBody);
        response.Wait();

        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            var errorMessage = response.Result.Content.ReadAsStringAsync();
            errorMessage.Wait();
            throw new Exception(errorMessage.ToString());
        }
    }
    
    public void AddCaCertificate(string f5Namespace, CaPostRoot reqBody)
    {
        var jsonReqBody = JsonSerializer.Serialize(reqBody);
        var stringReqBody = new StringContent(jsonReqBody, Encoding.UTF8, "application/json");

        var response = F5Client.PostAsync($"/api/config/namespaces/{f5Namespace}/trusted_ca_lists", stringReqBody);
        response.Wait();

        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            var errorMessage = response.Result.Content.ReadAsStringAsync();
            errorMessage.Wait();
            throw new Exception(errorMessage.ToString());
        }
    }

    public PostRoot FormatTlsCertificateRequest(ManagementJobCertificate mgmtJobCert)
    {
        X509Certificate2 certX509;
        try
        {
            // Load the certificate
            certX509 = new X509Certificate2(Convert.FromBase64String(mgmtJobCert.Contents), mgmtJobCert.PrivateKeyPassword,
                X509KeyStorageFlags.Exportable);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load certificate from contents.", ex);
        }
        
        string encodedCertUtf8Base64;
        try
        {
            // extract end entity and cert chain and encode in UTF-8 Base64
            string endEntityandChain = ExtractEndEntityandCertChain(mgmtJobCert.Contents, mgmtJobCert.PrivateKeyPassword);
            encodedCertUtf8Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(endEntityandChain));
        } catch (Exception ex)
        {
            throw new InvalidOperationException("Failed during certificate formatting or encoding.", ex);
        }

        // format private key
        string encodedPrivateKeyUtf8Base64 = "";
        if (certX509.HasPrivateKey)
        {
            RSA? rsa = certX509.GetRSAPrivateKey();
            if (rsa != null)
            {
                try
                {
                    // export the RSA key to PKCS#1 format and encode in UTF-8 Base64
                    byte[] privateKeyPkcs1Bytes = rsa.ExportRSAPrivateKey();
                    string privateKeyPkcs1Base64 = Convert.ToBase64String(privateKeyPkcs1Bytes);
                    string privateKeyPem = ConvertKeyToPemFormat(privateKeyPkcs1Base64);
                    encodedPrivateKeyUtf8Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKeyPem));
                } catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed during private key export or encoding.", ex);
                }
            }
        }
        
        var reqBody = new PostRoot
        {
            Metadata = new Metadata
            {
                Name = mgmtJobCert.Alias,
                Disable = false
            },
            Spec = new PostSpec
            {
                certificate_url = "string:///" + encodedCertUtf8Base64,
                PrivateKey = new PrivateKey
                {
                    ClearSecretInfo = new ClearSecretInfo
                    {
                        Location = "string:///" + encodedPrivateKeyUtf8Base64
                    }
                },
                DisableOcspStapling = new DisableOcspStapling()
            }
        };
        return reqBody;
    }
    
    public CaPostRoot FormatCaCertificateRequest(ManagementJobCertificate mgmtJobCert)
    {
        X509Certificate2 certX509;
        try
        {
            // Load the certificate
            certX509 = new X509Certificate2(Convert.FromBase64String(mgmtJobCert.Contents));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load certificate from contents.", ex);
        }
        
        string encodedCertUtf8Base64;
        try
        {
            // Convert cert to PEM format and encode in UTF-8 Base64
            string certBase64String = Convert.ToBase64String(certX509.RawData);
            string certPem = ConvertCertToPemFormat(certBase64String);
            encodedCertUtf8Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(certPem));
        } catch (Exception ex)
        {
            throw new InvalidOperationException("Failed during certificate formatting or encoding.", ex);
        }

        var reqBody = new CaPostRoot
        {
            Metadata = new Metadata
            {
                Name = mgmtJobCert.Alias,
                Disable = false
            },
            Spec = new CaSpec
            {
                trusted_ca_url = "string:///" + encodedCertUtf8Base64
            }
        };
        return reqBody;
    }

    public void RemoveTlsCertificate(string f5Namespace, string certName)
    {
        var response = F5Client.DeleteAsync($"/api/config/namespaces/{f5Namespace}/certificates/{certName}");
        response.Wait();
        var stringResponse = response.Result.Content.ReadAsStringAsync();
        stringResponse.Wait();
        
        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            var errorMessage = response.Result.Content.ReadAsStringAsync();
            errorMessage.Wait();
            throw new Exception(errorMessage.ToString());
        }
    }
    
    public void RemoveCaCertificate(string f5Namespace, string certName)
    {
        var response = F5Client.DeleteAsync($"/api/config/namespaces/{f5Namespace}/trusted_ca_lists/{certName}");
        response.Wait();
        var stringResponse = response.Result.Content.ReadAsStringAsync();
        stringResponse.Wait();
        
        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            var errorMessage = response.Result.Content.ReadAsStringAsync();
            errorMessage.Wait();
            throw new Exception(errorMessage.ToString());
        }
    }
    
    public bool CertificateExistsInF5(string f5Namespace, string alias)
    {
        var certsJson = GetTlsCertificatesFromF5(f5Namespace);
        var certs = JsonDocument.Parse(certsJson);
        
        // Iterate through the names of the cert items and return true if a name matching the alias exists
        return certs.RootElement
            .GetProperty("items")
            .EnumerateArray()
            .Any(item => item.TryGetProperty("name", out JsonElement nameElement) && 
                         nameElement.GetString() == alias);
    }
    
    public void ReplaceTlsCertificate(string f5Namespace, PostRoot reqBody)
    {
        var jsonReqBody = JsonSerializer.Serialize(reqBody);
        var stringReqBody = new StringContent(jsonReqBody, Encoding.UTF8, "application/json");

        var response = F5Client.PutAsync($"/api/config/namespaces/{f5Namespace}/certificates/{reqBody.Metadata.Name}", stringReqBody);
        response.Wait();

        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            var errorMessage = response.Result.Content.ReadAsStringAsync();
            errorMessage.Wait();
            throw new Exception(errorMessage.ToString());
        }
    }
    
    public void ReplaceCaCertificateInF5(string f5Namespace, CaPostRoot reqBody)
    {
        var jsonReqBody = JsonSerializer.Serialize(reqBody);
        var stringReqBody = new StringContent(jsonReqBody, Encoding.UTF8, "application/json");

        var response = F5Client.PutAsync($"/api/config/namespaces/{f5Namespace}/trusted_ca_lists/{reqBody.Metadata.Name}", stringReqBody);
        response.Wait();

        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            var errorMessage = response.Result.Content.ReadAsStringAsync();
            errorMessage.Wait();
            throw new Exception(errorMessage.ToString());
        }
    }
    
    public string GetNamespaces()
    {
        var response = F5Client.GetAsync($"/api/web/namespaces");
        response.Wait();
        var resp = response.Result.Content.ReadAsStringAsync();
        resp.Wait();

        //parse status code for error handling
        string statusCode = string.Empty;
        string[] respMessage = response.Result.ToString().Split(',');
        for (int i = 0; i < respMessage.Length; i++)
        {
            if (respMessage[i].Contains("StatusCode:"))
            {
                statusCode = respMessage[i].Trim().Substring("StatsCode: ".Length).Trim();
                break;
            }
        }
            
        if (statusCode != "200")
        {
            throw new Exception($"Error retrieving F5 certificate contents: {resp}");
        }
        
        return resp.Result;
    }
    
    public List<string> DiscoverNamespacesforCaStoreType()
    {
        List<string> namespacesList = new List<string>();
        try
        {
            var namespacesJson = GetNamespaces();
            var namespaces = JsonDocument.Parse(namespacesJson);
            
            var items = namespaces.RootElement.GetProperty("items").EnumerateArray();
            
            // Iterate through each cert in "items" JSON object
            foreach (var item in items)
            {
                if (item.TryGetProperty("name", out JsonElement nameElement))
                {
                    string? name = nameElement.GetString();
                    if (name != null)
                    {
                        namespacesList.Add("ca-" + name);
                    }
                    else
                    {
                        Log.LogDebug($"No namespaces found in F5.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.LogDebug($"Error retrieving F5 namespaces: {ex.Message}");
            throw new Exception($"Error retrieving F5 namespaces: {ex.Message}");
        }
        
        return namespacesList;
    }
    
    public List<string> DiscoverNamespacesforTlsStoreType()
    {
        List<string> namespacesList = new List<string>();
        try
        {
            var namespacesJson = GetNamespaces();
            var namespaces = JsonDocument.Parse(namespacesJson);
            
            var items = namespaces.RootElement.GetProperty("items").EnumerateArray();
            
            // Iterate through each cert in "items" JSON object
            foreach (var item in items)
            {
                if (item.TryGetProperty("name", out JsonElement nameElement))
                {
                    string? name = nameElement.GetString();
                    if (name != null)
                    {
                        namespacesList.Add("tls-" + name);
                    }
                    else
                    {
                        Log.LogDebug($"No namespaces found in F5.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.LogDebug($"Error retrieving F5 namespaces: {ex.Message}");
            throw new Exception($"Error retrieving F5 namespaces: {ex.Message}");
        }
        
        return namespacesList;
    }
    
    public bool JobCertIsAttachedToHttpLoadBalancer(string f5Namespace, string jobCertName)
    {
        var certsJson = GetHttpLoadBalancersFromF5(f5Namespace);
        var certs = JsonDocument.Parse(certsJson);
        var items = certs.RootElement.GetProperty("items").EnumerateArray();
            
        // iterate through each cert in "items" JSON object
        foreach (var item in items)
        {
            if (item.TryGetProperty("name", out JsonElement nameElement))
            {
                string? name = nameElement.GetString();
                if (name != null)
                {
                    string lbJson = GetHttpLoadBalancerFromF5(f5Namespace, name);
                    
                    JObject jsonResponse = JObject.Parse(lbJson);

                    // navigate to the 'https' object and then to 'tls_cert_params' -> 'certificates'
                    JToken? httpsSection = jsonResponse["spec"]?["https"]?["tls_cert_params"]?["certificates"];

                    if (httpsSection != null && httpsSection.Any())
                    {
                        foreach (JToken certificateToken in httpsSection)
                        {
                            JObject? certificate = certificateToken as JObject;

                            // check to see if job cert name matches certs tied to load balancer
                            if (certificate != null)
                            {
                                string certificateName = certificate["name"].ToString();
                                if (certificateName == jobCertName)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
}