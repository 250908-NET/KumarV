using Deadlock.Api.Models;
using Deadlock.Api.Repository;
using Deadlock.Api.Service;
using FluentAssertions;
using Moq;

namespace Deadlock.Tests;

public class HeroServiceTests
{
    private readonly Mock<IHeroRepository> _repo = new();
    private readonly HeroService _svc;

    public HeroServiceTests()
    {
        _svc = new HeroService(_repo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsHeroes()
    {
        var heroes = new List<Hero>
        {
            new() { Id = 1, Name = "A" },
            new() { Id = 2, Name = "B" },
        };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(heroes);

        var result = await _svc.GetAllAsync();

        result.Should().BeEquivalentTo(heroes);
        _repo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Found_ReturnsHero()
    {
        var h = new Hero { Id = 10, Name = "X" };
        _repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(h);

        var result = await _svc.GetByIdAsync(10);

        result.Should().Be(h);
        _repo.Verify(r => r.GetByIdAsync(10), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Hero?)null);

        var result = await _svc.GetByIdAsync(999);

        result.Should().BeNull();
        _repo.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CallsRepoAdd_ReturnsCreated()
    {
        var input = new Hero { Name = "New" };
        var created = new Hero { Id = 7, Name = "New" };
        _repo.Setup(r => r.AddAsync(input)).ReturnsAsync(created);

        var result = await _svc.CreateAsync(input);

        result.Should().Be(created);
        _repo.Verify(r => r.AddAsync(input), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CallsRepoUpdate_ReturnsBool()
    {
        var hero = new Hero { Id = 3, Name = "U" };
        _repo.Setup(r => r.UpdateAsync(3, hero)).ReturnsAsync(true);

        var ok = await _svc.UpdateAsync(3, hero);

        ok.Should().BeTrue();
        _repo.Verify(r => r.UpdateAsync(3, hero), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepoDelete_ReturnsBool()
    {
        _repo.Setup(r => r.DeleteAsync(5)).ReturnsAsync(true);

        var ok = await _svc.DeleteAsync(5);

        ok.Should().BeTrue();
        _repo.Verify(r => r.DeleteAsync(5), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Exists_ReflectsRepo(bool exists)
    {
        _repo.Setup(r => r.Exists(11)).ReturnsAsync(exists);

        var result = await _svc.Exists(11);

        result.Should().Be(exists);
        _repo.Verify(r => r.Exists(11), Times.Once);
    }
}
