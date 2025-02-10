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
public class RotuloController
{
    private static RotuloController instance;
    private BSBuilder builder;
    private BSConexion conexion;

    private RotuloController()
    {
        builder = BSBuilder.GetInstance();
        conexion = BSConexion.GetInstance();
    }

    public static RotuloController GetInstance()
    {
        if (instance == null)
        {
            instance = new RotuloController();
        }
        return instance;
    }

    public void Entra(Rotulo rotulo)
    {
        conexion.EnviarMensaje(builder.RotuloEntra(rotulo));
    }

    public void Sale()
    {
        conexion.EnviarMensaje(builder.RotuloSale());
    }

}
