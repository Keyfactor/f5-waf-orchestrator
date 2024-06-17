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
using System.Net;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Xml.Linq;

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

    private string HostName { get; set; }

    private HttpClient F5Client { get; }

    private ILogger _logger { get; }

    internal F5WafClient(string hostname, string apiToken)
    {
        _logger = LogHandler.GetClassLogger<F5WafClient>();
        _logger.MethodEntry(LogLevel.Debug);
        _logger.LogDebug("Initializing F5 Distributed Cloud client");

        HostName = "https://" + hostname;
        
        F5Client = new HttpClient(new HttpClientHandler());
        F5Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", $"APIToken {apiToken}");

        _logger.MethodExit(LogLevel.Debug);
    }

    internal string GetTlsCertificatesFromF5(string f5Namespace)
    {
        _logger.MethodEntry(LogLevel.Debug);

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{HostName}/api/config/namespaces/{f5Namespace}/certificates");
        string result = SubmitRequest(request);

        _logger.MethodExit(LogLevel.Debug);

        return result;
    }
    
    internal string GetCaCertificatesFromF5(string f5Namespace)
    {
        _logger.MethodEntry(LogLevel.Debug);

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{HostName}/api/config/namespaces/{f5Namespace}/trusted_ca_lists");
        string result = SubmitRequest(request);

        _logger.MethodExit(LogLevel.Debug);

        return result;
    }
    
    internal string GetHttpLoadBalancersFromF5(string f5Namespace)
    {
        _logger.MethodEntry(LogLevel.Debug);

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{HostName}/api/config/namespaces/{f5Namespace}/http_loadbalancers");
        string result = SubmitRequest(request);

        _logger.MethodExit(LogLevel.Debug);

        return result;
    }
    
    internal string GetHttpLoadBalancerFromF5(string f5Namespace, string certAlias)
    {
        _logger.MethodEntry(LogLevel.Debug);

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{HostName}/api/config/namespaces/{f5Namespace}/http_loadbalancers/{certAlias}?response_format=GET_RSP_FORMAT_DEFAULT");
        string result = SubmitRequest(request);

        _logger.MethodExit(LogLevel.Debug);

        return result;
    }
    
    internal string? GetTlsCertificateContentsFromF5(string f5Namespace, string certName)
    {
        _logger.MethodEntry(LogLevel.Debug);

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{HostName}/api/config/namespaces/{f5Namespace}/certificates/{certName}?response_format=GET_RSP_FORMAT_DEFAULT");
        string result = SubmitRequest(request);

        RootObject rootObject = JsonSerializer.Deserialize<RootObject>(result)
                                 ?? throw new InvalidOperationException("Deserialized RootObject is null.");

        _logger.MethodExit(LogLevel.Debug);

        return rootObject.spec?.certificate_url;
    }
    
    internal string? GetCaCertificateContentsFromF5(string f5Namespace, string certName)
    {
        _logger.MethodEntry(LogLevel.Debug);

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{HostName}/api/config/namespaces/{f5Namespace}/trusted_ca_lists/{certName}?response_format=GET_RSP_FORMAT_DEFAULT");
        string result = SubmitRequest(request);

        CaRootObject rootObject = JsonSerializer.Deserialize<CaRootObject>(result)
                                ?? throw new InvalidOperationException("Deserialized RootObject is null.");

        _logger.MethodExit(LogLevel.Debug);

        return rootObject.spec?.trusted_ca_url;
    }

    internal string GetNamespaces()
    {
        _logger.MethodEntry(LogLevel.Debug);

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{HostName}/api/web/namespaces");
        string result = SubmitRequest(request);

        _logger.MethodExit(LogLevel.Debug);

        return result;
    }

    internal (IEnumerable<string>, IEnumerable<string>) TlsCertificateRetrievalProcess(string f5Namespace)
    {
        _logger.MethodEntry(LogLevel.Debug);

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
                            _logger.LogDebug($"No certificates found in F5.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug($"Error retrieving F5 certificates: {ex.Message}");
            throw new F5WAFException($"Error retrieving F5 certificates: {ex.Message}");
        }

        _logger.MethodExit(LogLevel.Debug);

        return (certNames, encodedCerts);
    }
    
    internal (IEnumerable<string>, IEnumerable<string>) CaCertificateRetrievalProcess(string f5Namespace)
    {
        _logger.MethodEntry(LogLevel.Debug);

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
                            _logger.LogDebug($"No certificates found in F5.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug($"Error retrieving F5 certificates: {ex.Message}");
            throw new F5WAFException($"Error retrieving F5 certificates: {ex.Message}");
        }

        _logger.MethodExit(LogLevel.Debug);

        return (certNames, encodedCerts);
    }

    internal void AddCaOrTlsCertificate(string f5Namespace, PostRoot? tlsReqBody, CaPostRoot? caReqBody, bool isTLSCertificate)
    {
        _logger.MethodEntry(LogLevel.Debug);

        string certType = "trusted_ca_lists";
        var jsonReqBody = JsonSerializer.Serialize(caReqBody);

        if (isTLSCertificate)
        {
            certType = "certificates";
            jsonReqBody = JsonSerializer.Serialize(tlsReqBody);
        }

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{HostName}/api/config/namespaces/{f5Namespace}/{certType}");
        
        var stringReqBody = new StringContent(jsonReqBody, Encoding.UTF8, "application/json");
        request.Content = stringReqBody;
        
        SubmitRequest(request);

        _logger.MethodExit(LogLevel.Debug);
    }

    internal PostRoot FormatTlsCertificateRequest(ManagementJobCertificate mgmtJobCert)
    {
        _logger.MethodEntry(LogLevel.Debug);

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

        _logger.MethodExit(LogLevel.Debug);

        return reqBody;
    }
    
    internal CaPostRoot FormatCaCertificateRequest(ManagementJobCertificate mgmtJobCert)
    {
        _logger.MethodEntry(LogLevel.Debug);

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

        _logger.MethodExit(LogLevel.Debug);

        return reqBody;
    }

    internal void RemoveCaOrTlsCertificate(string f5Namespace, string certName, bool isTLSCertificate)
    {
        _logger.MethodEntry(LogLevel.Debug);

        string certType = isTLSCertificate ? "certificates" : "trusted_ca_lists";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{HostName}/api/config/namespaces/{f5Namespace}/{certType}/{certName}");
        string result = SubmitRequest(request);

        _logger.MethodExit(LogLevel.Debug);
    }

    internal bool CertificateExistsInF5(string f5Namespace, string alias, bool isTLSCertificate)
    {
        _logger.MethodEntry(LogLevel.Debug);

        var certsJson = isTLSCertificate ? GetTlsCertificatesFromF5(f5Namespace) : GetCaCertificatesFromF5(f5Namespace);
        var certs = JsonDocument.Parse(certsJson);

        _logger.MethodExit(LogLevel.Debug);

        // Iterate through the names of the cert items and return true if a name matching the alias exists
        return certs.RootElement
            .GetProperty("items")
            .EnumerateArray()
            .Any(item => item.TryGetProperty("name", out JsonElement nameElement) && 
                         nameElement.GetString() == alias);
    }
    
    internal void ReplaceCaOrTlsCertificate(string f5Namespace, PostRoot? tlsReqBody, CaPostRoot? caReqBody, bool isTLSCertificate)
    {
        _logger.MethodEntry(LogLevel.Debug);

        string certType = "trusted_ca_lists";
        string? metadataName = caReqBody?.Metadata.Name;
        var jsonReqBody = JsonSerializer.Serialize(caReqBody);

        if (isTLSCertificate)
        {
            certType = "certificates";
            metadataName = tlsReqBody?.Metadata.Name;
            jsonReqBody = JsonSerializer.Serialize(tlsReqBody);
        }

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{HostName}/api/config/namespaces/{f5Namespace}/{certType}/{metadataName}");

        var stringReqBody = new StringContent(jsonReqBody, Encoding.UTF8, "application/json");
        request.Content = stringReqBody;

        SubmitRequest(request);

        _logger.MethodExit(LogLevel.Debug);
    }
    
    internal List<string> DiscoverNamespacesforStoreType(string storeTypePrefix)
    {
        _logger.MethodEntry(LogLevel.Debug);

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
                        namespacesList.Add(storeTypePrefix + name);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug($"Error retrieving F5 namespaces: {ex.Message}");
            throw new F5WAFException($"Error retrieving F5 namespaces: {ex.Message}");
        }

        _logger.MethodExit(LogLevel.Debug);

        return namespacesList;
    }
    
    internal bool JobCertIsAttachedToHttpLoadBalancer(string f5Namespace, string jobCertName)
    {
        _logger.MethodEntry(LogLevel.Debug);

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
                                string? certificateName = certificate["name"]?.ToString();
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

        _logger.MethodExit(LogLevel.Debug);

        return false;
    }

    private string SubmitRequest(HttpRequestMessage request)
    {
        _logger.MethodEntry(LogLevel.Debug);
        
        var response = F5Client.SendAsync(request).Result;
        var result = response.Content.ReadAsStringAsync().Result;

        if (response.StatusCode != HttpStatusCode.OK &&
            response.StatusCode != HttpStatusCode.Accepted &&
            response.StatusCode != HttpStatusCode.Created &&
            response.StatusCode != HttpStatusCode.NoContent)
        {
            string errorMessage = $"Error calling {request.RequestUri}: {result}";
            _logger.LogError(errorMessage);
            _logger.MethodExit(LogLevel.Debug);
            throw new F5WAFException(errorMessage);
        }

        _logger.MethodExit(LogLevel.Debug);

        return result;
    }

    private string ExtractEndEntityandCertChain(string pfxData, string password)
    {
        _logger.MethodEntry(LogLevel.Debug);

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
                    throw new F5WAFException("No certificate chain found or no key entry exists.");
                }
                string[] pemCertificates = new string[chain.Length];
                for (int i = 0; i < chain.Length; i++)
                {
                    pemCertificates[i] = ConvertCertToPemFormat(Convert.ToBase64String(chain[i].Certificate.GetEncoded()));
                    endEntityandChain += pemCertificates[i];
                }
            }
        }

        _logger.MethodExit(LogLevel.Debug);

        return endEntityandChain;
    }

    private string ConvertCertToPemFormat(string base64EncodedCertificate)
    {
        _logger.MethodEntry(LogLevel.Debug);

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

        _logger.MethodExit(LogLevel.Debug);

        return builder.ToString();
    }

    private string ConvertKeyToPemFormat(string base64EncodedCertificate)
    {
        _logger.MethodEntry(LogLevel.Debug);

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

        _logger.MethodExit(LogLevel.Debug);

        return builder.ToString();
    }
}