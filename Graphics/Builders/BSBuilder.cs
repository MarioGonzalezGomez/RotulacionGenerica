using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Graphics.Builders;
public class BSBuilder
{
    private static BSBuilder? instance;
    private string _bd;
    private Config.Config config;

    private BSBuilder()
    {
        config = Config.Config.GetInstance();
        _bd = config.BrainStormOptions.Database;
    }

    public static BSBuilder GetInstance()
    {
        if (instance == null)
        {
            instance = new BSBuilder();
        }
        return instance;
    }

    //Mensajes comunes
    public string Reset()
    {
        return EventRunBuild("RESET");
    }
    public string Entra(string objeto)
    {
        return EventRunBuild($"{objeto}/ENTRA");
    }
    public string Encadena(string objeto)
    {
        return EventRunBuild($"{objeto}/ENCADENA");
    }
    public string Sale(string objeto)
    {
        return EventRunBuild($"{objeto}/SALE");
    }

    //CONSTRUCTORES
    private string EventBuild(string objeto, string propiedad, string values, int tipoItem)
    {
        string setOgo = "";
        if (tipoItem == 1)
        {
            setOgo = "itemset('";
        }
        if (tipoItem == 2)
        {
            setOgo = "itemgo('";
        }
        return $"{setOgo}<{_bd}>{objeto}','{propiedad}',{values});";
    }

    private string EventBuild(string objeto, string propiedad, int tipoItem)
    {
        string setOgo = "";
        if (tipoItem == 1)
        {
            setOgo = "itemset('";
        }
        if (tipoItem == 2)
        {
            setOgo = "itemgo('";
        }
        return $"{setOgo}<{_bd}>{objeto}','{propiedad}');";
    }

    private string EventRunBuild(string objeto, double delay = 0.0)
    {
        if (delay != 0.0)
        {
            return $"itemgo('<{_bd}>{objeto}','EVENT_RUN',0,{delay});";
        }
        else
        {
            return $"itemset('<{_bd}>{objeto}','EVENT_RUN');";
        }
    }
}
