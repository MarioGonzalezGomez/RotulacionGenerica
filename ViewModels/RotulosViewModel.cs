using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class RotulosViewModel : ObservableRecipient
{
    private readonly Controllers.Data.RotuloController dataController;
    private readonly Controllers.Data.TipoController tipoController;
    private readonly BSController graphicController;

    public RotulosViewModel()
    {
        dataController = Controllers.Data.RotuloController.GetInstance();
        tipoController = Controllers.Data.TipoController.GetInstance();
        graphicController = BSController.GetInstance();
        Tipos = new ObservableCollection<Tipo>();
        Rotulos = new ObservableCollection<Rotulo>();
        allRotulos = new List<Rotulo>();
    }
    public List<Rotulo> allRotulos;
    public ObservableCollection<Rotulo> Rotulos
    {
        get;
    }
    public ObservableCollection<Tipo> Tipos
    {
        get;
    }

    // Comando para cargar la lista de rotulos
    [RelayCommand]
    public Task CargarRotulos()
    {
        var listaRotulos = dataController.GetAllAsync();
        Rotulos.Clear();

        // Agrega cada rotulo a la colección (esto actualizará la vista)
        foreach (var rotulo in listaRotulos)
        {
            Rotulos.Add(rotulo);
            allRotulos.Add(rotulo);
        }

        return Task.CompletedTask;
    }

    // Comando para agregar un nuevo rotulo
    [RelayCommand]
    public async Task GuardarRotulo(Rotulo rotulo)
    {
        if (rotulo.id == 0)
        {
            dataController.Post(rotulo);
        }
        else
        {
            dataController.Put(rotulo);
        }
        await CargarRotulos();
    }

    [RelayCommand]
    public async Task EliminarRotulo(Rotulo rotulo)
    {

        dataController.Delete(rotulo);

        await CargarRotulos();
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
    public void RemoverNoCoincidentes(IEnumerable<Rotulo> filteredData)
    {
        for (int i = Rotulos.Count - 1; i >= 0; i--)
        {
            var item = Rotulos[i];
            if (!filteredData.Contains(item))
            {
                Rotulos.Remove(item);
            }
        }
    }

    public void RecuperarLista(IEnumerable<Rotulo> filteredData)
    {
        foreach (var item in filteredData)
        {
            if (!Rotulos.Contains(item))
            {
                Rotulos.Insert(item.posicion - 1, item);
            }
        }
    }

    public async void ActualizarPosiciones(List<Rotulo> ordenados)
    {

        for (int i = 0; i < ordenados.Count; i++)
        {
            ordenados[i].posicion = i + 1;
            dataController.Put(ordenados[i]);
        }
        await CargarRotulos();
    }
}
