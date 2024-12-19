using System.Diagnostics;
using System.Text.Json;
using Generico_Front.Controllers.Data;
using Generico_Front.Models;
using Generico_Front.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;

namespace Generico_Front.Views;

public sealed partial class RodillosPage : Page
{
    public RodillosViewModel ViewModel
    {
        get;
    }

    public RodillosPage()
    {
        ViewModel = App.GetService<RodillosViewModel>();
        InitializeComponent();

    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {

    }
}
