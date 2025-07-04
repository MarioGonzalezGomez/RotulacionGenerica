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

namespace Generico_Front.Views;

public sealed partial class VariosPage : Page
{

    private Config.Config config;
    public DateTimeOffset DefaultDate { get; set; } = DateTimeOffset.Now;

    private Localizacion localizacionSeleccionada;

    public VariosViewModel ViewModel
    {
        get;
    }

    public VariosPage()
    {
        ViewModel = App.GetService<VariosViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
        OcultarElementos();
        ValoresPorDefecto();
    }
    private void OcultarElementos()
    {
        if (config.PestanasActivas.Tiempos)
        {
            ContentArea.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        }
        else
        {
            ContentArea.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
        }
        if (config.PestanasActivas.Directos)
        {
            ContentArea.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
        }
        else
        {
            ContentArea.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
        }
    }

    private void ValoresPorDefecto()
    {
        listBoxTiempos.SelectedIndex = 0;
        tggEditor.IsOn = true;
        listLocalizaciones.ItemsSource = ViewModel.Localizaciones;
        ViewModel.CargarLocalizaciones();
    }

    //MOSTRAR MENSAJES
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

    //TIEMPOS
    private void listBoxTiempos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        tggActual.Visibility = Visibility.Collapsed;
        cmbBoxEditor.Visibility = Visibility.Collapsed;
        datePicker.Visibility = Visibility.Collapsed;
        timePicker.Visibility = Visibility.Collapsed;
        stckCronoPicker.Visibility = Visibility.Collapsed;
        stckBotoneraCrono.Visibility = Visibility.Collapsed;

