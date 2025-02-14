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
    private readonly RotuloController graphicController;
    private readonly Graphics.Conexion.BSConexion conexion;

    public RotulosViewModel()
    {
        dataController = Controllers.Data.RotuloController.GetInstance();
        tipoController = Controllers.Data.TipoController.GetInstance();
        graphicController = RotuloController.GetInstance();
        conexion = Graphics.Conexion.BSConexion.GetInstance();
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
        allRotulos.Clear();

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
            if (tipo.seAplicaA.Equals("Rotulos"))
            {
                Tipos.Add(tipo);
            }
        }
        if (Tipos.Count == 0)
        {
            Tipo porDefecto = new Tipo();
            porDefecto.id = 0;
            porDefecto.seAplicaA = "Rotulos";
            porDefecto.descripcion = "Tipo por defecto";
            porDefecto.numLineas = 1;
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

    [RelayCommand]
    public async Task EliminarTipo(Tipo tipo)
    {
        tipoController.Delete(tipo);
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
                try
                {
                    Rotulos.Insert(item.posicion - 1, item);
                }
                catch (ArgumentOutOfRangeException) { }

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

    //Acciones Gráficas
    public void Entra(Rotulo rotulo)
    {
        graphicController.Entra(rotulo);
    }
    public void Sale()
    {
        graphicController.Sale();
    }
}
