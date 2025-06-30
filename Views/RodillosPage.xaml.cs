using System.Diagnostics;
using System.Text.Json;
using Generico_Front.Controllers.Data;
using Generico_Front.Models;
using Generico_Front.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Generico_Front.Views;

public sealed partial class RodillosPage : Page
{
    Config.Config config;
    public RodillosViewModel ViewModel
    {
        get;
    }

    private object? selectedItem;
    private Cargo selectedCargo;
    private Persona selectedPersona;
    private Rodillo editado;

    public RodillosPage()
    {
        ViewModel = App.GetService<RodillosViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
        IniciarListas();
    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        ViewModel.CargarRodillos();
        ViewModel.CargarTipos();
        editado = new Rodillo();
        editado.cargos.AddRange(ViewModel.cargos);
        txtRuta.Text = config.RotulacionSettings.RutaRodillo;
        treeRodillo.ItemsSource = ViewModel.cargos;
        cmbTipos.ItemsSource = ViewModel.Tipos;
        cmbTiposEdicion.ItemsSource = ViewModel.Tipos;
        cmbTipos.SelectedItem = ViewModel.Tipos.FirstOrDefault(t => t.descripcion.Equals(config.RotulacionSettings.TipoRodillo));
        boxVelocidad.Text = config.RotulacionSettings.VelocidadRodillo.ToString();
        boxColumnas.SelectedIndex = config.RotulacionSettings.ColumnasRodillo - 1;
        boxLineas.Text = config.RotulacionSettings.LineasPorBloque.ToString();
    }

