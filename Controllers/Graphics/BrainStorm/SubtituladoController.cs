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
public class SubtituladoController
{
    private static SubtituladoController instance;
    private BSBuilder builder;
    private BSConexion conexion;

    private SubtituladoController()
    {
        builder = BSBuilder.GetInstance();
        conexion = BSConexion.GetInstance();
    }

    public static SubtituladoController GetInstance()
    {
        if (instance == null)
        {
            instance = new SubtituladoController();
        }
        return instance;
    }

    public void Entra(string subtitulo)
    {
        conexion.EnviarMensaje(builder.SubtituloEntra(subtitulo));
    }

    public void Encadena(string subtitulo)
    {
        conexion.EnviarMensaje(builder.SubtituloEncadena(subtitulo));
    }

    public void Sale()
    {
        conexion.EnviarMensaje(builder.SubtituloSale());
    }

}
