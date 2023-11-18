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
    public partial class RegistrarMedicoForm : Form
    {
        private Conexion conexion;
        private bool medicoSeleccionado = false;
        public RegistrarMedicoForm()
        {
            conexion = new Conexion();
            InitializeComponent();
            medicoSeleccionado = false;

            // Manejar el evento SelectedIndexChanged del ComboBox cmb_box_eli_modificar
            cmb_box_eli_modificar.SelectedIndexChanged += cmb_box_eli_modificar_SelectedIndexChanged;
            CargarMedicos();
        }

        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
        }
        private void CargarMedicos()
        {
            try
            {
                conexion.AbrirConexion();

                cmb_box_eli_modificar.Items.Clear(); // Limpiar el ComboBox antes de cargar los códigos de médicos

                // Obtener los códigos de los médicos disponibles
                string queryMedicos = "SELECT CódigoMedico FROM Medicos";
                using (OleDbCommand cmd = new OleDbCommand(queryMedicos, conexion.con))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string codigoMedico = reader["CódigoMedico"].ToString();
                            cmb_box_eli_modificar.Items.Add(codigoMedico);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener los códigos de médicos. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conexion.con != null && conexion.con.State == System.Data.ConnectionState.Open)
                {
                    conexion.CerrarConexion();
                }
            }
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            // Obtener los datos ingresados por el usuario.
            string codigo = txtCodigo.Text.Trim();
            string numeroColegiado = txtNumeroColegiado.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string apellidos = txtApellidos.Text.Trim();
            string correoElectronico = txt_correo.Text.Trim();
            string telefono = txt_cel.Text.Trim();
            string direccion = txt_direccion.Text.Trim();

            // Verificar que los campos obligatorios no estén vacíos.
            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(numeroColegiado) ||
                string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) ||
                string.IsNullOrEmpty(correoElectronico) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(direccion))
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos.", "Registro de Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar el código del médico
            if (!codigo.StartsWith("M") || codigo.Substring(1).Length == 0 || !codigo.Substring(1).All(char.IsDigit))
            {
                MessageBox.Show("El código del médico debe comenzar con la letra 'M' seguida de números.", "Registro de Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar el nombre del médico (debe empezar con "Dr.")
            if (!nombre.StartsWith("Dr.") && !nombre.StartsWith("Dra."))
            {
                MessageBox.Show("El nombre del médico debe comenzar con 'Dr.' o 'Dra.' seguido de un nombre.", "Registro de Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            try
            {
                conexion.AbrirConexion();

                // Verificar si ya existe un médico con el mismo código o número de colegiado
                string query = "SELECT COUNT(*) FROM Medicos WHERE CódigoMedico = @CódigoMedico OR NumeroColegiado = @NumeroColegiado";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@CódigoMedico", codigo);
                    cmd.Parameters.AddWithValue("@NumeroColegiado", numeroColegiado);

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Ya existe un médico registrado con el mismo código o número de colegiado.", "Registro de Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Insertar el nuevo médico en la base de datos
                query = "INSERT INTO Medicos (CódigoMedico, NumeroColegiado, Nombre, Apellidos, CorreoElectronico, Telefono, Direccion) " +
                        "VALUES (@codigo, @NumeroColegiado, @Nombre, @Apellidos, @CorreoElectronico, @Telefono, @Direccion)";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.Parameters.AddWithValue("@NumeroColegiado", numeroColegiado);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                    cmd.Parameters.AddWithValue("@CorreoElectronico", correoElectronico);
                    cmd.Parameters.AddWithValue("@Telefono", telefono);
                    cmd.Parameters.AddWithValue("@Direccion", direccion);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("El médico ha sido registrado exitosamente.", "Registro de Médico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarMedicos();
                        
                    }
                    else
                    {
                        MessageBox.Show("No se pudo registrar al médico. Por favor, intente nuevamente.", "Registro de Médico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar registrar al médico. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtNombre.Clear();
            txtApellidos.Clear();
            txtNumeroColegiado.Clear();
            txt_cel.Clear();
            txt_correo.Clear();
            txt_direccion.Clear();
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btn_modificar_Click(object sender, EventArgs e)
        {
            if (!medicoSeleccionado)
            {
                MessageBox.Show("Por favor, seleccione un médico de la lista para modificar.", "Modificar Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codigoMedicoSeleccionado = cmb_box_eli_modificar.SelectedItem.ToString();

            if (MessageBox.Show("¿Está seguro de modificar los datos del médico?", "Modificar Médico", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Obtener los datos ingresados por el usuario
                string codigo = txtCodigo.Text.Trim();
                string numeroColegiado = txtNumeroColegiado.Text.Trim();
                string nombre = txtNombre.Text.Trim();
                string apellidos = txtApellidos.Text.Trim();
                string correoElectronico = txt_correo.Text.Trim();
                string telefono = txt_cel.Text.Trim();
                string direccion = txt_direccion.Text.Trim();

                // Validar los campos obligatorios
                if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(numeroColegiado) ||
                    string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) ||
                    string.IsNullOrEmpty(correoElectronico) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(direccion))
                {
                    MessageBox.Show("Por favor, complete todos los campos requeridos.", "Modificar Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar el código del médico
                if (!codigo.StartsWith("M") || codigo.Substring(1).Length == 0 || !codigo.Substring(1).All(char.IsDigit))
                {
                    MessageBox.Show("El código del médico debe comenzar con la letra 'M' seguida de números.", "Modificar Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar el nombre del médico (debe empezar con "Dr.")
                if (!nombre.StartsWith("Dr."))
                {
                    MessageBox.Show("El nombre del médico debe comenzar con 'Dr.' seguido de un nombre.", "Modificar Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    conexion.AbrirConexion();

                    // Verificar si ya existe otro médico con el mismo código o número de colegiado
                    string query = "SELECT COUNT(*) FROM Medicos WHERE (CódigoMedico = @CódigoMedico OR NumeroColegiado = @NumeroColegiado) AND CódigoMedico <> @CódigoMedicoSeleccionado";
                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoMedico", codigo);
                        cmd.Parameters.AddWithValue("@NumeroColegiado", numeroColegiado);
                        cmd.Parameters.AddWithValue("@CódigoMedicoSeleccionado", codigoMedicoSeleccionado);

                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Ya existe otro médico registrado con el mismo código o número de colegiado.", "Modificar Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Actualizar los datos del médico en la base de datos
                    query = "UPDATE Medicos SET CódigoMedico = @CódigoMedico, NumeroColegiado = @NumeroColegiado, Nombre = @Nombre, Apellidos = @Apellidos, CorreoElectronico = @CorreoElectronico, Telefono = @Telefono, Direccion = @Direccion WHERE CódigoMedico = @CódigoMedicoSeleccionado";
                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoMedico", codigo);
                        cmd.Parameters.AddWithValue("@NumeroColegiado", numeroColegiado);
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                        cmd.Parameters.AddWithValue("@CorreoElectronico", correoElectronico);
                        cmd.Parameters.AddWithValue("@Telefono", telefono);
                        cmd.Parameters.AddWithValue("@Direccion", direccion);
                        cmd.Parameters.AddWithValue("@CódigoMedicoSeleccionado", codigoMedicoSeleccionado);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Los datos del médico han sido modificados exitosamente.", "Modificar Médico", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpiar los campos después de modificar los datos
                            LimpiarCampos();

                            // Actualizar los médicos disponibles en el ComboBox
                            CargarMedicos();
                            
                        }
                        else
                        {
                            MessageBox.Show("No se pudo modificar los datos del médico. Por favor, intente nuevamente.", "Modificar Médico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar modificar los datos del médico. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
        }

        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            if (!medicoSeleccionado)
            {
                MessageBox.Show("Por favor, seleccione un médico de la lista para eliminar.", "Eliminar Médico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codigoMedicoSeleccionado = cmb_box_eli_modificar.SelectedItem.ToString();

            if (MessageBox.Show("¿Está seguro de eliminar el registro del médico?", "Eliminar Médico", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    conexion.AbrirConexion();

                    // Eliminar el registro del médico de la base de datos
                    string query = "DELETE FROM Medicos WHERE CódigoMedico = @CódigoMedico";
                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoMedico", codigoMedicoSeleccionado);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("El médico ha sido eliminado exitosamente.", "Eliminar Médico", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Limpiar los campos después de eliminar el registro
                            LimpiarCampos();

                            // Actualizar los médicos disponibles en el ComboBox
                            CargarMedicos();
                            
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el médico. Por favor, intente nuevamente.", "Eliminar Médico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar eliminar al médico. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
        }

        private void cmb_box_eli_modificar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_box_eli_modificar.SelectedItem != null)
            {
                string codigoMedicoSeleccionado = cmb_box_eli_modificar.SelectedItem.ToString();

                try
                {
                    conexion.AbrirConexion();

                    // Obtener los datos del médico seleccionado
                    string query = "SELECT * FROM Medicos WHERE CódigoMedico = @CódigoMedico";
                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoMedico", codigoMedicoSeleccionado);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Mostrar los datos del médico en los campos correspondientes
                                txtCodigo.Text = reader["CódigoMedico"].ToString();
                                txtNumeroColegiado.Text = reader["NumeroColegiado"].ToString();
                                txtNombre.Text = reader["Nombre"].ToString();
                                txtApellidos.Text = reader["Apellidos"].ToString();
                                txt_correo.Text = reader["CorreoElectronico"].ToString();
                                txt_cel.Text = reader["Telefono"].ToString();
                                txt_direccion.Text = reader["Direccion"].ToString();
                                medicoSeleccionado = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar obtener los datos del médico. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
            else
            {
                // Limpiar los campos si no hay ningún valor seleccionado
                LimpiarCampos();
            }
        }
    }
}
