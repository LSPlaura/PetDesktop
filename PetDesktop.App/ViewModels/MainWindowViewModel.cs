using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using FluentResults;
using PetDesktop.App.Enums;
using PetDesktop.App.Views;
using PetDesktop.Back.Config;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.Pet;
using PetDesktop.Back.Services.SpriteSheet;

namespace PetDesktop.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly PetCreationOrchestrator _orchestrator;
    private readonly PetService _petService;
    private readonly SpriteSheetService _spriteSheetService;

    [ObservableProperty]
    private Pet? _actualPet;

    [ObservableProperty]
    private MovementDirection _petDirection = MovementDirection.Right;

    public MainWindowViewModel(
        PetCreationOrchestrator orchestrator, 
        PetService petService, 
        SpriteSheetService spriteSheetService)
    {
        _orchestrator = orchestrator;
        _petService = petService;
        _spriteSheetService = spriteSheetService;
    }

    [RelayCommand]
    private async Task CreateDefaultPetAsync()
    {
        var result = await _orchestrator.PetCreatorInitAsync();
        if (result.IsFailure)
        {
            ShowErrorAndShutdown("No se pudo inicializar el creador de mascotas.");
            return;
        }

        var pet = await _petService.GetById(Config.DefaultPetName);
        if (pet == null)
        {
            ShowErrorAndShutdown($"No se encontró la mascota '{Config.DefaultPetName}'.");
            return;
        }

        ActualPet = pet;
        MovingPet((pet.Name, "WalkRight"));
    }

    partial void OnPetDirectionChanged(MovementDirection value)
    {
        if (ActualPet == null) return;

        string animationName = value switch
        {
            MovementDirection.Right => "WalkRight",
            MovementDirection.Left => "WalkLeft",
            MovementDirection.Up => "WalkBackwards",
            MovementDirection.Down => "WalkForwards",
            _ => "DefaultRight"
        };

        MovingPet((ActualPet.Name, animationName));
    }

    private async void MovingPet((string PetName, string SpriteName) key)
    {
        var spriteSheetResult = await _spriteSheetService.GetByIdAsync(key);
        if (spriteSheetResult.IsFailure)
        {
            ShowErrorAndShutdown("No se encontró el SpriteSheet");
            return;
        }

        var spriteSheet = spriteSheetResult.Value;
        if (File.Exists(spriteSheet.Route))
        {
            using var fileStream = File.OpenRead(spriteSheet.Route);
            
        }
    }
    
    private void ShowErrorAndShutdown(string message)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow?.Hide();
        
            var errorWin = new ErrorWindow(message);
            errorWin.Show();
            desktop.MainWindow?.Close();
        }
    }
}