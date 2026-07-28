using Avalonia.Media.Imaging;
using Microsoft.Extensions.Configuration;

namespace PetDesktop.Back.Config;

public static class Config
{
    public static readonly IConfigurationRoot Settings = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", false, true)
        .Build();

    public static string SpriteSheetRoute { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PetDesktop", SpriteSheetFolder);

    public static string SpriteSheetFolder { get; } =
        Settings.GetValue<string>("PetSettings:SpriteSheetSettings:SpriteSheetFolder") ?? "SpriteSheets";

    public static string DefaultPetName { get; } = Settings.GetValue<string>("DefaultPet:Name") ?? "DefaultPet";
    public static string DefaultSpriteSheetName { get; } = Settings.GetValue<string>("DefaultPet:InitialAnimation") ?? "Default_Animation";

    public static string DefaultPetSpritesRoute { get; }= Settings.GetValue<string>("DefaultPet:Route") ??
                                                          "avares://PetDesktop.App/Assets/DefaultPet";
    public static int DefaultPetFrameWidth { get; } = Settings.GetValue<int>("DefaultPet:FrameWidth");
    public static int DefaultPetFrameHeight { get; } = Settings.GetValue<int>("DefaultPet:FrameHeight");
}