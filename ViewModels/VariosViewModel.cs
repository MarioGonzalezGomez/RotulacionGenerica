using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class VariosViewModel : ObservableRecipient
{
    private readonly VariosController graphicController;
    private Config.Config config;

    public VariosViewModel()
    {
        graphicController = VariosController.GetInstance();
        config = Config.Config.GetInstance();
    }
}
