using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Cargo
{
    public string nombre
    {
        get; set;
    }
    public List<Persona> personas
    {
        get; set;
    }
    public int orden
    {
        get; set;
    }

    public Cargo()
    {
        personas = new List<Persona>();
    }
}
