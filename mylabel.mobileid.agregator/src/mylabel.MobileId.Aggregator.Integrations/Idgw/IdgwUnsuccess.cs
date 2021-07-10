using System.Text.Json.Serialization;

namespace mylabel.MobileId.Aggregator.Integrations.Idgw
{
	public record IdgwUnsuccess(
		[property: JsonPropertyName("error")] string Error,
		[property: JsonPropertyName("error_description")] string? ErrorDescription);
}