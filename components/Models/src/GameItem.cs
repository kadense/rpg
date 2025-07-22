namespace Kadense.RPG.Models;

public class GameItem : GameBase
{
    public List<string> ImagePath { get; set; } = new List<string>();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<FileAttachment> Attachments { get; set; } = new List<FileAttachment>();
}