namespace Weeabot;

public class AnimeType {
    public MediaType Media { get; set; }
}

public class PageData {
    public PageType Page { get; set; }
}

public class PageType {
    public PageInfo PageInfo { get; set; }
    public List<MediaType> Media { get; set; }
}

public class PageInfo {
    public int Total { get; set; }
    public int CurrentPage { get; set; }
    public int LastPage { get; set; }
    public bool HasNextPage { get; set; }
    public int PerPage { get; set; }
}

public class MediaType {
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

public class MediaSmallType {
    public int Id;
    public MediaTitle Title;
}

public class MediaTitle {
    public string Romaji;
    public string English;
}

public class MediaCoverImageType {
    public string Large;
    public string Color;
}

public class StudioConnection {
    public List<NodesType> Nodes;
}

public class NodesType {
    public string Name;
}

public class RankingType {
    public int Rank;
}