using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Tipo
{
    public int id { get; set; }
    public string descripcion { get; set; }
    public int numLineas { get; set; }

    public override string? ToString() {
    return this.descripcion;
    }
}
