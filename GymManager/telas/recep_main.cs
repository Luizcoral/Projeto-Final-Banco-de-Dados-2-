using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GymManager
{
    public partial class recep_main : UserControl
    {
        public recep_main()
        {
            InitializeComponent();
            // Carrega as listas assim que a tela abre
            CarregarCombosIniciais();
        }

        private void CarregarCombosIniciais()
        {
            CarregarUnidades(); // Aba 1
            CarregarAlunos();   // Aba 2
            CarregarPlanos();   // Aba 2
        }

        // =============================================================
        // MÉTODOS AUXILIARES DE CARREGAMENTO (Comboboxes)
        // =============================================================
        private void CarregarUnidades()
        {
            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT UnidadeID, NomeUnidade FROM UNIDADES WHERE Status='A'", cn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    comboBox3.DataSource = dt; // Unidade (Aba Aluno)
                    comboBox3.DisplayMember = "NomeUnidade";
                    comboBox3.ValueMember = "UnidadeID";
                    comboBox3.SelectedIndex = -1;
                }
            }
            catch { }
        }

        private void CarregarAlunos()
        {
            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    // Traz Nome e CPF para facilitar a identificação
                    string sql = "SELECT AlunoID, Nome + ' (' + CPF + ')' as NomeCompleto FROM ALUNOS";
                    SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    comboBox1.DataSource = dt; // Aluno (Aba Assinatura)
                    comboBox1.DisplayMember = "NomeCompleto";
                    comboBox1.ValueMember = "AlunoID";
                    comboBox1.SelectedIndex = -1;
                }
            }
            catch { }
        }

        private void CarregarPlanos()
        {
            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT PlanoID, NomePlano FROM PLANOS", cn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    comboBox2.DataSource = dt; // Plano (Aba Assinatura)
                    comboBox2.DisplayMember = "NomePlano";
                    comboBox2.ValueMember = "PlanoID";
                    comboBox2.SelectedIndex = -1;
                }
            }
            catch { }
        }

        // =============================================================
        // ABA 1: CADASTRAR ALUNO
        // =============================================================
        private void btnCadastrarAluno_Click(object sender, EventArgs e)
        {
            // 1. Validação da Data de Nascimento (pois é um TextBox)
            DateTime dataNasc;
            if (!DateTime.TryParse(textBox4.Text, out dataNasc))
            {
                MessageBox.Show("Data de Nascimento inválida. Use o formato DD/MM/AAAA");
                return;
            }

            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_CadastrarAluno", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Mapeando os TextBoxes do seu Design
                        cmd.Parameters.AddWithValue("@Nome", textBox1.Text);
                        cmd.Parameters.AddWithValue("@CPF", textBox2.Text);
                        cmd.Parameters.AddWithValue("@Email", textBox3.Text);
                        cmd.Parameters.AddWithValue("@DataNascimento", dataNasc.ToShortDateString());

                        if (comboBox3.SelectedValue == null) { MessageBox.Show("Selecione uma Unidade"); return; }
                        cmd.Parameters.AddWithValue("@UnidadeID", comboBox3.SelectedValue);

                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Aluno cadastrado!");

                // Limpar campos
                textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear();

                // Recarrega a lista de alunos na outra aba para aparecer o novo cadastro
                CarregarAlunos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        // =============================================================
        // ABA 2: ASSINATURA & GERAÇÃO DE PAGAMENTOS (A Lógica Complexa)
        // =============================================================
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }

        private void btnCadastrarAssinatura_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null || comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Selecione o Aluno e o Plano.");
                return;
            }

            int alunoId = Convert.ToInt32(comboBox1.SelectedValue);
            int planoId = Convert.ToInt32(comboBox2.SelectedValue);
            DateTime dataInicio = dateTimePicker1.Value;

            using (SqlConnection cn = Banco.ObterConexao())
            {
                cn.Open();
                SqlTransaction transacao = cn.BeginTransaction(); // Inicia Transação Segura

                try
                {
                    // PASSO 1: Buscar detalhes do Plano (Preço e Duração)
                    decimal valorMensal = 0;
                    int duracaoMeses = 0;

                    string sqlPlano = "SELECT ValorMensal, DuracaoMeses FROM PLANOS WHERE PlanoID = @id";
                    using (SqlCommand cmdBusca = new SqlCommand(sqlPlano, cn, transacao))
                    {
                        cmdBusca.Parameters.AddWithValue("@id", planoId);
                        using (SqlDataReader reader = cmdBusca.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                valorMensal = Convert.ToDecimal(reader["ValorMensal"]);
                                duracaoMeses = Convert.ToInt32(reader["DuracaoMeses"]);
                            }
                        }
                    }

                    // Calcula data final
                    DateTime dataFim = dataInicio.AddMonths(duracaoMeses);

                    // PASSO 2: Inserir a Assinatura e pegar o ID gerado
                    string sqlAssinatura = @"
                        INSERT INTO ASSINATURAS (AlunoID_FK, PlanoID_FK, DataInicio, DataVencimento) 
                        VALUES (@aluno, @plano, @inicio, @fim);
                        SELECT SCOPE_IDENTITY();";

                    int novaAssinaturaID;
                    using (SqlCommand cmdAss = new SqlCommand(sqlAssinatura, cn, transacao))
                    {
                        cmdAss.Parameters.AddWithValue("@aluno", alunoId);
                        cmdAss.Parameters.AddWithValue("@plano", planoId);
                        cmdAss.Parameters.AddWithValue("@inicio", dataInicio);
                        cmdAss.Parameters.AddWithValue("@fim", dataFim.ToShortDateString());
                        novaAssinaturaID = Convert.ToInt32(cmdAss.ExecuteScalar());
                    }

                    // PASSO 3: Gerar os Pagamentos (Loop conforme duração)
                    // Se o plano for de 12 meses, gera 12 boletos pendentes
                    string sqlPagamento = "INSERT INTO PAGAMENTOS (AssinaturaID_FK, ValorNominal, DataVencimento, Status) VALUES (@assinatura, @valor, @vencimento, 'P')";

                    for (int i = 0; i < duracaoMeses; i++)
                    {
                        using (SqlCommand cmdPag = new SqlCommand(sqlPagamento, cn, transacao))
                        {
                            // Vencimento é: DataInicio + i meses (ex: Jan, Fev, Mar...)
                            DateTime vencimentoParcela = dataInicio.AddMonths(i + 1); // Primeira parcela vence em 1 mês

                            cmdPag.Parameters.AddWithValue("@assinatura", novaAssinaturaID);
                            cmdPag.Parameters.AddWithValue("@valor", valorMensal);
                            cmdPag.Parameters.AddWithValue("@vencimento", vencimentoParcela.ToShortDateString());
                            cmdPag.ExecuteNonQuery();
                        }
                    }

                    transacao.Commit(); // Confirma todas as gravações
                    MessageBox.Show($"Assinatura criada com sucesso!\nForam gerados {duracaoMeses} pagamentos pendentes.");
                }
                catch (Exception ex)
                {
                    transacao.Rollback(); // Cancela tudo se der erro
                    MessageBox.Show("Erro ao gerar assinatura: " + ex.Message);
                }
            }
        }
    }
}