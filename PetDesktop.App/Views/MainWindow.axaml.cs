using Avalonia.Controls;
using PetDesktop.App.ViewModels;

namespace PetDesktop.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        //DataContext = new MainWindowViewModel();
    }
}