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
using Windows.Storage.Pickers;

namespace Generico_Front.Views;

public sealed partial class GafasPage : Page
{

    Config.Config config;
    public GafasViewModel ViewModel
    {
        get;
    }

    public GafasPage()
    {
        ViewModel = App.GetService<GafasViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
    }

    //ACCIONES GRAPHICS (PLAY y STOP)
    private void btnPlay_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {

    }

}
