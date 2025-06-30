using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Data;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class RodillosViewModel : ObservableRecipient
{
    private readonly Controllers.Data.RodilloController dataController;
    private readonly TipoController tipoController;
    private readonly Controllers.Graphics.BrainStorm.RodilloController graphicController;
    private Config.Config config;

    public RodillosViewModel()
    {
        dataController = Controllers.Data.RodilloController.GetInstance();
        tipoController = TipoController.GetInstance();
        graphicController = Controllers.Graphics.BrainStorm.RodilloController.GetInstance();
        cargos = new ObservableCollection<Cargo>();
        allCargos = new List<Cargo>();
        Tipos = new ObservableCollection<Tipo>();
        config = Config.Config.GetInstance();

    }

    public ObservableCollection<Cargo> cargos
    {
        get;
    }
    public List<Cargo> allCargos;
    public ObservableCollection<Tipo> Tipos
    {
        get;
    }

    // Comando para cargar la lista de rodillos
    [RelayCommand]
    public Task CargarRodillos()
    {
        Rodillo rodillo = dataController.GetRodillo(config.RotulacionSettings.RutaRodillo);
        cargos.Clear();
        allCargos.Clear();

        // Agrega cada rodillo a la colección (esto actualizará la vista)
        foreach (var cargo in rodillo.cargos)
        {
            cargos.Add(cargo);
            allCargos.Add(cargo);
        }
        foreach (var cargo in allCargos)
        {
            cargo.orden = allCargos.IndexOf(cargo) + 1;
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task GuardarRodillo(Rodillo editado)
    {
        //Acciones de guardado
        dataController.SaveRodillo(config.RotulacionSettings.RutaRodillo, editado);
        return Task.CompletedTask;
    }

    //OPCIONES PARA OBTENER TIPOS
    [RelayCommand]
    public Task CargarTipos()
    {
        var listaTipos = tipoController.GetAllAsync();
        Tipos.Clear();
        foreach (var tipo in listaTipos)
        {
            if (tipo.seAplicaA.Equals("Rodillos"))
            {
                Tipos.Add(tipo);
            }
        }
        if (Tipos.Count == 0)
        {
            Tipo porDefecto = new Tipo();
            porDefecto.id = 0;
            porDefecto.seAplicaA = "Rodillos";
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


    //Acciones Gráficas
    public void Entra(Rodillo rodillo, string tipo)
    {
        graphicController.Entra(rodillo, tipo);
    }
    public void Sale()
    {
        graphicController.Sale();
    }
}
