using System.Globalization;
using System.IO;
using System.Collections.Generic;
using Microsoft.Reporting.WinForms;
using System.Diagnostics;

namespace ProjecteCobolDavid
{
    public partial class Form1 : Form
    {
        // Variables per recordar l'última columna per la qual s'ha ordenat
        private string _lastSortColumn = null;
        private bool _lastSortAscending = true;
        
        // Constructor del formulari
        public Form1()
        {
            InitializeComponent();
            
            // Carreguem les dades inicials a la graella
            dgvDespeses.DataSource = Despesa.CarregarDades(txtUsuari.Text);
            FormatCostColumn();
            
            // Establim el mes i any actuals per defecte
            cmbMes.SelectedIndex = DateTime.Now.Month - 1;
            numAny.Value = DateTime.Now.Year;
            
            // Afegim els events per actualitzar el filtre quan canvien els controls
            txtUsuari.TextChanged += (s, e) => AplicarFiltreTemporal();
            cmbMes.SelectedIndexChanged += (s, e) => AplicarFiltreTemporal();
            numAny.ValueChanged += (s, e) => AplicarFiltreTemporal();
            AplicarFiltreTemporal();
            
            // Configurem el NumericUpDown per mostrar 2 decimals
            try
            {
                numCost.DecimalPlaces = 2;
                numCost.Increment = 0.01m;
            }
            catch { }
            
            // Permetre ordenar la taula clicant a les capçaleres
            dgvDespeses.ColumnHeaderMouseClick += dgvDespeses_ColumnHeaderMouseClick;
        }
        
        // Botó guardar: enregistra una nova despesa
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Verifiquem que hi hagi un nom d'usuari
            if (string.IsNullOrWhiteSpace(txtUsuari.Text))
            {
                MessageBox.Show("Escriu un nom d'usuari primer!");
                return;
            }
            
