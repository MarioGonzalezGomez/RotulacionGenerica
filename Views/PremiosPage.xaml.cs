using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Generico_Front.Config;
using Generico_Front.Controllers.Data;
using Generico_Front.Models;
using Generico_Front.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;

namespace Generico_Front.Views;

public sealed partial class PremiosPage : Page
{

    private Config.Config config;
    public PremiosViewModel ViewModel
    {
        get;
    }
    private object? selectedItem;

    private bool inCategoria = false;
    private bool inNominado = false;
    private bool inLista = false;
    private bool inGafas = false;
    private bool inEntregadores = false;
    private bool inGanador = false;

    private bool isPremio;

    private List<Premio> editada;

    public PremiosPage()
    {
        ViewModel = App.GetService<PremiosViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
        IniciarListas();
    }

    //ACCIONES EN LAS LISTAS
    private void IniciarListas()
    {
        editada = new List<Premio>();
        ViewModel.CargarPremios();
        editada.AddRange(ViewModel.allPremios);
        treePremios.ItemsSource = ViewModel.premios;
        txtRuta.Text = config.RotulacionSettings.RutaPremios;
        EstadoInicialBotonera();
    }
    private void EstadoInicialBotonera()
    {
        btnCategoria.Visibility = config.RotulacionSettings.BtnCategoria ? Visibility.Visible : Visibility.Collapsed;
        CategoriaCheckBox.IsChecked = config.RotulacionSettings.BtnCategoria;
        grupoNominado.Visibility = config.RotulacionSettings.BtnNominado ? Visibility.Visible : Visibility.Collapsed;
        NominadoCheckBox.IsChecked = config.RotulacionSettings.BtnNominado;
        btnNominados.Visibility = config.RotulacionSettings.BtnNominados ? Visibility.Visible : Visibility.Collapsed;
        NominadosCheckBox.IsChecked = config.RotulacionSettings.BtnNominados;
        btnGafas.Visibility = config.RotulacionSettings.BtnGafas ? Visibility.Visible : Visibility.Collapsed;
        GafasCheckBox.IsChecked = config.RotulacionSettings.BtnGafas;
        btnEntregadores.Visibility = config.RotulacionSettings.BtnEntregadores ? Visibility.Visible : Visibility.Collapsed;
        EntregadoresCheckBox.IsChecked = config.RotulacionSettings.BtnEntregadores;
        btnGanador.Visibility = config.RotulacionSettings.BtnGanador ? Visibility.Visible : Visibility.Collapsed;
        GanadorCheckBox.IsChecked = config.RotulacionSettings.BtnGanador;
    }

