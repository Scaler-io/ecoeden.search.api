using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecoeden.Search.Api.Swagger;

public class SwaggerRemoveVersionFromRoute : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var modifiedPaths = new OpenApiPaths();
        foreach(var path in swaggerDoc.Paths)
        {
            string pathWithoutVersions = path.Key.Remove(0, 7);
            if (string.IsNullOrEmpty(pathWithoutVersions))
            {
                pathWithoutVersions = "/";
            }
            modifiedPaths.Add(pathWithoutVersions, path.Value);
        }

        swaggerDoc.Paths = modifiedPaths;
    }
}
