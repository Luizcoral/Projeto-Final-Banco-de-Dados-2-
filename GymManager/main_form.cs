using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GymManager
{
    public partial class main_form : Form
    {
        public main_form()
        {
            InitializeComponent();
        }

        private void btn_alunos_Click(object sender, EventArgs e)
        {
            panel_principal.Controls.Clear();
            panel_principal.Controls.Add(new alunos_main());
            //panel_principal.Dock = DockStyle.Fill;
        }
    }
}
