namespace ToDoAppPoc.Models;

public class ToDoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "NEW";

}