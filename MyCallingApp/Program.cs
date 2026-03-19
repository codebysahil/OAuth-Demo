using Microsoft.Identity.Client;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

// ── Step 1: Read secret from Key Vault ──────────────
Console.WriteLine("Reading secret from Key Vault...");

var keyVaultUri = new Uri("https://kv99023-oauth882-demo23.vault.azure.net/");
var credential = new DefaultAzureCredential();
var secretClient = new SecretClient(keyVaultUri, credential);

var secret = await secretClient.GetSecretAsync("CallingAppSecret");
var clientSecret = secret.Value.Value;

Console.WriteLine("Secret retrieved from Key Vault successfully");

// ── Config ──────────────────────────────────────────
var tenantId     = "c624c89c-71b0-4610-acfa-17114c1c0f47";
var clientId     = "929cb10c-a570-438a-9a2e-206b695b493f";
var scope        = "api://42b4fcc2-26a2-4815-a2b5-b0a4b8f9cffc/.default";
var apiUrl       = "https://my-api-oauth-demo.azurewebsites.net/weatherforecast";

// ── Step 2: Request token from Entra ID ─────────────
Console.WriteLine("\nRequesting token from Entra ID...");

var app = ConfidentialClientApplicationBuilder
    .Create(clientId)
    .WithClientSecret(clientSecret)
    .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
    .Build();

var result = await app.AcquireTokenForClient(new[] { scope })
                      .ExecuteAsync();

Console.WriteLine($"Token received! Expires: {result.ExpiresOn}");

#if DEBUG
Console.WriteLine($"Token preview: {result.AccessToken[..20]}...");
#endif

// ── Step 3: Call the protected API ──────────────────
Console.WriteLine("\nCalling protected API...");

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

var response = await httpClient.GetAsync(apiUrl);

Console.WriteLine($"API Response: {response.StatusCode}");
var body = await response.Content.ReadAsStringAsync();
Console.WriteLine($"Data: {body}");


