using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Weeabot;
#nullable enable
public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();
  

    private DiscordSocketClient? _client;
    private CommandService? _commands;
    private CommandHandler? _commandHandler;

    public async Task MainAsync()
    {
        new Test().TestFunction();
        _client = new DiscordSocketClient();
        _client.Log += Log;
        _commands = new CommandService();
        _commandHandler = new CommandHandler( _client, _commands );

        //  You can assign your bot token to a string, and pass that in to connect.
        //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
        const string token = "Mzc3ODU0ODUwMDIyNTA2NDk2.WgMuwA.9CSxrad9DfFjab-QrE6KYJm43ik";

        // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        // var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await _commandHandler.InstallCommandsAsync();
        var log = new LogMessage(LogSeverity.Debug, "MainAsync", "MainAsync Started.");
        await Log(log);

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}
