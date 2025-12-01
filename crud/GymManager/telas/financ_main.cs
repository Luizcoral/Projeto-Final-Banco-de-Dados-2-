using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GymManager
{
    public partial class financ_main : UserControl
    {
        public financ_main()
        {
            InitializeComponent();

            listView1.FullRowSelect = true;
            listView1.MultiSelect = false;

        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            CarregarFinanceiro(textBox3.Text);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CarregarFinanceiro(textBox3.Text);
        }

        // --- MÉTODO CORRIGIDO (SEM DataPagamento) ---
        private void CarregarFinanceiro(string termoBusca)
        {
            listView1.Items.Clear();

            if (string.IsNullOrWhiteSpace(termoBusca)) return;

            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();

                    // REMOVI 'P.DataPagamento' DO SELECT
                    string sql = @"
                        SELECT P.PagamentoID, P.DataVencimento, P.ValorNominal, P.Status, AL.Nome
                        FROM PAGAMENTOS P
                        JOIN ASSINATURAS A ON P.AssinaturaID_FK = A.AssinaturaID
                        JOIN ALUNOS AL ON A.AlunoID_FK = AL.AlunoID
                        WHERE AL.Nome LIKE @nome
                        ORDER BY AL.Nome, P.DataVencimento";

                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@nome", "%" + termoBusca + "%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int idPagamento = reader.GetInt32(0);
                                DateTime vencimento = reader.GetDateTime(1);
                                decimal valor = reader.GetDecimal(2);
                                string statusSigla = reader.GetString(3);
                                string nomeAluno = reader.GetString(4);

                                // Agora a lógica de "Pago" depende apenas do Status ser 'C'
                                bool estaPago = (statusSigla == "C");

                                string statusTexto = TraduzirStatus(statusSigla);

                                // Monta a linha
                                ListViewItem item = new ListViewItem(vencimento.ToShortDateString());
                                item.SubItems.Add("R$ " + valor.ToString("F2"));
                                item.SubItems.Add($"{statusTexto} - {nomeAluno}");
                                item.Tag = idPagamento;

                                // Cores
                                if (estaPago)
                                {
                                    item.ForeColor = Color.Green;
                                }
                                else if (statusSigla == "G" || (statusSigla == "P" && vencimento < DateTime.Today))
                                {
                                    item.ForeColor = Color.Red;
                                    item.Text += " (!)";
                                }
                                else
                                {
                                    item.ForeColor = Color.Black;
                                }

                                listView1.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro: " + ex.Message);
            }
        }

        // --- BOTÃO RECEBER CORRIGIDO ---
        private void btnReceber_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecione uma conta na lista.");
                return;
            }

            ListViewItem item = listView1.SelectedItems[0];

            if (item.ForeColor == Color.Green)
            {
                MessageBox.Show("Esta conta já está paga.");
                return;
            }

            if (MessageBox.Show("Confirmar recebimento?", "Financeiro", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int idPagamento = (int)item.Tag;
                    using (SqlConnection cn = Banco.ObterConexao())
                    {
                        cn.Open();
                        // REMOVI O UPDATE DA DATA, AGORA SÓ MUDA O STATUS PARA 'C'
                        string sql = "UPDATE PAGAMENTOS SET Status = 'C' WHERE PagamentoID = @id";
                        using (SqlCommand cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.AddWithValue("@id", idPagamento);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Recebido!");
                    CarregarFinanceiro(textBox3.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private string TraduzirStatus(string sigla)
        {
            if (sigla == "C") return "PAGO";
            if (sigla == "P") return "Pendente";
            if (sigla == "G") return "ATRASADO";
            return sigla;
        }
    }
}