        if (string.Equals(listBoxTiempos.SelectedItem.ToString(), "Reloj"))
        {
            tggActual.Content = "HORA DEL SISTEMA";
            CargarTimezones();
            tggActual.Visibility = Visibility.Visible;
            cmbBoxEditor.Visibility = Visibility.Visible;
            datePicker.Visibility = Visibility.Collapsed;
            timePicker.Visibility = Visibility.Visible;
            stckCronoPicker.Visibility = Visibility.Collapsed;
            stckBotoneraCrono.Visibility = Visibility.Collapsed;

        }
        if (string.Equals(listBoxTiempos.SelectedItem.ToString(), "Fecha"))
        {
            tggActual.Content = "FECHA DEL SISTEMA";
            tggActual.Visibility = Visibility.Visible;
            cmbBoxEditor.Visibility = Visibility.Collapsed;
            datePicker.Visibility = Visibility.Visible;
            timePicker.Visibility = Visibility.Collapsed;
            stckCronoPicker.Visibility = Visibility.Collapsed;
            stckBotoneraCrono.Visibility = Visibility.Collapsed;
        }
        if (string.Equals(listBoxTiempos.SelectedItem.ToString(), "Crono"))
        {
            CargarTiposCrono();
            tggActual.Visibility = Visibility.Collapsed;
            cmbBoxEditor.Visibility = Visibility.Visible;
            datePicker.Visibility = Visibility.Collapsed;
            timePicker.Visibility = Visibility.Collapsed;
            stckCronoPicker.Visibility = Visibility.Visible;
            stckBotoneraCrono.Visibility = Visibility.Visible;
        }
    }
    private void CargarTimezones()
    {
        var timeZones = TimeZoneInfo.GetSystemTimeZones();
        cmbBoxEditor.ItemsSource = timeZones;
        cmbBoxEditor.DisplayMemberPath = "DisplayName";
        cmbBoxEditor.SelectedValuePath = "Id";
        cmbBoxEditor.SelectedItem = TimeZoneInfo.Local;
    }
    private void CargarTiposCrono()
    {
        List<string> cronos = new List<string> { "Progresivo", "Regresivo" };
        cmbBoxEditor.ItemsSource = cronos;
        cmbBoxEditor.DisplayMemberPath = null;
        cmbBoxEditor.SelectedValuePath = null;
        cmbBoxEditor.SelectedIndex = 0;
    }

    private void tggEditor_Toggled(object sender, RoutedEventArgs e)
    {
        if (stckEdicion != null)
            stckEdicion.Visibility = tggEditor.IsOn ? Visibility.Visible : Visibility.Collapsed;
    }

    private void cmbBoxEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        tggActual.IsChecked = false;
    }
    private void datePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
    {
        tggActual.IsChecked = false;
    }

    private void btnResetCrono_Click(object sender, RoutedEventArgs e)
    {

    }
    private void btnPlayCrono_Click(object sender, RoutedEventArgs e)
    {

    }
    private void btnPauseCrono_Click(object sender, RoutedEventArgs e)
    {

    }

    //IN y OUT de TIEMPOS
    private void btnPlayTiempos_Click(object sender, RoutedEventArgs e)
    {
        if (string.Equals(listBoxTiempos.SelectedItem.ToString(), "Reloj"))
        {
            DateTime hora = ObtenerHora();
            ViewModel.EntraReloj(hora);
        }
        if (string.Equals(listBoxTiempos.SelectedItem.ToString(), "Fecha"))
        {
            DateTime fecha = tggActual.IsChecked == true ? DateTime.Today : datePicker.Date.DateTime;
            ViewModel.EntraFecha(fecha);
        }
        if (string.Equals(listBoxTiempos.SelectedItem.ToString(), "Crono"))
        {

            int horas = (int)boxHoras.Value;
            int minutos = (int)boxMinutos.Value;
            int segundos = (int)boxSegundos.Value;

            if (horas == 0 && minutos == 0 && segundos == 0)
            {
                ShowDialog("Definir crono", "No se ha definido ninguna duración para el cronómetro.");
            }
            else
            {
                TimeSpan crono = new TimeSpan(horas, minutos, segundos);
                ViewModel.EntraCrono(cmbBoxEditor.SelectedItem.ToString(), crono);
            }

        }
    }
    private DateTime ObtenerHora()
    {
        DateTime horaFinal;

        if (tggActual.IsChecked == true)
        {
            // Hora actual del sistema
            horaFinal = DateTime.Now;
        }
        else if (timePicker.Time != null && timePicker.Time != TimeSpan.Zero)
        {
            // Hora seleccionada manualmente (TimePicker)
            var hoy = DateTime.Today;
            horaFinal = hoy + timePicker.Time;
        }
        else if (cmbBoxEditor.SelectedItem is TimeZoneInfo zonaSeleccionada)
        {
            // Hora en la zona horaria seleccionada
            DateTime utcAhora = DateTime.UtcNow;
            horaFinal = TimeZoneInfo.ConvertTimeFromUtc(utcAhora, zonaSeleccionada);
        }
        else
        {
            // Valor por defecto si no se cumple nada
            horaFinal = DateTime.Now;
        }
        return horaFinal;
    }

    private void btnStopTiempos_Click(object sender, RoutedEventArgs e)
    {
        if (string.Equals(listBoxTiempos.SelectedItem.ToString(), "Reloj"))
        {
            ViewModel.SaleReloj();
        }
        if (string.Equals(listBoxTiempos.SelectedItem.ToString(), "Fecha"))
        {
            ViewModel.SaleFecha();
        }
        if (string.Equals(listBoxTiempos.SelectedItem.ToString(), "Crono"))
        {
            ViewModel.SaleCrono();
        }
    }

    //LOCALIZACIONES
    private void tggEditorLocalizaciones_Toggled(object sender, RoutedEventArgs e)
    {
        if (stckBotoneraEdicionLocalizaciones != null)
        {
            stckBotoneraEdicionLocalizaciones.Visibility = tggEditorLocalizaciones.IsOn ? Visibility.Visible : Visibility.Collapsed;
            boxPrincipalLocalizacion.Visibility = tggEditorLocalizaciones.IsOn ? Visibility.Visible : Visibility.Collapsed;
            boxSecundariolLocalizacion.Visibility = tggEditorLocalizaciones.IsOn ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    private void listLocalizaciones_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        var ordenado = sender.Items.Cast<Localizacion>().ToList();
        ViewModel.ActualizarPosiciones(ordenado);
    }
    private void listLocalizaciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (listLocalizaciones.SelectedItem != null)
        {
            localizacionSeleccionada = listLocalizaciones.SelectedItem as Localizacion;
            boxPrincipalLocalizacion.Text = localizacionSeleccionada.principal;
            boxSecundariolLocalizacion.Text = localizacionSeleccionada.secundario;
            txtRuta.Visibility = string.IsNullOrEmpty(localizacionSeleccionada.rutaVideo) ? Visibility.Collapsed : Visibility.Visible;
            txtRuta.Text = string.IsNullOrEmpty(localizacionSeleccionada.rutaVideo) ? "" : System.IO.Path.GetFileName(localizacionSeleccionada.rutaVideo);
            btnAddReloj.IsChecked = localizacionSeleccionada.conReloj;
        }
    }
    private void listLocalizaciones_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var clickedItem = ((FrameworkElement)e.OriginalSource).DataContext;

        if (clickedItem != null)
        {
            listLocalizaciones.SelectedItem = clickedItem;
        }
    }
    private void MenuFlyoutEditar_Click(object sender, RoutedEventArgs e)
    {
        if (!tggEditorLocalizaciones.IsOn)
        {
            tggEditorLocalizaciones.IsOn = true;
        }
    }

    private async void btnAddVideo_Click(object sender, RoutedEventArgs e)
    {
        if (localizacionSeleccionada != null)
        {

            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            var openPicker = new FileOpenPicker();

            var window = App.MainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".mov");
            openPicker.FileTypeFilter.Add(".avi");
            openPicker.FileTypeFilter.Add(".webm");
            openPicker.FileTypeFilter.Add(".wmv");

            var file = await abrirFilePicker(openPicker);
            if (file != null)
            {
                localizacionSeleccionada.rutaVideo = file.Path;
                txtRuta.Text = $"File: {file.Name}";
                txtRuta.Visibility = Visibility.Visible;
            }
            else
            {
                txtRuta.Text = System.IO.Path.GetFileName(localizacionSeleccionada.rutaVideo);
            }

            senderButton.IsEnabled = true;
        }
        else
        {
            ShowDialog("Seleccionar localización", "Debe seleccionar una localización antes de añadir un vídeo.");
        }
    }
    private async Task<Windows.Storage.StorageFile> abrirFilePicker(FileOpenPicker openPicker)
    {
        var file = await openPicker.PickSingleFileAsync();
        return file;
    }

    private void btnAddReloj_Click(object sender, RoutedEventArgs e)
    {
        if (localizacionSeleccionada != null)
        {
            localizacionSeleccionada.conReloj = btnAddReloj.IsChecked.Value;
        }
        else
        {
            ShowDialog("Seleccionar localización", "Debe seleccionar una localización antes de añadir un reloj.");
            btnAddReloj.IsChecked = false;
        }
    }

    private void MenuFlyoutBorrar_Click(object sender, RoutedEventArgs e)
    {
        if (listLocalizaciones.SelectedItem != null)
        {
            Localizacion actual = listLocalizaciones.SelectedItem as Localizacion;
            EliminarLocalizacion(actual);
        }
    }

    private void btnDeleteLocalizacion_Click(object sender, RoutedEventArgs e)
    {
        if (listLocalizaciones.SelectedItem != null)
        {
            Localizacion selected = listLocalizaciones.SelectedItem as Localizacion;
            EliminarLocalizacion(selected);
            boxPrincipalLocalizacion.Text = "";
            boxSecundariolLocalizacion.Text = "";
            txtRuta.Text = "";
            txtRuta.Visibility = Visibility.Collapsed;
        }
    }
    private async void EliminarLocalizacion(Localizacion localizacion)
    {
        Tip.Target = btnDeleteLocalizacion;
        Tip.Title = "Eliminada";
        await ViewModel.EliminarLocalizacion(localizacion);
        AbrirTip();
    }

    private void btnEditLocalizacion_Click(object sender, RoutedEventArgs e)
    {
        if (listLocalizaciones.SelectedItem != null)
        {
            Localizacion actual = listLocalizaciones.SelectedItem as Localizacion;
            Localizacion modificada = new Localizacion();
            modificada.id = actual.id;
            modificada.posicion = actual.posicion;
            modificada.principal = boxPrincipalLocalizacion.Text;
            modificada.secundario = boxSecundariolLocalizacion.Text;
            if (localizacionSeleccionada != null)
            {
                modificada.rutaVideo = localizacionSeleccionada.rutaVideo;
                modificada.conReloj = localizacionSeleccionada.conReloj;
            }
            else
            {
                modificada.rutaVideo = "";
            }
            EditarLocalizacion(modificada);
            listLocalizaciones.SelectedIndex = modificada.posicion - 1;
        }
    }
    private async void EditarLocalizacion(Localizacion localizacion)
    {
        Tip.Target = btnEditLocalizacion;
        Tip.Title = "Editada con éxito";
        await ViewModel.GuardarLocalizacion(localizacion);
        AbrirTip();
    }

    private void btnAddLozalizacion_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(boxPrincipalLocalizacion.Text))
        {
            Localizacion nueva = new Localizacion();
            nueva.id = 0;
            nueva.posicion = ViewModel.Localizaciones.Count > 0 ? ViewModel.Localizaciones.Max(l => l.posicion) : 0;
            nueva.principal = boxPrincipalLocalizacion.Text.Trim();
            nueva.secundario = boxSecundariolLocalizacion.Text.Trim();
            if (localizacionSeleccionada != null)
            {
                nueva.rutaVideo = localizacionSeleccionada.rutaVideo;
                nueva.conReloj = localizacionSeleccionada.conReloj;
            }
            else
            {
                nueva.rutaVideo = "";
            }
            GuardarLocalizacion(nueva);
            listLocalizaciones.SelectedIndex = ViewModel.Localizaciones.Count - 1;
        }
    }
    private async void GuardarLocalizacion(Localizacion localizacion)
    {
        Tip.Target = btnAddLozalizacion;
        Tip.Title = "Guardado";
        await ViewModel.GuardarLocalizacion(localizacion);
        AbrirTip();
    }

    //ACCIONES DE LOCALIZACIONES
    private void btnPlayLocalizacion_Click(object sender, RoutedEventArgs e)
    {
        if (localizacionSeleccionada != null)
        {
            ViewModel.EntraLocalizacion(localizacionSeleccionada);
        }
        else
        {
            Tip.Target = btnPlayLocalizacion;
            Tip.Title = "Seleccione un localizador";
            AbrirTip();
        }
    }

    private void btnStopLocalizacion_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.SaleLocalizacion();
    }
}
