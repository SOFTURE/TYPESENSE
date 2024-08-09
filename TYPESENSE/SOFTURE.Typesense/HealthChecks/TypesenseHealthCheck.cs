using System.Net;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using SOFTURE.Common.HealthCheck.Core;
using SOFTURE.Typesense.Settings;

namespace SOFTURE.Typesense.HealthChecks;

internal class TypesenseHealthCheck(HttpClient httpClient, IOptions<TypesenseSettings> settings) : CheckBase
{
    protected override async Task<Result> Check()
    {
        var typesenseSettings = settings.Value;
        
        var uri = new UriBuilder(typesenseSettings.Host) { Path = "/" }.Uri;
        
        var response = await httpClient.GetAsync(uri);
        
        return response.StatusCode == HttpStatusCode.NotFound 
            ? Result.Success() 
            : Result.Failure("Internal API is not available");
    }
}