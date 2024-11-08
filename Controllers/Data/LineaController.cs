using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;
using Microsoft.UI.Xaml.Shapes;

namespace Generico_Front.Controllers.Data;
public class LineaController : IBaseController<Linea>
{
    private static LineaController instance;
    private Cliente c;

    private LineaController()
    {
        c = Cliente.GetInstance();
    }

    public static LineaController GetInstance()
    {
        if (instance == null)
        {
            instance = new LineaController();
        }
        return instance;
    }


    public List<Linea> GetAllAsync()
    {
        string result = c.GetAsync("api/Lineas").Result;
        List<Linea> linea = JsonSerializer.Deserialize<List<Linea>>(result);
        return linea;
    }
    public Linea GetById(int id)
    {
        string result = c.GetAsync($"api/Lineas/{id}").Result;
        Linea linea = JsonSerializer.Deserialize<Linea>(result);
        return linea;
    }
    public Linea Post(Linea Linea)
    {
        string json = JsonSerializer.Serialize(Linea);
        string result = c.PostAsync($"api/Lineas", json).Result;
        Linea lineaResult = JsonSerializer.Deserialize<Linea>(result);
        return lineaResult;
    }
    public Linea Put(Linea linea)
    {
        string json = JsonSerializer.Serialize(linea);
        string result = c.PutAsync($"api/Lineas/{linea.id}", json).Result;
        Linea lineaResult = JsonSerializer.Deserialize<Linea>(result);
        return lineaResult;
    }
    public Linea Delete(Linea linea)
    {
        string result = c.DeleteAsync($"api/Lineas/{linea.id}").Result;
        Linea lineaResult = JsonSerializer.Deserialize<Linea>(result);
        return lineaResult;
    }
}
