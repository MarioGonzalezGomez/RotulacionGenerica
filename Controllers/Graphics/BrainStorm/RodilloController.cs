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
public class RodilloController
{
    private static RodilloController instance;
    private BSBuilder builder;
    private BSConexion conexion;

    private RodilloController()
    {
        builder = BSBuilder.GetInstance();
        conexion = BSConexion.GetInstance();
    }

    public static RodilloController GetInstance()
    {
        if (instance == null)
        {
            instance = new RodilloController();
        }
        return instance;
    }

    public void Entra(Rodillo rodillo)
    {
        conexion.EnviarMensaje(builder.RodilloEntra(rodillo));
    }

    public void Sale()
    {
        conexion.EnviarMensaje(builder.RodilloSale());
    }
}
