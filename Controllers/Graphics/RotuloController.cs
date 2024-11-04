using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Controllers.Data;

namespace Generico_Front.Controllers.Graphics;
public class RotuloController
{
    private static RotuloController instance;
    private Cliente c;

    private RotuloController()
    {
        c = Cliente.GetInstance();
    }

    public static RotuloController GetInstance()
    {
        if (instance == null)
        {
            instance = new RotuloController();
        }
        return instance;
    }
}