            // Validem que tots els camps obligatoris estiguin complerts
            if (!ValidateInputs(out string missing))
            {
                MessageBox.Show($"Falten els següents camps obligatoris: {missing}", "Camps obligatoris", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Creem un objecte Despesa amb les dades del formulari
            Despesa nova = new Despesa()
            {
                Nom = txtNom.Text,
                Cost = numCost.Value,  
                Data = dtpData.Value,  
                Tipus = cmbTipus.Text  
            };

            // Enviem la despesa al programa COBOL (guardar.exe)
            Despesa.EnviarACobol(nova, txtUsuari.Text);

            // Actualitzem la graella per mostrar la nova despesa
            AplicarFiltreTemporal();
            ActualitzarTotalCosts();
            FormatCostColumn();
            dgvDespeses.Refresh();
            MessageBox.Show("Despesa enregistrada correctament per EconoParse!");
        }

        // Botó actualitzar: recarrega totes les dades del fitxer
        private void btnActualitzar_Click(object sender, EventArgs e)
        {
            dgvDespeses.DataSource = Despesa.CarregarDades(txtUsuari.Text);
            FormatCostColumn();
        }

        // Botó esborrar fitxer: elimina tot el contingut de DESPESES.DAT
        private void btnBorrarDat_Click(object sender, EventArgs e)
        {
            // Demanem confirmació abans d'esborrar
            var resp = MessageBox.Show("Segur que vols esborrar tot el contingut de DESPESES.DAT?", "Confirmar esborrat", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resp != DialogResult.Yes) return;

            try
            {
                // Crida al mètode que esborra el fitxer
                Despesa.BorrarDat(txtUsuari.Text);
                
                // Recarreguem la graella buida
                dgvDespeses.DataSource = Despesa.CarregarDades(txtUsuari.Text);
                FormatCostColumn();
                MessageBox.Show("DESPESES.DAT s'ha esborrat correctament.", "Fet", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error esborrant DESPESES.DAT:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botó mostrar informe: crea un informe amb les dades filtrades
        private void btnMostrarInforme_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtenim les dades filtrades que es mostren al DataGridView
                var datosFiltrats = dgvDespeses.DataSource as List<Despesa>;

                // Verifiquem que hi hagi dades per mostrar
                if (datosFiltrats == null || datosFiltrats.Count == 0)
                {
                    MessageBox.Show("No hi ha dades filtrades per mostrar a l'informe.", "Avís", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Obrim la finestra de l'informe i li passem les dades filtrades
                var f = new ReportForm();
                f.LoadReport(datosFiltrats, txtUsuari.Text);
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error mostrant l'informe:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Formata la columna Cost per mostrar 2 decimals
        private void FormatCostColumn()
        {
            if (dgvDespeses.Columns.Contains("Cost"))
            {
                var col = dgvDespeses.Columns["Cost"];
                col.DefaultCellStyle.Format = "N2"; // Format amb 2 decimals
                col.DefaultCellStyle.FormatProvider = CultureInfo.InvariantCulture;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        // Event que es dispara quan fem clic a les capçaleres de la taula per ordenar
        private void dgvDespeses_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            // Validem que el clic sigui en una columna vàlida
            if (e.ColumnIndex < 0 || e.ColumnIndex >= dgvDespeses.Columns.Count) return;
            var col = dgvDespeses.Columns[e.ColumnIndex];
            
            // Obtenim el nom de la propietat de la columna
            var propName = string.IsNullOrEmpty(col.DataPropertyName) ? col.Name : col.DataPropertyName;
            if (string.IsNullOrEmpty(propName)) return;

            // Si fem clic a la mateixa columna, alternem l'ordre (ascendent/descendent)
            if (_lastSortColumn == propName)
                _lastSortAscending = !_lastSortAscending;
            else
            {
                // Si és una columna nova, comença amb ordre ascendent
                _lastSortColumn = propName;
                _lastSortAscending = true;
            }

            try
            {
                // Carreguem les dades i obtenim la propietat de la classe Despesa
                var list = Despesa.CarregarDades(txtUsuari.Text);
                var prop = typeof(Despesa).GetProperty(propName);
                if (prop == null)
                {
                    return;
                }

                // Ordenem la llista segons la propietat i la direcció
                IOrderedEnumerable<Despesa> sorted;
                if (_lastSortAscending)
                    sorted = list.OrderBy(x => prop.GetValue(x, null));
                else
                    sorted = list.OrderByDescending(x => prop.GetValue(x, null));

                // Actualitzem la graella amb les dades ordenades
                dgvDespeses.DataSource = sorted.ToList();
                FormatCostColumn();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al ordenar: " + ex.Message);
            }
        }

        // Valida que els camps obligatoris estiguin complerts
        private bool ValidateInputs(out string missing)
        {
            var missingList = new List<string>();
            
            // Comprovam cada camp obligatori
            if (string.IsNullOrWhiteSpace(txtNom.Text)) missingList.Add("Nom");
            if (numCost.Value <= 0m) missingList.Add("Cost");
            if (string.IsNullOrWhiteSpace(cmbTipus.Text)) missingList.Add("Tipus");
            
            // Retornam una cadena amb els camps que falten
            missing = string.Join(", ", missingList);
            return missingList.Count == 0;
        }

        // Botó esborrar: elimina una despesa seleccionada de la taula
        private void btnEsborrar1_Click(object sender, EventArgs e)
        {
            // Verificam que hi hagi una fila seleccionada
            if (dgvDespeses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Si us plau, selecciona una despesa per esborrar.", "Cap selecció", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtenim les dades de la fila seleccionada
            var selectedRow = dgvDespeses.SelectedRows[0];
            var nomValue = selectedRow.Cells["Nom"].Value;
            var costValue = selectedRow.Cells["Cost"].Value;
            var dataValue = selectedRow.Cells["Data"].Value;

            // Validam que els valors no siguin nuls
            if (nomValue == null || costValue == null || dataValue == null)
            {
                MessageBox.Show("No es pot obtenir les dades de la despesa seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Convertim els valors al tipus correcte
            string nomDespesa = nomValue.ToString();
            decimal costDespesa = Convert.ToDecimal(costValue);
            DateTime dataDespesa = Convert.ToDateTime(dataValue);

            // Demanem confirmació mostrant les dades de la despesa a esborrar
            var resp = MessageBox.Show($"Estàs segur que vols esborrar la despesa:\n\nNom: {nomDespesa.Trim()}\nCost: {costDespesa:N2}€\nData: {dataDespesa:yyyy-MM-dd}?", 
                "Confirmar esborrat", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;

            try
            {
                // Crida al mètode que esborra la despesa específica del fitxer
                Despesa.EsborrarDespesa(nomDespesa, costDespesa, dataDespesa, txtUsuari.Text);

                // Actualitzem la graella
                AplicarFiltreTemporal();
                ActualitzarTotalCosts();
                FormatCostColumn();

                MessageBox.Show("Despesa esborrada correctament.", "Fet", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error esborrant la despesa:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Aplica el filtre temporal (mes i any) a la taula
        private void AplicarFiltreTemporal()
        {
            // Si no hi ha usuari seleccionat, netegem la graella
            if (string.IsNullOrWhiteSpace(txtUsuari.Text))
            {
                dgvDespeses.DataSource = null;
                ActualitzarTotalCosts();
                return;
            }

            // Obtenim el mes i l'any seleccionats als controls
            int mes = cmbMes.SelectedIndex + 1;
            int any = (int)numAny.Value;

            if (mes <= 0) return;

            try
            {
                // Carreguem les dades filtrades per mes i any
                var dadesFiltrades = Despesa.CarregarDadesPerMes(txtUsuari.Text, mes, any);

                // Actualitzem la graella amb les dades filtrades
                dgvDespeses.DataSource = null;
                dgvDespeses.DataSource = dadesFiltrades;

                // Formategem i actualitzem el total
                FormatCostColumn();
                ActualitzarTotalCosts();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error filtrant dades: " + ex.Message);
            }
        }

        // Calcula i mostra el total dels costs de les despeses filtrades
        private void ActualitzarTotalCosts()
        {
            decimal total = 0;
            
            // Si hi ha dades al DataGridView, sumam tots els costs
            if (dgvDespeses.DataSource is List<Despesa> despeses)
            {
                total = despeses.Sum(d => d.Cost);
            }
            
            // Mostrem el total formatat amb 2 decimals
            txtTotal.Text = total.ToString("N2");
        }
    }
}
