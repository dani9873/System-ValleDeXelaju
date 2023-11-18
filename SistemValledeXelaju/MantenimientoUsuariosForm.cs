using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.OleDb;

namespace SistemValledeXelaju
{
    public partial class MantenimientoUsuariosForm : Form
    {
        private Conexion conexion;
        private bool usuarioSeleccionado = false;
        public MantenimientoUsuariosForm()
        {
            conexion = new Conexion();
            InitializeComponent();
            cmb_box_modificar.SelectedIndexChanged += cmb_box_modificar_SelectedIndexChanged;
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            try
            {
                conexion.AbrirConexion();

                cmb_box_modificar.Items.Clear(); // Limpiar el ComboBox antes de cargar los códigos de usuarios

                // Obtener los códigos de los usuarios disponibles
                string queryUsuarios = "SELECT CódigoUsuario FROM Usuarios";
                using (OleDbCommand cmd = new OleDbCommand(queryUsuarios, conexion.con))
                {
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string codigoUsuario = reader["CódigoUsuario"].ToString();
                            cmb_box_modificar.Items.Add(codigoUsuario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar obtener los códigos de usuarios. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conexion.con != null && conexion.con.State == System.Data.ConnectionState.Open)
                {
                    conexion.CerrarConexion();
                }
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
            // Obtener los datos ingresados por el usuario.
            string codigoUsuario = txtCodigo.Text.Trim();
            string nombreUsuario = txtNombreUsuario.Text.Trim();
            string contraseña = txtContraseña.Text.Trim();
            string nombreCompleto = txtNombreCompleto.Text.Trim();
            string puesto = cmbPuesto.SelectedItem?.ToString(); // Valor seleccionado en el ComboBox
            string numeroTelefono = txtNum.Text.Trim();

            // Verificar que los campos obligatorios no estén vacíos.
            if (string.IsNullOrEmpty(codigoUsuario) || string.IsNullOrEmpty(nombreUsuario) ||
                string.IsNullOrEmpty(contraseña) || string.IsNullOrEmpty(nombreCompleto) ||
                string.IsNullOrEmpty(puesto) || string.IsNullOrEmpty(numeroTelefono))
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos.", "Registro de Usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar el código del usuario
            if (!codigoUsuario.StartsWith("U") || codigoUsuario.Substring(1).Length == 0 || !codigoUsuario.Substring(1).All(char.IsDigit))
            {
                MessageBox.Show("El código del usuario debe comenzar con la letra 'U' seguida de números.", "Registro de Usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                conexion.AbrirConexion();

                // Verificar si ya existe un usuario con el mismo código o nombre de usuario
                string query = "SELECT COUNT(*) FROM Usuarios WHERE CódigoUsuario = @CódigoUsuario OR NombreUsuario = @NombreUsuario";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@CódigoUsuario", codigoUsuario);
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Ya existe un usuario registrado con el mismo código o nombre de usuario.", "Registro de Usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Cifrar la contraseña utilizando MD5
                string contraseñaEncriptada = conexion.EncriptarMD5(contraseña);

                // Insertar el nuevo usuario en la base de datos
                query = "INSERT INTO Usuarios (CódigoUsuario, NombreUsuario, Contraseña, NombreCompleto, Puesto, NumeroTelefono) " +
                        "VALUES (@CódigoUsuario, @NombreUsuario, @Contraseña, @NombreCompleto, @Puesto, @NumeroTelefono)";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@CódigoUsuario", codigoUsuario);
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Contraseña", contraseñaEncriptada); // Utilizar la contraseña encriptada
                    cmd.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);
                    cmd.Parameters.AddWithValue("@Puesto", puesto);
                    cmd.Parameters.AddWithValue("@NumeroTelefono", numeroTelefono);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("El usuario ha sido registrado exitosamente.", "Registro de Usuario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo registrar al usuario. Por favor, intente nuevamente.", "Registro de Usuario", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar registrar al usuario. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // Limpia los campos de entrada
            txtCodigo.Text = string.Empty;
            txtNombreUsuario.Text = string.Empty;
            txtContraseña.Text = string.Empty;
            txtNombreCompleto.Text = string.Empty;
            cmbPuesto.SelectedIndex = -1; // Reinicia la selección del ComboBox
            txtNum.Text = string.Empty;

        }

        private void btn_modificar_Click(object sender, EventArgs e)
        {
            string codigoUsuario = txtCodigo.Text.Trim();
            string nombreUsuario = txtNombreUsuario.Text.Trim();
            string nombreCompleto = txtNombreCompleto.Text.Trim();
            string puesto = cmbPuesto.SelectedItem?.ToString();
            string numeroTelefono = txtNum.Text.Trim();

            try
            {
                conexion.AbrirConexion();

                string query = "UPDATE Usuarios " +
                               "SET NombreUsuario = @NombreUsuario, NombreCompleto = @NombreCompleto, Puesto = @Puesto, NumeroTelefono = @NumeroTelefono " +
                               "WHERE CódigoUsuario = @CódigoUsuario";

                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);
                    cmd.Parameters.AddWithValue("@Puesto", puesto);
                    cmd.Parameters.AddWithValue("@NumeroTelefono", numeroTelefono);
                    cmd.Parameters.AddWithValue("@CódigoUsuario", codigoUsuario);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Los datos del usuario han sido modificados exitosamente.", "Modificar Usuario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo modificar los datos del usuario. Por favor, intente nuevamente.", "Modificar Usuario", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar modificar los datos del usuario. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }


        private void cmb_box_modificar_SelectedIndexChanged(object sender, EventArgs e)
        {
            string codigoUsuarioSeleccionado = cmb_box_modificar.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(codigoUsuarioSeleccionado))
            {
                try
                {
                    conexion.AbrirConexion();

                    string query = "SELECT CódigoUsuario, NombreUsuario, NombreCompleto, Puesto, NumeroTelefono " +
                                   "FROM Usuarios " +
                                   "WHERE CódigoUsuario = @CódigoUsuario";

                    using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                    {
                        cmd.Parameters.AddWithValue("@CódigoUsuario", codigoUsuarioSeleccionado);

                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            txtCodigo.Text = reader["CódigoUsuario"].ToString();
                            txtNombreUsuario.Text = reader["NombreUsuario"].ToString();
                            txtNombreCompleto.Text = reader["NombreCompleto"].ToString();
                            cmbPuesto.SelectedItem = reader["Puesto"].ToString();
                            txtNum.Text = reader["NumeroTelefono"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron datos para el usuario seleccionado.", "Modificar Usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            LimpiarCampos();
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener los datos del usuario. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
            else
            {
                LimpiarCampos();
            }
        }


    }
}