    //TREE VIEW
    private void treeRodillo_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        selectedItem = args.AddedItems.FirstOrDefault();
        if (selectedItem != null)
        {
            if (selectedItem is Cargo)
            {
                selectedCargo = selectedItem as Cargo;
            }
            if (selectedItem is Persona)
            {
                selectedCargo = treeRodillo.SelectedNode.Parent.Content as Cargo;

            }
            txtNombreCat.Text = selectedCargo.nombre;
            string personas = "";
            foreach (Persona per in selectedCargo.personas)
            {
                personas += per.nombre + "\n";
            }
            txtPersonas.Text = personas.TrimEnd('\n');
        }
    }

    private void treeRodillo_DragOver(object sender, DragEventArgs e)
    {
        // Evita el reordenamiento de elementos dentro del TreeView
        e.AcceptedOperation = DataPackageOperation.None;
    }
    private void treeRodillo_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var menu = new MenuFlyout();

        // Opción para Desplegar Todos
        var desplegarTodos = new MenuFlyoutItem { Text = "Desplegar todos" };
        desplegarTodos.Click += DesplegarTodos_Click;
        menu.Items.Add(desplegarTodos);

        // Opción para Replegar Todos
        var replegarTodos = new MenuFlyoutItem { Text = "Replegar todos" };
        replegarTodos.Click += ReplegarTodos_Click;
        menu.Items.Add(replegarTodos);

        menu.ShowAt(treeRodillo, e.GetPosition(treeRodillo));
    }

    private void DesplegarTodos_Click(object sender, RoutedEventArgs e)
    {
        // Recorre todos los elementos principales del TreeView
        foreach (var item in treeRodillo.RootNodes)
        {
            ExpandAllItems(item);
        }
    }
    private void ExpandAllItems(TreeViewNode node)
    {
        node.IsExpanded = true; // Despliega el nodo actual

        foreach (var child in node.Children)
        {
            ExpandAllItems(child); // Llama recursivamente para expandir los hijos
        }
    }

    private void ReplegarTodos_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in treeRodillo.RootNodes)
        {
            CollapseAllItems(item);
        }
    }
    private void CollapseAllItems(TreeViewNode node)
    {
        node.IsExpanded = false;

        foreach (var child in node.Children)
        {
            CollapseAllItems(child);
        }
    }

    //FILTRADO

    private void tggFiltrado_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggFiltrado.IsOn)
        {
            FiltradoPorCargo.Visibility = Visibility.Visible;
            FiltradoPorPersona.Visibility = Visibility.Visible;
        }
        else
        {
            FiltradoPorCargo.Visibility = Visibility.Collapsed;
            FiltradoPorPersona.Visibility = Visibility.Collapsed;
        }
    }
    private void FiltradoPorPersona_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allCargos
        .Where(c => c.personas.Any(p => p.nombre.IndexOf(FiltradoPorPersona.Text, StringComparison.OrdinalIgnoreCase) >= 0));
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorCargo_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allCargos
        .Where(c => c.nombre.IndexOf(FiltradoPorCargo.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    //EDICIÓN GENERAL
    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            stckEditior0.Visibility = Visibility.Visible;
            linkAjustes.Visibility = Visibility.Visible;
            cmbTipos.Visibility = Visibility.Visible;
            stckVelocidad.Visibility = Visibility.Visible;
            stckBotonera.Visibility = Visibility.Visible;
            if (cmbTipos.SelectedItem != null)
            {
                Tipo seleccionado = (Tipo)cmbTipos.SelectedValue;
                stckHorizontal.Visibility = seleccionado.descripcion.Equals("Horizontal") ? Visibility.Visible : Visibility.Collapsed;
            }
            gridEdicion.Visibility = Visibility.Visible;
        }
        else
        {
            stckEditior0.Visibility = Visibility.Collapsed;
            linkAjustes.Visibility = Visibility.Collapsed;
            cmbTipos.Visibility = Visibility.Collapsed;
            stckVelocidad.Visibility = Visibility.Collapsed;
            stckBotonera.Visibility = Visibility.Collapsed;
            stckHorizontal.Visibility = Visibility.Collapsed;
            gridEdicion.Visibility = Visibility.Collapsed;
        }
    }

    private void AbrirPanelAjustes(object sender, RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = true;
    }

    private async void filePicker_Click(object sender, RoutedEventArgs e)
    {
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;

        txtRuta.Text = "";

        var openPicker = new FileOpenPicker();

        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        openPicker.FileTypeFilter.Add(".txt");
        openPicker.FileTypeFilter.Add(".csv");

        var file = await abrirFilePicker(openPicker);
        if (file != null)
        {
            txtRuta.Text = file.Path;
            config.RotulacionSettings.RutaRodillo = file.Path;
            PonerRodilloEnLista();
        }
        else
        {
            txtRuta.Text = config.RotulacionSettings.RutaRodillo;
        }

        senderButton.IsEnabled = true;
    }

    private async Task<Windows.Storage.StorageFile> abrirFilePicker(FileOpenPicker openPicker)
    {
        var file = await openPicker.PickSingleFileAsync();
        return file;
    }

    private void PonerRodilloEnLista()
    {
        try
        {
            config.RotulacionSettings.RutaRodillo = txtRuta.Text;
            Config.Config.SaveConfig(config);
            ViewModel.CargarRodillos();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void cmbTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbTipos.SelectedIndex != -1)
        {
            Tipo seleccionado = (Tipo)cmbTipos.SelectedValue;
            config.RotulacionSettings.TipoRodillo = seleccionado.descripcion;
            if (tggEditor.IsOn)
                stckHorizontal.Visibility = seleccionado.descripcion.Equals("Horizontal") ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private async void btnModificar_Click(object sender, RoutedEventArgs e)
    {
        await AbrirFicheroYRefrescarUI(config.RotulacionSettings.RutaRodillo);

    }
    public async Task AbrirFicheroYRefrescarUI(string rutaFichero)
    {
        if (!File.Exists(rutaFichero))
        {
            ShowDialog("El fichero no existe", $"No puede encontrarse el fichero de créditos en la ruta {rutaFichero}");
            return;
        }

        try
        {
            var proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = $"\"{rutaFichero}\"",
                    UseShellExecute = true
                }
            };

            proceso.Start();
            await proceso.WaitForExitAsync();
            ViewModel.CargarRodillos();
        }
        catch (Exception ex)
        {
            ShowDialog("Error al abrir el fichero", ex.Message);
        }
    }

    private void btnGuardar_Click(object sender, RoutedEventArgs e)
    {
        GuardarTipo();
        Tip.Target = btnGuardar;
        Tip.Title = "Guardado con éxito";
        AbrirTip();
    }
    private void GuardarTipo()
    {
        if (cmbTipos.SelectedItem != null)
        {
            Tipo seleccionado = (Tipo)cmbTipos.SelectedValue;
            config.RotulacionSettings.TipoRodillo = seleccionado.descripcion;

            if (seleccionado.descripcion.Equals("Horizontal"))
            {
                config.RotulacionSettings.ColumnasRodillo = int.Parse(boxColumnas.SelectedValue.ToString());
                config.RotulacionSettings.LineasPorBloque = string.IsNullOrEmpty(boxLineas.Text) ? config.RotulacionSettings.LineasPorBloque : int.Parse(boxLineas.Text);
            }
        }
        config.RotulacionSettings.VelocidadRodillo = int.Parse(boxVelocidad.Text);
        Config.Config.SaveConfig(config);
    }

    //EDICION POR CARGO Y PERSONA
    private void btnEliminarEdicion_Click(object sender, RoutedEventArgs e)
    {
        if (selectedCargo != null)
        {
            // Cargo CEditado = editado.cargos.FirstOrDefault(c => c.nombre.Equals(selectedCargo.nombre, StringComparison.OrdinalIgnoreCase));
            Cargo CEditado = editado.cargos[selectedCargo.orden - 1];
            editado.cargos.Remove(CEditado);
            ViewModel.GuardarRodillo(editado);
            ViewModel.CargarRodillos();
            Tip.Target = btnEliminarEdicion;
            Tip.Title = "Eliminado con éxito";
            AbrirTip();
            txtNombreCat.Text = "";
            txtPersonas.Text = "";
            selectedCargo = null;
        }
        else
        {
            ShowDialog("Seleccionar cargo", "No se ha seleccionado ningún cargo para ser eliminado");
        }
    }
    private void btnModificarEdicion_Click(object sender, RoutedEventArgs e)
    {
        if (selectedCargo != null)
        {
            Cargo CModel = ViewModel.cargos[selectedCargo.orden - 1];
            Cargo CEditado = editado.cargos[selectedCargo.orden - 1];
            CModel.nombre = txtNombreCat.Text;
            List<string> personas = txtPersonas.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            CModel.personas.Clear();
            foreach (string p in personas)
            {
                Persona persona = new Persona { nombre = p.Trim() };
                persona.orden = personas.IndexOf(p);
                CModel.personas.Add(persona);
            }

            CEditado.nombre = CModel.nombre;
            CEditado.personas = CModel.personas;
            ViewModel.GuardarRodillo(editado);
            ViewModel.CargarRodillos();
            Tip.Target = btnModificarEdicion;
            Tip.Title = "Editado con éxito";
            AbrirTip();
            SeleccionarElemento();
        }
        else
        {
            ShowDialog("Seleccionar cargo", "No se ha seleccionado ningún cargo para ser eliminado");
        }
    }
    private void btnAddEdicion_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(txtNombreCat.Text))
        {
            Cargo nuevo = new Cargo();
            nuevo.orden = editado.cargos.Count;
            nuevo.nombre = txtNombreCat.Text;
            List<string> personas = txtPersonas.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (string p in personas)
            {
                Persona persona = new Persona { nombre = p.Trim() };
                persona.orden = personas.IndexOf(p);
                nuevo.personas.Add(persona);
            }
            editado.cargos.Add(nuevo);
            ViewModel.GuardarRodillo(editado);
            ViewModel.CargarRodillos();
            Tip.Target = btnAddEdicion;
            Tip.Title = "Añadido con éxito";
            AbrirTip();
            SeleccionarElemento(true);
        }
        else
        {
            ShowDialog("Rellenar nombre", "Debe al menos especificar un nombre para la nueva categoría");
        }
    }

    private void SeleccionarElemento(bool add = false)
    {

        Cargo seleccionado = add ? ViewModel.cargos[ViewModel.cargos.Count - 1] : ViewModel.cargos.FirstOrDefault(c => c.nombre.Equals(txtNombreCat.Text));
        if (seleccionado != null)
        {
            treeRodillo.SelectedItem = seleccionado;
            selectedCargo = seleccionado;
        }
    }

    //PANEL DESPLEGABLE AJUSTES EXTRA
    private void CerrarPanelAjustes(object sender, RoutedEventArgs e)
    {
        SplitView.IsPaneOpen = false;
    }

    private void cmbTiposEdicion_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbTiposEdicion.SelectedItem != null)
        {
            Tipo seleccionado = (Tipo)cmbTiposEdicion.SelectedValue;
            txtNombreTipo.Text = seleccionado.descripcion;
        }
    }

    private async void btnDeleteAjustes_Click(object sender, RoutedEventArgs e)
    {
        if (cmbTiposEdicion.SelectedItem != null)
        {
            Tipo seleccionado = cmbTiposEdicion.SelectedItem as Tipo;
            await ViewModel.EliminarTipo(seleccionado);
            Tip.Target = btnDeleteAjustes;
            Tip.Title = "Eliminado con éxito";
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
        if (cmbTiposEdicion.SelectedItem != null && !string.IsNullOrEmpty(nuevoNombre))
        {
            Tipo seleccionado = cmbTiposEdicion.SelectedItem as Tipo;
            seleccionado.descripcion = nuevoNombre;
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
            if (cmbTiposEdicion.SelectedItem != null)
            {
                Tipo seleccionado = cmbTiposEdicion.SelectedItem as Tipo;
                if (string.Equals(seleccionado.descripcion, nuevoNombre, StringComparison.OrdinalIgnoreCase))
                {
                    ShowDialog("Cambio de nombre", "No se puede guardar un nuevo tipo con el mismo nombre que uno ya registrado, por favor, edite el nombre actual si quiere guardarlo como un nuevo tipo de rodillo.");
                    return;
                }
            }
            Tipo nuevoTipo = new Tipo();
            nuevoTipo.id = 0;
            nuevoTipo.seAplicaA = "Rodillos";
            nuevoTipo.descripcion = nuevoNombre;

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

    //METODOS DE LANZADO DE MENSAJES
    private async void AbrirTip()
    {
        Tip.IsOpen = true;
        await Task.Delay(1500);
        Tip.IsOpen = false;
    }
    private void Tip_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
    {
        Tip.IsOpen = false;
    }

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

    //ACCIONES GRAPHICS (PLAY y STOP)
    private void btnPlay_Click(object sender, RoutedEventArgs e)
    {
        Rodillo r = new Rodillo();
        r.cargos = ViewModel.cargos.ToList();
        string tipo = config.RotulacionSettings.TipoRodillo;
        ViewModel.Entra(r, tipo);
    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Sale();
    }
}
