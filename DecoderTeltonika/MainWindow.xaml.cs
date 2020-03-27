using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;



namespace DecoderTeltonika
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BTNenviarArchivo_Click(object sender, RoutedEventArgs e)
        {
            if (DireccionLog.Text.Contains(@"\"))
            {
                OpenFileDialog abrirArchivo = new OpenFileDialog();
                abrirArchivo.DefaultExt = ".txt";
                abrirArchivo.Title = "Seleccionar archivo";
                abrirArchivo.Filter = "Documentos de texto (*.txt)|*.txt" + "|Todos los archivos (*.*)|*.*";


                try
                {
                    if (abrirArchivo.ShowDialog() == true)
                    {
                        string filename = abrirArchivo.FileName;
                        string logParseado = Archivo.Parsear(filename, DireccionLog.Text);
                        Process.Start("notepad.exe", logParseado);
                    }
                }
                catch
                {
                    MessageBox.Show("No se ha seleccionado un archivo válido. Es posible que el fichero contenga paquetes de Codec 8.");
                }
            }
            else MessageBox.Show("No ha especificado una dirección de guardado.");

        }

        private void BTNguardarEn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                DireccionLog.Text = dialog.SelectedPath;
            }
        }

    }
}
