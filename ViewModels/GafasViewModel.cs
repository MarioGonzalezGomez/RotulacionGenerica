using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class GafasViewModel : ObservableRecipient
{
    private readonly GafasController graphicController;
    private Config.Config config;

    public GafasViewModel()
    {
        graphicController = GafasController.GetInstance();
        config = Config.Config.GetInstance();
    }
}
