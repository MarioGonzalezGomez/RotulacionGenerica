using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Controllers.Data;
using Generico_Front.Graphics.Builders;
using Generico_Front.Graphics.Conexion;

namespace Generico_Front.Controllers.Graphics.Prime;
public class RotuloController
{
    private static RotuloController instance;
    private PrimeBuilder builder;
    private PrimeConexion conexion;

    private RotuloController()
    {
        builder = PrimeBuilder.GetInstance();
        conexion = PrimeConexion.GetInstance();
    }

    public static RotuloController GetInstance()
    {
        if (instance == null)
        {
            instance = new RotuloController();
        }
        return instance;
    }

    //ESPECIALES
    public void Reset()
    {
        conexion.EnviarMensaje(builder.Reset());
    }

    public void ConectarBrainStorm()
    {
        if (conexion.activo)
        {
            conexion.CerrarConexion();
        }
        conexion.AbrirConexion();
    }
}
