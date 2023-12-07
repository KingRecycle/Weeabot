using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Weeabot;

public class Program {
    public static Task Main( string[] args ) {
        return new Program().MainAsync();
    }


    DiscordSocketClient? _client;
    CommandService? _commands;
    CommandHandler? _commandHandler;

    public async Task MainAsync() {
        DiscordSocketConfig config = new() {
            UseInteractionSnowflakeDate = false,
        };
        _client = new DiscordSocketClient( config );
        _client.Log += Log;
        _commands = new CommandService();
        _commandHandler = new CommandHandler( _client, _commands );

        await _client.LoginAsync( TokenType.Bot, Environment.GetEnvironmentVariable( "WEEABOT_TOKEN" ) );
        await _client.StartAsync();
        await _client.SetGameAsync( "100% Orange Juice", null, ActivityType.Competing );

        var log = new LogMessage( LogSeverity.Debug, "MainAsync", "[Weeabot]: Main Started." );
        await Log( log );
        // Block this task until the program is closed.
        await Task.Delay( -1 );
    }


    static Task Log( LogMessage msg ) {
        Console.WriteLine( msg.ToString() );
        return Task.CompletedTask;
    }
}