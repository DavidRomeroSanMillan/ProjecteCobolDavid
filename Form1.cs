using System.Globalization;
using System.IO;
using System.Collections.Generic;
using Microsoft.Reporting.WinForms;
using System.Diagnostics;

namespace ProjecteCobolDavid
{
    public partial class Form1 : Form
    {
        private string _lastSortColumn = null;
        private bool _lastSortAscending = true;
        public Form1()
        {
            InitializeComponent();
            dgvDespeses.DataSource = Despesa.CarregarDades();
            FormatCostColumn();
            // Assegurem que el NumericUpDown faci servir 2 decimals
            try
            {
                numCost.DecimalPlaces = 2;
                numCost.Increment = 0.01m;
            }
            catch { }
            // Permetre ordenar clicant a les capçaleres
            dgvDespeses.ColumnHeaderMouseClick += dgvDespeses_ColumnHeaderMouseClick;
            

        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validem que els camps obligatoris estiguin omplerts
            if (!ValidateInputs(out string missing))
            {
                MessageBox.Show($"Falten els següents camps obligatoris: {missing}", "Camps obligatoris", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Creem l'objecte amb les dades dels controls del formulari
            Despesa nova = new Despesa()
            {
                Nom = txtNom.Text,
                Cost = numCost.Value,  // NumericUpDown.Value és decimal
                Data = dtpData.Value,  // Suposant que uses un DateTimePicker
                Tipus = cmbTipus.Text  // Suposant que uses un ComboBox
            };

            // Cridem al mètode que executa el COBOL
            Despesa.EnviarACobol(nova);

            // Actualitzem la graella (DataGridView)
            dgvDespeses.DataSource = Despesa.CarregarDades();
            FormatCostColumn();
            dgvDespeses.Refresh();
            MessageBox.Show("Despesa enregistrada correctament per EconoParse!");
        }

        private void btnActualitzar_Click(object sender, EventArgs e)
        {
            dgvDespeses.DataSource = Despesa.CarregarDades();
            FormatCostColumn();
        }

        private void btnBorrarDat_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show("Segur que vols esborrar tot el contingut de DESPESES.DAT?", "Confirmar esborrat", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resp != DialogResult.Yes) return;

            try
            {
                Despesa.BorrarDat();
                dgvDespeses.DataSource = Despesa.CarregarDades();
                FormatCostColumn();
                MessageBox.Show("DESPESES.DAT s'ha esborrat correctament.", "Fet", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error esborrant DESPESES.DAT:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMostrarInforme_Click(object sender, EventArgs e)
        {
            try
            {
                var datos = Despesa.CarregarDades();
                var f = new ReportForm();
                f.LoadReport(datos);
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error mostrant l'informe:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatCostColumn()
        {
            if (dgvDespeses.Columns.Contains("Cost"))
            {
                var col = dgvDespeses.Columns["Cost"];
                col.DefaultCellStyle.Format = "N2"; // 2 decimals
                col.DefaultCellStyle.FormatProvider = CultureInfo.InvariantCulture;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void dgvDespeses_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.ColumnIndex >= dgvDespeses.Columns.Count) return;
            var col = dgvDespeses.Columns[e.ColumnIndex];
            // Ternari DataPropertyName (ve del Binding), si no, usa Name
            var propName = string.IsNullOrEmpty(col.DataPropertyName) ? col.Name : col.DataPropertyName;
            if (string.IsNullOrEmpty(propName)) return;

            // Alternar ordre si el prem la mateixa columna
            if (_lastSortColumn == propName)
                _lastSortAscending = !_lastSortAscending;
            else
            {
                _lastSortColumn = propName;
                _lastSortAscending = true;
            }

            try
            {
                var list = Despesa.CarregarDades();
                var prop = typeof(Despesa).GetProperty(propName);
                if (prop == null)
                {
                    // Si no es troba la propietat, no fem res
                    return;
                }

                IOrderedEnumerable<Despesa> sorted;
                if (_lastSortAscending)
                    sorted = list.OrderBy(x => prop.GetValue(x, null));
                else
                    sorted = list.OrderByDescending(x => prop.GetValue(x, null));

                dgvDespeses.DataSource = sorted.ToList();
                FormatCostColumn();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al ordenar: " + ex.Message);
            }
        }

        private bool ValidateInputs(out string missing)
        {
            var missingList = new List<string>();
            if (string.IsNullOrWhiteSpace(txtNom.Text)) missingList.Add("Nom");
            if (numCost.Value <= 0m) missingList.Add("Cost");
            if (string.IsNullOrWhiteSpace(cmbTipus.Text)) missingList.Add("Tipus");
            // La data normalment sempre està present en DateTimePicker, però si vols podríem validar rang
            missing = string.Join(", ", missingList);
            return missingList.Count == 0;
        }

        private void btnEsborrar1_Click(object sender, EventArgs e)
        {
            // Comprovem si hi ha una fila seleccionada
            if (dgvDespeses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Si us plau, selecciona una despesa per esborrar.", "Cap selecció", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Agafem la primera fila seleccionada
            var selectedRow = dgvDespeses.SelectedRows[0];
            var nomValue = selectedRow.Cells["Nom"].Value;
            var costValue = selectedRow.Cells["Cost"].Value;
            var dataValue = selectedRow.Cells["Data"].Value;

            if (nomValue == null || costValue == null || dataValue == null)
            {
                MessageBox.Show("No es pot obtenir les dades de la despesa seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string nomDespesa = nomValue.ToString();
            decimal costDespesa = Convert.ToDecimal(costValue);
            DateTime dataDespesa = Convert.ToDateTime(dataValue);

            // Demanem confirmació
            var resp = MessageBox.Show($"Estàs segur que vols esborrar la despesa:\n\nNom: {nomDespesa.Trim()}\nCost: {costDespesa:N2}€\nData: {dataDespesa:yyyy-MM-dd}?", 
                "Confirmar esborrat", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;

            try
            {
                // Cridem al mètode per esborrar la despesa específica (nom + cost + data)
                Despesa.EsborrarDespesa(nomDespesa, costDespesa, dataDespesa);

                // Recarreguem el DataGridView
                dgvDespeses.DataSource = Despesa.CarregarDades();
                FormatCostColumn();

                MessageBox.Show("Despesa esborrada correctament.", "Fet", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error esborrant la despesa:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
