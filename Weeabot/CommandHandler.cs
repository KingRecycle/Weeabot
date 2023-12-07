using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Weeabot;

public class CommandHandler {
    readonly DiscordSocketClient _client;
    readonly CommandService _commands;

    readonly AnimeCommands _animeCommands;

    // Retrieve client and CommandService instance via ctor
    public CommandHandler( DiscordSocketClient client, CommandService commands ) {
        _commands = commands;
        _client = client;
        _client.Ready += Ready;
        _client.SlashCommandExecuted += SlashCommandHandler;
        _client.Log += Log;
        _animeCommands = new AnimeCommands( _client );
    }

    async Task Ready() {
        var animeCommand = new SlashCommandBuilder();
        animeCommand.WithName( "anime" )
            .WithDescription( "Search for an anime on Anilist." )
            .AddOption( "search", ApplicationCommandOptionType.String, "The anime you want to search for.", true );

        try {
            // With global commands we don't need the guild.
            await _client?.CreateGlobalApplicationCommandAsync( animeCommand.Build() )!;
            // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
            // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
        }
        catch ( HttpException exception ) {
            // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
            var json = JsonConvert.SerializeObject( exception.Reason, Formatting.Indented );

            // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
            Console.WriteLine( json );
        }
    }

    async Task SlashCommandHandler( SocketSlashCommand command ) {
        // Let's add a switch statement for the command name so we can handle multiple commands in one event.
        switch ( command.Data.Name ) {
            case "anime":
                await HandleAnimeSearchCommand( command );
                break;
        }
    }

    async Task HandleAnimeSearchCommand( SocketSlashCommand command ) {
        Console.WriteLine( "Anime Command called!" );
        await _animeCommands.AnimeSearch( command );
    }


    Task Log( LogMessage msg ) {
        Console.WriteLine( msg.ToString() );
        return Task.CompletedTask;
    }
}