using System.Drawing;
using Discord;
using Discord.WebSocket;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Color = Discord.Color;

namespace Weeabot;

public class AnimeCommands {
    const string ANILIST = "https://graphql.anilist.co/";
    const string ANILIST_ICON = "https://anilist.co/img/icons/icon.svg";
    const int DEFAULT_LIST_AMOUNT = 3;
    DiscordSocketClient _client;

    public AnimeCommands( DiscordSocketClient client ) {
        _client = client;
    }


    public async Task AnimeSearch( SocketSlashCommand command ) {
        Console.WriteLine( "===Anime Search Requested...===" );
        var animeRequest = new GraphQLRequest {
            Query = @"
              query($searchText: String) {
                Media (search: $searchText, sort: POPULARITY_DESC) {
                  id
                  description
                  averageScore
                  genres
                  siteUrl
                  bannerImage
                  title {
                    romaji
                    english
                  }
                  rankings {
                    rank
                  }
                  coverImage {
                    large
                    color
                  }
                  studios{
                    nodes{ 
                       name
                    }
                  }
                }
              }
            ",
            Variables = new {
                searchText = command.Data.Options.First().Value.ToString(),
            },
        };

        var graphQlHttpClient = new GraphQLHttpClient( ANILIST, new NewtonsoftJsonSerializer() );

        try {
            var response = await graphQlHttpClient.SendQueryAsync<AnimeType>( animeRequest );
            await command.RespondAsync( embed: CreateAnimeEmbed( response ).Build() );
        }
        catch ( GraphQLHttpRequestException e ) {
            await GraphHttpRequestExceptionHandler( command, e );
            throw;
        }
    }

    EmbedBuilder CreateAnimeEmbed( GraphQLResponse<AnimeType> graphQlResponse ) {
        var mediaResponse = graphQlResponse.Data.Media;
        var newDesc = mediaResponse.Description.Replace( "<br>", "" ).Replace( "<i>", "" ).Replace( "</i>", "" );
        var newGenres = string.Join<string>( ", ", mediaResponse.Genres );
        var studios = mediaResponse.Studios.Nodes.Count > 0 ? mediaResponse.Studios.Nodes[0].Name : "n/a";
        var rankOfAllTime = mediaResponse.Rankings.Count > 0 ? mediaResponse.Rankings[0].Rank.ToString() : "n/a";
        var embedBuilder = new EmbedBuilder {
            Title = "Anime Title Here",
            Description = "Anime Description Here",
        };
        embedBuilder.WithTitle( $"{mediaResponse.Title.Romaji} ( {mediaResponse.Title.English} )" );
        embedBuilder.WithUrl( mediaResponse.SiteURL );
        embedBuilder.WithImageUrl( mediaResponse.CoverImage.Large );
        embedBuilder.WithAuthor( studios );
        embedBuilder.WithDescription( newDesc );
        embedBuilder.WithThumbnailUrl( mediaResponse.Banner );
        embedBuilder.WithColor( GenerateRgb( mediaResponse.CoverImage.Color ) );
        embedBuilder.Fields.Add( new EmbedFieldBuilder().WithName( "Average Score" )
            .WithValue( mediaResponse.AverageScore ).WithIsInline( true ) );
        embedBuilder.Fields.Add( new EmbedFieldBuilder().WithName( "Ranked # All Time" ).WithValue( rankOfAllTime )
            .WithIsInline( true ) );
        embedBuilder.Fields.Add( new EmbedFieldBuilder().WithName( "Genres" ).WithValue( newGenres )
            .WithIsInline( true ) );
        embedBuilder.WithFooter( "https://anilist.co/", ANILIST_ICON );

        Console.WriteLine( $"{mediaResponse.Title.Romaji} : {mediaResponse.Id} : {mediaResponse.SiteURL}" );
        return embedBuilder;
    }

    static async Task GraphHttpRequestExceptionHandler( SocketSlashCommand command, GraphQLHttpRequestException e ) {
        await command.RespondAsync( $"Exception Triggered: {e.StatusCode}" );
        Console.WriteLine( e.StatusCode );
        Console.WriteLine( e.Content );
    }

    static Color GenerateRgb( string backgroundColor ) {
        var color = ColorTranslator.FromHtml( backgroundColor );
        int r = Convert.ToInt16( color.R );
        int g = Convert.ToInt16( color.G );
        int b = Convert.ToInt16( color.B );
        return new Color( r, g, b );
    }
}