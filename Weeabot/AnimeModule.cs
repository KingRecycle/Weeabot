using Discord;
using Discord.Commands;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace Weeabot;

public class AnimeModule : ModuleBase<SocketCommandContext>
{
    private const string ANILIST = "https://graphql.anilist.co/";

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
                  siteUrl
                  title {
                    romaji
                    english
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
        var response = await graphQlHttpClient.SendQueryAsync<AnimeType>( animeRequest );
        Console.WriteLine(response.Data);
        var mediaResponse = response.Data.Media;

        var embedBuilder = new EmbedBuilder
        {
            Title = "Test",
            Description = "I'm testing stuff."
        };
        embedBuilder.WithTitle( $"{mediaResponse.Title.Romaji} ( {mediaResponse.Title.English} )" );
        embedBuilder.WithUrl( mediaResponse.SiteURL );
        embedBuilder.WithImageUrl( mediaResponse.CoverImage.Large );

        Console.WriteLine($"{mediaResponse.Title.Romaji} : {mediaResponse.Id} : {mediaResponse.SiteURL}");
        await ReplyAsync( embed: embedBuilder.Build() );
    }

    // ReplyAsync is a method on ModuleBase
}

public class AnimeType
{
    public MediaType Media;
}
public class MediaType
{
    public int Id;
    public string SiteURL;
    public MediaTitle Title;
    public MediaCoverImageType CoverImage;
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