    //TREE VIEW
    private void treePremios_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        selectedItem = args.AddedItems.FirstOrDefault();
        CambiarElementosVisibles(selectedItem);
        if (selectedItem is Premio premio)
        {
            // Acción para Premio
            MostrarDetallesPremio(premio);
            MostrarPremioEdicion(premio);
        }
        else if (selectedItem is Nominado nominado)
        {
            // Acción para Nominado
            MostrarDetallesNominado(nominado);
            MostrarNominadoEdicion(nominado);
        }
    }
    private void MostrarDetallesPremio(Premio premio)
    {
        var textBoxes = new[] { txtInfo1, txtInfo2, txtInfo3, txtInfo4, txtInfo5, txtInfo6 };
        foreach (var textBox in textBoxes)
        {
            textBox.Visibility = Visibility.Collapsed;
        }

        if (premio.entregadores.Count > 0)
        {
            headerInfo.Text = "Entrega";
            headerInfo.Visibility = Visibility.Visible;
            for (int i = 0; i < premio.entregadores.Count; i++)
            {
                textBoxes[i].Text = premio.entregadores[i];
                textBoxes[i].Visibility = Visibility.Visible;
            }
        }
        else
        {
            headerInfo.Visibility = Visibility.Collapsed;
        }
        boxNombre.Header = "Nombre Categoría";
        headerInfo2.Text = "Ganador";
        if (premio.ganador != null)
        {
            txtRecoge.Text = premio.ganador.nombre;
            headerInfo2.Visibility = Visibility.Visible;
            txtRecoge.Visibility = Visibility.Visible;
        }
        else
        {
            headerInfo2.Visibility = Visibility.Collapsed;
            txtRecoge.Visibility = Visibility.Collapsed;
        }
        txtGanador.Visibility = Visibility.Collapsed;
    }
    private void MostrarPremioEdicion(Premio premio)
    {
        boxNombre.Text = premio.nombre;
        List<string> entregadores = new List<string>();
        entregadores.Add("Añadir entregador");
        entregadores.AddRange(premio.entregadores);
        comboEntregadores.ItemsSource = entregadores;
        comboEntregadores.SelectedIndex = premio.entregadores.Count > 0 ? 1 : 0;
        if (premio.entregadores.Count == 0) { boxEntregadorOtrabajo.Text = ""; }
        List<string> ganador = new List<string>();
        ganador.Add("Sin ganador establecido");
        ganador.AddRange(premio.nominados.Select(nom => nom.nombre));
        comboGanador.ItemsSource = ganador;
        comboGanador.SelectedIndex = premio.ganador != null ? ganador.IndexOf(premio.ganador.nombre) : 0;
        Grid.SetColumnSpan(boxEntregadorOtrabajo, 1);
        boxEntregadorOtrabajo.Height = 60;
        boxEntregadorOtrabajo.Header = "Editar entregador";
    }
    private void MostrarDetallesNominado(Nominado nominado)
    {

        var textBoxes = new[] { txtInfo1, txtInfo2, txtInfo3, txtInfo4, txtInfo5, txtInfo6 };
        foreach (var textBox in textBoxes)
        {
            textBox.Visibility = Visibility.Collapsed;
        }
        boxNombre.Header = "Nombre";
        headerInfo.Text = "Información complementaria:";
        txtInfo1.Text = nominado.trabajo;
        headerInfo.Visibility = Visibility.Visible;
        txtInfo1.Visibility = Visibility.Visible;

        headerInfo2.Text = "Recoge";
        if (!string.IsNullOrEmpty(nominado.recoge))
        {
            txtRecoge.Text = nominado.recoge;
            headerInfo2.Visibility = Visibility.Visible;
            txtRecoge.Visibility = Visibility.Visible;
        }
        else
        {
            headerInfo2.Visibility = Visibility.Collapsed;
            txtRecoge.Visibility = Visibility.Collapsed;
        }
        txtGanador.Visibility = nominado.ganador ? Visibility.Visible : Visibility.Collapsed;
    }
    private void MostrarNominadoEdicion(Nominado nominado)
    {
        boxNombre.Text = nominado.nombre;
        boxEntregadorOtrabajo.Text = nominado.trabajo;
        boxRecoge.Text = string.IsNullOrEmpty(nominado.recoge) ? "" : nominado.recoge;
        Grid.SetColumnSpan(boxEntregadorOtrabajo, 2);
        boxEntregadorOtrabajo.Height = 150;
        boxEntregadorOtrabajo.Header = "Información complementaria";
    }
    private void CambiarElementosVisibles(object objeto)
    {
        if (tggEditor.IsOn)
        {
            btnCategoria.Visibility = Visibility.Collapsed;
            grupoNominado.Visibility = Visibility.Collapsed;
            btnNominados.Visibility = Visibility.Collapsed;
            btnGafas.Visibility = Visibility.Collapsed;
            btnEntregadores.Visibility = Visibility.Collapsed;
            btnGanador.Visibility = Visibility.Collapsed;
            btnAdd.Visibility = Visibility.Collapsed;
            boxNombre.Visibility = Visibility.Visible;
            grupoEntregador.Visibility = Visibility.Visible;
            stckBotones.Visibility = Visibility.Visible;
            btnEliminar.Visibility = Visibility.Visible;
            btnGuardar.Visibility = Visibility.Visible;
            if (objeto is Premio)
            {
                comboGanador.Visibility = Visibility.Visible;
                comboEntregadores.Visibility = Visibility.Visible;
                btnAccionEntregador.Visibility = Visibility.Visible;
                boxRecoge.Visibility = Visibility.Collapsed;

            }
            if (objeto is Nominado)
            {
                boxRecoge.Visibility = Visibility.Visible;
                comboGanador.Visibility = Visibility.Collapsed;
                comboEntregadores.Visibility = Visibility.Collapsed;
                btnAccionEntregador.Visibility = Visibility.Collapsed;
            }

            if (objeto is null)
            {
                txtInfo1.Visibility = Visibility.Collapsed;
                txtInfo2.Visibility = Visibility.Collapsed;
                txtInfo3.Visibility = Visibility.Collapsed;
                txtInfo4.Visibility = Visibility.Collapsed;
                txtInfo5.Visibility = Visibility.Collapsed;
                txtInfo6.Visibility = Visibility.Collapsed;
            }
        }
        else
        {
            boxNombre.Visibility = Visibility.Collapsed;
            comboGanador.Visibility = Visibility.Collapsed;
            comboEntregadores.Visibility = Visibility.Collapsed;
            grupoEntregador.Visibility = Visibility.Collapsed;
            boxRecoge.Visibility = Visibility.Collapsed;
            stckBotones.Visibility = Visibility.Collapsed;
            EstadoInicialBotonera();
        }
    }
    private void treePremios_DragOver(object sender, DragEventArgs e)
    {
        // Evita el reordenamiento de elementos dentro del TreeView
        e.AcceptedOperation = DataPackageOperation.None;
    }
    private void treePremios_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        var originalSource = e.OriginalSource as FrameworkElement;
        if (originalSource == null) return;

        var container = FindParent<TreeViewItem>(originalSource);

        if (container != null)
        {
            // Selecciona el ítem clicado
            var item = container.DataContext;
            if (item != null)
            {
                treePremios.SelectedItem = item;
            }

            // Crear menú contextual
            var menu = new MenuFlyout();

            var addNominado = new MenuFlyoutItem { Text = "Añadir nominado" };
            addNominado.Click += AddNominado_Click;
            menu.Items.Add(addNominado);

            var desplegarTodos = new MenuFlyoutItem { Text = "Desplegar todos" };
            desplegarTodos.Click += DesplegarTodos_Click;
            menu.Items.Add(desplegarTodos);

            var replegarTodos = new MenuFlyoutItem { Text = "Replegar todos" };
            replegarTodos.Click += ReplegarTodos_Click;
            menu.Items.Add(replegarTodos);

            // ✅ Mostrar el menú en el TreeViewItem clicado
            menu.ShowAt(container, e.GetPosition(container));

            e.Handled = true; // Opcional: previene comportamiento extra no deseado
        }
    }
    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parent = VisualTreeHelper.GetParent(child);

        while (parent != null && !(parent is T))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parent as T;
    }

    private void AddNominado_Click(object sender, RoutedEventArgs e)
    {
        tggEditor.IsOn = true;
        isPremio = false;
        Nominado creado = new Nominado();
        creado.nombre = "";
        creado.trabajo = "";
        creado.recoge = "";
        creado.ganador = false;
        CambiarElementosVisibles(creado);
        MostrarDetallesNominado(creado);
        MostrarNominadoEdicion(creado);
        btnAdd.Visibility = Visibility.Visible;
        btnEliminar.Visibility = Visibility.Collapsed;
        btnGuardar.Visibility = Visibility.Collapsed;
    }

    private void DesplegarTodos_Click(object sender, RoutedEventArgs e)
    {
        // Recorre todos los elementos principales del TreeView
        foreach (var item in treePremios.RootNodes)
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
        foreach (var item in treePremios.RootNodes)
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
    private void FiltradoPorPremio_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allPremios
       .Where(c => c.nombre.IndexOf(FiltradoPorPremio.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPersona_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allPremios
       .Where(c => c.nominados.Any(n => n.nombre.IndexOf(FiltradoPorPersona.Text, StringComparison.OrdinalIgnoreCase) >= 0));
        ViewModel.RemoverNoCoincidentes(filtrada);
        ViewModel.RecuperarLista(filtrada);
    }

    private void FiltradoPorPelicula_TextChanged(object sender, TextChangedEventArgs e)
    {
        var filtrada = ViewModel.allPremios
      .Where(c => c.nominados.Any(n =>
           n.trabajo?.Contains(FiltradoPorPelicula.Text, StringComparison.OrdinalIgnoreCase) == true
        ));
        ViewModel.RemoverNoCoincidentes(filtrada);
        if (string.IsNullOrEmpty(FiltradoPorPelicula.Text))
            ViewModel.RecuperarLista(ViewModel.allPremios);
    }

    //EDICIÓN
    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (tggEditor.IsOn)
        {
            stckEditior0.Visibility = Visibility.Visible;
            stckEditior1.Visibility = Visibility.Visible;
            gridBotonesDisponibles.Visibility = Visibility.Visible;
            if (selectedItem != null)
            {
                CambiarElementosVisibles(selectedItem);
            }
        }
        else
        {
            stckEditior0.Visibility = Visibility.Collapsed;
            stckEditior1.Visibility = Visibility.Collapsed;
            gridBotonesDisponibles.Visibility = Visibility.Collapsed;
            CambiarElementosVisibles(selectedItem);
        }
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
            config.RotulacionSettings.RutaPremios = file.Path;
            PonerPremiosEnLista();
        }
        else
        {
            txtRuta.Text = config.RotulacionSettings.RutaPremios;
        }

        senderButton.IsEnabled = true;
    }
    private async Task<Windows.Storage.StorageFile> abrirFilePicker(FileOpenPicker openPicker)
    {
        var file = await openPicker.PickSingleFileAsync();
        return file;
    }
    private void PonerPremiosEnLista()
    {
        try
        {
            config.RotulacionSettings.RutaPremios = txtRuta.Text;
            Config.Config.SaveConfig(config);
            ViewModel.CargarPremios();
        }
        catch (Exception ex)
        {
            ShowDialog("Error", ex.ToString());
            throw;
        }
    }

    private async void btnModificarPremio_Click(object sender, RoutedEventArgs e)
    {
        await AbrirFicheroYRefrescarUI(config.RotulacionSettings.RutaPremios);

    }
    public async Task AbrirFicheroYRefrescarUI(string rutaFichero, bool recargar = true)
    {
        if (!File.Exists(rutaFichero))
        {
            ShowDialog("El fichero no existe", $"No puede encontrarse el fichero de premios en la ruta {rutaFichero}");
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
            ViewModel.CargarPremios();
        }
        catch (Exception ex)
        {
            ShowDialog("Error al abrir el fichero", ex.Message);
        }
    }

    private async void btnInfo_Click(object sender, RoutedEventArgs e)
    {
        string filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Resources", "InfoPremios.txt");
        await AbrirFicheroYRefrescarUI(filePath);
    }

    private void comboGanador_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (comboGanador.SelectedItem != null)
        {
            Premio currentPremio = selectedItem as Premio;
            Premio aEditar = editada.FirstOrDefault(p => string.Equals(p.nombre, currentPremio.nombre));

            if (aEditar != null)
            {
                if (comboGanador.SelectedIndex == 0)
                {
                    // Ningún ganador
                    aEditar.ganador = null;
                    foreach (var item in aEditar.nominados)
                    {
                        item.ganador = false;
                    }
                }
                else
                {
                    // Establecer ganador
                    Nominado nominado = aEditar.nominados.FirstOrDefault(nom => string.Equals(nom.nombre, comboGanador.SelectedItem.ToString()));
                    aEditar.ganador = nominado;
                    foreach (var item in aEditar.nominados)
                    {
                        item.ganador = (item == aEditar.ganador);
                    }
                }
            }
        }
    }

    private void comboEntregadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (comboEntregadores.SelectedItem != null && comboEntregadores.SelectedIndex != 0)
        {
            boxEntregadorOtrabajo.Text = comboEntregadores.SelectedItem.ToString();
            iconoAccionEntregador.Symbol = Symbol.Edit;
        }
        else
        {
            iconoAccionEntregador.Symbol = Symbol.Add;
        }
    }
    private void btnAccionEntregador_Click(object sender, RoutedEventArgs e)
    {
        if (comboEntregadores.SelectedItem != null)
        {
            Premio currentPremio = selectedItem as Premio;
            Premio aEditar = editada.FirstOrDefault(p => string.Equals(p.nombre, currentPremio.nombre));
            if (comboEntregadores.SelectedIndex != 0)
            {
                //EDITAR
                boxEntregadorOtrabajo.Header = "Editar entregador";
                if (aEditar != null)
                {
                    int index = aEditar.entregadores.FindIndex(e => string.Equals(e, comboEntregadores.Text));
                    if (index != -1)
                    {
                        aEditar.entregadores[index] = boxEntregadorOtrabajo.Text;
                    }
                }
            }
            else
            {
                //ADD
                boxEntregadorOtrabajo.Header = "Añadir entregador";
                if (aEditar != null)
                {
                    aEditar.entregadores.Add(boxEntregadorOtrabajo.Text);
                }
            }
        }
    }

    private void boxNombre_LostFocus(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(boxNombre.Text))
        {
            if (selectedItem is Premio)
            {
                Premio currentPremio = selectedItem as Premio;
                Premio aEditar = editada.FirstOrDefault(p => string.Equals(p.nombre, currentPremio.nombre));
                aEditar.nombre = boxNombre.Text;
            }
            if (selectedItem is Nominado)
            {
                Nominado currentNominado = selectedItem as Nominado;
                string nombreAntiguo = currentNominado.nombre;
                string nuevoNombre = boxNombre.Text;

                foreach (var premio in editada)
                {
                    foreach (var nominado in premio.nominados)
                    {
                        if (string.Equals(nominado.nombre, nombreAntiguo, StringComparison.OrdinalIgnoreCase))
                        {
                            nominado.nombre = nuevoNombre;
                        }
                    }
                }
            }
        }
    }

    private void boxEntregadorOtrabajo_LostFocus(object sender, RoutedEventArgs e)
    {
        if (selectedItem is Nominado)
        {
            Nominado currentNominado = selectedItem as Nominado;
            string trabajoAntiguo = currentNominado.trabajo;
            string nuevoTrabajo = boxEntregadorOtrabajo.Text;

            foreach (var premio in editada)
            {
                foreach (var nominado in premio.nominados)
                {
                    if (string.Equals(nominado.trabajo, trabajoAntiguo, StringComparison.OrdinalIgnoreCase))
                    {
                        nominado.trabajo = nuevoTrabajo;
                    }
                }
            }
        }
    }

    private void boxRecoge_LostFocus(object sender, RoutedEventArgs e)
    {
        Nominado currentNominado = selectedItem as Nominado;
        string recogeAntiguo = currentNominado.recoge;
        string nuevoRecoge = boxRecoge.Text;

        foreach (var premio in editada)
        {
            foreach (var nominado in premio.nominados)
            {
                if (string.Equals(nominado.recoge, recogeAntiguo, StringComparison.OrdinalIgnoreCase))
                {
                    nominado.recoge = nuevoRecoge;
                }
            }
        }
    }

    private async void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        var utimoSeleccionado = selectedItem;

        if (string.IsNullOrEmpty(boxNombre.Text))
        {
            ShowDialog("Falta nombre", "Debe al menos rellenar el campo \"Nombre\" para añadir una nueva categoría");
        }
        else
        {
            if (isPremio)
            {
                //ADD PREMIO

            }
            else
            {
                //ADD NOMINADO
                Premio ultimoPremio = utimoSeleccionado as Premio;
                Premio selected = editada.FirstOrDefault(p => string.Equals(p.nombre, ultimoPremio.nombre));
                Nominado nuevo = new Nominado();
                nuevo.nombre = boxNombre.Text;
                nuevo.trabajo = boxEntregadorOtrabajo.Text;
                nuevo.ganador = false;
                nuevo.recoge = boxRecoge.Text;
                selected.nominados.Add(nuevo);
            }
            btnGuardar_Click(sender, e);
        }
    }
    private void btnEliminar_Click(object sender, RoutedEventArgs e)
    {
        var utimoSeleccionado = selectedItem;

        if (utimoSeleccionado is Premio)
        {
            Premio ultimoPremio = utimoSeleccionado as Premio;
            Premio selected = editada.FirstOrDefault(p => string.Equals(p.nombre, ultimoPremio.nombre));
            editada.Remove(selected);
        }
        if (utimoSeleccionado is Nominado)
        {
            Premio parent = treePremios.SelectedNode.Parent.Content as Premio;
            Premio Pselected = editada.FirstOrDefault(p => string.Equals(p.nombre, parent.nombre));
            Nominado ultimoNominado = utimoSeleccionado as Nominado;
            Nominado Nselected = Pselected.nominados.FirstOrDefault(n => string.Equals(n.nombre, ultimoNominado.nombre));
            Pselected.nominados.Remove(Nselected);
        }
        if (utimoSeleccionado == null)
        {
            ShowDialog("Seleccionar elemento", "No hay un elemento seleccionado para eliminarlo. Asegúrate de que está resaltado en la lista el elemento a borrar");
        }
        else
        {
            Tip.Target = btnEliminar;
            Tip.Title = "Guarda para confirmar";
            AbrirTip();
        }

    }
    private async void btnGuardar_Click(object sender, RoutedEventArgs e)
    {
        var utimoSeleccionado = selectedItem;
        var pruebaNodo = treePremios.SelectedNode.Parent.Content;
        var ultimoNodo = treePremios.NodeFromContainer(treePremios.ContainerFromItem(selectedItem));

        await ViewModel.GuardarPremios(editada);
        await ViewModel.CargarPremios();

        if (utimoSeleccionado is Premio)
        {
            Premio ultimoPremio = utimoSeleccionado as Premio;
            Premio selected = ViewModel.premios.FirstOrDefault(p => string.Equals(p.nombre, ultimoPremio.nombre));
            treePremios.SelectedItem = selected;
        }
        if (utimoSeleccionado is Nominado)
        {
            if (ultimoNodo.Parent != null)
            {
                Premio parent = ultimoNodo.Parent.Content as Premio;
                Premio selected = ViewModel.premios.FirstOrDefault(p => string.Equals(p.nombre, parent.nombre));
                treePremios.SelectedItem = selected;
                treePremios.SelectedNode.IsExpanded = true;
            }
        }

        Tip.Target = btnGuardar;
        Tip.Title = "Guardado con éxito";
        AbrirTip();
    }

    //BOTONES DISPONIBLES
    private void CategoriaCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        btnCategoria.Visibility = Visibility.Visible;
        config.RotulacionSettings.BtnCategoria = true;
        Config.Config.SaveConfig(config);
    }
    private void CategoriaCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        btnCategoria.Visibility = Visibility.Collapsed;
        config.RotulacionSettings.BtnCategoria = false;
        Config.Config.SaveConfig(config);
    }

    private void NominadoCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        grupoNominado.Visibility = Visibility.Visible;
        config.RotulacionSettings.BtnNominado = true;
        Config.Config.SaveConfig(config);
    }
    private void NominadoCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        grupoNominado.Visibility = Visibility.Collapsed;
        config.RotulacionSettings.BtnNominado = false;
        Config.Config.SaveConfig(config);
    }

    private void NominadosCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        btnNominados.Visibility = Visibility.Visible;
        config.RotulacionSettings.BtnNominados = true;
        Config.Config.SaveConfig(config);
    }
    private void NominadosCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        btnNominados.Visibility = Visibility.Collapsed;
        config.RotulacionSettings.BtnNominados = false;
        Config.Config.SaveConfig(config);
    }

    private void GafasCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        btnGafas.Visibility = Visibility.Visible;
        config.RotulacionSettings.BtnGafas = true;
        Config.Config.SaveConfig(config);
    }
    private void GafasCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        btnGafas.Visibility = Visibility.Collapsed;
        config.RotulacionSettings.BtnGafas = false;
        Config.Config.SaveConfig(config);
    }

    private void EntregadoresCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        btnEntregadores.Visibility = Visibility.Visible;
        config.RotulacionSettings.BtnEntregadores = true;
        Config.Config.SaveConfig(config);
    }
    private void EntregadoresCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        btnEntregadores.Visibility = Visibility.Collapsed;
        config.RotulacionSettings.BtnEntregadores = false;
        Config.Config.SaveConfig(config);
    }

    private void GanadorCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        btnGanador.Visibility = Visibility.Visible;
        config.RotulacionSettings.BtnGanador = true;
        Config.Config.SaveConfig(config);
    }
    private void GanadorCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        btnGanador.Visibility = Visibility.Collapsed;
        config.RotulacionSettings.BtnGanador = false;
        Config.Config.SaveConfig(config);
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

    public async void MostrarFlyoutEnBoton(string texto, ToggleButton boton, int duracionMs = 2000)
    {
        // Crear el contenido del flyout
        var flyout = new Flyout
        {
            Content = new TextBlock
            {
                Text = texto,
                Padding = new Thickness(10),
                TextWrapping = TextWrapping.Wrap
            },
            Placement = FlyoutPlacementMode.Bottom
        };

        // Asignar el flyout al botón
        FlyoutBase.SetAttachedFlyout(boton, flyout);

        // Mostrarlo
        FlyoutBase.ShowAttachedFlyout(boton);

        // Esperar y luego cerrarlo (WinUI 3 no tiene método "Close", así que accedemos a la instancia abierta)
        await Task.Delay(duracionMs);

        if (flyout is Flyout actualFlyout && actualFlyout.IsOpen)
        {
            actualFlyout.Hide(); // Esta es la línea que lo cierra
        }
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
    private void btnCategoria_Click(object sender, RoutedEventArgs e)
    {
        Premio seleccionado = null;

        if (inCategoria)
        {
            //SALE CATEGORIA
            ViewModel.CategoriaSale();
            inCategoria = false;
        }
        else
        {
            if (selectedItem == null)
            {
                MostrarFlyoutEnBoton("No hay ninguna categoría seleccionada", btnCategoria);
                btnCategoria.IsChecked = false;
            }
            else
            {
                if (selectedItem is Nominado)
                {
                    Nominado nominado = selectedItem as Nominado;
                    seleccionado = ViewModel.allPremios.Find(pre => pre.nominados.Any(n => string.Equals(n.nombre, nominado.nombre)));
                }
                else
                {
                    seleccionado = selectedItem as Premio;
                }
                //IN con envio de info a BS
                ViewModel.CategoriaEntra(seleccionado);
                inCategoria = true;
            }
        }
    }

    private void btnNominado_Click(object sender, RoutedEventArgs e)
    {
        if (inNominado)
        {
            //ENCADENA NOMINADO
            inNominado = true;
            btnNominado.IsChecked = true;
        }
        else
        {
            if (selectedItem == null)
            {
                MostrarFlyoutEnBoton("No hay ninguna nominado seleccionado", btnNominado);
                btnNominado.IsChecked = false;
            }
            else
            {
                if (selectedItem is Premio)
                {
                    MostrarFlyoutEnBoton("Debe seleccionar un nominado", btnNominado);
                    btnNominado.IsChecked = false;
                }
                else
                {
                    //IN con envio de info a BS
                    Nominado nominado = selectedItem as Nominado;
                    Premio seleccionado = ViewModel.allPremios.Find(pre => pre.nominados.Any(n => string.Equals(n.nombre, nominado.nombre)));
                    ViewModel.NominadoEntra(seleccionado, nominado);
                    inNominado = true;
                }

            }
        }
    }
    private void btnNominadoSale_Click(object sender, RoutedEventArgs e)
    {
        if (inNominado)
        {
            //SALE
            ViewModel.NominadoSale();
            inNominado = false;
            btnNominado.IsChecked = false;
        }
    }

    private void btnNominados_Click(object sender, RoutedEventArgs e)
    {
        Premio seleccionado = null;

        if (inLista)
        {
            //SALE Lista de nominados
            inLista = false;
        }
        else
        {
            if (selectedItem == null)
            {
                MostrarFlyoutEnBoton("No hay ninguna categoría seleccionada", btnNominados);
                btnNominados.IsChecked = false;
            }
            else
            {
                if (selectedItem is Nominado)
                {
                    Nominado nominado = selectedItem as Nominado;
                    seleccionado = ViewModel.allPremios.Find(pre => pre.nominados.Any(n => string.Equals(n.nombre, nominado.nombre)));
                }
                else
                {
                    seleccionado = selectedItem as Premio;
                }
                //IN con envio de info a BS
                inLista = true;
            }
        }
    }

    private void btnGafas_Click(object sender, RoutedEventArgs e)
    {
        Premio seleccionado = null;

        if (inGafas)
        {
            //SALE Lista de nominados
            inGafas = false;
        }
        else
        {
            if (selectedItem == null)
            {
                MostrarFlyoutEnBoton("No hay ninguna categoría seleccionada", btnGafas);
                btnGafas.IsChecked = false;
            }
            else
            {
                if (selectedItem is Nominado)
                {
                    Nominado nominado = selectedItem as Nominado;
                    seleccionado = ViewModel.allPremios.Find(pre => pre.nominados.Any(n => string.Equals(n.nombre, nominado.nombre)));
                }
                else
                {
                    seleccionado = selectedItem as Premio;
                }
                //IN con envio de info a BS
                inGafas = true;
            }
        }
    }

    private void btnEntregadores_Click(object sender, RoutedEventArgs e)
    {
        Premio seleccionado = null;

        if (inEntregadores)
        {
            //SALE Lista de entregadores
            ViewModel.EntregadoresSale();
            inEntregadores = false;
        }
        else
        {
            if (selectedItem == null)
            {
                MostrarFlyoutEnBoton("No hay ninguna categoría seleccionada", btnEntregadores);
                btnEntregadores.IsChecked = false;
            }
            else
            {
                if (selectedItem is Nominado)
                {
                    Nominado nominado = selectedItem as Nominado;
                    seleccionado = ViewModel.allPremios.Find(pre => pre.nominados.Any(n => string.Equals(n.nombre, nominado.nombre)));
                }
                else
                {
                    seleccionado = selectedItem as Premio;
                }
                if (seleccionado.entregadores.Count > 0)
                {
                    //IN con envio de info a BS
                    ViewModel.EntregadoresEntra(seleccionado);
                    inEntregadores = true;
                }
                else
                {
                    MostrarFlyoutEnBoton("Esta categoría no tiene entregadores asignados", btnEntregadores);
                    btnEntregadores.IsChecked = false;
                    inEntregadores = false;
                }

            }
        }
    }

    private void btnGanador_Click(object sender, RoutedEventArgs e)
    {
        if (inGanador)
        {
            ViewModel.GanadorSale();
            inGanador = false;
        }
        else
        {
            if (selectedItem == null)
            {
                MostrarFlyoutEnBoton("No hay ninguna categoría seleccionada", btnGanador);
                btnGanador.IsChecked = false;
            }
            else
            {
                if (selectedItem is Premio)
                {
                    Premio p = selectedItem as Premio;
                    if (p.ganador != null)
                    {
                        ViewModel.GanadorEntra(p, p.ganador);
                    }
                    else
                    {
                        MostrarFlyoutEnBoton("No hay ganador por defecto\nSeleccione un \"nominado\"", btnGanador);
                        btnGanador.IsChecked = false;
                    }

                }
                else
                {
                    Nominado nominado = selectedItem as Nominado;
                    Premio seleccionado = ViewModel.allPremios.Find(pre => pre.nominados.Any(n => string.Equals(n.nombre, nominado.nombre)));
                    Premio editado = editada.FirstOrDefault(p => string.Equals(p.nombre, seleccionado.nombre));
                    seleccionado.ganador = seleccionado.nominados.FirstOrDefault(n => string.Equals(n.nombre, nominado.nombre));
                    editado.ganador = editado.nominados.FirstOrDefault(n => string.Equals(n.nombre, nominado.nombre));
                    ViewModel.GanadorEntra(seleccionado, nominado);
                    inGanador = true;
                }

            }
        }
    }


}
