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
    private Window? _window;
    
    private int _screenLeft;
    private int _screenRight;
    private int _screenTop;
    private int _screenBottom;
    private double _screenScaling = 1.0;
    
    private int _speed = 4;
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

        if (AssociatedObject is Window win)
        {
            InitializeWindow(win);
        }
        else if (AssociatedObject != null)
        {
            AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
        }

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(30)
        };
    
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.AttachedToVisualTree -= OnAttachedToVisualTree;
            var win = AssociatedObject.FindAncestorOfType<Window>();
            if (win != null)
            {
                InitializeWindow(win);
            }
        }
    }

    private void InitializeWindow(Window window)
    {
        _window = window;

        if (_window.IsLoaded) UpdateScreenDimensions();
        else _window.Loaded += OnWindowLoaded;
    }

    private void OnWindowLoaded(object? sender, EventArgs e)
    {
        if (_window != null)
        {
            _window.Loaded -= OnWindowLoaded;
            UpdateScreenDimensions();
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        
        if (_window != null)
        {
            _window.Loaded -= OnWindowLoaded;
            _window = null;
        }

        if (_timer != null)
        {
            _timer.Tick -= OnTick;
            _timer.Stop();
            _timer = null;
        }
    }
    
    private void UpdateScreenDimensions()
    {
        if (_window == null) return;

        var currentScreen = _window.Screens.ScreenFromWindow(_window) ?? _window.Screens.Primary;
        if (currentScreen != null)
        {
            _screenScaling = currentScreen.Scaling;
            _screenLeft = currentScreen.Bounds.X;
            _screenTop = currentScreen.Bounds.Y;
            _screenRight = _screenLeft + currentScreen.Bounds.Width;
            _screenBottom = _screenTop + currentScreen.Bounds.Height;
        }
    }
    
    private void OnTick(object? sender, EventArgs e)
    {
        if (_window == null) return;

        _secondsCounter++;
        
        if (_secondsCounter >= 500)
        {
            CurrentDirection = (MovementDirection)Random.Shared.Next(0, 4);
            _secondsCounter = 0;
        } 
        
        switch (CurrentDirection) 
        {
           case MovementDirection.Right:
               MoveRight(_window); break;
           
           case MovementDirection.Left:
               MoveLeft(_window); break;
           
           case MovementDirection.Up:
               MoveUp(_window); break;
           
           case MovementDirection.Down:
               MoveDown(_window); break; 
        }
    }

    private void MoveRight(Window window)
    {
        double width = window.Bounds.Width > 0 ? window.Bounds.Width : window.Width;
        int windowPixelWidth = (int)Math.Ceiling(width * _screenScaling);
        
        if (windowPixelWidth <= 0) return;

        int nextX = window.Position.X + _speed;

        if (nextX + windowPixelWidth >= _screenRight)
        {
            int clampedX = _screenRight - windowPixelWidth;
            window.Position = new PixelPoint(clampedX - _speed, window.Position.Y);
            CurrentDirection = MovementDirection.Left;
            return;
        }

        window.Position = new PixelPoint(nextX, window.Position.Y);
    }

    private void MoveLeft(Window window)
    {
        int nextX = window.Position.X - _speed;
    
        if (nextX <= _screenLeft)
        {
            window.Position = new PixelPoint(_screenLeft + _speed, window.Position.Y);
            CurrentDirection = MovementDirection.Right;
            return;
        }

        window.Position = new PixelPoint(nextX, window.Position.Y);
    }

    private void MoveUp(Window window)
    {
        int nextY = window.Position.Y - _speed;
    
        if (nextY <= _screenTop)
        {
            window.Position = new PixelPoint(window.Position.X, _screenTop + _speed);
            CurrentDirection = MovementDirection.Down;
            return;
        }

        window.Position = new PixelPoint(window.Position.X, nextY);
    }

    private void MoveDown(Window window)
    {
        double height = window.Bounds.Height > 0 ? window.Bounds.Height : window.Height;
        int windowPixelHeight = (int)Math.Ceiling(height * _screenScaling);

        if (windowPixelHeight <= 0) return;

        int nextY = window.Position.Y + _speed;
    
        if (nextY + windowPixelHeight >= _screenBottom)
        {
            int clampedY = _screenBottom - windowPixelHeight;
            window.Position = new PixelPoint(window.Position.X, clampedY - _speed);
            CurrentDirection = MovementDirection.Up;
            return;
        }

        window.Position = new PixelPoint(window.Position.X, nextY);
    }
}