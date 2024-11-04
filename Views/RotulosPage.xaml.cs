using Generico_Front.Controllers.Data;
using Generico_Front.Models;
using Generico_Front.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Generico_Front.Views;

public sealed partial class RotulosPage : Page
{
    public RotulosViewModel ViewModel
    {
        get;
    }

    public RotulosPage()
    {
        ViewModel = App.GetService<RotulosViewModel>();
        InitializeComponent();
        LVRotulos.ItemsSource = ViewModel.Rotulos;
    }
}
