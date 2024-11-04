using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models
{
    public class Linea
    {
        public int id
        {
            get; set;
        }
        public string texto
        {
            get; set;
        }
        public Propiedades propiedades
        {
            get; set;
        }
    }
}
