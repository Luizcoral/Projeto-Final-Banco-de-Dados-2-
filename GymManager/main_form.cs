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


        private void button1_Click(object sender, EventArgs e)
        {
            panel_principal.Controls.Clear();
            panel_principal.Controls.Add(new recep_main());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel_principal.Controls.Clear();
            panel_principal.Controls.Add(new financ_main());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel_principal.Controls.Clear();
            panel_principal.Controls.Add(new admin_main());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel_principal.Controls.Clear();
            panel_principal.Controls.Add(new catraca_main());
        }
    }
}
