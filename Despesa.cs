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

        public static void EnviarACobol(Despesa d)
        {
            // Formatem les dades per tenir la longitud fixa que COBOL espera
            string nomFix = d.Nom.PadRight(30).Substring(0, 30);
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

            // Després d'executar, intentem llegir l'última línia del fitxer per comprovar què s'ha escrit
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

        // Esborra tot el contingut del fitxer .dat
        public static void BorrarDat()
        {
            string path = "DESPESES.DAT";
            try
            {
                // Si existeix, trunquem el fitxer a 0 bytes
                if (File.Exists(path))
                {
                    File.WriteAllText(path, string.Empty, Encoding.UTF8);
                }
                else
                {
                    // Creem un fitxer buit per si s'espera la seva existència
                    using (var fs = File.Create(path)) { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error borrando DESPESES.DAT: " + ex.Message);
                throw;
            }
        }

        // Esborra una despesa específica que coincideix amb Nom, Cost i Data
        public static void EsborrarDespesa(string nom, decimal cost, DateTime data)
        {
            string path = "DESPESES.DAT";
            try
            {
                if (!File.Exists(path)) return;

                // Llegim totes les línies del fitxer
                var totes = File.ReadAllLines(path);

                // Creem una nova llista sense la línia que correspon exactament al nom, cost i data
                var noves = new List<string>();
                bool trobat = false;

                foreach (var linia in totes)
                {
                    if (linia.Length >= 68) // Longitud total del registre
                    {
                        string nomAlFitxer = linia.Substring(0, 30).Trim();

                        // Parsegem el cost (8 dígits en posició 30-37)
                        string costText = linia.Substring(30, 8).Trim();
                        decimal costAlFitxer = 0m;
                        if (int.TryParse(costText, out int centsVal))
                        {
                            costAlFitxer = centsVal / 100m;
                        }

                        // Parsegem la data (10 caracteres en posició 38-47)
                        string dataText = linia.Substring(38, 10).Trim();
                        DateTime dataAlFitxer = DateTime.MinValue;
                        DateTime.TryParseExact(dataText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataAlFitxer);

                        // Comprovem si nom, cost i data coincideixen
                        if (nomAlFitxer == nom.Trim() && costAlFitxer == cost && dataAlFitxer.Date == data.Date && !trobat)
                        {
                            trobat = true;
                            continue;
                        }
                    }

                    // Mantenim totes les altres línies
                    noves.Add(linia);
                }

                // Reescrivim el fitxer sense línies en blanc
                if (noves.Count > 0)
                {
                    File.WriteAllLines(path, noves, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllText(path, string.Empty, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error esborrant despesa: " + ex.Message);
                throw;
            }
        }
    }
}
