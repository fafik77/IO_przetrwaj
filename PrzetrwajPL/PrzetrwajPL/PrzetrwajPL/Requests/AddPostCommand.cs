namespace PrzetrwajPL.Requests;

public class AddPostCommand
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public int IdCategory { get; set; }
    public string? CustomCategory { get; set; }
    public int IdRegion { get; set; }
}
