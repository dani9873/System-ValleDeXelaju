using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemValledeXelaju
{
    public partial class RegistrarCamasForm : Form
    {
        private Conexion conexion;

        public RegistrarCamasForm()
        {
            InitializeComponent();
            conexion = new Conexion();
            CargarCodigoPlantas();
            dtt_registro.Value = DateTime.Today;
            CargarCamasDisponibles();
        }
        private void CargarCodigoPlantas()
        {
            try
            {
                conexion.AbrirConexion();
                string query = "SELECT Id FROM Plantas";
                OleDbCommand command = new OleDbCommand(query, conexion.con);
                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    cmbPlanta.Items.Add(reader["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los códigos de planta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void CargarCamasDisponibles()
        {
            cmb_box_eliminar.Items.Clear();

            try
            {
                conexion.AbrirConexion();

                string query = "SELECT CódigoCama, CódigoPlanta " +
                               "FROM Camas";

                OleDbCommand command = new OleDbCommand(query, conexion.con);
                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string codigoCama = reader["CódigoCama"].ToString();
                    string codigoPlanta = reader["CódigoPlanta"].ToString();
                    string camaInfo = $"{codigoCama} - {codigoPlanta}";
                    cmb_box_eliminar.Items.Add(camaInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las camas disponibles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Obtener los valores ingresados por el usuario desde los controles del formulario
            string codigoCama = txtCodigo_Camas.Text;
            string codigoPlanta = cmbPlanta.SelectedItem?.ToString();
            DateTime fechaAsignacion = dtt_registro.Value;

            // Validar que todos los campos estén llenos
            if (string.IsNullOrWhiteSpace(codigoCama) || string.IsNullOrWhiteSpace(codigoPlanta))
            {
                MessageBox.Show("Por favor, completa todos los campos antes de guardar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validar que el código de cama comience con la letra "C" seguida de números enteros
            if (!codigoCama.StartsWith("C"))
            {
                MessageBox.Show("El código de cama debe comenzar con la letra 'C'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Detener el proceso de registro si el código de cama es inválido
            }

            // Verificar si el código de cama ya existe en la misma planta
            string queryVerifyCama = "SELECT COUNT(*) FROM Camas WHERE CódigoCama = @CodigoCama AND CódigoPlanta = @CodigoPlanta";
            try
            {
                conexion.AbrirConexion();
                using (var verifyCommand = new OleDbCommand(queryVerifyCama, conexion.con))
                {
                    verifyCommand.Parameters.AddWithValue("@CodigoCama", codigoCama);
                    verifyCommand.Parameters.AddWithValue("@CodigoPlanta", codigoPlanta);

                    int count = (int)verifyCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("El código de cama ya existe en la misma planta. Por favor, ingrese un código de cama único.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la conexión con la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                conexion.CerrarConexion();
            }

            // Insertar los datos en la tabla "Camas" utilizando la clase de conexión
            // Establecer el estado automáticamente como "disponible" (true) al insertar una nueva cama
            string queryInsertCama = "INSERT INTO Camas (CódigoCama, CódigoPlanta, FechaAsignacion, Estado) " +
                                    "VALUES (@CodigoCama, @CodigoPlanta, @FechaAsignacion, true)";

            string queryUpdateNumCamas = "UPDATE Plantas SET NumCamas = NumCamas + 1 WHERE Id = @CodigoPlanta";

            try
            {
                conexion.AbrirConexion();
                using (var insertCommand = new OleDbCommand(queryInsertCama, conexion.con))
                {
                    insertCommand.Parameters.AddWithValue("@CodigoCama", codigoCama);
                    insertCommand.Parameters.AddWithValue("@CodigoPlanta", codigoPlanta);
                    insertCommand.Parameters.AddWithValue("@FechaAsignacion", fechaAsignacion);

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // La cama se insertó correctamente, ahora actualizamos el campo NumCamas en la tabla Plantas
                        using (var updateCommand = new OleDbCommand(queryUpdateNumCamas, conexion.con))
                        {
                            updateCommand.Parameters.AddWithValue("@CodigoPlanta", codigoPlanta);
                            updateCommand.ExecuteNonQuery();
                        }

                        MessageBox.Show("Registro de cama exitoso.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario(); // Limpia los controles del formulario después del registro exitoso
                        CargarCamasDisponibles();
                    }
                    else
                    {
                        MessageBox.Show("Error al registrar la cama.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la conexión con la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }



        private void LimpiarFormulario()
        {
            // Limpiar los controles del formulario para permitir un nuevo registro
            txtCodigo_Camas.Text = string.Empty;
            cmbPlanta.SelectedIndex = -1;
            dtt_registro.Value = DateTime.Today;
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            // Verificar si se ha seleccionado una cama en el ComboBox
            if (cmb_box_eliminar.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, seleccione una cama para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Obtener la información de la cama seleccionada en el ComboBox
            string camaInfo = cmb_box_eliminar.SelectedItem.ToString();
            string codigoCama = camaInfo.Split('-')[0].Trim();
            string codigoPlanta = camaInfo.Split('-')[1].Trim();

            // Mostrar un mensaje de advertencia si la cama está asignada a algún paciente
            if (CamaAsignadaAPaciente(codigoCama))
            {
                MessageBox.Show("La cama está asignada a un paciente. No se puede eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mostrar un mensaje de confirmación antes de eliminar la cama
            var confirmResult = MessageBox.Show("¿Está seguro de que desea eliminar esta cama?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.Yes)
            {
                // Eliminar la cama de la base de datos y actualizar el campo NumCamas en la tabla Plantas
                string queryDeleteCama = "DELETE FROM Camas WHERE CódigoCama = @CodigoCama AND CódigoPlanta = @CodigoPlanta";
                string queryUpdateNumCamas = "UPDATE Plantas SET NumCamas = NumCamas - 1 WHERE Id = @CodigoPlanta";

                try
                {
                    conexion.AbrirConexion();
                    using (var deleteCommand = new OleDbCommand(queryDeleteCama, conexion.con))
                    {
                        deleteCommand.Parameters.AddWithValue("@CodigoCama", codigoCama);
                        deleteCommand.Parameters.AddWithValue("@CodigoPlanta", codigoPlanta);
                        int rowsAffected = deleteCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Actualizar el campo NumCamas en la tabla Plantas
                            using (var updateCommand = new OleDbCommand(queryUpdateNumCamas, conexion.con))
                            {
                                updateCommand.Parameters.AddWithValue("@CodigoPlanta", codigoPlanta);
                                updateCommand.ExecuteNonQuery();
                            }

                            MessageBox.Show("Cama eliminada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarCamasDisponibles(); // Volver a cargar las camas disponibles en el ComboBox
                        }
                        else
                        {
                            MessageBox.Show("Error al eliminar la cama. Por favor, intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error en la conexión con la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
        }



        private bool CamaAsignadaAPaciente(string codigoCama)
        {
            try
            {
                conexion.AbrirConexion();

                string query = "SELECT COUNT(*) FROM Pacientes WHERE CódigoCamaAsignada = @CodigoCama";
                OleDbCommand command = new OleDbCommand(query, conexion.con);
                command.Parameters.AddWithValue("@CodigoCama", codigoCama);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar si la cama está asignada a un paciente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }


        private void cmb_box_eliminar_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}