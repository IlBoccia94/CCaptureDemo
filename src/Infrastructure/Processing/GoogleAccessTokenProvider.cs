using Google.Apis.Auth.OAuth2;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Processing;

public interface IGoogleAccessTokenProvider
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}

public sealed class GoogleAccessTokenProvider(IOptions<GoogleCloudOptions> options) : IGoogleAccessTokenProvider
{
    private readonly GoogleCloudOptions _options = options.Value;

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var credential = GoogleCredential
            .FromFile(_options.ServiceAccountJsonPath)
            .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

        return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);
    }
}
