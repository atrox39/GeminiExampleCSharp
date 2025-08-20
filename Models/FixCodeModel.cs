using Newtonsoft.Json;

namespace GeminiExample.Models
{
  public class FixCodeModel
  {
    [JsonProperty("code", Required = Required.Always)]
    public string Code { get; set; } = null!;
    [JsonProperty("language", Required = Required.Always)]
    public string Language { get; set; } = null!;
    [JsonProperty("fixType", Required = Required.Always)]
    public string FixType { get; set; } = null!;
    [JsonProperty("fixDescription", Required = Required.Always)]
    public string FixDescription { get; set; } = null!;
  }
}
