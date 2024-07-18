using Syncfusion.Maui.Graphics.Internals;
using Syncfusion.Maui.SignaturePad;

namespace Ejercicio2._2_Grupo2.Views;

public partial class PageInit : ContentPage
{
    Controllers.FirmasController controller;

    public PageInit()
    {
        InitializeComponent();
        controller = new Controllers.FirmasController();
        SfSignaturePad signaturePad = new SfSignaturePad();

        txtNombres.TextChanged += OnFieldChanged;
        txtDescrip.TextChanged += OnFieldChanged;

        btnDownloadFirma.IsEnabled = false;
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        ImageSource? source = signaturePad.ToImageSource();
        string base64String = await ImageSourceToBase64(source);

        string Nombres = txtNombres.Text;

        if (string.IsNullOrEmpty(Nombres))
        {
            await DisplayAlert("Error", "Por favor ingrese el nombre", "OK");
            return;
        }

        string Descripcion = txtDescrip.Text;

        if (string.IsNullOrEmpty(Descripcion))
        {
            await DisplayAlert("Error", "Por favor ingrese una breve descripcion", "OK");
            return;
        }

        var firma = new Models.Firmas
        {
            nombres = txtNombres.Text,
            descripcion = txtDescrip.Text,
            ImageFirma = base64String,
            Fecha = DateTime.Now
        };

        try
        {
            if (controller != null)
            {
                if (await controller.Store(firma) > 0)
                {
                    await DisplayAlert("Aviso", "Registro Ingresado con Exito!", "OK");
                    await Navigation.PopAsync();

                    signaturePad.Clear();
                    txtNombres.Text = string.Empty;
                    txtDescrip.Text = string.Empty;
                }
                else
                {
                    await DisplayAlert("Error", "Ocurrio un Error", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrio un Error: {ex.Message}", "OK");
        }
    }

    private async Task<String> ImageSourceToBase64(ImageSource imageSource)
    {
        if (imageSource is StreamImageSource streamImageSource)
        {
            using (var stream = await streamImageSource.Stream(CancellationToken.None))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
        }
        return null;
    }

    private void OnClearButtonClicked(object? sender, EventArgs e)
    {
        signaturePad.Clear();
    }

    private async void OnDownloadButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            ImageSource? source = signaturePad.ToImageSource();

            string fileName = txtNombres.Text;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                await DisplayAlert("Error", "Por favor ingrese un nombre para el archivo.", "OK");
                return;
            }

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            fileName = $"firma_{fileName}.png";

            string folderPath = string.Empty;

#if ANDROID
            folderPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
#endif

            if (!string.IsNullOrEmpty(folderPath))
            {
                string filePath = Path.Combine(folderPath, fileName);
                if (File.Exists(filePath))
                {
                    await DisplayAlert("Error", "Ya existe un archivo con ese nombre. Por favor ingrese un nombre diferente.", "OK");
                    return;
                }

                var imageStream = await ((StreamImageSource)source).Stream(System.Threading.CancellationToken.None);
                using (var memoryStream = new MemoryStream())
                {
                    await imageStream.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();

                    File.WriteAllBytes(filePath, imageBytes);

                    await DisplayAlert("Guardado", "La firma se ha guardado correctamente en: " + filePath, "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la ruta de la carpeta de descargas.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Ocurrió un error al guardar la imagen: " + ex.Message, "OK");
        }
    }

    private void OnFieldChanged(object sender, TextChangedEventArgs e)
    {
        ValidateFields();
    }

    private void ValidateFields()
    {
        btnDownloadFirma.IsEnabled = !string.IsNullOrEmpty(txtNombres.Text) &&
                                     !string.IsNullOrEmpty(txtDescrip.Text);
    }
}
