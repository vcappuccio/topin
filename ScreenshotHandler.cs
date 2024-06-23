using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using System;
using System.Diagnostics;

#pragma warning disable OPENAI001

public class ScreenshotHandler
{
    private readonly OpenAIClient _openAIClient;
    private readonly FileClient _fileClient;
    private readonly AssistantClient _assistantClient;

    public ScreenshotHandler()
    {
        string apiKey = "your-api-key-h";
        try
        {
            _openAIClient = new OpenAIClient(apiKey);
            _fileClient = _openAIClient.GetFileClient();
            _assistantClient = _openAIClient.GetAssistantClient();
            Console.WriteLine($"ScreenshotHandler initialized with API key: {apiKey}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing OpenAI clients: {ex.Message}");
        }
    }

    public void SaveScreenshotAndCallAssistant(string screenshotPath)
    {
        try
        {
            Console.WriteLine($"Saving screenshot and calling assistant with path: {screenshotPath}");
            CallVisionAssistant(screenshotPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveScreenshotAndCallAssistant: {ex.Message}");
        }
    }

    private void CallVisionAssistant(string screenshotPath)
    {
        try
        {
            Console.WriteLine($"Uploading file: {screenshotPath}");
            OpenAIFileInfo uploadedFile = _fileClient.UploadFile(screenshotPath, FileUploadPurpose.Vision);
            Console.WriteLine($"File uploaded with ID: {uploadedFile.Id}");

            Console.WriteLine("Creating assistant");
            Assistant assistant = _assistantClient.CreateAssistant("gpt-4o", new AssistantCreationOptions
            {
                Instructions = "You are a useful assistant that replies using a funny style and always in Spanish."
            });
            Console.WriteLine($"Assistant created with ID: {assistant.Id}");

            Console.WriteLine("Creating thread");
            AssistantThread thread = _assistantClient.CreateThread(new ThreadCreationOptions
            {
                InitialMessages = {
                    new ThreadInitializationMessage(
                    new [] {
                        "Hello, assistant! Please describe this image for me:",
                        MessageContent.FromImageFileId(uploadedFile.Id)
                    })
                }
            });
            Console.WriteLine($"Thread created with ID: {thread.Id}");

            Console.WriteLine("Creating run streaming");
            var streamingUpdates = _assistantClient.CreateRunStreaming(thread, assistant);

            foreach (StreamingUpdate streamingUpdate in streamingUpdates)
            {
                if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
                {
                    Console.WriteLine("--- Run started! ---");
                }
                if (streamingUpdate is MessageContentUpdate contentUpdate)
                {
                    Console.Write(contentUpdate.Text);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CallVisionAssistant: {ex.Message}");
        }
    }
}
