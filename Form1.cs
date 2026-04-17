using System.Globalization;
using System.IO;
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
            // Aseguramos que el NumericUpDown use 2 decimales y paso 0.01
            try
            {
                numCost.DecimalPlaces = 2;
                numCost.Increment = 0.01m;
            }
            catch { }
            // Permitir ordenar clicando en las cabeceras
            dgvDespeses.ColumnHeaderMouseClick += dgvDespeses_ColumnHeaderMouseClick;
            

        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Creem l'objecte amb les dades dels controls del formulari
            Despesa nova = new Despesa()
            {
                Nom = txtNom.Text,
                Cost = numCost.Value, // NumericUpDown.Value és decimal
                Data = dtpData.Value,                  // Suposant que uses un DateTimePicker
                Tipus = cmbTipus.Text                  // Suposant que uses un ComboBox
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
            // Prefer DataPropertyName (viene del Binding), si no, usa Name
            var propName = string.IsNullOrEmpty(col.DataPropertyName) ? col.Name : col.DataPropertyName;
            if (string.IsNullOrEmpty(propName)) return;

            // Alternar orden si se pulsa la misma columna
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
                    // Si no se encuentra la propiedad, no hacemos nada
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
    }
}
