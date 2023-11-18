using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemValledeXelaju
{
    public partial class ListadoGenerar1 : Form
    {
        public ListadoGenerar1()
        {
            InitializeComponent();
        }



        private void btn_listado_p_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListarInformacion11 listarInformacion1 = new ListarInformacion11();
            listarInformacion1.ShowDialog();
        }

        private void btn_listado_pl_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListarInfo21 listarinfo2 = new ListarInfo21();
            listarinfo2.ShowDialog();
        }

        private void btn_listado_diag_p_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListaInfo31 listaInfo3 = new ListaInfo31();
            listaInfo3.ShowDialog();
        }

        private void btn_listado_diag_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListadoInfo41 listaInfo3 = new ListadoInfo41();
            listaInfo3.ShowDialog();
        }

        private void btn_listado_med_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListadoInfo51 listadoInfo5 = new ListadoInfo51();
            listadoInfo5.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            MainForm2 mainForm = new MainForm2();
            mainForm.ShowDialog();
        }
    }
}
