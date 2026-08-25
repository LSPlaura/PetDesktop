using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetDesktop.Back.Config;
using PetDesktop.Back.Entities;
using PetDesktop.Back.Repositories.Pet;
using PetDesktop.Back.Repositories.SpriteSheet;
using PetDesktop.Back.Services.Pet;
using PetDesktop.Back.Services.SpriteSheet;

namespace PetDesktop.App.Infraestructure;

public static class DependenciesProvider
{
    public static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        RegisterRepository(services);
        RegisterGenerator(services);
        RegisterOrchestator(services);
        RegisterServices(services);
        return services.BuildServiceProvider();
    }

    private static void RegisterRepository(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(Config.ConnectionString));

        services.AddTransient<IPetRepository, PetRepository>(sp => new PetRepository(sp.GetRequiredService<AppDbContext>()));
        services.AddTransient<ISpriteSheetRepository, SpriteSheetRepository>(sp => new SpriteSheetRepository(sp.GetRequiredService<AppDbContext>()));
    }
    
     private static void RegisterGenerator(IServiceCollection services)
     {
         services.AddTransient<SpriteSheetGenerator>(sp => new SpriteSheetGenerator());
     }
     
     private static void RegisterServices(IServiceCollection services)
     {
         services.AddTransient<PetService>(sp => new PetService(sp.GetRequiredService<IPetRepository>()));
         services.AddTransient<SpriteSheetService>(sp => new SpriteSheetService(sp.GetRequiredService<SpriteSheetGenerator>(),sp.GetRequiredService<ISpriteSheetRepository>()));
     }
    
    private static void RegisterOrchestator(IServiceCollection services)
    {
        services.AddTransient<PetCreationOrchestrator>(sp => 
            new PetCreationOrchestrator(
                sp.GetRequiredService<SpriteSheetService>(),
                sp.GetRequiredService<PetService>(),
                sp.GetRequiredService<SpriteSheetGenerator>()
            )
        );
    }
}