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

    public void Entra(Rodillo rodillo, string tipo)
    {
        switch (tipo)
        {
            case "Vertical":
                conexion.EnviarMensaje(builder.RodilloEntra(rodillo));
                break;
            case "Horizontal":
                //TODO: Meter datos extra desde config
                EnviarRodilloAsync(rodillo, 2, 4, 5);
                break;
            case "Paginado":
                conexion.EnviarMensaje(builder.RodilloEntraPaginado(rodillo));
                break;
            default:
                break;
        }
    }

    public async Task EnviarRodilloAsync(Rodillo rodillo, int columnas, int maxLinesPerBloque, double tiempo = 1.0)
    {
        var señales = builder.RodilloEntraHorizontal(rodillo, columnas, maxLinesPerBloque, tiempo);

        foreach (var señal in señales)
        {
            conexion.EnviarMensaje(señal);
            await Task.Delay((int)(tiempo * 1000));
        }
    }

    public void Sale()
    {
        conexion.EnviarMensaje(builder.RodilloSale());
    }
}
