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
    public partial class MainForm2 : Form
    {
        public MainForm2()
        {
            InitializeComponent();
        }

        private void btn_registro_p_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            RegistrarPacienteForm registrarPacienteForm = new RegistrarPacienteForm();
            registrarPacienteForm.ShowDialog();
        }

        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }

        private void btn_asignar_camas_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            AsignarCamasForm asignarCamasForm = new AsignarCamasForm();
            asignarCamasForm.ShowDialog();
        }

        private void btn_traslado_pacientes_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            RegistrarCamasForm trasladarPacientesForm = new RegistrarCamasForm();
            trasladarPacientesForm.ShowDialog();
        }

        private void btn_dar_alta_pacientes_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            DarAltaPacienteForm darAltaPacienteForm = new DarAltaPacienteForm();
            darAltaPacienteForm.ShowDialog();
        }

        private void btn_regis_medicos_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            RegistrarMedicoForm registrarMedicoForm = new RegistrarMedicoForm();
            registrarMedicoForm.ShowDialog();
        }

        private void btn_asignar_medico_paciente_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            AsignarMedicosForm asignarMedicosForm = new AsignarMedicosForm();
            asignarMedicosForm.ShowDialog();
        }

        private void btn_regis_diagnostico_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            RegistrarDiagnosticoForm registrarDiagnosticoForm = new RegistrarDiagnosticoForm();
            registrarDiagnosticoForm.ShowDialog();
        }

        private void btn_asignar_diagnostico_pacientes_visitas_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            AsignarDiagnosticoForm asignarDiagnosticoForm = new AsignarDiagnosticoForm();
            asignarDiagnosticoForm.ShowDialog();
        }

        private void btnVisitas_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            VisitasMedicasForm visitasMedicasForm = new VisitasMedicasForm();
            visitasMedicasForm.ShowDialog();
        }

        private void btn_Rep_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            ReportesForm reportesForm = new ReportesForm();
            reportesForm.ShowDialog();
        }

        private void btn_listados_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            ListadoGenerar1 listadosGenerar = new ListadoGenerar1();
            listadosGenerar.ShowDialog();
        }

        private void btn_cerrar_sesion_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }
    }
}
