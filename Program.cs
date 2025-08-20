using GeminiExample.Models;
using GeminiExample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IAIService, AIService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapPost("/api/fix_code", async (CodeRequest codeRequest, IAIService aiService) => {
  if (codeRequest == null)
  {
    return Results.BadRequest("Request body cannot be null.");
  }
  try
  {
    var prompt = @"You are a code assistant. Your task is to analyze the provided code and suggest improvements or fixes. The response should be in JSON format with the following fields:
- code: The fixed or improved code as a string.
- language: The programming language of the code (e.g., 'C#', 'Python').
- fixType: A brief description of the type of fix or improvement made (e.g., 'Syntax Error', 'Performance Improvement').
- fixDescription: A detailed description of the changes made and why they were necessary.
Here is the code to analyze and fix:
```" + codeRequest.Code + @"```
Please provide the fixed code in the specified JSON format.";
    var response = await aiService.GenerateSchemaResponse<FixCodeModel>(prompt);
    return response is not null ? Results.Ok(response) : Results.NoContent();
  }
  catch (Exception ex)
  {
    return Results.Problem(ex.Message);
  }
});

app.Run();
