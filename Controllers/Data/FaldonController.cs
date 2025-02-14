using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Data;
public class FaldonController : IBaseController<Faldon>
{
    private static FaldonController instance;
    private Cliente c;

    private FaldonController()
    {
        c = Cliente.GetInstance();
    }

    public static FaldonController GetInstance()
    {
        if (instance == null)
        {
            instance = new FaldonController();
        }
        return instance;
    }


    public List<Faldon> GetAllAsync()
    {
        string result = c.GetAsync("api/Faldones").Result;
        List<Faldon> faldon;
        if (!string.IsNullOrEmpty(result))
        {
            faldon = JsonSerializer.Deserialize<List<Faldon>>(result);
        }
        else
        {
            faldon = new List<Faldon>();
        }
        return faldon;
    }
    public Faldon GetById(int id)
    {
        string result = c.GetAsync($"api/Faldones/{id}").Result;
        Faldon faldon = JsonSerializer.Deserialize<Faldon>(result);
        return faldon;
    }
    public Faldon Post(Faldon faldon)
    {
        string json = JsonSerializer.Serialize(faldon);
        string result = c.PostAsync($"api/Faldones", json).Result;
        Faldon faldonResult = JsonSerializer.Deserialize<Faldon>(result);
        return faldonResult;
    }
    public Faldon Put(Faldon faldon)
    {
        string json = JsonSerializer.Serialize(faldon);
        _ = c.PutAsync($"api/Faldones/{faldon.id}", json).Result;
        return faldon;
    }
    public Faldon Delete(Faldon faldon)
    {
        string result = c.DeleteAsync($"api/Faldones/{faldon.id}").Result;
        return faldon;
    }
}
