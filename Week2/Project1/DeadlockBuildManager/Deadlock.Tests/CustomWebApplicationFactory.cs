using System.IO;
using System.Linq;
using Deadlock.Api.Data;
using Deadlock.Api.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Deadlock.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IHeroService> HeroSvcMock { get; } = new();
    public Mock<IBuildService> BuildSvcMock { get; } = new();
    public Mock<IItemService> ItemSvcMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // replace dbcontext with inmemory db
            var dbDesc = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<DeadlockDbContext>)
            );
            if (dbDesc is not null)
                services.Remove(dbDesc);
            services.AddDbContext<DeadlockDbContext>(opt =>
                opt.UseInMemoryDatabase("DeadlockTestDb")
            );

            // rplace services with mocks singletons so we can configure per test pl;us its in memory so its easier this way
            var heroDescriptor = services.Single(d => d.ServiceType == typeof(IHeroService));
            var buildDescriptor = services.Single(d => d.ServiceType == typeof(IBuildService));
            var itemDescriptor = services.Single(d => d.ServiceType == typeof(IItemService));

            //remove the ones we called
            services.Remove(heroDescriptor);
            services.Remove(buildDescriptor);
            services.Remove(itemDescriptor);

            //replace with our new singleton format
            services.AddSingleton<IHeroService>(sp => HeroSvcMock.Object);
            services.AddSingleton<IBuildService>(sp => BuildSvcMock.Object);
            services.AddSingleton<IItemService>(sp => ItemSvcMock.Object);
        });
    }
}
