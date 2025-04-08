using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Data;
public class TipoController : IBaseController<Tipo>
{
    private static TipoController instance;
    private Cliente c;

    private TipoController()
    {
        c = Cliente.GetInstance();
    }

    public static TipoController GetInstance()
    {
        if (instance == null)
        {
            instance = new TipoController();
        }
        return instance;
    }


    public List<Tipo> GetAllAsync()
    {
        string result = c.GetAsync("api/Tipos").Result;
        List<Tipo> tipo;
        if (!string.IsNullOrEmpty(result))
        {
            tipo = JsonSerializer.Deserialize<List<Tipo>>(result);
        }
        else
        {
            tipo = new List<Tipo>();
        }
        return tipo;
    }

    public Tipo GetById(int id)
    {
        string result = c.GetAsync($"api/Tipos/{id}").Result;
        Tipo tipo = JsonSerializer.Deserialize<Tipo>(result);
        return tipo;
    }
    public Tipo Post(Tipo tipo)
    {
        string json = JsonSerializer.Serialize(tipo);
        string result = c.PostAsync($"api/Tipos", json).Result;
        Tipo tipoResult = JsonSerializer.Deserialize<Tipo>(result);
        return tipoResult;
    }
    public Tipo Put(Tipo tipo)
    {
        string json = JsonSerializer.Serialize(tipo);
        _ = c.PutAsync($"api/Tipos/{tipo.id}", json).Result;
        return tipo;
    }
    public Tipo Delete(Tipo tipo)
    {
        string result = c.DeleteAsync($"api/Tipos/{tipo.id}").Result;
        return tipo;
    }
}
