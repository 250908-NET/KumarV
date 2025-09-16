using TaskApi.Models;
using Xunit;

public class TaskItemTests //sanity check for entity test
{
    [Fact]
    public void Defaults_Are_Correct()
    {
        var t = new TaskItem();
        Assert.False(t.IsComplete);
        Assert.Equal(Priority.Medium, t.Priority);
        Assert.Null(t.DueDate);
    }
}
