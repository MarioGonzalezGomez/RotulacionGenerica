using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class PremiosViewModel : ObservableRecipient
{
    private readonly Controllers.Data.PremioController dataController;
    private readonly PremioController graphicController;
    private Config.Config config;

    public PremiosViewModel()
    {
        dataController = Controllers.Data.PremioController.GetInstance();
        graphicController = PremioController.GetInstance();
        premios = new ObservableCollection<Premio>();
        allPremios = new List<Premio>();
        config = Config.Config.GetInstance();
    }

    public ObservableCollection<Premio> premios
    {
        get;
    }
    public List<Premio> allPremios;


    // Comando para cargar la lista de premios
    [RelayCommand]
    public Task CargarPremios()
    {
        List<Premio> dataPremios = dataController.GetPremios(config.RotulacionSettings.RutaPremios);
        premios.Clear();
        allPremios.Clear();

        //Agrega cada premio a la colección (esto actualizará la vista)
        foreach (var premio in dataPremios)
        {
            premios.Add(premio);
            allPremios.Add(premio);
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task GuardarPremios(List<Premio> premios)
    {
        dataController.SavePremios(config.RotulacionSettings.RutaPremios, premios);
    }


    //OPCIONES PARA EL FILTRADO
    public void RemoverNoCoincidentes(IEnumerable<Premio> filteredData)
    {
        for (int i = premios.Count - 1; i >= 0; i--)
        {
            var item = premios[i];
            if (!filteredData.Contains(item))
            {
                premios.Remove(item);
            }
        }
    }

    public void RecuperarLista(IEnumerable<Premio> filteredData)
    {
        foreach (var item in filteredData)
        {
            if (!premios.Contains(item))
            {
                try
                {
                    int orden = filteredData.ToList().IndexOf(item);
                    premios.Insert(orden, item);
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }
    }
}
