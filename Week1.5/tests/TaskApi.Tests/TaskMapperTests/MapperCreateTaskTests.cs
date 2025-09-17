using System;
using TaskApi.Dtos;
using TaskApi.Mappers;
using TaskApi.Models;
using Xunit;

public class MapperCreateTaskTests
{
    [Fact]
    public void CreateTask_MapsFields_AndSetsDefaultPriority_AndTimestamps()
    {
        var dto = new TaskCreateDto
        {
            Title = "Buy milk",
            Description = "2% milk",
            IsComplete = false,
            Priority = null, // should default to Medium
            DueDate = new DateTime(2025, 02, 01),
        };

        // Capture time bounds since we use DateTime.UtcNow inside the mapper
        var before = DateTime.UtcNow;

        var entity = dto.CreateTask();

        var after = DateTime.UtcNow;

        Assert.Equal("Buy milk", entity.Title);
        Assert.Equal("2% milk", entity.Description);
        Assert.False(entity.IsComplete);
        Assert.Equal(Priority.Medium, entity.Priority); // default check priority in next test
        Assert.Equal(new DateTime(2025, 02, 01), entity.DueDate);

        // Assert timestamps are set and within expected range
        Assert.True(
            entity.CreatedAt >= before && entity.CreatedAt <= after,
            $"CreatedAt {entity.CreatedAt:o} should be between {before:o} and {after:o}"
        );

        Assert.True(
            entity.UpdatedAt >= before && entity.UpdatedAt <= after,
            $"UpdatedAt {entity.UpdatedAt:o} should be between {before:o} and {after:o}"
        );

        // should also == eachother
        Assert.True(Math.Abs((entity.UpdatedAt - entity.CreatedAt).TotalMilliseconds) < 5_000);
    }

    [Fact]
    public void CreateTask_RespectsProvidedPriority()
    {
        var dto = new TaskCreateDto { Title = "Pay bills", Priority = Priority.High };

        var entity = dto.CreateTask();

        Assert.Equal(Priority.High, entity.Priority);
        Assert.Equal("Pay bills", entity.Title);
    }
}
