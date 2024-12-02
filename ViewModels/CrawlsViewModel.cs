using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Generico_Front.Controllers.Graphics.BrainStorm;
using Generico_Front.Models;

namespace Generico_Front.ViewModels;

public partial class CrawlsViewModel : ObservableRecipient
{
    private readonly Controllers.Data.CrawlController dataController;
    private readonly CrawlController graphicController;

    public CrawlsViewModel()
    {
        dataController = Controllers.Data.CrawlController.GetInstance();
        graphicController = CrawlController.GetInstance();
        Crawls = new ObservableCollection<Crawl>();
        CrawlsEmision = new ObservableCollection<Crawl>();
        allCrawls = new List<Crawl>();
    }
    public List<Crawl> allCrawls;
    public ObservableCollection<Crawl> Crawls
    {
        get;
    }
    public ObservableCollection<Crawl> CrawlsEmision
    {
        get;
    }


    // Comando para cargar la lista de crawls
    [RelayCommand]
    public Task CargarCrawls()
    {
        var listaCrawls = dataController.GetAllAsync();
        Crawls.Clear();

        // Agrega cada crawl a la colección (esto actualizará la vista)
        foreach (var crawl in listaCrawls)
        {
            Crawls.Add(crawl);
            allCrawls.Add(crawl);
        }

        return Task.CompletedTask;
    }

    // Comando para agregar un nuevo crawl
    [RelayCommand]
    public async Task GuardarCrawl(Crawl crawl)
    {
        if (crawl.id == 0)
        {
            dataController.Post(crawl);
        }
        else
        {
            dataController.Put(crawl);
        }
        await CargarCrawls();
    }

    [RelayCommand]
    public async Task EliminarCrawl(Crawl crawl)
    {

        dataController.Delete(crawl);

        await CargarCrawls();
    }


    //OPCIONES PARA EL FILTRADO
    public void RemoverNoCoincidentes(IEnumerable<Crawl> filteredData)
    {
        for (int i = Crawls.Count - 1; i >= 0; i--)
        {
            var item = Crawls[i];
            if (!filteredData.Contains(item))
            {
                Crawls.Remove(item);
            }
        }
    }

    public void RecuperarLista(IEnumerable<Crawl> filteredData)
    {
        foreach (var item in filteredData)
        {
            if (!Crawls.Contains(item))
            {
                Crawls.Insert(item.posicion - 1, item);
            }
        }
    }

    public async void ActualizarPosiciones(List<Crawl> ordenados)
    {

        for (int i = 0; i < ordenados.Count; i++)
        {
            ordenados[i].posicion = i + 1;
            dataController.Put(ordenados[i]);
        }
        await CargarCrawls();
    }
}
