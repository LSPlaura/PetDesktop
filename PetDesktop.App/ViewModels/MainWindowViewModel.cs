using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetDesktop.App.Enums;
using PetDesktop.App.Views;
using PetDesktop.Back.Config;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.Pet;
using PetDesktop.Back.Services.SpriteSheet;
using Serilog;
using SkiaSharp;

namespace PetDesktop.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private static readonly ILogger Log = Serilog.Log.ForContext<MainWindowViewModel>();

    private readonly PetCreationOrchestrator _orchestrator;
    private readonly PetService _petService;
    private readonly SpriteSheetService _spriteSheetService;

    private DispatcherTimer? _frameTimer;
    private SKBitmap? _currentSheetBitmap;
    private int _frameWidth;
    private int _frameHeight;
    private int _totalFrames;
    private int _currentFrameIndex = 0;

    [ObservableProperty]
    private Pet? _actualPet;

    [ObservableProperty]
    private MovementDirection _petDirection = MovementDirection.Right;

    // Propiedad enlazada al control <Image Source="{Binding CurrentFrame}" />
    [ObservableProperty]
    private Bitmap? _currentFrame;

    public MainWindowViewModel(
        PetCreationOrchestrator orchestrator, 
        PetService petService, 
        SpriteSheetService spriteSheetService)
    {
        _orchestrator = orchestrator;
        _petService = petService;
        _spriteSheetService = spriteSheetService;

        Log.Information("Inicializando MainWindowViewModel...");
        SetupFrameTimer();
    }

    private void SetupFrameTimer()
    {
        Log.Debug("Configurando el timer de animaciones a ~10 FPS (100ms)...");
        // Timer a ~10 FPS (100ms por fotograma)
        _frameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _frameTimer.Tick += OnFrameTick;
    }

    [RelayCommand]
    private async Task Start()
    {
        if (!_petService.ExistsAsync(Config.DefaultPetName).Result.Value)
        {
            Log.Information("Iniciando la creación de la mascota por defecto...");
            var result = await _orchestrator.PetCreatorInitAsync();
            if (result.IsFailure)
            {
                Log.Error("Falló la inicialización del creador de mascotas: {Error}", result.Error);
                ShowErrorAndShutdown("No se pudo inicializar el creador de mascotas.");
                return;
            }

            Log.Information("Obteniendo datos de la mascota '{DefaultPetName}'...", Config.DefaultPetName);
            var pet = await _petService.GetById(Config.DefaultPetName);
            if (pet == null)
            {
                Log.Error("No se encontró la mascota '{DefaultPetName}' en la base de datos.", Config.DefaultPetName);
                ShowErrorAndShutdown($"No se encontró la mascota '{Config.DefaultPetName}'.");
                return;
            }

            ActualPet = pet;
            Log.Information("Mascota '{PetName}' cargada con éxito. Cargando animación inicial 'WalkRight'...", ActualPet.Name);
        
            // Cargar la animación inicial por defecto
            await LoadAnimationAsync("WalkRight");
            _frameTimer?.Start();
            Log.Information("Timer de fotogramas iniciado.");
        }
        else
        {
            var pet = await _petService.GetById(Config.DefaultPetName);
            if (pet == null)
            {
                Log.Error("No se encontró la mascota '{DefaultPetName}' en la base de datos.", Config.DefaultPetName);
                ShowErrorAndShutdown($"No se encontró la mascota '{Config.DefaultPetName}'.");
                return;
            }

            ActualPet = pet;
            Log.Information("Mascota '{PetName}' cargada con éxito. Cargando animación inicial 'WalkRight'...", ActualPet.Name);
        
            // Cargar la animación inicial por defecto
            await LoadAnimationAsync("WalkRight");
            _frameTimer?.Start();
            Log.Information("Timer de fotogramas iniciado.");
        }
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

        Log.Debug("Dirección cambiada a {Direction}. Solicitando carga de animación '{AnimationName}'...", value, animationName);
        _ = LoadAnimationAsync(animationName);
    }

    private async Task LoadAnimationAsync(string animationName)
    {
        if (ActualPet == null)
        {
            Log.Warning("Intento de cargar la animación '{AnimationName}' sin tener una mascota activa.", animationName);
            return;
        }

        var key = (ActualPet.Name, animationName);
        Log.Debug("Buscando el SpriteSheet con clave ({PetName}, {AnimationName})...", key.Item1, key.Item2);
        var spriteSheetResult = await _spriteSheetService.GetByIdAsync(key);

        if (spriteSheetResult.IsFailure)
        {
            Log.Error("Error al obtener la animación '{AnimationName}' del servicio: {Error}", animationName, spriteSheetResult.Error);
            return;
        }

        var spriteSheet = spriteSheetResult.Value;

        if (!File.Exists(spriteSheet.Route))
        {
            Log.Error("El archivo de imagen del SpriteSheet no existe en la ruta: {Route}", spriteSheet.Route);
            return;
        }

        try
        {
            Log.Debug("Cargando el archivo de imagen desde {Route}...", spriteSheet.Route);
            // Cargar la imagen completa del SpriteSheet en memoria
            using var stream = File.OpenRead(spriteSheet.Route);
            var newBitmap = SKBitmap.Decode(stream);

            if (newBitmap != null)
            {
                _currentSheetBitmap?.Dispose();
                _currentSheetBitmap = newBitmap;
                _frameWidth = spriteSheet.FrameWidth;
                _frameHeight = spriteSheet.FrameHeight;
                _totalFrames = _currentSheetBitmap.Width / _frameWidth;
                _currentFrameIndex = 0;

                Log.Information("Animación '{AnimationName}' cargada correctamente. Dimensiones: {Width}x{Height}, Cuadros totales: {TotalFrames}", 
                    animationName, _frameWidth, _frameHeight, _totalFrames);

                RenderCurrentFrame();
            }
            else
            {
                Log.Error("SkiaSharp no pudo decodificar el archivo de imagen en {Route}", spriteSheet.Route);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Excepción al cargar la animación '{AnimationName}' desde la ruta {Route}", animationName, spriteSheet.Route);
        }
    }

    private void OnFrameTick(object? sender, EventArgs e)
    {
        if (_currentSheetBitmap == null || _totalFrames == 0) return;

        _currentFrameIndex = (_currentFrameIndex + 1) % _totalFrames;
        RenderCurrentFrame();
    }

    private void RenderCurrentFrame()
    {
        if (_currentSheetBitmap == null) return;

        try
        {
            // 1. Definir el área del cuadro a recortar en el SpriteSheet
            int sourceX = _currentFrameIndex * _frameWidth;
            var cropRect = SKRectI.Create(sourceX, 0, _frameWidth, _frameHeight);

            // 2. Extraer el sub-bitmap con SkiaSharp
            using var croppedSKBitmap = new SKBitmap(_frameWidth, _frameHeight);
            if (!_currentSheetBitmap.ExtractSubset(croppedSKBitmap, cropRect))
            {
                Log.Warning("SkiaSharp falló al extraer el cuadro {FrameIndex} con rectángulo {CropRect}", _currentFrameIndex, cropRect);
                return;
            }

            // 3. Convertir el SKBitmap a Avalonia Bitmap
            using var image = SKImage.FromBitmap(croppedSKBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var memoryStream = new MemoryStream();
            
            data.SaveTo(memoryStream);
            memoryStream.Position = 0;

            // 4. Asignar el nuevo Bitmap a la UI
            CurrentFrame = new Bitmap(memoryStream);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error durante el renderizado del fotograma actual ({FrameIndex})", _currentFrameIndex);
        }
    }

    private void ShowErrorAndShutdown(string message)
    {
        Log.Warning("Mostrando pantalla de error y cerrando aplicación. Mensaje: {ErrorMessage}", message);
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow?.Hide();
            var errorWin = new ErrorWindow(message);
            errorWin.Show();
        }
    }
}