using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Controllers.Data;
using Generico_Front.Graphics.Builders;
using Generico_Front.Graphics.Conexion;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Graphics.BrainStorm;
public class CrawlController
{
    private static CrawlController instance;
    private BSBuilder builder;
    private BSConexion conexion;

    private CrawlController()
    {
        builder = BSBuilder.GetInstance();
        conexion = BSConexion.GetInstance();
    }

    public static CrawlController GetInstance()
    {
        if (instance == null)
        {
            instance = new CrawlController();
        }
        return instance;
    }

    public void Entra(Crawl crawl)
    {
        conexion.EnviarMensaje(builder.CrawlEntra(crawl));
    }
    public void EntraLista(List<Crawl> crawls)
    {
        conexion.EnviarMensaje(builder.CrawlEntraLista(crawls));
    }

    public void Sale()
    {
        conexion.EnviarMensaje(builder.CrawlSale());
    }
}
