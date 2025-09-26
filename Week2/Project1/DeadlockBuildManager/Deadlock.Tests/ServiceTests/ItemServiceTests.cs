using Deadlock.Api.Models;
using Deadlock.Api.Repository;
using Deadlock.Api.Service;
using FluentAssertions;
using Moq;

namespace Deadlock.Tests;

public class ItemServiceTests
{
    private readonly Mock<IItemRepository> _mockRepo = new();
    private readonly ItemService _svc;

    public ItemServiceTests()
    {
        _svc = new ItemService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItems()
    {
        var expected = new List<Item>
        {
            new()
            {
                Id = 1,
                Name = "Boots",
                Price = 1200,
                Color = Color.Purple,
            },
            new()
            {
                Id = 2,
                Name = "Sword",
                Price = 800,
                Color = Color.Orange,
            },
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var result = await _svc.GetAllAsync();

        result.Should().BeEquivalentTo(expected);
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Found_ReturnsItem()
    {
        var item = new Item
        {
            Id = 99,
            Name = "Shield",
            Price = 500,
            Color = Color.Green,
        };
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(item);

        var result = await _svc.GetByIdAsync(99);

        result.Should().Be(item);
        _mockRepo.Verify(r => r.GetByIdAsync(99), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(12345)).ReturnsAsync((Item?)null);

        var result = await _svc.GetByIdAsync(12345);

        result.Should().BeNull();
        _mockRepo.Verify(r => r.GetByIdAsync(12345), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidItem_ReturnsCreated()
    {
        var input = new Item
        {
            Name = "Potion",
            Price = 50,
            Color = Color.Purple,
        };
        var created = new Item
        {
            Id = 7,
            Name = "Potion",
            Price = 50,
            Color = Color.Purple,
        };
        _mockRepo.Setup(r => r.AddAsync(input)).ReturnsAsync(created);

        var result = await _svc.CreateAsync(input);

        result.Should().Be(created);
        _mockRepo.Verify(r => r.AddAsync(input), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CallsRepo_AndReturnsTrue()
    {
        var updated = new Item
        {
            Id = 4,
            Name = "Axe",
            Price = 900,
            Color = Color.Orange,
        };
        _mockRepo.Setup(r => r.UpdateAsync(4, updated)).ReturnsAsync(true);

        var ok = await _svc.UpdateAsync(4, updated);

        ok.Should().BeTrue();
        _mockRepo.Verify(r => r.UpdateAsync(4, updated), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepo_AndReturnsTrue()
    {
        _mockRepo.Setup(r => r.DeleteAsync(12)).ReturnsAsync(true);

        var ok = await _svc.DeleteAsync(12);

        ok.Should().BeTrue();
        _mockRepo.Verify(r => r.DeleteAsync(12), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Exists_ReflectsRepo(bool exists)
    {
        _mockRepo.Setup(r => r.Exists(11)).ReturnsAsync(exists);

        var result = await _svc.Exists(11);

        result.Should().Be(exists);
        _mockRepo.Verify(r => r.Exists(11), Times.Once);
    }

    [Fact]
    public void Ctor_WithNullRepo_Throws()
    {
        Action act = () => new ItemService(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
