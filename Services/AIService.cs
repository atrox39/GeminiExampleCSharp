using Mscc.GenerativeAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using dotenv.net;
using Newtonsoft.Json.Schema.Generation;

namespace GeminiExample.Services
{
  public interface IAIService
  {
    Task<T?> GenerateSchemaResponse<T>(string prompt);
  }
  public class AIService : IAIService
  {
    private readonly GoogleAI _googleAI;
    private readonly GenerativeModel _generativeModel;

    public AIService()
    {
      DotEnv.Load(options: new DotEnvOptions(envFilePaths: [".env"], probeForEnv: true));
      string API_KEY = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
      if (string.IsNullOrEmpty(API_KEY))
      {
        throw new InvalidOperationException("API key is null or empty. Please set the GOOGLE_API_KEY environment variable.");
      }
      _googleAI = new GoogleAI(apiKey: API_KEY);
      _generativeModel = _googleAI.GenerativeModel(model: Model.Gemini20Flash);
    }

    public async Task<T?> GenerateSchemaResponse<T>(string prompt)
    {
      if (string.IsNullOrEmpty(prompt))
      {
        throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));
      }
      JSchemaGenerator generator = new();
      JSchema schema = generator.Generate(typeof(T));
      Console.WriteLine(schema.ToString());
      var response = await _generativeModel.GenerateContent(prompt, generationConfig: new GenerationConfig
      {
        Temperature = 0.7f,
        ResponseMimeType = "application/json",
        ResponseSchema = schema.ToString(),
      });
      if (response is not null && response.Text is not null)
      {
        try
        {
          var json = response.Text;
          if (string.IsNullOrEmpty(json))
          {
            throw new InvalidOperationException("Response text is null or empty.");
          }
          return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException("Deserialized object is null.");
        }
        catch (JsonException ex)
        {
          throw new InvalidOperationException("Failed to deserialize response text.", ex);
        }
      }
      return default;
    }
  }
}
