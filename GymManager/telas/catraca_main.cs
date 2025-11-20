using System;
using System.Data.SqlClient;
using System.Drawing; // Para usar cores (Color.Red/Green)
using System.Windows.Forms;

namespace GymManager
{
    public partial class catraca_main : UserControl
    {
        public catraca_main()
        {
            InitializeComponent();
        }

        // =========================================================
        // BOTÃO ENTRADA (Tipo 'E')
        // =========================================================
        private void btnEntrada_Click(object sender, EventArgs e)
        {
            RegistrarAcesso('E');
        }

        // =========================================================
        // BOTÃO SAÍDA (Tipo 'S')
        // =========================================================
        private void btnSaida_Click(object sender, EventArgs e)
        {
            RegistrarAcesso('S');
        }

        // =========================================================
        // LÓGICA CENTRAL DA CATRACA
        // =========================================================
        private void RegistrarAcesso(char tipo)
        {
            // 1. Validação do Input
            int alunoId;
            if (!int.TryParse(textBox1.Text, out alunoId))
            {
                MessageBox.Show("Digite um Código de Aluno válido (número).");
                return;
            }

            using (SqlConnection cn = Banco.ObterConexao())
            {
                cn.Open();

                try
                {
                    // PASSO 1: Verificar se o aluno existe e pegar dados básicos
                    // (Também verificamos se a assinatura está vencida por data)
                    string sqlVerificacao = @"
                        SELECT A.Nome, A.UnidadeID_FK, 
                               ISNULL(MAX(ASS.DataVencimento), '1900-01-01') as VencimentoAssinatura
                        FROM ALUNOS A
                        LEFT JOIN ASSINATURAS ASS ON A.AlunoID = ASS.AlunoID_FK
                        WHERE A.AlunoID = @id
                        GROUP BY A.Nome, A.UnidadeID_FK";

                    string nomeAluno = "";
                    int unidadeId = 0;
                    DateTime dataVencimento = DateTime.MinValue;

                    using (SqlCommand cmdBusca = new SqlCommand(sqlVerificacao, cn))
                    {
                        cmdBusca.Parameters.AddWithValue("@id", alunoId);
                        using (SqlDataReader reader = cmdBusca.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nomeAluno = reader["Nome"].ToString();
                                unidadeId = Convert.ToInt32(reader["UnidadeID_FK"]);
                                dataVencimento = Convert.ToDateTime(reader["VencimentoAssinatura"]);
                            }
                            else
                            {
                                MessageBox.Show("Aluno não encontrado!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    // PASSO 2: Validação de Data (Regra do C#)
                    if (dataVencimento < DateTime.Today)
                    {
                        MessageBox.Show($"BLOQUEADO!\nAluno: {nomeAluno}\nMotivo: Assinatura Vencida em {dataVencimento.ToShortDateString()}",
                            "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // PASSO 3: Tentar inserir o acesso (Aqui o TRIGGER do Banco entra em ação)
                    // Se tiver pagamento atrasado, o comando abaixo vai falhar e cair no 'catch'
                    string sqlInsert = "INSERT INTO ACESSOS_CATRACA (AlunoID_FK, UnidadeID_FK, TipoAcesso) VALUES (@aluno, @unidade, @tipo)";

                    using (SqlCommand cmdInsert = new SqlCommand(sqlInsert, cn))
                    {
                        cmdInsert.Parameters.AddWithValue("@aluno", alunoId);
                        cmdInsert.Parameters.AddWithValue("@unidade", unidadeId);
                        cmdInsert.Parameters.AddWithValue("@tipo", tipo);
                        cmdInsert.ExecuteNonQuery();
                    }

                    // Se chegou aqui, deu tudo certo
                    string msgTipo = (tipo == 'E') ? "ENTRADA LIBERADA" : "SAÍDA REGISTRADA";
                    MessageBox.Show($"{msgTipo}\nBem-vindo(a), {nomeAluno}!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBox1.Text = ""; // Limpa para o próximo
                    textBox1.Focus();
                }
                catch (SqlException ex)
                {
                    // O código 50000 é o erro genérico gerado pelo RAISERROR do SQL Server
                    // O Trigger que você criou retorna a mensagem "Acesso bloqueado: pagamento em atraso."
                    if (ex.Number == 50000 || ex.Class > 10)
                    {
                        MessageBox.Show($"BLOQUEADO PELO SISTEMA!\nMotivo: {ex.Message}", "Financeiro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {
                        MessageBox.Show("Erro técnico: " + ex.Message);
                    }
                }
            }
        }
    }
}