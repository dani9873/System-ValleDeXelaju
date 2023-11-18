using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemValledeXelaju
{
    public partial class VisualizarRepForm : Form
    {
        public VisualizarRepForm()
        {
            InitializeComponent();
        }


        public VisualizarRepForm(ReportDataSource reportDataSource)
        {
            InitializeComponent();

            // Configura el ReportViewer con el ReportDataSource
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);

            // Refrescar y mostrar el informe
            reportViewer1.RefreshReport();
        }

        private void VisualizarRepForm_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }

        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            ReportesForm reportesForm = new ReportesForm();
            reportesForm.ShowDialog();
        }
    }
}
