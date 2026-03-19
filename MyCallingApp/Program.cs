using Microsoft.Identity.Client;

// ── Config ──────────────────────────────────────────
var tenantId   = "c624c89c-71b0-4610-acfa-17114c1c0f47";
var clientId   = "929cb10c-a570-438a-9a2e-206b695b493f";
var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET") 
                   ?? throw new Exception("CLIENT_SECRET not set");
var scope      = "api://42b4fcc2-26a2-4815-a2b5-b0a4b8f9cffc/.default";
var apiUrl = "https://my-api-oauth-demo.azurewebsites.net/weatherforecast";


// ── Step 1: Request a token from Entra ID ───────────
Console.WriteLine("Requesting token from Entra ID...");

var app = ConfidentialClientApplicationBuilder
    .Create(clientId)
    .WithClientSecret(clientSecret)
    .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
    .Build();

var result = await app.AcquireTokenForClient(new[] { scope })
                      .ExecuteAsync();



// ── Step 2: Call the protected API ──────────────────
Console.WriteLine("\nCalling protected API...");

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

var response = await httpClient.GetAsync(apiUrl);

Console.WriteLine($"API Response: {response.StatusCode}");
var body = await response.Content.ReadAsStringAsync();
Console.WriteLine($"Data: {body}");