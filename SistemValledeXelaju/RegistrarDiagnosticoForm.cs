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
    public partial class RegistrarDiagnosticoForm : Form
    {
        private Conexion conexion;
        public RegistrarDiagnosticoForm()
        {
            InitializeComponent();
            conexion = new Conexion();
        }

        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
        }
        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            txtCodigo.Clear();
            txtDescripcion.Clear();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Obtener la descripción del diagnóstico ingresada por el usuario
            string codigo = txtCodigo.Text.Trim();
            string descripcionDiagnostico = txtDescripcion.Text.Trim();

            if (!codigo.StartsWith("D") || codigo.Substring(1).Length == 0 || !codigo.Substring(1).All(char.IsDigit))
            {
                MessageBox.Show("El código del diagnóstico debe comenzar con la letra 'D' seguida de números.", "Registro de Diagnóstico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(descripcionDiagnostico) || string.IsNullOrEmpty(codigo))
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos y agrega una descripción.", "Registro de Diagnóstico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                conexion.AbrirConexion();

                // Verificar si ya existe un diagnóstico con el mismo código o descripción
                string query = "SELECT COUNT(*) FROM Diagnosticos WHERE CódigoDiagnostico = @CodigoDiagnostico OR Descripcion = @Descripcion";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@CodigoDiagnostico", codigo);
                    cmd.Parameters.AddWithValue("@Descripcion", descripcionDiagnostico);

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Ya existe un diagnóstico registrado con el mismo código o descripción.", "Registro de Diagnóstico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Insertar el nuevo diagnóstico en la base de datos
                query = "INSERT INTO Diagnosticos (CódigoDiagnostico, Descripcion) VALUES (@CodigoDiagnostico, @Descripcion)";
                using (OleDbCommand cmd = new OleDbCommand(query, conexion.con))
                {
                    cmd.Parameters.AddWithValue("@CodigoDiagnostico", codigo);
                    cmd.Parameters.AddWithValue("@Descripcion", descripcionDiagnostico);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("El diagnóstico ha sido registrado exitosamente.", "Registro de Diagnóstico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo registrar el diagnóstico. Por favor, intente nuevamente.", "Registro de Diagnóstico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar registrar el diagnóstico. Detalles del error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                conexion.CerrarConexion();

            }
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtDescripcion.Clear();
        }
    }
}