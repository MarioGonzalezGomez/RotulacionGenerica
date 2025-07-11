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

    public override string? ToString()
    {
        string result = nombre;
        foreach (var persona in personas)
        {
            result += $"\\n{persona.nombre}";
        }
        return result;
    }
    public string ToStringDestacadoNombre()
    {
        string result = $"\\\\f<TitularRodillo>{nombre}\\\\f";
        foreach (var persona in personas)
        {
            result += $"\\n\\\\f<CuerpoRodillo>{persona.nombre}\\\\f";
        }
        return result;
    }
}
