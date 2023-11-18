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
    public partial class AsignarDiagnosticoForm : Form
    {
        private Conexion conexion;

        public AsignarDiagnosticoForm()
        {
            InitializeComponent();
            conexion = new Conexion();
            CargarPacientes();
            CargarDiagnosticos();
            CargarMedicos();
        }
        private int codigoPacienteSeleccionado = -1; // Variable para almacenar el código del paciente seleccionado
        private int codigoDiagnosticoSeleccionado = -1; // Variable para almacenar el código del diagnóstico seleccionado
        private void CargarMedicos()
        {
            try
            {
                conexion.AbrirConexion();
                string query = "SELECT id, CódigoMedico, Nombre, Apellidos FROM Medicos";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int idMedico = Convert.ToInt32(reader["id"]);
                        string codigoMedico = reader["CódigoMedico"].ToString();
                        string nombreCompleto = $"{reader["CódigoMedico"]} {reader["Nombre"]} {reader["Apellidos"]}";
                        cmbMedicos.Items.Add(new KeyValuePair<int, string>(idMedico, nombreCompleto));
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los médicos. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
        private int ObtenerCodigoMedicoSeleccionado()
        {
            KeyValuePair<int, string> selectedItem = (KeyValuePair<int, string>)cmbMedicos.SelectedItem;
            return selectedItem.Key;
        }
        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
        }
        private void CargarDiagnosticos()
        {
            try
            {
                conexion.AbrirConexion();
                string query = "SELECT id, CódigoDiagnostico, Descripcion FROM Diagnosticos";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int idDiagnostico = Convert.ToInt32(reader["id"]);
                        string códigoDiagnostico = reader["CódigoDiagnostico"].ToString();
                        string descripción = $"{reader["CódigoDiagnostico"]} {reader["Descripcion"]}";
                        cmbDiagnostico.Items.Add(new KeyValuePair<int, string>(idDiagnostico, descripción));
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los diagnósticos. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
        // Método para cargar los pacientes en el combobox
        private void CargarPacientes()
        {
            try
            {
                conexion.AbrirConexion();
                string query = "SELECT  id, CódigoPaciente, Nombre, Apellidos FROM Pacientes";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int idPaciente = Convert.ToInt32(reader["id"]);
                        string codigoPaciente = reader["CódigoPaciente"].ToString();
                        string nombreCompleto = $"{reader["CódigoPaciente"]} {reader["Nombre"]} {reader["Apellidos"]}";
                        cmbPacientes.Items.Add(new KeyValuePair<int, string>(idPaciente, nombreCompleto));
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los pacientes. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }
        private int ObtenerCodigoPacienteSeleccionado()
        {
            KeyValuePair<int, string> selectedItem = (KeyValuePair<int, string>)cmbPacientes.SelectedItem;
            return selectedItem.Key;
        }

        private void btnAsignar_Click(object sender, EventArgs e)
        {
            try
            {
                if (codigoPacienteSeleccionado == -1 || codigoDiagnosticoSeleccionado == -1)
                {
                    MessageBox.Show("Por favor, seleccione un paciente y un diagnóstico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int codigoMedicoResponsable = ObtenerCodigoMedicoSeleccionado();

                string observaciones = txtObservaciones.Text;

                conexion.AbrirConexion();
                string query = "INSERT INTO DetallesDiagnosticos (CódigoMedico, CódigoDiagnostico, CódigoPaciente, Observaciones) " +
                               "VALUES (@codigoMedico, @codigoDiagnostico, @codigoPaciente, @observaciones)";

                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@codigoMedico", codigoMedicoResponsable);
                    cmd.Parameters.AddWithValue("@codigoDiagnostico", codigoDiagnosticoSeleccionado);
                    cmd.Parameters.AddWithValue("@codigoPaciente", codigoPacienteSeleccionado);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);


                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Diagnóstico asignado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo asignar el diagnóstico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar el diagnóstico. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private int ObtenerCodigoDiagnosticoSeleccionado()
        {
            KeyValuePair<int, string> selectedItem = (KeyValuePair<int, string>)cmbDiagnostico.SelectedItem;
            return selectedItem.Key;
        }


        private void cmbPacientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Al seleccionar un paciente, actualizamos la variable del código seleccionado
            codigoPacienteSeleccionado = ObtenerCodigoPacienteSeleccionado();
        }

        private void cmbDiagnostico_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Al seleccionar un diagnóstico, actualizamos la variable del código seleccionado
            codigoDiagnosticoSeleccionado = ObtenerCodigoDiagnosticoSeleccionado();
        }

    }
}