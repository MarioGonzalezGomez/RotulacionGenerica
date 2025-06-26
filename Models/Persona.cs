using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Persona
{
    public string nombre
    {
        get; set;
    }
    public int orden
    {
        get; set;
    }

    public override string? ToString() => base.ToString();
}
