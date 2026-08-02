using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using FluentResults;
using PetDesktop.App.Views;
using PetDesktop.Back.Config;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.Pet;

namespace PetDesktop.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private PetCreationOrchestrator _orchestrator;
    private PetService _petService;
    [ObservableProperty]
    private Pet _actualPet;
    public MainWindowViewModel(PetCreationOrchestrator orchestrator, PetService petService)
    {
        _orchestrator = orchestrator;
        _petService = petService;
    }
    
    private void ShowErrorAndShutdown(string message)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow?.Hide();
        
            var errorWin = new ErrorWindow(message);
            errorWin.Show();
        }
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
    }
}