using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Data;
public class RotuloController : IBaseController<Rotulo>
{
    private static RotuloController instance;
    private Cliente c;

    private RotuloController()
    {
        c = Cliente.GetInstance();
    }

    public static RotuloController GetInstance()
    {
        if (instance == null)
        {
            instance = new RotuloController();
        }
        return instance;
    }


    public List<Rotulo> GetAllAsync()
    {
        string result = c.GetAsync("api/Rotulos").Result;
        List<Rotulo> rotulo;
        if (!string.IsNullOrEmpty(result))
        {
            rotulo = JsonSerializer.Deserialize<List<Rotulo>>(result);
        }
        else
        {
            rotulo = new List<Rotulo>();
        }
        return rotulo;
    }
    public Rotulo GetById(int id)
    {
        string result = c.GetAsync($"api/Rotulos/{id}").Result;
        Rotulo rotulo = JsonSerializer.Deserialize<Rotulo>(result);
        return rotulo;
    }
    public Rotulo Post(Rotulo rotulo)
    {
        string json = JsonSerializer.Serialize(rotulo);
        string result = c.PostAsync($"api/Rotulos", json).Result;
        Rotulo rotuloResult = JsonSerializer.Deserialize<Rotulo>(result);
        return rotuloResult;
    }
    public Rotulo Put(Rotulo rotulo)
    {
        string json = JsonSerializer.Serialize(rotulo);
        _ = c.PutAsync($"api/Rotulos/{rotulo.id}", json).Result;
        return rotulo;
    }
    public Rotulo Delete(Rotulo rotulo)
    {
        string result = c.DeleteAsync($"api/Rotulos/{rotulo.id}").Result;
        return rotulo;
    }
}
