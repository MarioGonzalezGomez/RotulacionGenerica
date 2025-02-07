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
        cargos = new ObservableCollection<Cargo>();
        allCargos = new List<Cargo>();
        config = Config.Config.GetInstance();
    }

    public ObservableCollection<Cargo> cargos
    {
        get;
    }
    public List<Cargo> allCargos;


    // Comando para cargar la lista de premios
    [RelayCommand]
    public Task CargarPremios()
    {
        Premio premio = dataController.GetPremio(config.RotulacionSettings.RutaPremios);
        cargos.Clear();
        allCargos.Clear();

        // Agrega cada premio a la colección (esto actualizará la vista)
        // foreach (var cargo in premio.cargos)
        // {
        //  cargos.Add(cargo);
        //  allCargos.Add(cargo);
        // }

        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task GuardarPremio(Premio premio)
    {
        dataController.SavePremio(config.RotulacionSettings.RutaPremios, premio);
    }


    //OPCIONES PARA EL FILTRADO
    public void RemoverNoCoincidentes(IEnumerable<Cargo> filteredData)
    {
        for (int i = cargos.Count - 1; i >= 0; i--)
        {
            var item = cargos[i];
            if (!filteredData.Contains(item))
            {
                cargos.Remove(item);
            }
        }
    }

    public void RecuperarLista(IEnumerable<Cargo> filteredData)
    {
        foreach (var item in filteredData)
        {
            if (!cargos.Contains(item))
            {
                try
                {
                    cargos.Insert(item.orden - 1, item);
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }
    }

    public async void ActualizarPosiciones(List<Cargo> ordenados)
    {

        for (int i = 0; i < ordenados.Count; i++)
        {
            ordenados[i].orden = i + 1;

        }
        // await CargarPremios();
    }
}
