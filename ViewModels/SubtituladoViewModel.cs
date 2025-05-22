using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Data;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class SubtituladoViewModel : ObservableRecipient
{
    private readonly Controllers.Data.SubtituladoController dataController;
    private readonly Controllers.Graphics.BrainStorm.SubtituladoController graphicController;
    private Config.Config config;

    public SubtituladoViewModel()
    {
        graphicController = Controllers.Graphics.BrainStorm.SubtituladoController.GetInstance();
        dataController = Controllers.Data.SubtituladoController.GetInstance();
        config = Config.Config.GetInstance();
        Subtitulos = new ObservableCollection<string>();
        allSubtitulos = new List<string>();
    }
    public List<string> allSubtitulos;
    public ObservableCollection<string> Subtitulos
    {
        get;
    }

    // Comando para cargar la lista de rodillos
    [RelayCommand]
    public Task CargarSubtitulos()
    {
        List<string> subtitulos = dataController.GetSubtitulos(config.RotulacionSettings.RutaSubtitulos);
        Subtitulos.Clear();
        allSubtitulos.Clear();

        foreach (var linea in subtitulos)
        {
            Subtitulos.Add(linea);
            allSubtitulos.Add(linea);
        }

        return Task.CompletedTask;
    }

    //OPCIONES PARA EL FILTRADO
    public void RemoverNoCoincidentes(IEnumerable<string> filteredData)
    {
        for (int i = Subtitulos.Count - 1; i >= 0; i--)
        {
            var item = Subtitulos[i];
            if (!filteredData.Contains(item))
            {
                Subtitulos.Remove(item);
            }
        }
    }

    public void RecuperarLista(IEnumerable<string> filteredData)
    {
        foreach (var item in filteredData)
        {
            if (!Subtitulos.Contains(item))
            {
                try
                {
                    int orden = allSubtitulos.IndexOf(item);
                    Subtitulos.Insert(orden, item);
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }
    }


    //Acciones Gráficas
    public void Entra(string subtitulo)
    {
        graphicController.Entra(subtitulo);
    }
    public void Encadena(string subtitulo)
    {
        graphicController.Encadena(subtitulo);
    }
    public void Sale()
    {
        graphicController.Sale();
    }
}
