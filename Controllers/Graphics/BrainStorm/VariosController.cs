using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Controllers.Data;
using Generico_Front.Graphics.Builders;
using Generico_Front.Graphics.Conexion;
using Generico_Front.Models;
using Microsoft.UI.Xaml.Controls;

namespace Generico_Front.Controllers.Graphics.BrainStorm;
public class VariosController
{
    private static VariosController instance;
    private BSBuilder builder;
    private BSConexion conexion;

    private VariosController()
    {
        builder = BSBuilder.GetInstance();
        conexion = BSConexion.GetInstance();
    }

    public static VariosController GetInstance()
    {
        if (instance == null)
        {
            instance = new VariosController();
        }
        return instance;
    }
   
    //TIEMPOS
    public void EntraReloj(DateTime hora)
    {
        conexion.EnviarMensaje(builder.RelojEntra(hora));
    }
    public void SaleReloj()
    {
        conexion.EnviarMensaje(builder.RelojSale());
    }

    public void EntraFecha(DateTime fecha)
    {
        conexion.EnviarMensaje(builder.FechaEntra(fecha));
    }
    public void SaleFecha()
    {
        conexion.EnviarMensaje(builder.FechaSale());
    }

    public void EntraCrono(string tipo, TimeSpan crono)
    {
        conexion.EnviarMensaje(builder.CronoEntra(tipo, crono));
    }
    public void SaleCrono()
    {
        conexion.EnviarMensaje(builder.CronoSale());
    }
}
