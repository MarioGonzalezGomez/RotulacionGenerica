using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Data;
public class RodilloController : IBaseController<Rodillo>
{
    private static RodilloController instance;
    private Cliente c;

    private RodilloController()
    {
        c = Cliente.GetInstance();
    }

    public static RodilloController GetInstance()
    {
        if (instance == null)
        {
            instance = new RodilloController();
        }
        return instance;
    }


    public List<Rodillo> GetAllAsync()
    {
        string result = c.GetAsync("api/Rodillos").Result;
        List<Rodillo> crawl;
        if (!string.IsNullOrEmpty(result))
        {
            crawl = JsonSerializer.Deserialize<List<Rodillo>>(result);
        }
        else
        {
            crawl = new List<Rodillo>();
        }
        return crawl;
    }
    public Rodillo GetById(int id)
    {
        string result = c.GetAsync($"api/Rodillos/{id}").Result;
        Rodillo crawl = JsonSerializer.Deserialize<Rodillo>(result);
        return crawl;
    }
    public Rodillo Post(Rodillo Rodillo)
    {
        string json = JsonSerializer.Serialize(Rodillo);
        string result = c.PostAsync($"api/Rodillos", json).Result;
        Rodillo crawlResult = JsonSerializer.Deserialize<Rodillo>(result);
        return crawlResult;
    }
    public Rodillo Put(Rodillo crawl)
    {
        string json = JsonSerializer.Serialize(crawl);
        string result = c.PutAsync($"api/Rodillos/{crawl.id}", json).Result;
        return crawl;
    }
    public Rodillo Delete(Rodillo Rodillo)
    {
        string result = c.DeleteAsync($"api/Rodillos/{Rodillo.id}").Result;
        return Rodillo;
    }
}
