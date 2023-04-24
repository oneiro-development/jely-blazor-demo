namespace JelyUI;

using Microsoft.Identity.Client;

class AzureAuthHandler : DelegatingHandler {

    readonly IConfidentialClientApplication _app;
    readonly ILogger<AzureAuthHandler> _logger;

    public AzureAuthHandler(IConfidentialClientApplication app, ILogger<AzureAuthHandler> logger) =>
        (_app, _logger) = (app, logger);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        var result = await _app.AcquireTokenForClient(new[] { $"api://{_app.AppConfig.ClientId}/.default" })
            .ExecuteAsync(cancellationToken);

        _logger.LogInformation("Got token with type {type}:\n{token}", result.TokenType, result.AccessToken);
        request.Headers.Authorization = new("Bearer", result.AccessToken);


        return await base.SendAsync(request, cancellationToken);
    }
}