using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;

namespace PetDesktop.App.Views;

public partial class ErrorWindow : Window
{
    public ErrorWindow(string message)
    {
        InitializeComponent();
        ErrorText.Text = message;
    }
    
    private void OnAcceptClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }
}