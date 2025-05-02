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

    public VariosViewModel ViewModel
    {
        get;
    }

    public VariosPage()
    {
        ViewModel = App.GetService<VariosViewModel>();
        config = Config.Config.GetInstance();
        InitializeComponent();
        ValoresPorDefecto();
    }

    private void ValoresPorDefecto()
    {
        listBoxTiempos.SelectedIndex = 0;
        tggEditor.IsOn = true;
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

    private void listLocalizaciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void tggEditorLocalizaciones_Toggled(object sender, RoutedEventArgs e)
    {

    }

    private void btnDeleteLocalizacion_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnEditLocalizacion_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnAddLozalizacion_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnPlayLocalizacion_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnStopLocalizacion_Click(object sender, RoutedEventArgs e)
    {

    }






    //LOCALIZACIONES

}
