using System;
using TaskApi.Dtos;
using TaskApi.Mappers;
using TaskApi.Models;
using Xunit;

public class MapperUpdateTaskTests
{
    [Fact]
    public void UpdateTask_ReplacesAllMutableFields_And_BumpsUpdatedAt()
    {
        var entity = new TaskItem
        {
            Id = 7,
            Title = "Old Title",
            Description = "Old Desc",
            IsComplete = false,
            Priority = Priority.Low,
            DueDate = null,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
        };

        var dto = new TaskUpdateDto
        {
            Title = "New Title",
            Description = "New Desc",
            IsComplete = true,
            Priority = Priority.High,
            DueDate = new DateTime(2030, 12, 31),
        };

        var before = DateTime.UtcNow;

        entity.UpdateTask(dto);

        var after = DateTime.UtcNow;

        //  fields updated
        Assert.Equal("New Title", entity.Title);
        Assert.Equal("New Desc", entity.Description);
        Assert.True(entity.IsComplete);
        Assert.Equal(Priority.High, entity.Priority);
        Assert.Equal(new DateTime(2030, 12, 31), entity.DueDate);

        //  immutable fields unchanged
        Assert.Equal(7, entity.Id);
        Assert.True(entity.CreatedAt < entity.UpdatedAt);

        //  UpdatedAt is within call window
        Assert.InRange(entity.UpdatedAt, before, after);
    }
}
