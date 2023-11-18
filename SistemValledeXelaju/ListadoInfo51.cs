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
    
    public partial class ListadoInfo51 : Form
    {
        private Conexion conexion;
        public ListadoInfo51()
        {
            conexion = new Conexion();
            InitializeComponent();
        }

        private void btn_regresar_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            ListadoGenerar1 listadosGenerar = new ListadoGenerar1();
            listadosGenerar.ShowDialog();
        }

        private void ListadoInfo5_Load(object sender, EventArgs e)
        {
            try
            {
                // Abre la conexión a la base de datos
                conexion.AbrirConexion();

                // Consulta para obtener los Médicos
                string query = "SELECT Id, CódigoMedico, NumeroColegiado, Nombre, Apellidos, CorreoElectronico, Telefono, Direccion FROM Medicos";

                // Crea un adaptador de datos y un DataSet
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);
                DataSet dataSet = new DataSet();

                // Llena el DataSet con los datos obtenidos de la consulta
                adapter.Fill(dataSet, "Medicos");

                // Asigna los datos al DataGridView
                dataGridView1.DataSource = dataSet.Tables["Medicos"];

                // Cierra la conexión a la base de datos
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar los Médicos: " + ex.Message);
            }
        }

        private void txt_buscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string codigoMedico = txt_buscar.Text;

                // Abre la conexión a la base de datos
                conexion.AbrirConexion();

                // Consulta para buscar Médicos por código, número de colegiado, nombre o apellidos
                string query = "SELECT Id, CódigoMedico, NumeroColegiado, Nombre, Apellidos, CorreoElectronico, Telefono, Direccion FROM Medicos WHERE CódigoMedico LIKE '%" + codigoMedico + "%' OR NumeroColegiado LIKE '%" + codigoMedico + "%' OR Nombre LIKE '%" + codigoMedico + "%' OR Apellidos LIKE '%" + codigoMedico + "%'";

                // Crea un adaptador de datos y un DataSet
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);
                DataSet dataSet = new DataSet();

                // Llena el DataSet con los datos obtenidos de la consulta
                adapter.Fill(dataSet, "Medicos");

                // Asigna los datos al DataGridView
                dataGridView1.DataSource = dataSet.Tables["Medicos"];

                // Cierra la conexión a la base de datos
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar los Médicos: " + ex.Message);
            }
        }
    }
}
