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
    private Config.Config config;

    private RodilloController()
    {
        builder = BSBuilder.GetInstance();
        conexion = BSConexion.GetInstance();
        config = Config.Config.GetInstance();
    }

    public static RodilloController GetInstance()
    {
        if (instance == null)
        {
            instance = new RodilloController();
        }
        return instance;
    }

    public void Entra(Rodillo rodillo, string tipo)
    {
        switch (tipo)
        {
            case "Vertical":
                conexion.EnviarMensaje(builder.RodilloEntra(rodillo));
                break;
            case "VerticalDestacado":
                conexion.EnviarMensaje(builder.RodilloEntraDestacadoNombre(rodillo));
                break;
            case "Horizontal":
                //TODO: Meter datos extra desde config
                EnviarRodilloAsync(rodillo, 2, 4);
                break;
            case "Paginado":
                conexion.EnviarMensaje(builder.RodilloEntraPaginado(rodillo));
                break;
            default:
                break;
        }
    }

    public async Task EnviarRodilloAsync(Rodillo rodillo, int columnas, int maxLinesPerBloque)
    {
        double tiempoTotal = config.RotulacionSettings.VelocidadRodillo;
        var señales = builder.RodilloEntraHorizontal(rodillo, columnas, maxLinesPerBloque);
        int cantidad = señales.Count;
        double delay = cantidad > 1 ? tiempoTotal / (cantidad - 1) : 0;

        foreach (var señal in señales)
        {
            conexion.EnviarMensaje(señal);
            await Task.Delay((int)(delay * 1000));
        }
    }

    public void Sale()
    {
        conexion.EnviarMensaje(builder.RodilloSale());
    }
}
