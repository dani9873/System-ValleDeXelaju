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
    public partial class RegistrarPacienteForm : Form
    {
        private Conexion conexion;
        private List<Tuple<string, string>> camasDisponibles;
        // Diccionario para almacenar las camas disponibles
        public RegistrarPacienteForm()
        {
            InitializeComponent();
            conexion = new Conexion();
            camasDisponibles = new List<Tuple<string, string>>();
            CargarCamasDisponibles();
            CargarPacientes();

            // Manejar el evento SelectedValueChanged del ComboBox cmb_box_eli_modificar
            cmb_box_eli_modificar.SelectedValueChanged += cmb_box_eli_modificar_SelectedValueChanged;
        }



        private void CargarPacientes()
        {
            try
            {
                conexion.AbrirConexion();

                cmb_box_eli_modificar.Items.Clear(); // Limpiar el ComboBox antes de cargar los códigos de pacientes

                // Obtener los códigos de los pacientes disponibles
                string queryPacientes = "SELECT CódigoPaciente FROM Pacientes";
                using (OleDbCommand cmd = new OleDbCommand(queryPacientes, conexion.con))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string codigoPaciente = reader["CódigoPaciente"].ToString();
                            cmb_box_eli_modificar.Items.Add(codigoPaciente);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener los códigos de pacientes. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conexion.con != null && conexion.con.State == System.Data.ConnectionState.Open)
                {
                    conexion.CerrarConexion();
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Obtener los datos ingresados por el usuario
            string codigo = txtCodigo.Text.Trim();
            string dpi = txtDPI.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string apellidos = txtApellidos.Text.Trim();
            DateTime fechaNacimiento = dtpFechaNacimiento.Value;

            string codigoPlantaCama = GetCodigoPlantaFromSelectedBed();


            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(dpi) || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) || dtpFechaIngreso.Value == dtpFechaIngreso.MinDate || cmbCamas.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos y seleccione una cama.", "Registro de Paciente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar la condición del código
            if (!codigo.StartsWith("P") || codigo.Substring(1).Length == 0 || !codigo.Substring(1).All(char.IsDigit))
            {
                MessageBox.Show("El código debe comenzar con la letra 'P' seguida de números.", "Registro de Paciente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Agregar "PL" al CódigoPlanta
            string codigoPlanta = "PL" + codigoPlantaCama;

            try
            {
                conexion.AbrirConexion();

                // Verificar si ya existe un paciente con el mismo código
                string query = "SELECT COUNT(*) FROM Pacientes WHERE CódigoPaciente = @CódigoPaciente";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@CódigoPaciente", codigo);

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Ya existe un paciente registrado con el mismo código.", "Registro de Paciente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Obtener la cama seleccionada
                string codigoCama = ((KeyValuePair<string, string>)cmbCamas.SelectedItem).Key;

                // Insertar el nuevo paciente en la base de datos
                query = "INSERT INTO Pacientes (CódigoPaciente, DPI, Nombre, Apellidos, FechaNacimiento, CódigoCamaAsignada, CodigoPlanta) VALUES (@CódigoPaciente, @DPI, @Nombre, @Apellidos, @FechaNacimiento, @CódigoCamaAsignada, @CodigoPlanta)";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@CódigoPaciente", codigo);
                    cmd.Parameters.AddWithValue("@DPI", dpi);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);
                    cmd.Parameters.AddWithValue("@CódigoCamaAsignada", codigoCama);
                    cmd.Parameters.AddWithValue("@CodigoPlanta", codigoPlanta);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Obtener el Id del paciente recién insertado
                        query = "SELECT Id FROM Pacientes WHERE CódigoPaciente = @CódigoPaciente";
                        using (OleDbCommand getIdCmd = new OleDbCommand(query, conexion.con))
                        {
                            getIdCmd.Parameters.AddWithValue("@CódigoPaciente", codigo);
                            int pacienteId = (int)getIdCmd.ExecuteScalar();

                            // Insertar el ingreso del paciente en la tabla "Ingresos"
                            query = "INSERT INTO Ingresos (CódigoPaciente, FechaIngreso) VALUES (@CódigoPaciente, @FechaIngreso)";
                            using (OleDbCommand insertIngresoCmd = new OleDbCommand(query, conexion.con))
                            {
                                insertIngresoCmd.Parameters.AddWithValue("@CódigoPaciente", pacienteId);
                                insertIngresoCmd.Parameters.AddWithValue("@FechaIngreso", dtpFechaIngreso.Value);
                                insertIngresoCmd.ExecuteNonQuery();
                            }
                        }


                        MessageBox.Show("El paciente ha sido registrado exitosamente.", "Registro de Paciente", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Actualizar el estado de la cama a no disponible
                        query = "UPDATE Camas SET Estado = False WHERE CódigoCama = @CódigoCama";
                        using (OleDbCommand updateCmd = new OleDbCommand(query, conexion.con))
                        {
                            updateCmd.Parameters.AddWithValue("@CódigoCama", codigoCama);
                            updateCmd.ExecuteNonQuery();
                        }

                        // Actualizar las camas disponibles en el ComboBox
                        CargarCamasDisponibles();
                        CargarPacientes();
                        // Limpiar los campos después del registro exitoso
                        LimpiarCampos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo registrar al paciente. Por favor, intente nuevamente.", "Registro de Paciente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar registrar al paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private string GetCodigoPlantaFromSelectedBed()
        {
            string codigoPlanta = "";

            // Obtener el CódigoPlanta de la cama seleccionada
            if (cmbCamas.SelectedItem is KeyValuePair<string, string> selectedBed)
            {
                string codigoCama = selectedBed.Key;

                try
                {
                    conexion.AbrirConexion();

                    string queryGetPlanta = "SELECT CódigoPlanta FROM Camas WHERE CódigoCama = @CódigoCama";
                    using (OleDbCommand cmd = new OleDbCommand(queryGetPlanta, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoCama", codigoCama);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            codigoPlanta = result.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener el CódigoPlanta de la cama seleccionada. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }

            return codigoPlanta;
        }
        private void CargarCamasDisponibles()
        {
            try
            {
                conexion.AbrirConexion();

                cmbCamas.Items.Clear(); // Limpiar el ComboBox antes de cargar las camas disponibles
                camasDisponibles.Clear(); // Limpiar el diccionario de camas disponibles

                // Obtener las camas disponibles
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
                            cmbCamas.Items.Add(new KeyValuePair<string, string>(codigoCama, nombreCama));
                            camasDisponibles.Add(new Tuple<string, string>(codigoCama, nombreCama));
                        }
                    }
                }

                cmbCamas.DisplayMember = "Value"; // Establecer la propiedad DisplayMember para mostrar el nombre de la cama en el ComboBox
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener las camas disponibles. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conexion.con != null && conexion.con.State == System.Data.ConnectionState.Open)
                {
                    conexion.CerrarConexion();
                }
            }
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtDPI.Clear();
            txtNombre.Clear();
            txtApellidos.Clear();
            dtpFechaNacimiento.Value = DateTime.Now;
            cmbCamas.SelectedIndex = -1; // Deseleccionar cualquier cama en el ComboBox
        }



        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btn_modificar_Click(object sender, EventArgs e)
        {
            string codigoPacienteSeleccionado = cmb_box_eli_modificar.SelectedItem.ToString();

            if (MessageBox.Show("¿Está seguro de modificar los datos del paciente?", "Modificar Paciente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    conexion.AbrirConexion();

                    // Actualizar los datos del paciente en la base de datos
                    string query = "UPDATE Pacientes SET DPI = @DPI, Nombre = @Nombre, Apellidos = @Apellidos, FechaNacimiento = @FechaNacimiento WHERE CódigoPaciente = @CódigoPaciente";
                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@DPI", txtDPI.Text.Trim());
                        cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@Apellidos", txtApellidos.Text.Trim());
                        cmd.Parameters.AddWithValue("@FechaNacimiento", dtpFechaNacimiento.Value);
                        cmd.Parameters.AddWithValue("@CódigoPaciente", codigoPacienteSeleccionado);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Los datos del paciente han sido modificados exitosamente.", "Modificar Paciente", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Actualizar la lista de pacientes en el ComboBox
                            CargarPacientes();

                            // Limpiar los campos después de modificar el registro
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo modificar los datos del paciente. Por favor, intente nuevamente.", "Modificar Paciente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar modificar los datos del paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
        }


        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            string codigoPacienteSeleccionado = cmb_box_eli_modificar.SelectedItem.ToString();

            if (MessageBox.Show("¿Está seguro de eliminar el registro del paciente?", "Eliminar Paciente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    conexion.AbrirConexion();

                    // Obtener la cama asignada al paciente antes de eliminarlo
                    string codigoCamaAsignada = "";
                    string queryGetCamaAsignada = "SELECT CódigoCamaAsignada FROM Pacientes WHERE CódigoPaciente = @CódigoPaciente";
                    using (OleDbCommand cmd = new OleDbCommand(queryGetCamaAsignada, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoPaciente", codigoPacienteSeleccionado);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            codigoCamaAsignada = result.ToString();
                        }
                    }

                    // Eliminar el registro del paciente de la base de datos
                    string queryEliminarPaciente = "DELETE FROM Pacientes WHERE CódigoPaciente = @CódigoPaciente";
                    using (OleDbCommand cmdEliminarPaciente = new OleDbCommand(queryEliminarPaciente, conexion.con))
                    {
                        cmdEliminarPaciente.Parameters.AddWithValue("@CódigoPaciente", codigoPacienteSeleccionado);
                        int rowsAffected = cmdEliminarPaciente.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Si el paciente tenía una cama asignada, actualizar el estado de la cama a disponible
                            if (!string.IsNullOrEmpty(codigoCamaAsignada))
                            {
                                string queryActualizarCama = "UPDATE Camas SET Estado = True WHERE CódigoCama = @CódigoCama";
                                using (OleDbCommand cmdActualizarCama = new OleDbCommand(queryActualizarCama, conexion.con))
                                {
                                    cmdActualizarCama.Parameters.AddWithValue("@CódigoCama", codigoCamaAsignada);
                                    cmdActualizarCama.ExecuteNonQuery();
                                }
                            }

                            MessageBox.Show("El paciente ha sido eliminado exitosamente.", "Eliminar Paciente", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Actualizar la lista de pacientes en el ComboBox
                            CargarPacientes();
                            CargarCamasDisponibles();
                            // Limpiar los campos después de eliminar el registro
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el paciente. Por favor, intente nuevamente.", "Eliminar Paciente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar eliminar al paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
        }


        private void RegistrarPacienteForm_Load(object sender, EventArgs e)
        {

        }

        private void cmb_box_eli_modificar_SelectedValueChanged(object sender, EventArgs e)
        {
            // Verificar si hay un valor seleccionado y si es del tipo correcto (código del paciente)
            if (cmb_box_eli_modificar.SelectedItem != null && cmb_box_eli_modificar.SelectedItem is string codigoPacienteSeleccionado && codigoPacienteSeleccionado.StartsWith("P"))
            {
                try
                {
                    conexion.AbrirConexion();

                    // Obtener los datos del paciente seleccionado
                    string query = "SELECT CódigoPaciente, DPI, Nombre, Apellidos, FechaNacimiento FROM Pacientes WHERE CódigoPaciente = @CódigoPaciente";
                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoPaciente", codigoPacienteSeleccionado);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Mostrar los datos del paciente en los campos correspondientes
                                txtCodigo.Text = reader["CódigoPaciente"].ToString();
                                txtDPI.Text = reader["DPI"].ToString();
                                txtNombre.Text = reader["Nombre"].ToString();
                                txtApellidos.Text = reader["Apellidos"].ToString();
                                dtpFechaNacimiento.Value = Convert.ToDateTime(reader["FechaNacimiento"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar obtener los datos del paciente. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
            else
            {
                // Limpiar los campos si no hay ningún valor seleccionado o si el tipo seleccionado no es válido
                LimpiarCampos();
            }
        }

        private void cmb_box_eli_modificar_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
