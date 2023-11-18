using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemValledeXelaju
{
    public partial class ReportesForm : Form
    {
        private Conexion conexion;
        public ReportesForm()
        {
            conexion = new Conexion();
            InitializeComponent();
            grupo_Fechas.Visible = false;
            grupo_fechasIng.Visible = false;
            grupo_Paciente.Visible = false;
            grupo_visitas.Visible = false;
        }

        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
        }



        private void btnReportes_Click(object sender, EventArgs e)
        {
            if (check_Paciente_Plantas.Checked)
            {
                ObtenerPacientes();
            }
            else if (check_Pacientes_dos_Fechas.Checked)
            {
                ObtenerAltasPorFechas();
            }
            else if (check_Ing_fechas.Checked)
            {
                ObtenerIngresosPorFechas();
            }
            else if (check_ListPlantas.Checked)
            {
                ObtenerCamasPlantas();
            }
            else if (check_PacientesMedicos.Checked)
            {
                if (int.TryParse(txt_paciente.Text, out int codigoPaciente))
                {
                    ObtenerMedicosPacientes(codigoPaciente);
                }
                else
                {
                    MessageBox.Show("Ingrese un número válido en el campo Código de Paciente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (check_DiagPacientes.Checked)
            {
                if (int.TryParse(txt_paciente.Text, out int codigoPaciente))
                {
                    ObtenerDiagnosticosPacientes(codigoPaciente);
                }
                else
                {
                    MessageBox.Show("Ingrese un número válido en el campo Código de Paciente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (checkBox1.Checked)
            {
                if (int.TryParse(txt_paciente.Text, out int codigoMedico))
                {
                    ObtenerDiagnosticosMedicos(codigoMedico);
                }
                else
                {
                    MessageBox.Show("Ingrese un número válido en el campo Código de Médico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (check_Diagnosticos.Checked)
            {
                ObtenerDiagnosticos();
            }
            else if (check_Visitas.Checked)
            {
                ObtenerVisitas();
            }
        }


        private void ObtenerPacientes()
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT P.CódigoPaciente, P.DPI, P.Nombre, P.Apellidos, P.CódigoCamaAsignada " +
                               "FROM Pacientes AS P " +
                               "INNER JOIN Camas AS C ON P.CódigoCamaAsignada = C.CódigoCama " +
                               "ORDER BY C.CódigoPlanta, P.CódigoPaciente";

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);

                DataSet2 ds = new DataSet2();
                adapter.Fill(ds, ds.Tables[0].TableName);

                ReportDataSource rds = new ReportDataSource("DataSet2", ds.Tables[0]);

                VisualizarRepForm visualizarForm = new VisualizarRepForm(rds);
                visualizarForm.Show(); // Mostrar en modo no modal
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los pacientes. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }


        private void ObtenerAltasPorFechas()
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT CódigoPaciente, FechaIngreso, FechaSalida " +
                               "FROM Ingresos " +
                               "WHERE FechaSalida >= @FechaInicial AND FechaSalida <= @FechaFinal";

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);

                if (FInicial.Enabled)
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaInicial", FInicial.Value);
                else
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaInicial", DBNull.Value);

                if (FFinal.Enabled)
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaFinal", FFinal.Value);
                else
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaFinal", DBNull.Value);

                DataSet2 ds = new DataSet2();
                adapter.Fill(ds, ds.Tables[0].TableName);

                ReportDataSource rds = new ReportDataSource("DataSet2", ds.Tables[0]);

                VisorPacientesFechasForm visorPacientesFechasForm = new VisorPacientesFechasForm(rds);
                visorPacientesFechasForm.Show(); // Mostrar en modo no modal
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los ingresos. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void ObtenerIngresosPorFechas()
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT CódigoPaciente, FechaIngreso, FechaSalida " +
                               "FROM Ingresos " +
                               "WHERE FechaIngreso >= @FechaInicial AND FechaIngreso <= @FechaFinal";

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);

                if (FInicial1.Enabled)
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaInicial", FInicial1.Value);
                else
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaInicial", DBNull.Value);

                if (FFinal2.Enabled)
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaFinal", FFinal2.Value);
                else
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaFinal", DBNull.Value);

                DataSet2 ds = new DataSet2();
                adapter.Fill(ds, ds.Tables[0].TableName);

                ReportDataSource rds = new ReportDataSource("DataSet2", ds.Tables[0]);

                VisorPacientesIngFechasForm visorPacientesIngFechasForm = new VisorPacientesIngFechasForm(rds);
                visorPacientesIngFechasForm.Show(); // Mostrar en modo no modal
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los ingresos. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void ObtenerCamasPlantas()
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT CódigoCama, CódigoPlanta, Estado " +
                               "FROM Camas " +
                               "ORDER BY CódigoPlanta, CódigoCama";

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);

                DataSet2 ds = new DataSet2();
                adapter.Fill(ds, ds.Tables[0].TableName);

                ReportDataSource rds = new ReportDataSource("DataSet2", ds.Tables[0]);

                VisorListCamasForm visorListCamasForm = new VisorListCamasForm(rds);
                visorListCamasForm.Show(); // Mostrar en modo no modal
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener las camas. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
        
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txt_paciente.Text, out int codigoPaciente))
            {
                ObtenerMedicosPacientes(codigoPaciente);
            }
            else
            {
                MessageBox.Show("Ingrese un número válido en el campo Código de Paciente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ObtenerMedicosPacientes(int codigoPaciente)
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT CódigoPaciente, CódigoMedico, FechaVisita " +
                               "FROM VisitasMedicas " +
                               "WHERE CódigoPaciente = @CodigoPaciente " +
                               "ORDER BY FechaVisita";

                OleDbCommand command = new OleDbCommand(query, conexion.con);
                command.Parameters.AddWithValue("@CodigoPaciente", codigoPaciente);

                OleDbDataAdapter adapter = new OleDbDataAdapter(command);

                DataSet2 ds = new DataSet2();
                adapter.Fill(ds, ds.Tables[0].TableName);

                ReportDataSource rds = new ReportDataSource("DataSet2", ds.Tables[0]);

                // Aquí asumimos que tienes un formulario llamado "VisorMedicosPacientesForm"
                VisorMedicosPacienteForm visorMedicosPacientesForm = new VisorMedicosPacienteForm(rds);
                visorMedicosPacientesForm.Show(); // Mostrar en modo no modal
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los médicos que han visitado al paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void ObtenerDiagnosticosPacientes(int codigoPaciente)
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT CódigoPaciente, Observaciones, CódigoDiagnostico, CódigoMedico " +
                               "FROM DetallesDiagnosticos " +
                               "WHERE CódigoPaciente = @CodigoPaciente";

                OleDbCommand command = new OleDbCommand(query, conexion.con);
                command.Parameters.AddWithValue("@CodigoPaciente", codigoPaciente);

                OleDbDataAdapter adapter = new OleDbDataAdapter(command);

                DataSet2 ds = new DataSet2();
                adapter.Fill(ds, ds.Tables[0].TableName);

                ReportDataSource rds = new ReportDataSource("DataSet2", ds.Tables[0]);

                // Aquí asumimos que tienes un formulario llamado "VisorDiagnosticosPacienteForm"
                VisorDiagnosPacientesForm visorDiagnosPacientesForm = new VisorDiagnosPacientesForm(rds);
                visorDiagnosPacientesForm.Show(); // Mostrar en modo no modal
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los diagnósticos hechos al paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void ObtenerDiagnosticosMedicos(int codigoMedico)
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT CódigoPaciente, Observaciones, CódigoDiagnostico, CódigoMedico " +
                               "FROM DetallesDiagnosticos " +
                               "WHERE CódigoMedico = @CodigoMedico";

                OleDbCommand command = new OleDbCommand(query, conexion.con);
                command.Parameters.AddWithValue("@CodigoMedico", codigoMedico);

                OleDbDataAdapter adapter = new OleDbDataAdapter(command);

                DataSet2 ds = new DataSet2();
                adapter.Fill(ds, ds.Tables[0].TableName);

                ReportDataSource rds = new ReportDataSource("DataSet2", ds.Tables[0]);

                // Aquí asumimos que tienes un formulario llamado "VisorDiagnosticosPacienteForm"
                VisorDiagnosticosMedicos visorDiagnosticosMedicos = new VisorDiagnosticosMedicos(rds);
                visorDiagnosticosMedicos.Show(); // Mostrar en modo no modal
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los diagnósticos hechos al paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void ObtenerDiagnosticos()
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT Id, CódigoDiagnostico, Descripcion, Observaciones " +
                               "FROM Diagnosticos";

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);

                DataSet2 ds = new DataSet2();
                adapter.Fill(ds, ds.Tables[0].TableName);

                ReportDataSource rds = new ReportDataSource("DataSet2", ds.Tables[0]);

                VisorDiagnosticos visorDiagnosticos = new VisorDiagnosticos(rds);
                visorDiagnosticos.Show(); // Mostrar en modo no modal
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los diagnósticos. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
        private void ObtenerVisitas()
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT CódigoPaciente, CódigoMedico, FechaVisita " +
                               "FROM VisitasMedicas " +
                               "WHERE FechaVisita >= @FechaInicial AND FechaVisita <= @FechaFinal";

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);

                if (FInicial2.Enabled)
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaInicial", FInicial2.Value);
                else
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaInicial", DBNull.Value);

                if (FFinal1.Enabled)
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaFinal", FFinal1.Value);
                else
                    adapter.SelectCommand.Parameters.AddWithValue("@FechaFinal", DBNull.Value);

                DataSet2 ds = new DataSet2();
                adapter.Fill(ds, ds.Tables[0].TableName);

                ReportDataSource rds = new ReportDataSource("DataSet2", ds.Tables[0]);

                // Aquí asumimos que tienes un formulario llamado "VisorVisitasMedicasForm"
                VisorVisitasMedicasForm visorVisitasMedicasForm = new VisorVisitasMedicasForm(rds);
                visorVisitasMedicasForm.Show(); // Mostrar en modo no modal
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener las visitas médicas. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }



        private void check_Pacientes_dos_Fechas_CheckedChanged(object sender, EventArgs e)
        {
            grupo_Fechas.Visible = check_Pacientes_dos_Fechas.Checked;
        }

        private void check_Ing_fechas_CheckedChanged(object sender, EventArgs e)
        {
            grupo_fechasIng.Visible = check_Ing_fechas.Checked;
        }

        private void check_PacientesMedicos_CheckedChanged(object sender, EventArgs e)
        {
            grupo_Paciente.Visible = check_PacientesMedicos.Checked;
        }

        private void check_DiagPacientes_CheckedChanged(object sender, EventArgs e)
        {
            grupo_Paciente.Visible = check_DiagPacientes.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            grupo_Paciente.Visible = checkBox1.Checked;
        }

        private void check_Visitas_CheckedChanged(object sender, EventArgs e)
        {
            grupo_visitas.Visible = check_Visitas.Checked;
        }
    }
}
