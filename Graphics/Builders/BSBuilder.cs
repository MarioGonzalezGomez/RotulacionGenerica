using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Models;
using Newtonsoft.Json.Linq;

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

    //ROTULO
    public string RotuloEntra(Rotulo rotulo)
    {
        string mensaje = "";
        for (int i = 0; i < 4; i++)
        {
            string texto = i < rotulo.lineas.Count ? rotulo.lineas[i].texto : "";
            mensaje += CambiaTexto($"Rotulo/linea0{i + 1}", texto);
        }
        mensaje += Entra("Rotulo");

        return mensaje;
    }
    public string RotuloSale()
    {
        return Sale("Rotulo");
    }
    //FALDON
    public string FaldonEntra(Faldon faldon)
    {
        string mensaje = "";
        if (!string.IsNullOrEmpty(faldon.titulo.texto)) {
            mensaje += CambiaTexto("Faldon/Titular", faldon.titulo.texto);
        }
        mensaje += CambiaTexto($"Faldon/Txt", faldon.texto.texto);
        mensaje += Entra("Faldon");

        return mensaje;
    }
    public string FaldonSale()
    {
        return Sale("Faldon");
    }
    //CRAWL
    public string CrawlEntra(Crawl crawl)
    {
        string mensaje = "";
        mensaje += EventBuild("Crawl/Txt", "TEXT_TRAVEL_SPEED", crawl.velocidad, 1);
        //TODO: Add los \f \s o modificaciones necesarios en caso de ser titulo
        string texto = crawl.esTitulo ? $"'{crawl.linea.texto}'" : $"'{crawl.linea.texto}'";
        mensaje += EventBuild("Crawl/Txt", "TEXT_FEEDER_RESET", 1);
        mensaje += EventBuild("Crawl/Txt", "TEXT_FEEDER_LIST", $"[{texto}]", 1);
        mensaje += Entra("Crawl");
        return mensaje;
    }
    public string CrawlEntraLista(List<Crawl> crawls)
    {
        string mensaje = "";
        string texto = "";
        mensaje += EventBuild("Crawl/Txt", "TEXT_TRAVEL_SPEED", config.RotulacionSettings.VelocidadCrawl, 1);

        foreach (Crawl c in crawls)
        {
            //TODO: Add los \f \s o modificaciones necesarios en caso de ser titulo
            texto += c.esTitulo ? $"'{c.linea.texto}'" : $"'{c.linea.texto}'";
            if (crawls.IndexOf(c) != crawls.Count - 1)
            {
                texto += ",";
            }
        }
        mensaje += EventBuild("Crawl/Txt", "TEXT_FEEDER_RESET", 1);
        mensaje += EventBuild("Crawl/Txt", "TEXT_FEEDER_LIST", $"[{texto}]", 1);
        mensaje += Entra("Crawl");
        return mensaje;
    }
    public string CrawlSale()
    {
        return Sale("Crawl");
    }
    //CREDITOS
    public string CreditosEntra()
    {
        return Entra("Rodillo");
    }
    public string CreditosSale()
    {
        return Sale("Rodillo");
    }

    //MOSCA
    public string MoscaEntra()
    {
        return Entra("Mosca");
    }
    public string MoscaSale()
    {
        return Sale("Mosca");
    }

    //Mensajes comunes
    public string Reset()
    {
        return EventRunBuild("Reset");
    }
    private string Entra(string objeto)
    {
        return EventRunBuild($"{objeto}/Entra");
    }
    private string Encadena(string objeto)
    {
        return EventRunBuild($"{objeto}/Encadena");
    }
    private string Sale(string objeto)
    {
        return EventRunBuild($"{objeto}/Sale");
    }

    private string CambiaTexto(string objeto, string texto)
    {
        return EventBuild(objeto, "TEXT_STRING", $"'{texto}'", 1);
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
    private string EventBuild(string objeto, string propiedad, int value, int tipoItem)
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
        return $"{setOgo}<{_bd}>{objeto}','{propiedad}',{value});";
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
