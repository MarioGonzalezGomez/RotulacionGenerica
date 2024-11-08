using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;
using Windows.Media.Protection.PlayReady;

namespace Generico_Front.Controllers.Data;
public class PropiedadesController : IBaseController<Propiedades>
{
    private static PropiedadesController instance;
    private Cliente c;

    private PropiedadesController()
    {
        c = Cliente.GetInstance();
    }

    public static PropiedadesController GetInstance()
    {
        if (instance == null)
        {
            instance = new PropiedadesController();
        }
        return instance;
    }


    public List<Propiedades> GetAllAsync()
    {
        string result = c.GetAsync("api/Propiedades").Result;
        List<Propiedades> propiedades = JsonSerializer.Deserialize<List<Propiedades>>(result);
        return propiedades;
    }
    public Propiedades GetById(int id)
    {
        string result = c.GetAsync($"api/Propiedades/{id}").Result;
        Propiedades propiedades = JsonSerializer.Deserialize<Propiedades>(result);
        return propiedades;
    }
    public Propiedades Post(Propiedades propiedades)
    {
        string json = JsonSerializer.Serialize(propiedades);
        string result = c.PostAsync($"api/Propiedades", json).Result;
        Propiedades propiedadesResult = JsonSerializer.Deserialize<Propiedades>(result);
        return propiedadesResult;
    }
    public Propiedades Put(Propiedades propiedades)
    {
        string json = JsonSerializer.Serialize(propiedades);
        string result = c.PutAsync($"api/Propiedades/{propiedades.id}", json).Result;
        Propiedades propiedadesResult = JsonSerializer.Deserialize<Propiedades>(result);
        return propiedadesResult;
    }
    public Propiedades Delete(Propiedades propiedades)
    {
        string result = c.DeleteAsync($"api/Propiedades/{propiedades.id}").Result;
        Propiedades propiedadesResult = JsonSerializer.Deserialize<Propiedades>(result);
        return propiedadesResult;
    }
}
