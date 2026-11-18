using Plugin.Maui.Audio;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private DateTime ultimaPulsacion = DateTime.MinValue;
        private bool estaCompleto = false;

        // Banderas independientes para controlar cada sonido
        private bool sonido30Reproducido = false;
        private bool sonido100Reproducido = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnBotonSensibilidadClicked(object sender, EventArgs e)
        {
            DateTime ahora = DateTime.Now;

            if (estaCompleto)
                return;

            if (ultimaPulsacion != DateTime.MinValue)
            {
                double tiempoTranscurridoMs = (ahora - ultimaPulsacion).TotalMilliseconds;

                if (tiempoTranscurridoMs < 150)
                {
                    BarraSensibilidad.Progress = Math.Min(1.0, BarraSensibilidad.Progress + 0.15);
                }
                else
                {
                    BarraSensibilidad.Progress = Math.Max(0.0, BarraSensibilidad.Progress - 0.3);
                }
            }
            else
            {
                BarraSensibilidad.Progress = 0.1;
            }

            ultimaPulsacion = ahora;

            // 1. Primer sonido al alcanzar o superar el 30% (0.3)
            if (BarraSensibilidad.Progress >= 0.3 && !sonido30Reproducido)
            {
                sonido30Reproducido = true;
                await ReproducirSonidoAsync("sonido_30.mp3");
            }

            // 2. Segundo sonido y cambio a verde al alcanzar el 100% (1.0)
            if (BarraSensibilidad.Progress >= 1.0 && !sonido100Reproducido)
            {
                sonido100Reproducido = true;
                estaCompleto = true;

                BotonSensibilidad.BackgroundColor = Colors.Green;
                BotonSensibilidad.TextColor = Colors.White;
                BotonSensibilidad.Text = "¡Sensibilidad máxima!";
                EstadoLabel.Text = "¡Completado! Presionaste lo suficientemente rápido.";

                await ReproducirSonidoAsync("sonido_100.mp3");
            }
        }

        private async Task ReproducirSonidoAsync(string nombreArchivo)
        {
            try
            {
                var stream = await FileSystem.OpenAppPackageFileAsync(nombreArchivo);
                var player = AudioManager.Current.CreatePlayer(stream);
                player.Play();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error de audio", $"No se pudo reproducir {nombreArchivo}: {ex.Message}", "OK");
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            // Restablecer la barra, estados y banderas de ambos sonidos
            BarraSensibilidad.Progress = 0.0;
            ultimaPulsacion = DateTime.MinValue;
            estaCompleto = false;
            sonido30Reproducido = false;
            sonido100Reproducido = false;

            BotonSensibilidad.ClearValue(Button.BackgroundColorProperty);
            BotonSensibilidad.ClearValue(Button.TextColorProperty);
            BotonSensibilidad.Text = "¡Presióname rápido!";
            EstadoLabel.Text = "Presiona el botón muy rápido para llenar la barra";
        }
    }
}