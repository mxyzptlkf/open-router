using System.Collections.Generic;

namespace func.openrouter;

public static class Const
{
    public static class Command
    {
        public const string Help = """\help""";
        public const string Clear = """\cls""";
        public const string Reset = """\clc""";
        public const string Exit = """\end""";
    }
    public static class Message
    {
        public static readonly List<string> Funny = new()
        {
            "???", 
            "(Silence...) Try typing something!", 
            "Hello...? Anyone there? 👀", 
            "Your keyboard okay? 🧐", 
            "Ah, the sound of nothingness... poetic!", 
            "Is this a game of charades? 😆", 
            "Type something, I promise I won't bite! 🐉",
        };
        public const string Error = "Error";
        public const string Start = "Hello, let's talk! Type [{0}] to see available options...";
        public const string Menu = "Available commands: \n[{0}] - Show this menu \n[{1}] - Clear chat history \n[{2}] - Clear screen \n[{3}] - Exit";
        public const string Clear = "Screen has been cleared!";
        public const string Reset = "Chat history has been cleared!";
        public const string Exit = "Goodbye! Have a great day! 👋";
        
    }
    public static class Dump
    {
        public const string Alice = "Alice";
        public const string Bob = "Bob";
    }
    public static class Config
    {
        public const string BaseUrl = "https://openrouter.ai/api/v1/chat/completions";
        public const string Schema = "Bearer";
        public const string ApiKey = "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";
    }
    public static class Role
    {
        public const string User = "user";
        public const string Assistant = "Assistant";
        
    }
    public static class Header
    {
        public static class Key
        {
            public const string ContentType = "Content-Type";
            public const string Auth = "Authorization";
        }
        public static class Value
        {
            public const string ContentType = "application/json";
            public const string Auth = $"{Config.Schema} {Config.ApiKey}";
        }
    }
}

public static class Dic
{
    public static class Config
    {
        public static readonly Dictionary<string, string> Model = new()
        {
            {"llama-4", "meta-llama/llama-4-maverick:free"},
            {"quasar-alpha", "openrouter/quasar-alpha"},
            {"deepseek-v3", "deepseek/deepseek-chat-v3-0324:free"},
            {"gemini-2.5", "google/gemini-2.5-pro-exp-03-25:free"}
        };
    }
}