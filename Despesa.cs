using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Globalization;

namespace ProjecteCobolDavid
{
    public class Despesa
    {
        // Max 30
        public string Nom { get; set; }
        // Format 000000.00
        public decimal Cost { get; set; }
        // YYYY-MM-DD
        public DateTime Data { get; set; }
        // Max 20
        public string Tipus { get; set; }

        // Hace público para que el Form pueda llamarlo cuando quiera guardar usando el programa COBOL
        public static void EnviarACobol(Despesa d)
        {
            // Formatem les dades per tenir la longitud fixa que COBOL espera
            string nomFix = d.Nom.PadRight(30).Substring(0, 30);
            // En lloc d'enviar els cèntims, enviem el valor amb decimals (p. ex. 25.37)
            // COBOL acceptarà aquest format en un PIC 9(06)V99 i el desarà correctament
            string costFix = d.Cost.ToString("0.00", CultureInfo.InvariantCulture);
            string dataFix = d.Data.ToString("yyyy-MM-dd");
            string tipusFix = d.Tipus.PadRight(20).Substring(0, 20);
            string arguments = $"\"{nomFix}\" \"{costFix}\" \"{dataFix}\" \"{tipusFix}\"";

            

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "guardar.exe",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }

            // Després d'executar, intentionem llegir l'última línia del fitxer per comprovar què s'ha escrit
            try
            {
                string path = "DESPESES.DAT";
                if (File.Exists(path))
                {
                    var all = File.ReadAllLines(path);
                    if (all.Length > 0)
                    {
                        string last = all[all.Length - 1];
                        Debug.WriteLine("Última línia escrita: " + last);
                        if (last.Length >= 38)
                        {
                         Debug.WriteLine("Cost raw al DAT: '" + last.Substring(30, 8) + "'");
                        }
                    }
                }
            }
            catch { }
        }
        public static List<Despesa> CarregarDades()
        {
            var llista = new List<Despesa>();
            string path = "DESPESES.DAT";

            if (!File.Exists(path)) return llista;

            var linies = File.ReadAllLines(path);

            foreach (var linia in linies)
            {
                // Si la línia és massa curta per tenir almenys el Nom i el Cost, la saltem
                if (linia.Length < 38) continue;

                try
                {
                    Despesa d = new Despesa();

                    // Llegim el Nom (fins a 30 o el que hi hagi)
                    d.Nom = linia.Substring(0, Math.Min(30, linia.Length)).Trim();

                    // Llegim el Cost (està entre la posició 30 i la 38)
                    if (linia.Length >= 38)
                    {
                        string costText = linia.Substring(30, 8).Trim();
                        // El fitxer guarda el cost en cèntims com 8 dígits (per ex. 00001234 = 12.34)
                        if (int.TryParse(costText, out int centsVal))
                        {
                            d.Cost = centsVal / 100m;
                        }
                        else
                        {
                            d.Cost = 0m;
                        }
                    }

                    // Llegim la Data (entre la 38 i la 48)
                    if (linia.Length >= 48)
                    {
                        string dataText = linia.Substring(38, 10).Trim();
                        // Intentem parsejar amb el format que guarda el COBOL
                        DateTime.TryParseExact(dataText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataAux);
                        d.Data = dataAux;
                    }

                    // Llegim el Tipus (de la 48 fins al final)
                    if (linia.Length > 48)
                    {
                        int rest = linia.Length - 48;
                        d.Tipus = linia.Substring(48, Math.Min(20, rest)).Trim();
                    }

                    llista.Add(d);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error processant línia: " + ex.Message);
                }
            }
            return llista;
        }

        // Borra todo el contenido del fichero DESPESES.DAT (vacía el fichero)
        public static void BorrarDat()
        {
            string path = "DESPESES.DAT";
            try
            {
                // Si existe, truncamos el fichero a 0 bytes
                if (File.Exists(path))
                {
                    File.WriteAllText(path, string.Empty, Encoding.UTF8);
                }
                else
                {
                    // Creamos un fichero vacío por si se espera su existencia
                    using (var fs = File.Create(path)) { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error borrando DESPESES.DAT: " + ex.Message);
                throw;
            }
        }
    }
}
