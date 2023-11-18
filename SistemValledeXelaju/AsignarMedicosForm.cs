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
    public partial class AsignarMedicosForm : Form
    {
        private Conexion conexion;
        private Dictionary<int, string> medicosDictionary = new Dictionary<int, string>();


        public AsignarMedicosForm()
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
                string query = "SELECT CódigoMedico, Nombre, Apellidos FROM Medicos";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string nombreCompleto = $"{reader["Nombre"]} {reader["Apellidos"]}";

                        // Agregar el nombre completo del médico al ComboBox.
                        cmbMedicos.Items.Add(nombreCompleto);
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


        // Método para cargar los pacientes en el combobox
        private void CargarPacientes()
        {
            try
            {
                conexion.AbrirConexion();
                string query = "SELECT CódigoPaciente, Nombre, Apellidos FROM Pacientes";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string codigoPaciente = reader["CódigoPaciente"].ToString();
                        string nombreCompleto = $"{reader["Nombre"]} {reader["Apellidos"]}";
                        cmbPacientes.Items.Add(new KeyValuePair<string, string>(codigoPaciente, nombreCompleto));
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

        // Métodos para obtener los códigos seleccionados de médico y paciente
        private int ObtenerCodigoMedicoSeleccionado()
        {
            string nombreMedicoSeleccionado = cmbMedicos.SelectedItem.ToString();
            return medicosDictionary.FirstOrDefault(x => x.Value == nombreMedicoSeleccionado).Key;
        }

        private string ObtenerCodigoPacienteSeleccionado()
        {
            KeyValuePair<string, string> selectedItem = (KeyValuePair<string, string>)cmbPacientes.SelectedItem;
            return selectedItem.Key;
        }

        private void cmbMedicos_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Al seleccionar un médico, actualizamos la variable del código seleccionado
            int codigoMedicoSeleccionado = ObtenerCodigoMedicoSeleccionado();
        }

        private void cmbPacientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Al seleccionar un paciente, actualizamos la variable del código seleccionado
            string codigoPacienteSeleccionado = ObtenerCodigoPacienteSeleccionado();
        }

        private void btnAsignar_Click_1(object sender, EventArgs e)
        {
            // Obtener el médico y paciente seleccionados
            if (cmbMedicos.SelectedItem == null || cmbPacientes.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un médico y un paciente disponibles.", "Asignar Médico a Paciente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int codigoMedico = ObtenerCodigoMedicoSeleccionado();
            string codigoPaciente = ObtenerCodigoPacienteSeleccionado();
            try
                {
                    conexion.AbrirConexion();


                    // Verificar si el médico ya está asignado al paciente
                    string query = "SELECT COUNT(*) FROM VisitasMedicas WHERE CódigoMedico = @CódigoMedico AND CódigoPaciente = @CódigoPaciente";
                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoMedico", codigoMedico);
                        cmd.Parameters.AddWithValue("@CódigoPaciente", codigoPaciente);

                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("El médico ya está asignado al paciente seleccionado.", "Asignar Médico a Paciente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Insertar la nueva visita médica en la base de datos
                    query = "INSERT INTO VisitasMedicas (CódigoMedico, CódigoPaciente, FechaVisita) VALUES (@CódigoMedico, @CódigoPaciente, @FechaVisita)";
                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoMedico", codigoMedico);
                        cmd.Parameters.AddWithValue("@CódigoPaciente", codigoPaciente);
                        cmd.Parameters.AddWithValue("@FechaVisita", DateTime.Now); // Fecha y hora actual como fecha de la visita

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("El médico ha sido asignado al paciente exitosamente.", "Asignar Médico a Paciente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo asignar al médico al paciente. Por favor, intente nuevamente.", "Asignar Médico a Paciente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar asignar al médico al paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }

    }

