using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class RotulosViewModel : ObservableRecipient
{
    private readonly Controllers.Data.RotuloController dataController;
    private readonly Controllers.Graphics.RotuloController graphicController;

    public RotulosViewModel()
    {
        dataController = Controllers.Data.RotuloController.GetInstance();
        graphicController = Controllers.Graphics.RotuloController.GetInstance();
        Rotulos = new ObservableCollection<Rotulo>();
    }

    public ObservableCollection<Rotulo> Rotulos
    {
        get;
    }

    // Comando para cargar la lista de rotulos
    [RelayCommand]
    public async Task CargarRotulos()
    {
        var listaRotulos = dataController.GetAll();
        Rotulos.Clear();

        // Agrega cada rotulo a la colección (esto actualizará la vista)
        foreach (var rotulo in listaRotulos)
        {
            Rotulos.Add(rotulo);
        }
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
        await CargarRotulos(); // Recargar la lista después de guardar
    }

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
                Rotulos.Add(item);
            }
        }
    }
}
