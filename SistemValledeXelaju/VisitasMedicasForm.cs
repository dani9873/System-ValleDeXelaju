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
    public partial class VisitasMedicasForm : Form
    {
        private Conexion conexion;
        private Dictionary<int, string> medicosDictionary = new Dictionary<int, string>();


        public VisitasMedicasForm()
        {
            InitializeComponent();
            conexion = new Conexion();
            CargarMedicos();
            CargarPacientes();
        }
        // Método para cargar los médicos en el combobox
        private void CargarMedicos()
        {
            try
            {
                conexion.AbrirConexion();
                string query = "SELECT Id, CódigoMedico, Nombre, Apellidos FROM Medicos";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int idMedico = Convert.ToInt32(reader["Id"]);
                        string nombreCompleto = $"{reader["Nombre"]} {reader["Apellidos"]}";
                        string codigoMedico = reader["CódigoMedico"].ToString();
                        medicosDictionary.Add(idMedico, codigoMedico);

                        // Agregar el nombre completo del médico con su Id al ComboBox.
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

        // Método para cargar los pacientes en el ComboBox
        private void CargarPacientes()
        {
            try
            {
                conexion.AbrirConexion();
                string query = "SELECT Id, CódigoPaciente, Nombre, Apellidos FROM Pacientes";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int idPaciente = Convert.ToInt32(reader["Id"]);
                        string codigoPaciente = reader["CódigoPaciente"].ToString();
                        string nombreCompleto = $"{reader["Nombre"]} {reader["Apellidos"]}";

                        // Agregar el nombre completo del paciente con su Id al ComboBox.
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


        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
        }

        private void cmbMedicos_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
           private void cmbPacientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnAsignar_Click(object sender, EventArgs e)
        {
            // Verificar que ambos ComboBox estén seleccionados y que la FechaVisita esté ingresada.
            if (cmbPacientes.SelectedIndex != -1 && cmbMedicos.SelectedIndex != -1 && dtpFechaVisita.Value != null)
            {
                // Obtener el IdPaciente y IdMedico seleccionados.
                int CódigoPaciente = ObtenerIdPacienteSeleccionado();
                int CódigoMedico = ObtenerIdMedicoSeleccionado();

                // Obtener la FechaVisita del DateTimePicker.
                DateTime fechaVisita = dtpFechaVisita.Value;

                try
                {
                    conexion.AbrirConexion();
                    // Insertar la información en la tabla "VisitasMedicas".
                    string query = "INSERT INTO VisitasMedicas (CódigoPaciente, CódigoMedico, FechaVisita) VALUES (?, ?, ?)";
                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("?", CódigoPaciente);
                        cmd.Parameters.AddWithValue("?", CódigoMedico);
                        cmd.Parameters.AddWithValue("?", fechaVisita);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Asignación exitosa.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Error al asignar la visita médica.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al asignar la visita médica. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un paciente y un médico, e ingrese la fecha de visita.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private int ObtenerIdMedicoSeleccionado()
        {
            KeyValuePair<int, string> selectedItem = (KeyValuePair<int, string>)cmbMedicos.SelectedItem;
            return selectedItem.Key;
        }

        private int ObtenerIdPacienteSeleccionado()
        {
            KeyValuePair<int, string> selectedItem = (KeyValuePair<int, string>)cmbPacientes.SelectedItem;
            return selectedItem.Key;
        }


    }
}
