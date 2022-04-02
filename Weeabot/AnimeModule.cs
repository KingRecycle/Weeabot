using System.Drawing;
using Discord;
using Discord.Commands;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Color = Discord.Color;

namespace Weeabot;

public class AnimeModule : ModuleBase<SocketCommandContext>
{
    private const string ANILIST = "https://graphql.anilist.co/";
    private const string ANILIST_ICON = "https://anilist.co/img/icons/icon.svg";

    // ~say hello world -> hello world
    [Command("anime")]
    [Summary("Shows Anilist info on a searched Anime.")]
    public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
    {
        var animeRequest = new GraphQLRequest {
            Query = @"
              query($searchText: String) {
                Media (search: $searchText, type: ANIME) {
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
            Variables = new
            {
                searchText = echo
            }
        };

        var graphQlHttpClient = new GraphQLHttpClient( ANILIST, new NewtonsoftJsonSerializer() );

        try
        {
            var response = await graphQlHttpClient.SendQueryAsync<AnimeType>( animeRequest );

            Console.WriteLine(response.Data);
            var mediaResponse = response.Data.Media;
            var newDesc = mediaResponse.Description.Replace( "<br>", "" );
            var newGenres = string.Join<string>(", ", mediaResponse.Genres);


            var embedBuilder = new EmbedBuilder
            {
                Title = "Test",
                Description = "I'm testing stuff."
            };
            embedBuilder.WithTitle( $"{mediaResponse.Title.Romaji} ( {mediaResponse.Title.English} )" );
            embedBuilder.WithUrl( mediaResponse.SiteURL );
            embedBuilder.WithImageUrl( mediaResponse.CoverImage.Large );
            embedBuilder.WithAuthor( mediaResponse.Studios.Nodes[0].Name );
            embedBuilder.WithDescription( newDesc );
            embedBuilder.WithThumbnailUrl( mediaResponse.Banner );
            embedBuilder.WithColor( GenerateRgb( mediaResponse.CoverImage.Color ) );
            embedBuilder.Fields.Add( new EmbedFieldBuilder().WithName("Average Score").WithValue(mediaResponse.AverageScore).WithIsInline( true ));
            embedBuilder.Fields.Add( new EmbedFieldBuilder().WithName( "Ranked # All Time" ).WithValue( mediaResponse.Rankings[0].Rank ).WithIsInline( true ) );
            embedBuilder.Fields.Add( new EmbedFieldBuilder().WithName( "Genres" ).WithValue( newGenres ).WithIsInline( true ) );
            embedBuilder.WithFooter( "https://anilist.co/" );

            Console.WriteLine($"{mediaResponse.Title.Romaji} : {mediaResponse.Id} : {mediaResponse.SiteURL}");
            await ReplyAsync( embed: embedBuilder.Build() );
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
            throw;
        }
    }

    public Color GenerateRgb( string backgroundColor )
    {
        var color = ColorTranslator.FromHtml( backgroundColor );
        int r = Convert.ToInt16( color.R );
        int g = Convert.ToInt16( color.G );
        int b = Convert.ToInt16( color.B );
        return new Color( r, g, b );
    }
}

public class AnimeType
{
    public MediaType Media;
}
public class MediaType
{
    public int Id;
    public string Description;
    public int AverageScore;
    public string SiteURL;
    public string Banner;
    public List<string> Genres;
    public MediaTitle Title;
    public MediaCoverImageType CoverImage;
    public StudioConnection Studios;
    public List<RankingType> Rankings;
}
public class MediaTitle
{
    public string Romaji;
    public string English;
}

public class MediaCoverImageType
{
    public string Large;
    public string Color;
}

public class StudioConnection
{
    public List<NodesType> Nodes;
}

public class NodesType
{
    public string Name;
}

public class RankingType
{
    public int Rank;
}
