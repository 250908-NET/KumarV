namespace Deadlock.Tests;

using System.Net;
using System.Net.Http.Json;
using Deadlock.Api.DTO;
using Deadlock.Api.Models;
using FluentAssertions;
using Moq;

//lets mock up our db
public class ProgramEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProgramEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        // RESET the in mem db
        _factory.HeroSvcMock.Reset();
        _factory.BuildSvcMock.Reset();
        _factory.ItemSvcMock.Reset();

        _client = factory.CreateClient();
    }

    // heroes
    [Fact]
    public async Task GetHeroes_ReturnsMappedDtos()
    {
        //Arrange
        _factory
            .HeroSvcMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(
                new List<Hero>
                {
                    new() { Id = 1, Name = "Bebop" },
                    new() { Id = 2, Name = "Rocksteady" },
                }
            );

        // Act
        var res = await _client.GetAsync("/heroes");

        // Assert
        res.EnsureSuccessStatusCode();
        var dtos = await res.Content.ReadFromJsonAsync<List<HeroReadDto>>();
        dtos!.Should().HaveCount(2);
        dtos.Select(d => d.Name).Should().BeEquivalentTo(new[] { "Bebop", "Rocksteady" });

        _factory.HeroSvcMock.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task PostHeroes_CreatesAndReturnsReadDto()
    {
        // Arrange
        var input = new HeroCreateDto("Shredder");
        _factory
            .HeroSvcMock.Setup(s => s.CreateAsync(It.IsAny<Hero>()))
            .ReturnsAsync(
                (Hero h) =>
                {
                    h.Id = 42;
                    return h;
                }
            );

        // Act
        var res = await _client.PostAsJsonAsync("/heroes", input);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await res.Content.ReadFromJsonAsync<HeroReadDto>();
        dto!.Id.Should().Be(42);
        dto.Name.Should().Be("Shredder");

        _factory.HeroSvcMock.Verify(
            s => s.CreateAsync(It.Is<Hero>(h => h.Name == "Shredder")),
            Times.Once
        );
    }

    // items

    [Fact]
    public async Task PostItems_InvalidColor_ReturnsBadRequest()
    {
        var bad = new ItemCreateDto("Bad", 1, "NotAColor");

        var res = await _client.PostAsJsonAsync("/items", bad);

        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _factory.ItemSvcMock.Verify(s => s.CreateAsync(It.IsAny<Item>()), Times.Never);
    }

    [Fact]
    public async Task PostItems_Valid_CreatesAndReturnsReadDto()
    {
        var input = new ItemCreateDto("Shield", 500, "Green");

        _factory
            .ItemSvcMock.Setup(s => s.CreateAsync(It.IsAny<Item>()))
            .ReturnsAsync(
                (Item it) =>
                {
                    it.Id = 10;
                    return it;
                }
            );

        var res = await _client.PostAsJsonAsync("/items", input);

        res.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await res.Content.ReadFromJsonAsync<ItemReadDto>();
        dto!.Id.Should().Be(10);
        dto.Name.Should().Be("Shield");
        dto.Color.Should().Be("Green");

        _factory.ItemSvcMock.Verify(
            s =>
                s.CreateAsync(
                    It.Is<Item>(i => i.Name == "Shield" && i.Price == 500 && i.Color == Color.Green)
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task PutItems_NotFound_Returns404()
    {
        // svc returns null endpoint should 404
        _factory.ItemSvcMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Item?)null);

        var res = await _client.PutAsJsonAsync("/items/99", new ItemUpdateDto(null, 1000, null));

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _factory.ItemSvcMock.Verify(s => s.UpdateAsync(99, It.IsAny<Item>()), Times.Never);
    }

    [Fact]
    public async Task PutItems_UpdateOk_ReturnsNoContent()
    {
        var existing = new Item
        {
            Id = 7,
            Name = "Boots",
            Price = 1200,
            Color = Color.Purple,
        };
        _factory.ItemSvcMock.Setup(s => s.GetByIdAsync(7)).ReturnsAsync(existing);
        _factory.ItemSvcMock.Setup(s => s.UpdateAsync(7, It.IsAny<Item>())).ReturnsAsync(true);

        var res = await _client.PutAsJsonAsync(
            "/items/7",
            new ItemUpdateDto("Boots+", 1300, "Orange")
        );

        res.StatusCode.Should().Be(HttpStatusCode.NoContent); //no content since we dont return the items after update
        _factory.ItemSvcMock.Verify(
            s =>
                s.UpdateAsync(
                    7,
                    It.Is<Item>(i =>
                        i.Name == "Boots+" && i.Price == 1300 && i.Color == Color.Orange
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteItems_Deleted_ReturnsNoContent()
    {
        _factory.ItemSvcMock.Setup(s => s.DeleteAsync(5)).ReturnsAsync(true);

        var res = await _client.DeleteAsync("/items/5");

        res.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _factory.ItemSvcMock.Verify(s => s.DeleteAsync(5), Times.Once);
    }

    // builds
    //

    [Fact]
    public async Task PostBuilds_HeroNotFound_ReturnsBadRequest()
    {
        var dto = new BuildCreateDto("Crit", "desc", HeroId: 1, ItemIds: null);

        //fixed so if it doesnt exist bad req
        _factory.HeroSvcMock.Setup(s => s.Exists(1)).ReturnsAsync(false);

        // should not try to create a build
        _factory
            .BuildSvcMock.Setup(s => s.CreateAsync(It.IsAny<Build>()))
            .Throws(new Exception("Should not be called"));

        var res = await _client.PostAsJsonAsync("/builds", dto);

        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _factory.HeroSvcMock.Verify(s => s.Exists(1), Times.Once);
        _factory.BuildSvcMock.Verify(s => s.CreateAsync(It.IsAny<Build>()), Times.Never);
    }

    [Fact]
    public async Task PostBuilds_HeroExists_ReturnsCreated()
    {
        var dto = new BuildCreateDto("Crit", "desc", HeroId: 2, ItemIds: null);

        _factory.HeroSvcMock.Setup(s => s.Exists(2)).ReturnsAsync(true);
        _factory
            .BuildSvcMock.Setup(s => s.CreateAsync(It.IsAny<Build>()))
            .ReturnsAsync(
                (Build b) =>
                {
                    b.Id = 77;
                    return b;
                }
            );

        var res = await _client.PostAsJsonAsync("/builds", dto);

        res.StatusCode.Should().Be(HttpStatusCode.Created);
        var read = await res.Content.ReadFromJsonAsync<BuildReadDto>();
        read!.Id.Should().Be(77);
        read.Name.Should().Be("Crit");
        read.HeroId.Should().Be(2);

        _factory.BuildSvcMock.Verify(
            s => s.CreateAsync(It.Is<Build>(b => b.Name == "Crit" && b.HeroId == 2)),
            Times.Once
        );
    }

    [Fact]
    public async Task PostBuilds_ItemMissing_ReturnsBadRequest()
    {
        var dto = new BuildCreateDto(
            "Crit",
            "desc",
            HeroId: 2,
            ItemIds: new List<int> { 100, 200 }
        );

        // hero must exist to reach the item loop
        _factory.HeroSvcMock.Setup(s => s.Exists(2)).ReturnsAsync(true);

        //  should short-circuit with 400 since we are missing an item
        _factory.ItemSvcMock.Setup(s => s.GetByIdAsync(100)).ReturnsAsync((Item?)null);

        var res = await _client.PostAsJsonAsync("/builds", dto);

        res.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        // should not be attempted when an item is missing
        _factory.BuildSvcMock.Verify(s => s.CreateAsync(It.IsAny<Build>()), Times.Never);
    }

    [Fact]
    public async Task PostBuilds_Success_ReturnsCreatedReadDto()
    {
        var dto = new BuildCreateDto("Crit", "desc", HeroId: 2, ItemIds: new List<int> { 10, 20 });

        _factory.HeroSvcMock.Setup(s => s.Exists(2)).ReturnsAsync(true); // fixed this took forever to notice ;(
        _factory
            .ItemSvcMock.Setup(s => s.GetByIdAsync(10))
            .ReturnsAsync(
                new Item
                {
                    Id = 10,
                    Name = "Boots",
                    Price = 100,
                    Color = Color.Purple,
                }
            );
        _factory
            .ItemSvcMock.Setup(s => s.GetByIdAsync(20))
            .ReturnsAsync(
                new Item
                {
                    Id = 20,
                    Name = "Sword",
                    Price = 200,
                    Color = Color.Orange,
                }
            );

        _factory
            .BuildSvcMock.Setup(s => s.CreateAsync(It.IsAny<Build>()))
            .ReturnsAsync(
                (Build b) =>
                {
                    b.Id = 77;
                    return b;
                }
            );

        var res = await _client.PostAsJsonAsync("/builds", dto);

        res.StatusCode.Should().Be(HttpStatusCode.Created);
        var read = await res.Content.ReadFromJsonAsync<BuildReadDto>();
        read!.Id.Should().Be(77);
        read.Name.Should().Be("Crit");
        read.HeroId.Should().Be(2);

        _factory.BuildSvcMock.Verify(
            s =>
                s.CreateAsync(
                    It.Is<Build>(b => b.Name == "Crit" && b.HeroId == 2 && b.Items.Count == 2)
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetBuildsById_NotFound_Returns404()
    {
        _factory.BuildSvcMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Build?)null); //shoudlnt exist

        var res = await _client.GetAsync("/builds/999");

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBuildsById_WithItems_ReturnsOkDto()
    {
        var build = new Build
        {
            Id = 5,
            Name = "Tank",
            Desc = "beefy",
            HeroId = 1,
            Items = new List<Item>
            {
                new()
                {
                    Id = 1,
                    Name = "Armor",
                    Price = 300,
                    Color = Color.Green,
                },
                new()
                {
                    Id = 2,
                    Name = "Boots",
                    Price = 100,
                    Color = Color.Purple,
                },
            },
        };

        _factory.BuildSvcMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(build);

        var res = await _client.GetAsync("/builds/5");

        res.EnsureSuccessStatusCode();
        var dto = await res.Content.ReadFromJsonAsync<BuildReadWithItemsDto>();
        dto!.Id.Should().Be(5);
        dto.Items.Should().HaveCount(2);
        dto.Items[0].Name.Should().Be("Armor");
    }
}
