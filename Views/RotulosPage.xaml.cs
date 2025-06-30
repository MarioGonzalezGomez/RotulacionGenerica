using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Controllers.Data;
using Generico_Front.Models;
using Generico_Front.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.Contacts;

namespace Generico_Front.Views;

public sealed partial class RotulosPage : Page
{
    public RotulosViewModel ViewModel
    {
        get;
    }
    private Rotulo? seleccionado = null;
    private static bool rotuloIn = false;

    public RotulosPage()
    {
        ViewModel = App.GetService<RotulosViewModel>();
        InitializeComponent();
        IniciarListas();
    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarRotulos();
        LVRotulos.ItemsSource = ViewModel.Rotulos;
        ViewModel.CargarTipos();
        cmbTipos.ItemsSource = ViewModel.Tipos;
        LVRotulos.ContainerContentChanging += LVRotulos_ContainerContentChanging;
        cmbTiposEditor.ItemsSource = ViewModel.Tipos;
    }
    private void LVRotulos_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.ItemContainer is ListViewItem container && args.Item is Rotulo rotulo)
        {
            var ellipse = FindVisualChildByName<Ellipse>(container, "Ellipse");

            if (ellipse != null)
            {
                Tipo actual = ViewModel.Tipos.FirstOrDefault(t => string.Equals(t.descripcion, rotulo.tipo.descripcion));
                switch (ViewModel.Tipos.IndexOf(actual))
                {
                    case 0:
                        ellipse.Fill = new SolidColorBrush(Colors.DarkGreen);
                        break;
                    case 1:
                        ellipse.Fill = new SolidColorBrush(Colors.DarkBlue);
                        break;
                    case 2:
                        ellipse.Fill = new SolidColorBrush(Colors.DarkGoldenrod);
                        break;
                    case 3:
                        ellipse.Fill = new SolidColorBrush(Colors.DarkMagenta);
                        break;
                    case 4:
                        ellipse.Fill = new SolidColorBrush(Colors.DarkRed);
                        break;
                    case 5:
                        ellipse.Fill = new SolidColorBrush(Colors.DarkViolet);
                        break;
                    default:
                        ellipse.Fill = new SolidColorBrush(Colors.Gray);
                        break;
                }
            }
        }
    }

    private T FindVisualChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
    {
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild && typedChild.Name == name)
                return typedChild;

            var result = FindVisualChildByName<T>(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private void LVRotulos_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        var ordenado = sender.Items.Cast<Rotulo>().ToList();
        ViewModel.ActualizarPosiciones(ordenado);
    }

    private void LVRotulos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LVRotulos.SelectedIndex != -1)
        {
            seleccionado = (Rotulo)LVRotulos.SelectedItem;
            var textBoxes = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };

            for (int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i].Text = i < seleccionado.lineas.Count ? seleccionado.lineas[i].texto : string.Empty;
            }

            Tipo tipo = ViewModel.Tipos.FirstOrDefault(t => t.id == seleccionado.tipo.id);
            cmbTiposEditor.SelectedIndex = ViewModel.Tipos.IndexOf(tipo);
            txtorden.Text = seleccionado.posicion.ToString();
        }
    }

    private void LVRotulos_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var clickedItem = ((FrameworkElement)e.OriginalSource).DataContext;

        if (clickedItem != null)
        {
            LVRotulos.SelectedItem = clickedItem;
        }
    }

    private void MenuFlyoutEditar_Click(object sender, RoutedEventArgs e)
    {
        if (!tggEditor.IsOn)
        {
            tggEditor.IsOn = true;
        }
    }

    private void MenuFlyoutBorrar_Click(object sender, RoutedEventArgs e)
    {
        if (LVRotulos.SelectedItem != null)
        {
            Rotulo actual = LVRotulos.SelectedItem as Rotulo;
            EliminarRotulo(actual);
        }
    }

    //OPCIONES DE FILTRADO

    private void tggFiltrado_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggFiltrado.IsOn)
        {
            FiltradoPorNombre.Visibility = Visibility.Visible;
            FiltradoPorPosicion.Visibility = Visibility.Visible;
            FiltradoPorCargo.Visibility = Visibility.Visible;
        }
        else
        {
            FiltradoPorNombre.Visibility = Visibility.Collapsed;
            FiltradoPorPosicion.Visibility = Visibility.Collapsed;
            FiltradoPorCargo.Visibility = Visibility.Collapsed;
        }
    }
    private void FiltradoPorNombre_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allRotulos
    .Where(r => r.lineas[0].texto.IndexOf(FiltradoPorNombre.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allRotulos
    .Where(r => r.ToString().IndexOf(FiltradoPorCargo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPosicion_TextChanged(object sender, TextChangedEventArgs e)
    {
        IEnumerable<Rotulo> filtrada;
        if (int.TryParse(FiltradoPorPosicion.Text, out int value))
        {
            filtrada = ViewModel.allRotulos.Where(r => r.posicion >= value);
        }
        else
        {
            filtrada = ViewModel.allRotulos;
        }
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    //ACCIONES EN EDICIÓN
    private void BtnAddRotulo_Click(object sender, RoutedEventArgs e)
    {
        if (!tggEditor.IsOn) { tggEditor.IsOn = true; }
        if (ViewModel.Rotulos.Count > 0)
        {
            var maxPosicion = ViewModel.Rotulos.Max(r => r.posicion);
            txtorden.Text = $"{maxPosicion + 1}";
        }
        else
        {
            txtorden.Text = "1";
        }
    }

    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            linkAjustes.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            stckEditior3.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        else
        {
            linkAjustes.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior1.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior2.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            stckEditior3.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    private void cmbTiposEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbTiposEditor.SelectedIndex != -1)
        {
            Tipo seleccionado = (Tipo)cmbTiposEditor.SelectedValue;
            switch (seleccionado.numLineas)
            {
                case 1:
                    txtLinea1.Visibility = Visibility.Visible;
                    txtLinea2.Visibility = Visibility.Collapsed;
                    txtLinea3.Visibility = Visibility.Collapsed;
                    txtLinea4.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    txtLinea1.Visibility = Visibility.Visible;
                    txtLinea2.Visibility = Visibility.Visible;
                    txtLinea3.Visibility = Visibility.Collapsed;
                    txtLinea4.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    txtLinea1.Visibility = Visibility.Visible;
                    txtLinea2.Visibility = Visibility.Visible;
                    txtLinea3.Visibility = Visibility.Visible;
                    txtLinea4.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    txtLinea1.Visibility = Visibility.Visible;
                    txtLinea2.Visibility = Visibility.Visible;
                    txtLinea3.Visibility = Visibility.Visible;
                    txtLinea4.Visibility = Visibility.Visible;
                    break;
                default: break;
            }
        }
    }

    private void btnEliminarRotulo_Click(object sender, RoutedEventArgs e)
    {
        if (LVRotulos.SelectedItem != null)
        {
            Rotulo actual = LVRotulos.SelectedItem as Rotulo;
            EliminarRotulo(actual);
        }
    }
    private async void EliminarRotulo(Rotulo aEliminar)
    {
        Tip.Target = btnEliminarRotulo;
        Tip.Title = "Rótulo eliminado";
        await ViewModel.EliminarRotulo(aEliminar);
        ViewModel.ActualizarPosiciones(ViewModel.allRotulos);
        AbrirTip();
        var textLineas = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };
        foreach (var item in textLineas)
        {
            item.Text = "";
        }
        seleccionado = null;
    }

    private void btnModificarRotulo_Click(object sender, RoutedEventArgs e)
    {
        if (LVRotulos.SelectedItem != null)
        {
            Rotulo actual = LVRotulos.SelectedItem as Rotulo;
            Rotulo modificado = new Rotulo();
            var textLineas = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };
            modificado.id = actual.id;
            modificado.posicion = int.Parse(txtorden.Text);
            modificado.tipo = new Tipo();
            modificado.tipo.id = (cmbTiposEditor.SelectedValue as Tipo).id;
            modificado.tipo.seAplicaA = (cmbTiposEditor.SelectedValue as Tipo).seAplicaA;
            modificado.tipo.descripcion = (cmbTiposEditor.SelectedValue as Tipo).descripcion;
            modificado.tipo.numLineas = (cmbTiposEditor.SelectedValue as Tipo).numLineas;
            modificado.lineas = new List<Linea>();

            for (int i = 0; i < modificado.tipo.numLineas; i++)
            {
                Linea nuevaLinea = new Linea();
                if (i < actual.lineas.Count)
                {
                    nuevaLinea.id = actual.lineas[i].id;
                }
                else
                {
                    nuevaLinea.id = 0;
                }
                nuevaLinea.texto = textLineas[i].Text;
                modificado.lineas.Add(nuevaLinea);
            }
            if (string.Equals(actual.tipo.descripcion, modificado.tipo.descripcion))
            {
                ModificarRotulo(modificado);
            }
            else
            {
                EliminarRotulo(actual);
                modificado.id = 0;
                GuardarRotuloNuevo(modificado);
            }
            seleccionado = ViewModel.allRotulos.FirstOrDefault(rot => rot.lineas[0].id == modificado.lineas[0].id);
            LVRotulos.SelectedItem = seleccionado;
        }
    }
    private async void ModificarRotulo(Rotulo modificado)
    {
        Tip.Target = btnModificarRotulo;
        Tip.Title = "Modificado con éxito";
        await ViewModel.GuardarRotulo(modificado);
        AbrirTip();
    }

    private void btnGuardarRotulo_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtLinea1.Text))
        {
            Rotulo nuevoRotulo = new Rotulo();
            var textLineas = new[] { txtLinea1, txtLinea2, txtLinea3, txtLinea4 };
            var maxPosicion = ViewModel.Rotulos.Count > 0 ? ViewModel.Rotulos.Max(r => r.posicion) : 0;
            List<Linea> lineas = new List<Linea>();
            nuevoRotulo.id = 0;
            nuevoRotulo.posicion = maxPosicion + 1;
            nuevoRotulo.tipo = new Tipo();
            nuevoRotulo.tipo.id = (cmbTiposEditor.SelectedValue as Tipo).id;
            nuevoRotulo.tipo.seAplicaA = (cmbTiposEditor.SelectedValue as Tipo).seAplicaA;
            nuevoRotulo.tipo.descripcion = (cmbTiposEditor.SelectedValue as Tipo).descripcion;

            for (int i = 0; i < textLineas.Length; i++)
            {
                if (textLineas[i].Visibility == Visibility.Visible && !string.IsNullOrEmpty(textLineas[i].Text))
                {
                    Linea linea = new Linea();
                    linea.id = 0;
                    linea.texto = textLineas[i].Text;
                    lineas.Add(linea);
                }
            }
            nuevoRotulo.lineas = lineas;
            GuardarRotuloNuevo(nuevoRotulo);
            seleccionado = ViewModel.allRotulos[ViewModel.allRotulos.Count - 1];
            LVRotulos.SelectedItem = seleccionado;
        }
    }
    private async void GuardarRotuloNuevo(Rotulo nuevo)
    {
        Tip.Target = btnGuardarRotulo;
        Tip.Title = "Guardado con éxito";
        await ViewModel.GuardarRotulo(nuevo);
        AbrirTip();
    }

    private async void AbrirTip()
    {
        Tip.IsOpen = true;
        await Task.Delay(1500);
        Tip.IsOpen = false;
    }


    //ACCIONES EN AJUSTES ADICIONALES
    private void AbrirPanelAjustes(object sender, RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = true;
    }

    private void CerrarPanelAjustes(object sender, RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = false;
    }

    private void cmbTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbTipos.SelectedIndex != -1)
        {
            Tipo tipoSeleccionado = (Tipo)cmbTipos.SelectedItem;
            txtNombreTipo.Text = tipoSeleccionado.descripcion;
            cmbNumLineas.SelectedIndex = tipoSeleccionado.numLineas - 1;
            txtNombreTipo.Visibility = Visibility.Visible;
            stackNumlineas.Visibility = Visibility.Visible;
        }
        else
        {
            txtNombreTipo.Visibility = Visibility.Collapsed;
            stackNumlineas.Visibility = Visibility.Collapsed;
        }
    }

    private async void btnDeleteAjustes_Click(object sender, RoutedEventArgs e)
    {
        if (cmbTipos.SelectedItem != null)
        {
            Tipo selected = cmbTipos.SelectedItem as Tipo;
            await ViewModel.EliminarTipo(selected);
            Tip.Target = btnDeleteAjustes;
            Tip.Title = "Tipo eliminado";
            AbrirTip();
        }
        else
        {
            ShowDialog("Seleccionar tipo", "No se ha seleccionado ningún tipo para ser eliminado");
        }
    }

    private async void btnEditAjustes_Click(object sender, RoutedEventArgs e)
    {
        string nuevoNombre = txtNombreTipo.Text?.Trim();
        if (cmbTipos.SelectedItem != null && !string.IsNullOrEmpty(nuevoNombre))
        {
            Tipo seleccionado = cmbTipos.SelectedItem as Tipo;
            seleccionado.descripcion = nuevoNombre;
            seleccionado.numLineas = cmbNumLineas.SelectedItem != null ? cmbNumLineas.SelectedIndex + 1 : seleccionado.numLineas;
            await ViewModel.GuardarTipo(seleccionado);
            Tip.Target = btnEditAjustes;
            Tip.Title = "Editado con éxito";
            AbrirTip();
        }
        else
        {
            ShowDialog("Seleccionar tipo", "No se ha seleccionado ningún tipo para ser editado");
        }
    }

    private async void btnSaveAjustes_Click(object sender, RoutedEventArgs e)
    {
        string nuevoNombre = txtNombreTipo.Text?.Trim();

        if (!string.IsNullOrEmpty(nuevoNombre))
        {
            if (cmbTipos.SelectedItem != null)
            {
                Tipo seleccionado = cmbTipos.SelectedItem as Tipo;
                if (string.Equals(seleccionado.descripcion, nuevoNombre, StringComparison.OrdinalIgnoreCase))
                {
                    ShowDialog("Cambio de nombre", "No se puede guardar un nuevo tipo con el mismo nombre que uno ya registrado, por favor, edite el nombre actual si quiere guardarlo como un nuevo tipo de Rótulo.");
                    return;
                }
            }
            Tipo nuevoTipo = new Tipo();
            nuevoTipo.id = 0;
            nuevoTipo.seAplicaA = "Rotulos";
            nuevoTipo.descripcion = nuevoNombre;
            if (cmbNumLineas.SelectedItem != null)
            {
                nuevoTipo.numLineas = cmbNumLineas.SelectedIndex + 1;
            }
            else
            {
                ShowDialog("Falta número de líneas", "No se ha especificado el número de líneas que tendrá este nuevo Tipo. Por favor, rellene ese campo");
                return;
            }


            await ViewModel.GuardarTipo(nuevoTipo);
            Tip.Target = btnSaveAjustes;
            Tip.Title = "Creado con éxito";
            AbrirTip();
        }
        else
        {
            ShowDialog("Seleccionar tipo", "No se ha seleccionado ningún tipo para ser editado");
            return;
        }
    }

    private async void btnDeleteList_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "Confirmación de borrado",
            Content = "¿Estás seguro de que quieres vaciar la lista?\nEsto eliminará de la base de datos todos los rótulos actuales",
            CloseButtonText = "Cancelar",
            PrimaryButtonText = "Confirmar",
            DefaultButton = ContentDialogButton.Close
        };

        var result = await dialog.ShowAsync();

        if (result.Equals(ContentDialogResult.Primary))
        {
            await ViewModel.BorrarTodos();
        }
    }

    //ENVIAR MENSAJES UI
    public async void ShowDialog(string titulo, string content)
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = titulo,
            Content = content,
            CloseButtonText = "Cerrar",
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }

    //ENTRA Y SALE
    private void btnEntra_Click(object sender, RoutedEventArgs e)
    {
        if (seleccionado != null)
        {
            if (!rotuloIn)
            {
                ViewModel.Entra(seleccionado);
                rotuloIn = true;
            }
            else
            {
                ViewModel.Encadena(seleccionado);
            }
        }
    }

    private void btnSale_Click(object sender, RoutedEventArgs e)
    {
        if (rotuloIn)
        {
            ViewModel.Sale();
            rotuloIn = false;
        }
    }


}
