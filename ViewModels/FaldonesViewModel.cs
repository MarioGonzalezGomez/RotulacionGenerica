using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class FaldonesViewModel : ObservableRecipient
{
    private readonly Controllers.Data.FaldonController dataController;
    private readonly Controllers.Data.TipoController tipoController;
    private readonly FaldonController graphicController;

    public FaldonesViewModel()
    {
        dataController = Controllers.Data.FaldonController.GetInstance();
        tipoController = Controllers.Data.TipoController.GetInstance();
        graphicController = FaldonController.GetInstance();
        Tipos = new ObservableCollection<Tipo>();
        Faldones = new ObservableCollection<Faldon>();
        allFaldones = new List<Faldon>();
    }
    public List<Faldon> allFaldones;
    public ObservableCollection<Faldon> Faldones
    {
        get;
    }
    public ObservableCollection<Tipo> Tipos
    {
        get;
    }

    // Comando para cargar la lista de faldones
    [RelayCommand]
    public Task CargarFaldones()
    {
        var listaFaldones = dataController.GetAllAsync();
        Faldones.Clear();
        allFaldones.Clear();

        // Agrega cada faldon a la colección (esto actualizará la vista)
        foreach (var faldon in listaFaldones)
        {
            Faldones.Add(faldon);
            allFaldones.Add(faldon);
        }

        return Task.CompletedTask;
    }

    // Comando para agregar un nuevo faldon
    [RelayCommand]
    public async Task GuardarFaldon(Faldon faldon)
    {
        if (faldon.id == 0)
        {
            dataController.Post(faldon);
        }
        else
        {
            dataController.Put(faldon);
        }
        await CargarFaldones();
    }

    [RelayCommand]
    public async Task EliminarFaldon(Faldon faldon)
    {

        dataController.Delete(faldon);

        await CargarFaldones();
    }

    //OPCIONES PARA OBTENER TIPOS
    [RelayCommand]
    public Task CargarTipos()
    {
        var listaTipos = tipoController.GetAllAsync();
        Tipos.Clear();

        foreach (var tipo in listaTipos)
        {
            Tipos.Add(tipo);
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task GuardarTipo(Tipo tipo)
    {
        if (tipo.id == 0)
        {
            tipoController.Post(tipo);
        }
        else
        {
            tipoController.Put(tipo);
        }
        await CargarTipos();
    }

    //OPCIONES PARA EL FILTRADO
    public void RemoverNoCoincidentes(IEnumerable<Faldon> filteredData)
    {
        for (int i = Faldones.Count - 1; i >= 0; i--)
        {
            var item = Faldones[i];
            if (!filteredData.Contains(item))
            {
                Faldones.Remove(item);
            }
        }
    }

    public void RecuperarLista(IEnumerable<Faldon> filteredData)
    {
        foreach (var item in filteredData)
        {
            if (!Faldones.Contains(item))
            {
                try
                {
                    Faldones.Insert(item.posicion - 1, item);
                }
                catch (ArgumentOutOfRangeException) { }

            }
        }
    }

    public async void ActualizarPosiciones(List<Faldon> ordenados)
    {

        for (int i = 0; i < ordenados.Count; i++)
        {
            ordenados[i].posicion = i + 1;
            dataController.Put(ordenados[i]);
        }
        await CargarFaldones();
    }

    //Acciones Gráficas
    public void Entra(Faldon faldon)
    {
        graphicController.Entra(faldon);
    }
    public void Sale()
    {
        graphicController.Sale();
    }
}
