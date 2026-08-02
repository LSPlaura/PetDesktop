using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using PetDesktop.App.Enums;

namespace PetDesktop.App.Behaviors;

public class AnimationBehaviour : Behavior<Control>
{
    private DispatcherTimer? _timer;
    private int _speed = 2;
    private int _screenWidth = 1920;
    private int _screenHeight = 1080;
    private int _secondsCounter = 0;
    public static readonly StyledProperty<MovementDirection> CurrentDirectionProperty =
        AvaloniaProperty.Register<AnimationBehaviour, MovementDirection>(
            nameof(CurrentDirection), 
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);
    public MovementDirection CurrentDirection
    {
        get => GetValue(CurrentDirectionProperty);
        set => SetValue(CurrentDirectionProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        UpdateScreenDimensions();
        
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(30)
        };
        
        _timer.Tick += OnTick;
        _timer.Start();
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        
        if (_timer != null)
        {
            _timer.Tick -= OnTick;
            _timer.Stop();
            _timer = null;
        }
    }

    private void UpdateScreenDimensions()
    {
        var window = AssociatedObject?.FindAncestorOfType<Window>();
        if (window == null) return;

        var currentScreen = window.Screens.ScreenFromWindow(window) ?? window.Screens.Primary;
        if (currentScreen != null)
        {
            _screenWidth = currentScreen.Bounds.Width;
            _screenHeight = currentScreen.Bounds.Height;
        }
    }
    
    private void OnTick(object? sender, EventArgs e)
    {
        var window = AssociatedObject?.FindAncestorOfType<Window>();
        if (window == null) return;

        _secondsCounter++;
        
        if (_secondsCounter >= 1000)
        {
            CurrentDirection = (MovementDirection)Random.Shared.Next(0, 4);
            _secondsCounter = 0;
        } 
        
        switch (CurrentDirection) 
        {
           case MovementDirection.Right:
               MoveRight(window); break;
           
           case MovementDirection.Left:
               MoveLeft(window); break;
           
           case MovementDirection.Up:
               MoveUp(window); break;
           
           case MovementDirection.Down:
               MoveDown(window); break; 
        }
    }

    private void MoveRight(Window window)
    {
        int nextX = window.Position.X + _speed;
    
        if (nextX + (int)window.Width > _screenWidth)
        {
            CurrentDirection = MovementDirection.Left;
            MoveLeft(window);
            return;
        }

        window.Position = new PixelPoint(nextX, window.Position.Y);
    }

    private void MoveLeft(Window window)
    {
        int nextX = window.Position.X - _speed;
    
        if (nextX < 0)
        {
            CurrentDirection = MovementDirection.Right;
            MoveRight(window);
            return;
        }

        window.Position = new PixelPoint(nextX, window.Position.Y);
    }

    private void MoveUp(Window window)
    {
        int nextY = window.Position.Y + _speed;
    
        if (nextY + (int)window.Height > _screenHeight)
        {
            CurrentDirection = MovementDirection.Down;
            return;
        }

        window.Position = new PixelPoint(window.Position.X, nextY);
    }

    private void MoveDown(Window window)
    {
        int nextY = window.Position.Y - _speed;
    
        if (nextY < 0)
        {
            CurrentDirection = MovementDirection.Up;
            MoveDown(window);
            return;
        }

        window.Position = new PixelPoint(window.Position.X, nextY);
    }
}