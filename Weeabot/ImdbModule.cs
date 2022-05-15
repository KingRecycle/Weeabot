using System.Drawing;
using Discord;
using Discord.Commands;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using IMDbApiLib;
using IMDbApiLib.Models;
using Color = Discord.Color;

namespace Weeabot;

public class ImdbModule : ModuleBase<SocketCommandContext>
{
  private const string APIKey = "k_yh7stzla";
  private const string SearchString = $"https://imdb-api.com/en/API/SearchMovie/{APIKey}/";

  // ~say hello world -> hello world
  [Command("movie")]
  [Summary("Shows IMDB info on a searched movie.")]
  public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
  {
    var result = QuerySearch( echo );
    await ReplyAsync( result.ToString());
  }

  public async Task<SearchData> QuerySearch( string searchText)
  {
    var apiLib = new ApiLib( APIKey );
    var data = await apiLib.SearchTitleAsync( searchText );
    return data;
  }
}
