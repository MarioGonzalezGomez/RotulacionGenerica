using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class RodillosViewModel : ObservableRecipient
{
    private readonly Controllers.Data.RodilloController dataController;
    private readonly RodilloController graphicController;

    public RodillosViewModel()
    {
        dataController = Controllers.Data.RodilloController.GetInstance();
        graphicController = RodilloController.GetInstance();
        Rodillos = new ObservableCollection<Rodillo>();
        RodillosEmision = new ObservableCollection<Rodillo>();
    }

    public ObservableCollection<Rodillo> Rodillos
    {
        get;
    }
    public ObservableCollection<Rodillo> RodillosEmision
    {
        get;
    }

    // Comando para cargar la lista de rodillos
    [RelayCommand]
    public Task CargarRodillos()
    {
        var listaRodillos = dataController.GetAllAsync();
        Rodillos.Clear();

        // Agrega cada rodillo a la colección (esto actualizará la vista)
        foreach (var rodillo in listaRodillos)
        {
                Rodillos.Add(rodillo);
        }

        return Task.CompletedTask;
    }

    // Comando para agregar un nuevo rodillo
    [RelayCommand]
    public async Task GuardarRodillo(Rodillo rodillo)
    {
        if (rodillo.id == 0)
        {
            dataController.Post(rodillo);
        }
        else
        {
            dataController.Put(rodillo);
        }
        await CargarRodillos();
    }

    [RelayCommand]
    public async Task EliminarRodillo(Rodillo rodillo)
    {

        dataController.Delete(rodillo);

        await CargarRodillos();
    }


    //OPCIONES PARA EL FILTRADO
    public void RemoverNoCoincidentes(IEnumerable<Rodillo> filteredData)
    {
        for (int i = Rodillos.Count - 1; i >= 0; i--)
        {
            var item = Rodillos[i];
            if (!filteredData.Contains(item))
            {
                Rodillos.Remove(item);
            }
        }
    }

    public void RecuperarLista(IEnumerable<Rodillo> filteredData)
    {
        foreach (var item in filteredData)
        {
            if (!Rodillos.Contains(item))
            {
                try
                {
                   // Rodillos.Insert(item.posicion - 1, item);
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }
    }

    public async void ActualizarPosiciones(List<Rodillo> ordenados)
    {

        for (int i = 0; i < ordenados.Count; i++)
        {
          //  ordenados[i].posicion = i + 1;
            dataController.Put(ordenados[i]);
        }
        await CargarRodillos();
    }
}
