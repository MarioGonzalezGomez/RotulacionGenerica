using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Graphics.Conexion;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class VariosViewModel : ObservableRecipient
{
    private readonly Controllers.Data.LocalizacionController dataController;
    private readonly VariosController graphicController;
    private Config.Config config;

    public VariosViewModel()
    {
        dataController = Controllers.Data.LocalizacionController.GetInstance();
        graphicController = VariosController.GetInstance();
        config = Config.Config.GetInstance();
        Localizaciones = new List<Localizacion>();
    }
    public List<Localizacion> Localizaciones;

    // Comando para cargar la lista de localizacions
    [RelayCommand]
    public Task CargarLocalizaciones()
    {
        var listaLocalizaciones = dataController.GetAllAsync();
        Localizaciones.Clear();

        // Agrega cada localizacion a la colección (esto actualizará la vista)
        foreach (var localizacion in listaLocalizaciones)
        {
            Localizaciones.Add(localizacion);
        }
        return Task.CompletedTask;
    }

    // Comando para agregar un nuevo localizacion
    [RelayCommand]
    public async Task GuardarLocalizacion(Localizacion localizacion)
    {
        if (localizacion.id == 0)
        {
            dataController.Post(localizacion);
        }
        else
        {
            dataController.Put(localizacion);
        }
        await CargarLocalizaciones();
    }

    [RelayCommand]
    public async Task EliminarLocalizacion(Localizacion localizacion)
    {

        dataController.Delete(localizacion);

        await CargarLocalizaciones();
    }

    public async void ActualizarPosiciones(List<Localizacion> ordenadas)
    {

        for (int i = 0; i < ordenadas.Count; i++)
        {
            ordenadas[i].posicion = i + 1;
            dataController.Put(ordenadas[i]);
        }
        await CargarLocalizaciones();
    }

    //Acciones Gráficas
    public void EntraReloj(DateTime hora)
    {
        graphicController.EntraReloj(hora);
    }
    public void SaleReloj()
    {
        graphicController.SaleReloj();
    }

    public void EntraFecha(DateTime fecha)
    {
        graphicController.EntraFecha(fecha);
    }
    public void SaleFecha()
    {
        graphicController.SaleFecha();
    }

    public void EntraCrono(string tipo, TimeSpan crono)
    {
        graphicController.EntraCrono(tipo, crono);
    }
    public void SaleCrono()
    {
        graphicController.SaleCrono();
    }

    public void EntraLocalizacion(Localizacion localizacion)
    {
        //graphicController.Entra(localizacion);
    }
    public void SaleLocalizacion()
    {
        // graphicController.Sale();
    }
}
