using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace ProjecteCobolDavid
{
    public class ReportForm : Form
    {
        private ReportViewer rpv;

        public ReportForm()
        {
            rpv = new ReportViewer();
            rpv.Dock = DockStyle.Fill;
            this.Controls.Add(rpv);
            this.Text = "Informe despeses";
            this.WindowState = FormWindowState.Maximized;
        }

        public void LoadReport(List<Despesa> datos)
        {
            rpv.Reset();
            rpv.ProcessingMode = ProcessingMode.Local;
            rpv.LocalReport.DataSources.Clear();

            string reportPath = Path.Combine(Application.StartupPath, "ReportDespeses.rdlc");
            rpv.LocalReport.ReportPath = reportPath;

            var rds = new ReportDataSource("DataSet1", datos);
            rpv.LocalReport.DataSources.Add(rds);

            var estadisticas = Despesa.CalcularEstadisticas(datos);
            var estadisticasDs = new ReportDataSource("EstadisticasDataSet", 
                new List<EstadisticaResultado> { estadisticas });
            rpv.LocalReport.DataSources.Add(estadisticasDs);

            var top3Ds = new ReportDataSource("Top3DataSet", estadisticas.Top3Gastos);
            rpv.LocalReport.DataSources.Add(top3Ds);

            rpv.RefreshReport();
        }
    }
}
