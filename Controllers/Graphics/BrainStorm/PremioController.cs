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
public class PremioController
{
    private static PremioController instance;
    private BSBuilder builder;
    private BSConexion conexion;

    private PremioController()
    {
        builder = BSBuilder.GetInstance();
        conexion = BSConexion.GetInstance();
    }

    public static PremioController GetInstance()
    {
        if (instance == null)
        {
            instance = new PremioController();
        }
        return instance;
    }


    public void CategoriaEntra(Premio premio)
    {
        conexion.EnviarMensaje(builder.CategoriaEntra(premio));
    }
    public void CategoriaSale()
    {
        conexion.EnviarMensaje(builder.CategoriaSale());
    }

    public void NominadoEntra(Premio premio, Nominado nominado)
    {
        conexion.EnviarMensaje(builder.NominadoEntra(premio, nominado));
    }
    public void NominadoSale()
    {
        conexion.EnviarMensaje(builder.NominadoSale());
    }

    public void EntregadoresEntra(Premio premio)
    {
        conexion.EnviarMensaje(builder.EntregadoresEntra(premio));
    }
    public void EntregadoresSale()
    {
        conexion.EnviarMensaje(builder.EntregadoresSale());
    }

    public void GanadorEntra(Premio premio, Nominado nominado)
    {
        conexion.EnviarMensaje(builder.GanadorEntra(premio, nominado));
    }
    public void GanadorSale()
    {
        conexion.EnviarMensaje(builder.GanadorSale());
    }

}
