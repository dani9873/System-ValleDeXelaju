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
    public partial class AsignarCamasForm : Form
    {
        private Conexion conexion;
        private List<(string codigoCama, string codigoPlanta)> camasDisponibles;

        public AsignarCamasForm()
        {
            InitializeComponent();
            conexion = new Conexion();
            camasDisponibles = new List<(string, string)>(); // Lista de tuplas (códigoCama, códigoPlanta)
            CargarPlantasYCamasDisponibles();
            CargarPacientesDisponibles();

        }
        private void CargarPacientesDisponibles()
        {
            try
            {
                conexion.AbrirConexion();

                string queryPacientes = "SELECT P.CódigoPaciente, P.Nombre, P.Apellidos, P.CódigoCamaAsignada, P.CodigoPlanta FROM Pacientes AS P LEFT JOIN Plantas AS PL ON P.CodigoPlanta = PL.CódigoPlanta;";

                using (OleDbCommand cmd = new OleDbCommand(queryPacientes, conexion.con))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string codigoPaciente = reader["CódigoPaciente"].ToString();
                            string nombre = reader["Nombre"].ToString();
                            string apellidos = reader["Apellidos"].ToString();
                            string nombreapellidos = $"{nombre} {apellidos}";

                            // Obtener el código de la cama asignada al paciente
                            string codigoCamaAsignada = reader["CódigoCamaAsignada"].ToString();

                            if (!string.IsNullOrEmpty(codigoCamaAsignada))
                            {
                                nombreapellidos += $" (Cama {codigoCamaAsignada}";

                                // Obtener el código de la planta asociada a la cama asignada
                                string codigoPlanta = reader["CodigoPlanta"].ToString();
                                if (!string.IsNullOrEmpty(codigoPlanta))
                                {
                                    nombreapellidos += $", Planta {codigoPlanta}";
                                }

                                nombreapellidos += ")";
                            }

                            cmbbox_paciente.Items.Add(new KeyValuePair<string, string>(codigoPaciente, nombreapellidos));
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

        // Método para cargar las plantas y camas disponibles en el formulario
        private void CargarPlantasYCamasDisponibles()
        {
            try
            {

                conexion.AbrirConexion();
                // Limpiar la lista de camas y cargar las camas disponibles de la planta seleccionada
                cmbCamas.Items.Clear();
                camasDisponibles.Clear(); // Limpiar el diccionario de camas disponibles

                string queryCamas = "SELECT CódigoCama, CódigoPlanta FROM Camas WHERE Estado = True";
                using (OleDbCommand cmd = new OleDbCommand(queryCamas, conexion.con))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string codigoCama = reader["CódigoCama"].ToString();
                            string codigoPlanta = reader["CódigoPlanta"].ToString();
                            string nombreCama = $"Cama {codigoCama} (Planta {codigoPlanta})";
                            cmbCamas.Items.Add(nombreCama);

                            // Agregar la tupla (códigoCama, códigoPlanta) a la lista de camas disponibles
                            camasDisponibles.Add((codigoCama, codigoPlanta));
                        }
                    }
                }
                cmbCamas.DisplayMember = "Value"; // Establecer la propiedad DisplayMember para mostrar el nombre de la cama en el ComboBox
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener las camas disponibles de la planta seleccionada. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btn_asignar_Click(object sender, EventArgs e)
        {
            // Obtener el paciente seleccionado y la cama seleccionada
            if (cmbbox_paciente.SelectedItem == null || cmbCamas.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un paciente y una cama disponible.", "Asignar Cama a Paciente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codigoPaciente = ((KeyValuePair<string, string>)cmbbox_paciente.SelectedItem).Key;
            string camaSeleccionada = cmbCamas.SelectedItem.ToString();
            string codigoCama = "";
            string codigoPlanta = "";
            foreach (var cama in camasDisponibles)
            {
                string nombreCama = $"Cama {cama.codigoCama} (Planta {cama.codigoPlanta})";
                if (nombreCama == camaSeleccionada)
                {
                    codigoCama = cama.codigoCama;
                    codigoPlanta = cama.codigoPlanta;
                    break;
                }
            }
            try
            {
                conexion.AbrirConexion();

                // Verificar si el paciente ya tiene asignada una cama
                string queryPaciente = "SELECT CódigoCamaAsignada FROM Pacientes WHERE CódigoPaciente = @CódigoPaciente";
                using (OleDbCommand cmdPaciente = new OleDbCommand(queryPaciente, conexion.con))
                {
                    cmdPaciente.Parameters.AddWithValue("@CódigoPaciente", codigoPaciente);
                    string codigoCamaAsignada = cmdPaciente.ExecuteScalar() as string;

                    // Si el paciente tenía una cama asignada, liberarla y marcarla como disponible
                    if (!string.IsNullOrEmpty(codigoCamaAsignada))
                    {
                        string queryLiberarCama = "UPDATE Camas SET Estado = True WHERE CódigoCama = @CódigoCama";
                        using (OleDbCommand cmdLiberarCama = new OleDbCommand(queryLiberarCama, conexion.con))
                        {
                            cmdLiberarCama.Parameters.AddWithValue("@CódigoCama", codigoCamaAsignada);
                            cmdLiberarCama.ExecuteNonQuery();
                        }
                    }
                }

                // Asignar la nueva cama al paciente
                string queryAsignarCama = "UPDATE Pacientes SET CódigoCamaAsignada = @CódigoCama WHERE CódigoPaciente = @CódigoPaciente";
                using (OleDbCommand cmdAsignarCama = new OleDbCommand(queryAsignarCama, conexion.con))
                {
                    cmdAsignarCama.Parameters.AddWithValue("@CódigoCama", codigoCama);
                    cmdAsignarCama.Parameters.AddWithValue("@CódigoPaciente", codigoPaciente);
                    cmdAsignarCama.ExecuteNonQuery();
                }

                // Marcar la cama seleccionada como no disponible
                string queryMarcarCama = "UPDATE Camas SET Estado = False WHERE CódigoCama = @CódigoCama AND CódigoPlanta = @CódigoPlanta";
                using (OleDbCommand cmdMarcarCama = new OleDbCommand(queryMarcarCama, conexion.con))
                {
                    cmdMarcarCama.Parameters.AddWithValue("@CódigoCama", codigoCama);
                    cmdMarcarCama.Parameters.AddWithValue("@CódigoPlanta", codigoPlanta);
                    cmdMarcarCama.ExecuteNonQuery();
                }

                MessageBox.Show("La cama ha sido asignada exitosamente al paciente.", "Asignar Cama a Paciente", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar los ComboBox después de asignar correctamente la cama
                cmbbox_paciente.SelectedIndex = -1;
                cmbCamas.SelectedIndex = -1;

                // Actualizar la lista de pacientes disponibles para reflejar el cambio en la cama asignada
                cmbbox_paciente.Items.Clear();
                CargarPacientesDisponibles();

                // Actualizar la lista de camas disponibles
                cmbCamas.Items.Clear();
                camasDisponibles.Clear();
                CargarPlantasYCamasDisponibles();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar asignar la cama al paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void cmbbox_paciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbbox_paciente.SelectedItem != null)
            {
                // Obtener el código del paciente seleccionado
                string codigoPacienteSeleccionado = ((KeyValuePair<string, string>)cmbbox_paciente.SelectedItem).Key;

                // Verificar si el paciente ya tiene una cama asignada
                try
                {
                    conexion.AbrirConexion();
                    string queryPaciente = "SELECT CódigoCamaAsignada FROM Pacientes WHERE CódigoPaciente = @CódigoPaciente";
                    using (OleDbCommand cmdPaciente = new OleDbCommand(queryPaciente, conexion.con))
                    {
                        cmdPaciente.Parameters.AddWithValue("@CódigoPaciente", codigoPacienteSeleccionado);
                        string codigoCamaAsignada = cmdPaciente.ExecuteScalar() as string;

                        if (!string.IsNullOrEmpty(codigoCamaAsignada))
                        {
                            // El paciente ya tiene una cama asignada, mostrar advertencia
                            MessageBox.Show("El paciente seleccionado ya tiene asignada la cama " + codigoCamaAsignada + ". Al asignar una nueva cama, se reasignará la cama al paciente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar verificar la cama asignada al paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cmbCamas_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtener el índice de la planta correspondiente a la cama seleccionada
            if (cmbCamas.SelectedItem != null)
            {
                string camaSeleccionada = cmbCamas.SelectedItem.ToString();
                foreach (var cama in camasDisponibles)
                {
                    string nombreCama = $"Cama {cama.codigoCama} (Planta {cama.codigoPlanta})";
                    if (nombreCama == camaSeleccionada)
                    {
                        // La cama seleccionada coincide con la tupla (códigoCama, códigoPlanta)
                        // Mostrar la planta asociada en el ComboBox de pacientes
                        for (int i = 0; i < cmbbox_paciente.Items.Count; i++)
                        {
                            if (((KeyValuePair<string, string>)cmbbox_paciente.Items[i]).Key == cama.codigoPlanta)
                            {
                                cmbbox_paciente.SelectedIndex = i;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}