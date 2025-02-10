using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Controllers.Data;
using Generico_Front.Graphics.Builders;
using Generico_Front.Graphics.Conexion;

namespace Generico_Front.Controllers.Graphics.BrainStorm;
//Clase para signals generales o especiales que afecten a varios tipos de rotulo
public class BSController
{
    private static BSController instance;
    private BSBuilder builder;
    private BSConexion conexion;

    private BSController()
    {
        builder = BSBuilder.GetInstance();
        conexion = BSConexion.GetInstance();
    }

    public static BSController GetInstance()
    {
        if (instance == null)
        {
            instance = new BSController();
        }
        return instance;
    }

    //ESPECIALES
    public void Reset()
    {
        conexion.EnviarMensaje(builder.Reset());
    }

    //MOSCA
    public void MoscaEntra()
    {
        conexion.EnviarMensaje(builder.MoscaEntra());
    }
    public void MoscaSale()
    {
        conexion.EnviarMensaje(builder.MoscaSale());
    }

}
