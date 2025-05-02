using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Data;
public class LocalizacionController : IBaseController<Localizacion>
{
    private static LocalizacionController instance;
    private Cliente c;

    private LocalizacionController()
    {
        c = Cliente.GetInstance();
    }

    public static LocalizacionController GetInstance()
    {
        if (instance == null)
        {
            instance = new LocalizacionController();
        }
        return instance;
    }


    public List<Localizacion> GetAllAsync()
    {
        string result = c.GetAsync("api/Localizacion").Result;
        List<Localizacion> localizacion;
        if (!string.IsNullOrEmpty(result))
        {
            localizacion = JsonSerializer.Deserialize<List<Localizacion>>(result);
        }
        else
        {
            localizacion = new List<Localizacion>();
        }
        return localizacion;
    }
    public Localizacion GetById(int id)
    {
        string result = c.GetAsync($"api/Localizacion/{id}").Result;
        Localizacion localizacion = JsonSerializer.Deserialize<Localizacion>(result);
        return localizacion;
    }
    public Localizacion Post(Localizacion localizacion)
    {
        string json = JsonSerializer.Serialize(localizacion);
        string result = c.PostAsync($"api/Localizacion", json).Result;
        Localizacion localizacionResult = JsonSerializer.Deserialize<Localizacion>(result);
        return localizacionResult;
    }
    public Localizacion Put(Localizacion localizacion)
    {
        string json = JsonSerializer.Serialize(localizacion);
        _ = c.PutAsync($"api/Localizacion/{localizacion.id}", json).Result;
        return localizacion;
    }
    public Localizacion Delete(Localizacion localizacion)
    {
        string result = c.DeleteAsync($"api/Localizacion/{localizacion.id}").Result;
        return localizacion;
    }
}
