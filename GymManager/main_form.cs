using System;
using System.Windows.Forms;

namespace GymManager
{
    public partial class main_form : Form
    {
        public main_form()
        {
            InitializeComponent();
            button1_Click(null,null); // Abre a tela de Recepção por padrão ao iniciar
        }

        // Método auxiliar que gerencia o "Fechamento" e a "Abertura"
        private void Navegar(UserControl novaTela)
        {
            // 1. FECHAR A TELA ATUAL (Se houver alguma)
            // Verifica se existe algum controle dentro do painel
            if (panel_principal.Controls.Count > 0)
            {
                // Pega o controle atual
                Control telaAtual = panel_principal.Controls[0];

                // Remove do painel visualmente
                panel_principal.Controls.Clear();

                // DESTRÓI o controle na memória (Isso garante que ela foi "fechada" de verdade)
                telaAtual.Dispose();
            }

            // 2. ABRIR A NOVA TELA
            // Define que a nova tela vai preencher todo o espaço
            novaTela.Dock = DockStyle.Fill;

            // Adiciona a nova tela ao painel
            panel_principal.Controls.Add(novaTela);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Cria uma NOVA instância da Recepção (zera os campos)
            Navegar(new recep_main());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Cria uma NOVA instância do Financeiro
            Navegar(new financ_main());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Cria uma NOVA instância do Admin
            Navegar(new admin_main());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Cria uma NOVA instância da Catraca
            Navegar(new catraca_main());
        }
    }
}