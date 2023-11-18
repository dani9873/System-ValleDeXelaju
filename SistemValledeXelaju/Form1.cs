using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace SistemValledeXelaju
{
    public partial class Form1 : Form
    {
        private Conexion conexion;

        public Form1()
        {
            InitializeComponent();
            conexion = new Conexion();
        }

        private void btn_inicio_Click(object sender, EventArgs e)
        {
            string usuario = txt_user.Text.Trim();
            string contrasenia = txt_contra.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasenia))
            {
                MessageBox.Show("Por favor, ingrese su nombre de usuario y contraseña.", "Inicio de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                conexion.AbrirConexion();

                string query = "SELECT Contraseña, Puesto FROM Usuarios WHERE NombreUsuario = @Usuario";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hashGuardado = reader["Contraseña"].ToString();
                            string puesto = reader["Puesto"].ToString();

                            // Verificar la contraseña utilizando el método VerificarMD5 de la clase Conexion
                            bool contraseniaValida = conexion.VerificarMD5(contrasenia, hashGuardado);

                            if (contraseniaValida)
                            {
                                // Verificar el Puesto del usuario para determinar qué formulario mostrar a continuación
                                if (puesto.Contains("Administrador"))
                                {
                                    MainForm mainForm = new MainForm();
                                    mainForm.Show();
                                    this.Hide();
                                }
                                else if (puesto == "Registrador")
                                {
                                    MessageBox.Show("El usuario registrado no tiene acceso al mantenimiento de usuarios.", "Inicio de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    MainForm2 mainForm2 = new MainForm2();
                                    mainForm2.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    MessageBox.Show("Puesto desconocido. Por favor, contacte al administrador del sistema.", "Inicio de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Credenciales inválidas. Por favor, verifique su nombre de usuario y contraseña.", "Inicio de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Credenciales inválidas. Por favor, verifique su nombre de usuario y contraseña.", "Inicio de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar iniciar sesión. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }


        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OcultarDatosIngresados();
        }

        private void OcultarDatosIngresados()
        {
            txt_contra.PasswordChar = '*';
            txt_contra.UseSystemPasswordChar = true;
        }
    }
}
