using System;
using System.Collections.Generic;
using System.Linq;
using TaskApi.Dtos;
using TaskApi.Models;
using TaskApi.Validation;
using Xunit;

namespace TaskApi.Tests;

public class ValidationTests
{
    [Fact]
    public void TaskCreateDto_Valid_ReturnsOkTrue()
    {
        var dto = new TaskCreateDto //shouldpass this test
        {
            Title = "Buy milk",
            Description = "2% carton",
            IsComplete = false,
            Priority = Priority.Medium,
            DueDate = new DateTime(2030, 1, 1),
        };

        var (ok, errors) = TaskValidator.Validate(dto);

        Assert.True(ok);
        Assert.Empty(errors);
    }

    [Fact]
    public void TaskCreateDto_Title_Required()
    {
        var dto = new TaskCreateDto
        {
            Title = "", // empty triggers Required tag
            Description = "ok",
        };

        var (ok, errors) = TaskValidator.Validate(dto);

        Assert.False(ok);
        Assert.Contains(nameof(TaskCreateDto.Title), errors.Keys);
        Assert.Contains(
            errors[nameof(TaskCreateDto.Title)],
            msg => msg.Contains("required", StringComparison.OrdinalIgnoreCase)
        );
    }

    [Fact]
    public void TaskCreateDto_Title_MaxLength_100()
    {
        var dto = new TaskCreateDto
        {
            Title = new string('x', 101), // too long triggers MaxLength(100) tag
            Description = "ok",
        };

        var (ok, errors) = TaskValidator.Validate(dto);

        Assert.False(ok);
        Assert.Contains(nameof(TaskCreateDto.Title), errors.Keys);
        Assert.Contains(
            errors[nameof(TaskCreateDto.Title)],
            msg =>
                msg.Contains("maximum length", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("100")
        );
    }

    [Fact]
    public void TaskCreateDto_OptionalFields_CanBeNull()
    {
        var dto = new TaskCreateDto
        {
            Title = "Only title required",
            Description = null,
            Priority = null,
            DueDate = null,
        };

        var (ok, errors) = TaskValidator.Validate(dto);

        Assert.True(ok);
        Assert.Empty(errors);
    }

    [Fact]
    public void TaskUpdateDto_Title_Required_Fails()
    {
        var dto = new TaskUpdateDto
        {
            Title = "", // empty so required fails
            IsComplete = false,
        };

        var (ok, errors) = TaskValidator.Validate(dto);

        Assert.False(ok);
        Assert.Contains(nameof(TaskUpdateDto.Title), errors.Keys);
        Assert.Contains(
            errors[nameof(TaskUpdateDto.Title)],
            m => m.Contains("required", StringComparison.OrdinalIgnoreCase)
        );
    }
}
