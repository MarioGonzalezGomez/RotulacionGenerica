using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
            if (i == 0)
            {
                mensaje += CambiaTexto($"Rotulo/Txt0{i + 2}", texto);
            }
            else if (i == 1)
            {
                mensaje += CambiaTexto($"Rotulo/Txt0{i}", texto);
            }
            else {
                mensaje += CambiaTexto($"Rotulo/Txt0{i + 1}", texto);
            }
           
        }
        mensaje += EventRunBuild($"Rotulo/numLineas/0{rotulo.lineas.Count}") + "\n";
        mensaje += EventRunBuild($"Rotulo/Tipos/Actuaciones") + "\n";
        //TODO:Esto no es generico, sustituir por el comentado despues
        //mensaje += Entra("Rotulo");
        mensaje += EventRunBuild("Rotulo/Entra_Actuaciones") + "\n";
        //mensaje += EventBuild("Rotulo", "OBJ_CULL", "False",1);

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
        if (!string.IsNullOrEmpty(faldon.titulo.texto))
        {
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

    //CREDITOS-RODILLO
    public string RodilloEntra(Rodillo rodillo)
    {
        string signal = EventBuild("Rodillo/Txt01", "TEXT_TRAVEL_DURATION_LIMIT", config.RotulacionSettings.VelocidadRodillo, 1);
        signal += $"\n{CambiaTexto("Rodillo/Txt01", rodillo.ToString())}";
        signal += $"\n{Entra("Rodillo")}";
        return signal;
    }
    public string RodilloSale()
    {
        return Sale("Rodillo");
    }

    //PREMIOS
    public string CategoriaEntra(Premio premio)
    {
        string mensaje = "";
        mensaje += CambiaTexto($"Rotulo/Txt/Txt01", premio.nombre);
        mensaje += EventRunBuild($"Rotulo/numLineas/01");
        mensaje += Entra("Rotulo");

        return mensaje;
    }
    public string CategoriaSale()
    {
        return Sale("Rotulo");
    }

    public string NominadoEntra(Premio premio, Nominado nominado)
    {
        string mensaje = "";
        mensaje += CambiaTexto($"Rotulo/Txt/Txt01", premio.nombre);
        mensaje += CambiaTexto($"Rotulo/Txt/Txt02", nominado.nombre);
        mensaje += CambiaTexto($"Rotulo/Txt/Txt03", nominado.trabajo);
        mensaje += EventRunBuild($"Rotulo/numLineas/03");
        mensaje += Entra("Rotulo");

        return mensaje;
    }
    public string NominadoSale()
    {
        return Sale("Rotulo");
    }

    public string EntregadoresEntra(Premio premio)
    {
        string mensaje = "";
        mensaje += CambiaTexto($"Rotulo/Txt/Txt01", premio.nombre);
        string entregadores = "";
        foreach (string entregador in premio.entregadores)
        {
            entregadores += $"{entregador}\n";
        }
        mensaje += CambiaTexto($"Rotulo/Txt/Txt02", entregadores);
        mensaje += EventRunBuild($"Rotulo/numLineas/02");
        mensaje += Entra("Rotulo");

        return mensaje;
    }
    public string EntregadoresSale()
    {
        return Sale("Rotulo");
    }

    //No Generico, adaptado para talia
    public string GanadorEntra(Premio premio, Nominado nominado)
    {
        string mensaje = "";
        mensaje += CambiaTexto($"Rotulo/Txt01", premio.nombre) + "\n";
        mensaje += CambiaTexto($"Rotulo/Txt02", nominado.nombre) + "\n";
        mensaje += CambiaTexto($"Rotulo/Txt03", nominado.trabajo) + "\n";

        if (premio.nombre.Length < 30 && nominado.nombre.Length < 22 && string.IsNullOrEmpty(nominado.trabajo))
        {
            mensaje += EventRunBuild($"Rotulo/numLineas/01") + "\n";
        }
        else if (premio.nombre.Length < 36 && nominado.nombre.Length < 30 && nominado.trabajo.Length < 91)
        {
            mensaje += EventRunBuild($"Rotulo/numLineas/02") + "\n";
        }
        else if (premio.nombre.Length < 57 && nominado.nombre.Length < 37 && nominado.trabajo.Length < 121)
        {
            mensaje += EventRunBuild($"Rotulo/numLineas/03") + "\n";
        }
        else
        {
            mensaje += EventRunBuild($"Rotulo/numLineas/04") + "\n";
        }

        if (string.IsNullOrEmpty(nominado.trabajo)) {
            mensaje += EventRunBuild($"Rotulo/numLineas/excepcion01") + "\n";
        }
       
       // if (premio.nombre.Length < 30) {
       //     mensaje += EventBuild("Rotulo/Txt01", "TEXT_BLOCK_MAXSIZE", "(337.4,130.34)", 1) + "\n";
       //     mensaje += EventBuild("Rotulo/Txt/Txt01", "OBJ_DISPLACEMENT[0]", 0, 1) + "\n";
       // } else {
       //     mensaje += EventBuild("Rotulo/Txt01", "TEXT_BLOCK_MAXSIZE", "(367.4, 140.34)", 1) + "\n";
       //     mensaje += EventBuild("Rotulo/Txt/Txt01", "OBJ_DISPLACEMENT[0]", -19, 1) + "\n";
       // }
       //
       // if (nominado.nombre.Length < 20)
       // {
       //     mensaje += EventBuild("Rotulo/Txt02", "TEXT_BLOCK_MAXSIZE", "(467, 40)", 1) + "\n";
       //     mensaje += EventBuild("Rotulo/Txt/Txt03", "OBJ_DISPLACEMENT[2]", -18, 1) + "\n";
       // }
       // else {
       //     mensaje += EventBuild("Rotulo/Txt02", "TEXT_BLOCK_MAXSIZE", "(467, 63)", 1) + "\n";
       //     mensaje += EventBuild("Rotulo/Txt/Txt03", "OBJ_DISPLACEMENT[2]",-32, 1) + "\n";
       // }

        mensaje += EventRunBuild("Rotulo/Entra",0.2);
      //  mensaje += EventBuild("Rotulo", "OBJ_CULL", "False", 1);
        return mensaje;
    }
    public string GanadorSale()
    {
        return Sale("Rotulo");
    }

    //VARIOS
    //TIEMPOS
    public string RelojEntra(DateTime hora)
    {
        //Cargar tiempo con hora.Hour; hora.Minute...
        string signal = "";
        signal += $"\n{Entra("Reloj")}";
        return signal;
    }
    public string RelojSale()
    {
        return Sale("Reloj");
    }

    public string FechaEntra(DateTime fecha)
    {
        //Cargarfecha
        string signal = "";
        signal += $"\n{Entra("Fecha")}";
        return signal;
    }
    public string FechaSale()
    {
        return Sale("Fecha");
    }

    public string CronoEntra(string tipo, TimeSpan crono)
    {
        //Cargar crono y tipo de crono
        string signal = "";
        signal += $"\n{Entra("Fecha")}";
        return signal;
    }
    public string CronoSale()
    {
        return Sale("Crono");
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
