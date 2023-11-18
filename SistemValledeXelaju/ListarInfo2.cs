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
    public partial class ListarInfo2 : Form
    {
        private Conexion conexion;
        public ListarInfo2()
        {
            conexion = new Conexion();
            InitializeComponent();
        }

        private void btn_cerrar_sesion_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            ListadosGenerar listadosGenerar = new ListadosGenerar();
            listadosGenerar.ShowDialog();
        }

        private void ListarInfo2_Load(object sender, EventArgs e)
        {
            try
            {
                // Abre la conexión a la base de datos
                conexion.AbrirConexion();

                // Consulta para obtener las Plantas
                string query = "SELECT Id, CódigoPlanta, Nombre, NumCamas FROM Plantas";

                // Crea un adaptador de datos y un DataSet
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);
                DataSet dataSet = new DataSet();

                // Llena el DataSet con los datos obtenidos de la consulta
                adapter.Fill(dataSet, "Plantas");

                // Asigna los datos al DataGridView
                dataGridView1.DataSource = dataSet.Tables["Plantas"];

                // Cierra la conexión a la base de datos
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar las Plantas: " + ex.Message);
            }
        }

        private void txt_buscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string codigoPlanta = txt_buscar.Text;

                // Abre la conexión a la base de datos
                conexion.AbrirConexion();

                // Consulta para buscar Plantas por código o nombre
                string query = "SELECT Id, CódigoPlanta, Nombre, NumCamas FROM Plantas WHERE CódigoPlanta LIKE '%" + codigoPlanta + "%' OR Nombre LIKE '%" + codigoPlanta + "%'";

                // Crea un adaptador de datos y un DataSet
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);
                DataSet dataSet = new DataSet();

                // Llena el DataSet con los datos obtenidos de la consulta
                adapter.Fill(dataSet, "Plantas");

                // Asigna los datos al DataGridView
                dataGridView1.DataSource = dataSet.Tables["Plantas"];

                // Cierra la conexión a la base de datos
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar las Plantas: " + ex.Message);
            }
        }
    }
}
