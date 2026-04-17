using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ProjecteCobolDavid
{
    public class Despesa
    {
        // Max 30
        public string Nom { get; set; }
        // Format 000000.00
        public double Cost { get; set; }
        // YYYY-MM-DD
        public DateTime Data { get; set; }
        // Max 20
        public string Tipus { get; set; }

        private void EnviarACobol(Despesa d)
        {
            // Formatem les dades per tenir la longitud fixa que COBOL espera
            string nomFix = d.Nom.PadRight(30).Substring(0, 30);
            string costFix = d.Cost.ToString("000000.00").Replace(",", ""); // Sense coma per a COBOL
            string dataFix = d.Data.ToString("yyyy-MM-dd").PadRight(10); string tipusFix = d.Tipus.PadRight(20).Substring(0, 20);
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
        }
        public List<Despesa> CarregarDades()
        {
            var llista = new List<Despesa>();
            string path = "DESPESES.DAT";

            if (!File.Exists(path)) return llista;

            foreach (var linia in File.ReadAllLines(path))
            {
                if (linia.Length >= 68) // Longitud total del registre (30+8+10+20)
                {
                    llista.Add(new Despesa
                    {
                        Nom = linia.Substring(0, 30).Trim(),
                        // Convertim el format 00000000 (sense coma) a double
                        Cost = double.Parse(linia.Substring(30, 8)) / 100,
                        Data = DateTime.Parse(linia.Substring(38, 10).Trim()),
                        Tipus = linia.Substring(48, 20).Trim()
                    });
                }
            }
            return llista;
        }
    }
}
