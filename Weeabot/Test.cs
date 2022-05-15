namespace Weeabot;

public class Test
{
    public async void TestFunction()
    {
        ImdbModule module = new ImdbModule();
        var result= await module.QuerySearch("Parasite");
        Console.WriteLine(result.Results[0].Description);
    }
}