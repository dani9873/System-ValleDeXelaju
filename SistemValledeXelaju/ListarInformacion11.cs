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
    public partial class ListarInformacion11 : Form
    {
        private Conexion conexion;
        public ListarInformacion11()
        {
            conexion = new Conexion();
            InitializeComponent();
        }

        private void ListarInformacion1_Load(object sender, EventArgs e)
        {
            try
            {
                // Abre la conexión a la base de datos
                conexion.AbrirConexion();

                // Consulta para obtener los Pacientes
                string query = "SELECT Id, CódigoPaciente, DPI, Nombre, Apellidos, FechaNacimiento, CódigoCamaAsignada FROM Pacientes";

                // Crea un adaptador de datos y un DataSet
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);
                DataSet dataSet = new DataSet();

                // Llena el DataSet con los datos obtenidos de la consulta
                adapter.Fill(dataSet, "Pacientes");

                // Asigna los datos al DataGridView
                dataGridView1.DataSource = dataSet.Tables["Pacientes"];

                // Cierra la conexión a la base de datos
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar los Pacientes: " + ex.Message);
            }
        }

        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            ListadoGenerar1 listadosGenerar = new ListadoGenerar1();
            listadosGenerar.ShowDialog();
        }

        private void txt_buscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string nombre = txt_buscar.Text;

                // Abre la conexión a la base de datos
                conexion.AbrirConexion();

                // Consulta para buscar Pacientes por nombre, CódigoPaciente o DPI
                string query = "SELECT Id, CódigoPaciente, DPI, Nombre, Apellidos, FechaNacimiento, CódigoCamaAsignada, CodigoPlanta FROM Pacientes WHERE CódigoPaciente LIKE '%" + nombre + "%' OR DPI LIKE '%" + nombre + "%' OR Nombre LIKE '%" + nombre + "%'";

                // Crea un adaptador de datos y un DataSet
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);
                DataSet dataSet = new DataSet();

                // Llena el DataSet con los datos obtenidos de la consulta
                adapter.Fill(dataSet, "Pacientes");

                // Asigna los datos al DataGridView
                dataGridView1.DataSource = dataSet.Tables["Pacientes"];

                // Cierra la conexión a la base de datos
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar los Pacientes: " + ex.Message);
            }

        }
    }
}
