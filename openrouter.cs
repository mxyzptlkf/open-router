using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace func.openrouter;

public class Program
{
    private void print(string? content, ConsoleColor color, bool noNewLine)
    {
        Console.ForegroundColor = color;
        if (noNewLine)
            Console.Write(content);
        else
            Console.WriteLine(content);
    }
    private void addmessage(Request request, string role, string? prompt)
    {
        request.messages.Add(new Request.MessagesData()
        {
            role = role,
            content = new List<Request.ContentData>()
            {
                new Request.ContentData()
                {
                    type = "text",
                    text = prompt
                }
            }
        });
    }
    private async Task sendrequest(Request request)
    {
        using HttpClient client = new();
        using HttpRequestMessage req = new(HttpMethod.Post, Const.Config.BaseUrl)
        {
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, Const.Header.Value.ContentType)
        };
        req.Headers.Add(Const.Header.Key.Auth, Const.Header.Value.Auth);
        var response = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        var fullResponse = new StringBuilder();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("data: "))
            {
                line = line.Substring(6);

                if (line.Trim() == "[DONE]") break;

                var jsonLine = JsonNode.Parse(line);
                var contentText = jsonLine?["choices"]?[0]?["delta"]?["content"]?.ToString();
                print(contentText, ConsoleColor.Green, true);
                fullResponse.Append(contentText);
            }
        }

        print(null, ConsoleColor.White, false);
        addmessage(request, Const.Role.Assistant, fullResponse.ToString());
    }
    public async Task main(string model)
    {
        try
        {
            print(string.Format(Const.Message.Start, Const.Command.Help), ConsoleColor.Cyan, false);
            Request request = new()
            {
                model = Dic.Config.Model[model],
                messages = [],
                stream = true
            };

            while (true)
            {
                print($"{Const.Dump.Bob}: ", ConsoleColor.Magenta, true);
                Console.ForegroundColor = ConsoleColor.White;
                string? userInput = Console.ReadLine();
                print($"{Const.Dump.Alice}: ", ConsoleColor.Magenta, true);

                switch (userInput?.ToLower())
                {
                    case Const.Command.Help:
                        print(string.Format(Const.Message.Menu, Const.Command.Help, Const.Command.Reset, Const.Command.Clear, Const.Command.Exit) , ConsoleColor.Cyan, false);
                        continue;
                    case Const.Command.Clear:
                        Console.Clear();
                        print($"{Const.Dump.Alice}: ", ConsoleColor.Magenta, true);
                        print(Const.Message.Clear, ConsoleColor.Cyan, false);
                        continue;
                    case Const.Command.Reset:
                        request.messages = new List<Request.MessagesData>();
                        print(Const.Message.Reset, ConsoleColor.Cyan, false);
                        continue;
                    case Const.Command.Exit:
                        print(Const.Message.Exit, ConsoleColor.Cyan, false);
                        return;
                    default:
                        if (string.IsNullOrWhiteSpace(userInput))
                        {
                            print(Const.Message.Funny[new Random().Next(Const.Message.Funny.Count)], ConsoleColor.Yellow, false);
                            continue;
                        }
                        break;
                }

                addmessage(request, Const.Role.User, userInput);
                await sendrequest(request);
            }
        }
        catch (Exception ex)
        {
            print(ex.Message, ConsoleColor.Red, false);
        }
    }
    public class Request
    {
        public required string model { get; set; }
        public required List<MessagesData> messages { get; set; }
        public required bool stream { get; set; }
        public class MessagesData
        {
            public required string role { get; set; }
            public required List<ContentData> content { get; set; }
        }
        public class ContentData
        {
            public required string type { get; set; }
            public string? text { get; set; }
        }
    }
}