using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Localizacion
{
    public int id
    {
        get; set;
    }
    public int posicion
    {
        get; set;
    }
    public string principal
    {
        get; set;
    }
    public string secundario
    {
        get; set;
    }
}
