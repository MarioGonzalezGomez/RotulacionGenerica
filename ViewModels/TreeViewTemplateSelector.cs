using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace Generico_Front.ViewModels;
public class TreeViewTemplateSelector : DataTemplateSelector
{
    public DataTemplate CargoTemplate
    {
        get; set;
    }
    public DataTemplate PersonaTemplate
    {
        get; set;
    }
    public DataTemplate PremioTemplate
    {
        get; set;
    }
    public DataTemplate NominadoTemplate
    {
        get; set;
    }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is Cargo)
        {
            return CargoTemplate;
        }
        else if (item is Premio)
        {
            return PremioTemplate;
        }
        else if (item is Nominado)
        {
            return NominadoTemplate;
        }
        else if (item is Persona)
        {
            return PersonaTemplate;
        }

        return base.SelectTemplateCore(item);
    }
}
