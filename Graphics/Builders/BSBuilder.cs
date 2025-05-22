using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Models;
using Newtonsoft.Json.Linq;
using Windows.Foundation.Collections;
using Windows.System.Threading.Core;

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
            mensaje += CambiaTexto($"Rotulo/Txt0{i + 1}", texto) + "\n";
        }
        mensaje += EventRunBuild($"Rotulo/numLineas/0{rotulo.lineas.Count}") + "\n";
        mensaje += EventRunBuild($"Rotulo/Tipos/{rotulo.tipo.descripcion}") + "\n";
        mensaje += Entra("Rotulo");
        return mensaje;
    }
    public string RotuloEncadena(Rotulo rotulo)
    {
        string mensaje = "";
        mensaje += Encadena("Rotulo") + "\n";
        for (int i = 0; i < 4; i++)
        {
            string texto = i < rotulo.lineas.Count ? rotulo.lineas[i].texto : "";
            mensaje += CambiaTextoDelay($"Rotulo/Txt0{i + 1}", texto, 0.45) + "\n";
        }
        mensaje += EventRunBuild($"Rotulo/numLineas/0{rotulo.lineas.Count}", 0.5) + "\n";
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
    public string FaldonEncadena(Faldon faldon)
    {
        string mensaje = "";
        mensaje += Encadena("Faldon") + "\n";
        if (!string.IsNullOrEmpty(faldon.titulo.texto))
        {
            mensaje += CambiaTextoDelay("Faldon/Titular", faldon.titulo.texto, 0.45);
        }
        mensaje += CambiaTextoDelay($"Faldon/Txt", faldon.texto.texto, 0.45);
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
    public string RodilloEntraDestacadoNombre(Rodillo rodillo)
    {
        string signal = EventBuild("Rodillo/Txt01", "TEXT_TRAVEL_DURATION_LIMIT", config.RotulacionSettings.VelocidadRodillo, 1);
        signal += $"\n{CambiaTexto("Rodillo/Txt01", rodillo.ToStringDestacadoNombre())}";
        signal += $"\n{Entra("Rodillo")}";
        return signal;
    }
    public List<string> RodilloEntraHorizontal(Rodillo rodillo, int columnas = 2, int maxLinesPerBloque = 4)
    {
        List<string> señales = new List<string>();
        bool entra = true;
        // 1. Preparar todas las líneas
        List<string> lineas = new List<string>();
        foreach (Cargo c in rodillo.cargos)
        {
            if (c.personas.Count == 0)
            {
                lineas.Add($"{c.nombre}\\\\n");
            }
            else
            {
                foreach (Persona p in c.personas)
                {
                    string linea = $"{c.nombre}  \\\\f<Titular>{p.nombre}\\\\f\\\\n";
                    lineas.Add(linea);
                }
            }
        }

        // 2. Dividir en bloques considerando líneas dobles
        int lineaActual = 0;
        bool primeraTanda = true;

        while (lineaActual < lineas.Count)
        {
            List<string> bloques = new List<string>();
            List<int> lineasUsadas = new List<int>();

            for (int b = 0; b < columnas; b++)
            {
                bloques.Add("");
                lineasUsadas.Add(0);
            }

            int bloqueActual = 0;

            while (bloqueActual < columnas && lineaActual < lineas.Count)
            {
                string linea = lineas[lineaActual];
                bool esDoble = linea.Contains("#");
                int peso = esDoble ? 2 : 1;

                if (lineasUsadas[bloqueActual] + peso <= maxLinesPerBloque)
                {
                    string lineaLimpia = esDoble ? linea.Replace("#", "") : linea;
                    bloques[bloqueActual] += lineaLimpia;

                    if (esDoble)
                    {
                        bloques[bloqueActual] += "\\\\n"; // Segunda línea visual en blanco
                    }

                    lineasUsadas[bloqueActual] += peso;
                    lineaActual++;
                }
                else
                {
                    bloqueActual++;
                }
            }

            // 3. Construir señal
            string señal = "";
            for (int j = 0; j < columnas; j++)
            {
                int bloqueId = (primeraTanda ? j + 1 : columnas + j + 1);
                string ruta = $"Rodillo/Txt{bloqueId.ToString("D2")}";
                señal += $"\n{CambiaTexto(ruta, bloques[j].Replace("#", ""))}\n";
            }

            if (entra)
            {
                señal += $"\n{Entra("Rodillo")}";
                entra = false;
            }
            else
            {
                string evento = primeraTanda ? "Rodillo/Encadena/02" : "Rodillo/Encadena/01";
                señal += $"\n{EventRunBuild(evento, 0)}";
            }

            señales.Add(señal);
            primeraTanda = !primeraTanda;
        }

        return señales;
    }

    //TODO
    public string RodilloEntraPaginado(Rodillo rodillo)
    {
        return "";
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

        if (string.IsNullOrEmpty(nominado.trabajo))
        {
            mensaje += EventRunBuild($"Rotulo/numLineas/excepcion01") + "\n";
        }



        mensaje += EventRunBuild("Rotulo/Entra");
        return mensaje;
    }
    public string GanadorSale()
    {
        return Sale("Rotulo");
    }

    //SUBTITULADO
    public string SubtituloEntra(string subtitulo)
    {
        string mensaje = "";
        mensaje += CambiaTexto($"Subtitulos/Txt0", subtitulo) + "\n";
        mensaje += Entra("Subtitulos");
        return mensaje;
    }
    public string SubtituloEncadena(string subtitulo)
    {
        string mensaje = "";
        mensaje += Encadena("Subtitulo") + "\n";
        mensaje += CambiaTexto($"Subtitulos/Txt0", subtitulo) + "\n";
        return mensaje;
    }
    public string SubtituloSale()
    {
        return Sale("Subtitulos");
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

    //Adaptar para auqellos que tengan texto
    public string LocalizacionEntra(Localizacion local)
    {
        string signal = "";
        signal += CambiaTexto("Localizador/Txt01", local.principal) + "\n";
        signal += CambiaTexto("Localizador/Txt02", local.secundario) + "\n";
        signal += Entra("Localizador");
        return signal;
    }
    public string LocalizacionSale()
    {
        return Sale("Localizador");
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
    private string CambiaTextoDelay(string objeto, string texto, double delay)
    {
        return $"itemgo('<{_bd}>{objeto}','TEXT_STRING', '{texto}', 0, {delay});";
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

    private string ItemGoBuild(string objeto, string propiedad, string values, double tiempo, double delay = 0.0)
    {
        return $"itemgo('<{_bd}>{objeto}','{propiedad}', {values}, {tiempo}, {delay});";
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
