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
public class FaldonController
{
    private static FaldonController instance;
    private BSBuilder builder;
    private BSConexion conexion;

    private FaldonController()
    {
        builder = BSBuilder.GetInstance();
        conexion = BSConexion.GetInstance();
    }

    public static FaldonController GetInstance()
    {
        if (instance == null)
        {
            instance = new FaldonController();
        }
        return instance;
    }
    public void Entra(Faldon faldon)
    {
        conexion.EnviarMensaje(builder.FaldonEntra(faldon));
    }

    public void Sale()
    {
        conexion.EnviarMensaje(builder.FaldonSale());
    }

}
