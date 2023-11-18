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
    public partial class ListadosGenerar : Form
    {
        public ListadosGenerar()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
        }

        private void btn_listado_p_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListarInformacion1 listarInformacion1 = new ListarInformacion1();
            listarInformacion1.ShowDialog();
        }

        private void btn_listado_pl_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListarInfo2 listarinfo2 = new ListarInfo2();
            listarinfo2.ShowDialog();
        }

        private void btn_listado_diag_p_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListaInfo3 listaInfo3 = new ListaInfo3();
            listaInfo3.ShowDialog();
        }

        private void btn_listado_diag_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListadoInfo4 listadoInfo4 = new ListadoInfo4();
            listadoInfo4.ShowDialog();
        }

        private void btn_listado_med_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
            ListadoInfo5 listadoInfo5 = new ListadoInfo5();
            listadoInfo5.ShowDialog();
        }
    }
}
