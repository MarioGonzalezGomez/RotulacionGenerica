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

        // Agrega cada usuario a la colección (esto actualizará la vista)
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
}
