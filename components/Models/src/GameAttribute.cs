namespace Kadense.RPG.Models;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class GameAttribute : Attribute
{
    public GameAttribute(string name)
    {
        Name = name;
    }
    public string Name { get; set; }
    public string? Description { get; set; }
}