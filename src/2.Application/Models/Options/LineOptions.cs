namespace PocLineAPI.Application;

public class LineOptions
{
    public required string ChannelSecret { get; set; }
    public required string ChannelId { get; set; }
    public required string APIBaseUrl { get; set; }
}
