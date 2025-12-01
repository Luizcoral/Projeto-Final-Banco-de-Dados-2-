using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GymManager
{
    public partial class admin_main : UserControl // Ou Form, dependendo de como você criou
    {
        public admin_main()
        {
            InitializeComponent();
            carregarBeneficiosNoCombo(); // Carrega a lista ao abrir a tela
        }

        // ========================================================
        // ABA 1: BENEFÍCIOS (button1, textBox1, textBox3)
        // ========================================================
        private void btnSalvarBeneficio_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    string sql = "INSERT INTO BENEFICIOS (NomeBeneficio, Descricao) VALUES (@nome, @desc)";

                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@nome", textBox1.Text);
                        cmd.Parameters.AddWithValue("@desc", textBox3.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Benefício cadastrado com sucesso!");
                limparCamposBeneficio();
                carregarBeneficiosNoCombo(); // Atualiza a lista na outra aba
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar benefício: " + ex.Message);
            }
        }

        private void limparCamposBeneficio()
        {
            textBox1.Text = "";
            textBox3.Text = "";
        }

        // ========================================================
        // ABA 2: UNIDADES (button2, textBox7, textBox9, radioButtons)
        // ========================================================
        private void btnSalvarUnidade_Click(object sender, EventArgs e)
        {
            try
            {
                char status = 'A'; // Padrão Ativo
                if (radioButton2.Checked) status = 'I'; // Se "Inativo" estiver marcado

                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    string sql = "INSERT INTO UNIDADES (NomeUnidade, Endereco, Status) VALUES (@nome, @end, @status)";

                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@nome", textBox7.Text);
                        cmd.Parameters.AddWithValue("@end", textBox9.Text);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Unidade cadastrada com sucesso!");
                textBox7.Text = "";
                textBox9.Text = "";
                radioButton1.Checked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar unidade: " + ex.Message);
            }
        }

        // ========================================================
        // ABA 3: PLANOS (button3, textBox4, textBox6, textBox5, comboBox1)
        // ========================================================

        // Método auxiliar para preencher o ComboBox
        private void carregarBeneficiosNoCombo()
        {
            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    string sql = "SELECT BeneficioID, NomeBeneficio FROM BENEFICIOS";
                    SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    comboBox1.DataSource = dt;
                    comboBox1.DisplayMember = "NomeBeneficio"; // O que o usuário vê
                    comboBox1.ValueMember = "BeneficioID";     // O valor real (ID)
                    comboBox1.SelectedIndex = -1; // Começa sem seleção
                }
            }
            catch { } // Silencioso para não travar na inicialização se banco falhar
        }

        private void btnSalvarPlano_Click(object sender, EventArgs e)
        {
            // Validação básica de números
            decimal valorMensal;
            int duracao;

            if (!decimal.TryParse(textBox6.Text, out valorMensal))
            {
                MessageBox.Show("Digite um valor válido para o preço.");
                return;
            }
            if (!int.TryParse(textBox5.Text, out duracao))
            {
                MessageBox.Show("Digite um número inteiro para a duração.");
                return;
            }

            using (SqlConnection cn = Banco.ObterConexao())
            {
                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction(); // Usamos transação pois vamos gravar em 2 tabelas

                try
                {
                    // 1. Inserir o Plano e recuperar o ID gerado (IDENTITY)
                    string sqlPlano = "INSERT INTO PLANOS (NomePlano, ValorMensal, DuracaoMeses) VALUES (@nome, @valor, @duracao); SELECT SCOPE_IDENTITY();";

                    int novoPlanoID;

                    using (SqlCommand cmd = new SqlCommand(sqlPlano, cn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@nome", textBox4.Text);
                        cmd.Parameters.AddWithValue("@valor", valorMensal);
                        cmd.Parameters.AddWithValue("@duracao", duracao);

                        // Executa e pega o ID criado
                        novoPlanoID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 2. Vincular o Benefício selecionado no ComboBox (Tabela N:N)
                    if (comboBox1.SelectedValue != null)
                    {
                        int beneficioID = Convert.ToInt32(comboBox1.SelectedValue);
                        string sqlVinculo = "INSERT INTO PLANOS_BENEFICIOS (PlanoID_FK, BeneficioID_FK) VALUES (@pid, @bid)";

                        using (SqlCommand cmdVinculo = new SqlCommand(sqlVinculo, cn, transaction))
                        {
                            cmdVinculo.Parameters.AddWithValue("@pid", novoPlanoID);
                            cmdVinculo.Parameters.AddWithValue("@bid", beneficioID);
                            cmdVinculo.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit(); // Confirma tudo
                    MessageBox.Show("Plano cadastrado com sucesso!");

                    textBox4.Text = "";
                    textBox6.Text = "";
                    textBox5.Text = "";
                    comboBox1.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Desfaz se der erro no meio
                    MessageBox.Show("Erro ao salvar plano: " + ex.Message);
                }
            }
        }


    }
}