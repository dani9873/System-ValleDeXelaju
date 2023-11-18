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
    public partial class ListaInfo3 : Form
    {
        private Conexion conexion;
        public ListaInfo3()
        {
            conexion = new Conexion();

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close(); // Cierra la ventana actual
            ListadosGenerar listadosGenerar = new ListadosGenerar();
            listadosGenerar.ShowDialog();
        }
        private void ListaInfo3_Load(object sender, EventArgs e)
        {
            try
            {
                // Abre la conexión a la base de datos
                conexion.AbrirConexion();

                // Consulta para obtener los diagnósticos por paciente
                string query = "SELECT dd.Id, d.CódigoDiagnostico, d.Descripcion, dd.Observaciones, p.Nombre, p.Apellidos " +
                               "FROM (DetallesDiagnosticos AS dd " +
                               "INNER JOIN Diagnosticos AS d ON dd.CódigoDiagnostico = d.Id) " +
                               "INNER JOIN Pacientes AS p ON dd.CódigoPaciente = p.Id;";

                // Crea un adaptador de datos y un DataSet
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conexion.con);
                DataSet dataSet = new DataSet();

                // Llena el DataSet con los datos obtenidos de la consulta
                adapter.Fill(dataSet, "DiagnosticosPorPaciente");

                // Asigna los datos al DataGridView
                dataGridView1.DataSource = dataSet.Tables["DiagnosticosPorPaciente"];

                // Cierra la conexión a la base de datos
                conexion.CerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al listar los diagnósticos por paciente: " + ex.Message);
            }
        }
    }
}
