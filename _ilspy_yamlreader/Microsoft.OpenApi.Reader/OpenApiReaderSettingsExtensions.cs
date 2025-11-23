using Microsoft.OpenApi.YamlReader;

namespace Microsoft.OpenApi.Reader;

public static class OpenApiReaderSettingsExtensions
{
	public static void AddYamlReader(this OpenApiReaderSettings settings)
	{
		OpenApiYamlReader openApiYamlReader = new OpenApiYamlReader();
		settings.TryAddReader("yaml", (IOpenApiReader)(object)openApiYamlReader);
		settings.TryAddReader("yml", (IOpenApiReader)(object)openApiYamlReader);
	}
}
