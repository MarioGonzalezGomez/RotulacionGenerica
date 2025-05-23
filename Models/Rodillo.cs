﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Rodillo
{
    public List<Cargo> cargos
    {
        get; set;
    }
    public Rodillo()
    {
        cargos = new List<Cargo>();
    }

    public override string? ToString()
    {
        string result = "";
        foreach (var cargo in cargos)
        {
            result += $"{cargo.ToString()}\\n\\n";
        }
        return result;
    }

    public string ToStringDestacadoNombre()
    {
        string result = "";
        foreach (var cargo in cargos)
        {
            result += $"{cargo.ToStringDestacadoNombre()}\\n\\n";
        }
        return result;
    }
}
