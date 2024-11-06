using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;

public class Rotulo
{
    public int id
    {
        get; set;
    }
    public int posicion
    {
        get; set;
    }
    public Tipo? tipo
    {
        get; set;
    }
    public List<Linea>? lineas
    {
        get; set;
    }

}
