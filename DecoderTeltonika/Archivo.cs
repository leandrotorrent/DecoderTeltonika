using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DecoderTeltonika
{
    class Archivo
    {
        public static string Parsear(string filenameOriginal, string direccion)
        {
            string pathParseado = direccion + @"\";
            DateTime fecha = DateTime.Now;
            string fechaFN = fecha.ToString("yyyyMMdd_HHmmss");
            string filenameParseado = "LogTeltonika" + fechaFN + ".txt";
            string fullFilenameParseado = pathParseado + filenameParseado;
            using (StreamReader lector = new StreamReader(filenameOriginal))
            {
                using (StreamWriter sw = new StreamWriter(fullFilenameParseado))
                {
                    int contadorPaquete = 0;
                    while (lector.Peek() > -1)
                    {
                        string linea = lector.ReadLine();
                        Paquete paquete = new Paquete(linea);

                        if (!String.IsNullOrEmpty(linea) && linea.Contains("CAFE") && linea.Length > 86 )
                        {
                            contadorPaquete++;
                            sw.Write("========================================================================");
                            sw.WriteLine();
                            sw.Write("Paquete " + contadorPaquete);
                            sw.WriteLine();
                            sw.Write("========================================================================");
                            sw.WriteLine();
                            Escribir(sw, paquete);
                        }
                        
                    }
                }
            }
            return fullFilenameParseado;
        }

        public static void Escribir(StreamWriter sw, Paquete paquete)
        {
            Dictionary<string, string> diccionarioParseado = new Dictionary<string, string>();
            diccionarioParseado = paquete.Parsear();
            
            int contadorAvlData = 1;
            foreach (KeyValuePair<string, string> elemento in diccionarioParseado)
            {
                if (elemento.Key.Contains("Fecha y Hora"))
                {
                    sw.WriteLine();
                    sw.Write("AVL Data " + contadorAvlData.ToString());
                    sw.WriteLine();
                    contadorAvlData++;
                }

                if (elemento.Key.Contains("."))
                {
                    if (elemento.Key.Contains(".ID") || elemento.Key.Contains(".Valor"))
                    {
                        sw.Write("      " + elemento.Key.Substring(6) + ": " + elemento.Value);
                        sw.WriteLine();
                    }
                    else
                    {
                        sw.Write("   " + elemento.Key.Substring(3) + ": " + elemento.Value);
                        sw.WriteLine();
                    }
                }

                else
                {
                    sw.Write(elemento.Key + ": " + elemento.Value);
                    sw.WriteLine();
                }

                
                


            }

        }

    }
}
