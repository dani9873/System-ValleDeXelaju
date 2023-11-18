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
    public partial class ListadoInfo41 : Form
    {
        private Conexion conexion;
        public ListadoInfo41()
        {
            conexion = new Conexion();
            InitializeComponent();
        }

        private void ListadoInfo4_Load(object sender, EventArgs e)
        {
            try
            {
                // Abre la conexión a la base de datos
                conexion.AbrirConexion();

                // Consulta para obtener los Diagnósticos
                string query = "SELECT CódigoDiagnostico, Descripcion, Id, Observaciones FROM Diagnosticos";

                // Crea un adaptador de datos y un DataSet
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);
                DataSet dataSet = new DataSet();

                // Llena el DataSet con los datos obtenidos de la consulta
                adapter.Fill(dataSet, "Diagnosticos");

                // Asigna los datos al DataGridView
                dataGridView1.DataSource = dataSet.Tables["Diagnosticos"];

                // Cierra la conexión a la base de datos
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar los Diagnósticos: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
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
                string codigoDiagnostico = txt_buscar.Text;

                // Abre la conexión a la base de datos
                conexion.AbrirConexion();

                // Consulta para buscar Diagnósticos por código o descripción
                string query = "SELECT CódigoDiagnostico, Descripcion, Id, Observaciones FROM Diagnosticos WHERE CódigoDiagnostico LIKE '%" + codigoDiagnostico + "%' OR Descripcion LIKE '%" + codigoDiagnostico + "%'";

                // Crea un adaptador de datos y un DataSet
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);
                DataSet dataSet = new DataSet();

                // Llena el DataSet con los datos obtenidos de la consulta
                adapter.Fill(dataSet, "Diagnosticos");

                // Asigna los datos al DataGridView
                dataGridView1.DataSource = dataSet.Tables["Diagnosticos"];

                // Cierra la conexión a la base de datos
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar los Diagnósticos: " + ex.Message);
            }
        }
    }
}
