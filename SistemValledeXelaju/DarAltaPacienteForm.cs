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
    public partial class DarAltaPacienteForm : Form
    {
        private Conexion conexion;
        public DarAltaPacienteForm()
        {
            conexion = new Conexion();
            InitializeComponent();
            CargarPacientesDisponibles();
        }
        private void CargarPacientesDisponibles()
        {
            try
            {
                conexion.AbrirConexion();
                string queryPacientes = "SELECT P.Id, P.Nombre, P.Apellidos " +
                        "FROM Pacientes AS P " +
                        "LEFT JOIN Ingresos AS I ON P.CódigoPaciente = CStr(I.CódigoPaciente) " +
                        "WHERE I.FechaSalida IS NULL OR I.FechaSalida = ''";




                using (OleDbCommand cmd = new OleDbCommand(queryPacientes, conexion.con))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string Id = reader["Id"].ToString();
                            string nombre = reader["Nombre"].ToString();
                            string apellidos = reader["Apellidos"].ToString();
                            string nombreCompleto = $"{nombre} {apellidos}";

                            cmbbox_paciente.Items.Add(new KeyValuePair<string, string>(Id, nombreCompleto));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener los pacientes disponibles. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
        }



        private void btnDarAlta_Click(object sender, EventArgs e)
        {
            // Verificar que haya un paciente seleccionado en el ComboBox
            if (cmbbox_paciente.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un paciente antes de dar de alta.", "Dar de Alta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar que se haya seleccionado una fecha en el DateTimePicker
            if (dtpFechaAlta.Value == dtpFechaAlta.MinDate)
            {
                MessageBox.Show("Por favor, seleccione una fecha de alta válida.", "Dar de Alta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el PacienteId y el NombreCompleto del paciente seleccionado
            string pacienteIdSeleccionado = ((KeyValuePair<string, string>)cmbbox_paciente.SelectedItem).Key;
            string nombreCompletoSeleccionado = ((KeyValuePair<string, string>)cmbbox_paciente.SelectedItem).Value;

            try
            {
                conexion.AbrirConexion();

                // Verificar si el paciente tiene un registro en la tabla Ingresos
                string queryVerificarIngreso = "SELECT COUNT(*) FROM Ingresos WHERE CódigoPaciente = @PacienteId";
                using (OleDbCommand cmdVerificarIngreso = new OleDbCommand(queryVerificarIngreso, conexion.con))
                {
                    cmdVerificarIngreso.Parameters.AddWithValue("@PacienteId", pacienteIdSeleccionado);
                    int ingresoCount = (int)cmdVerificarIngreso.ExecuteScalar();

                    if (ingresoCount > 0)
                    {
                        // Actualizar la FechaSalida del paciente en el registro de ingreso existente
                        string queryDarAlta = "UPDATE Ingresos SET FechaSalida = @FechaSalida WHERE CódigoPaciente = CInt(@PacienteId)";

                        using (OleDbCommand cmdDarAlta = new OleDbCommand(queryDarAlta, conexion.con))
                        {
                            // Format the FechaSalida parameter to a compatible format for the database
                            string fechaSalidaFormatted = dtpFechaAlta.Value.ToString("yyyy-MM-dd HH:mm:ss");
                            cmdDarAlta.Parameters.AddWithValue("@FechaSalida", fechaSalidaFormatted);
                            cmdDarAlta.Parameters.AddWithValue("@PacienteId", pacienteIdSeleccionado);
                            cmdDarAlta.ExecuteNonQuery();

                            MessageBox.Show($"Se ha dado de alta al paciente {nombreCompletoSeleccionado} exitosamente.", "Dar de Alta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarPacientesDisponibles();
                        }
                    }
                    else
                    {
                        MessageBox.Show("El paciente seleccionado no tiene un ingreso registrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar dar de alta al paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void cmbbox_paciente_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbbox_paciente.SelectedItem != null)
            {
                // Obtener el PacienteId del paciente seleccionado
                string pacienteIdSeleccionado = ((KeyValuePair<string, string>)cmbbox_paciente.SelectedItem).Key;

                // Verificar si el paciente ya tiene una fecha de salida en la tabla Ingresos
                try
                {
                    conexion.AbrirConexion();
                    string queryVerificarFechaSalida = "SELECT COUNT(*) FROM Ingresos WHERE CódigoPaciente = CInt(@PacienteId) AND FechaSalida IS NOT NULL";

                    using (OleDbCommand cmdVerificarFechaSalida = new OleDbCommand(queryVerificarFechaSalida, conexion.con))
                    {
                        cmdVerificarFechaSalida.Parameters.AddWithValue("@PacienteId", pacienteIdSeleccionado);
                        int fechaSalidaCount = (int)cmdVerificarFechaSalida.ExecuteScalar();

                        if (fechaSalidaCount > 0)
                        {
                            // El paciente ya tiene una fecha de salida registrada
                            MessageBox.Show("El paciente seleccionado ya tiene una fecha de salida registrada.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            // El paciente no tiene una fecha de salida registrada
                            MessageBox.Show("El paciente seleccionado no tiene una fecha de salida registrada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar verificar la fecha de salida del paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
        }
    }
}
