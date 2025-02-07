using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Controllers.Data;
using Generico_Front.Graphics.Builders;
using Generico_Front.Graphics.Conexion;

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

}
