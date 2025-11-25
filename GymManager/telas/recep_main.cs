using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GymManager
{
    public partial class recep_main : UserControl
    {
        // Variável para guardar o ID do aluno selecionado na Aba 3 (Detalhes)
        private int _alunoIdEdicao = 0;

        public recep_main()
        {
            InitializeComponent();
            ConfigurarListasTab3();
            CarregarDadosIniciais();

            // --- VINCULAÇÃO DE EVENTOS MANUAIS ---

            // Aba 1 (Cadastro)
            this.btnCadastrarAluno.Click += new EventHandler(this.btnCadastrarAluno_Click);

            // Aba 2 (Assinatura)
            this.btnCadastrarAssinatura.Click += new EventHandler(this.btnCadastrarAssinatura_Click);

            // Aba 3 (Detalhes/Edição)
            this.textBox5.TextChanged += (s, e) => CarregarListaAlunosTab3(textBox5.Text);
            this.listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            this.btnSalvar.Click += new EventHandler(this.btnSalvar_Click);

            // Evento ao trocar de aba (para atualizar as listas automaticamente)
            this.tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
        }

        private void ConfigurarListasTab3()
        {
            // Lista Lateral (Busca)
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Clear();
            listView1.Columns.Add("Nome", 180);

            // Lista de Pagamentos
            listViewPagamentos.View = View.Details;
            listViewPagamentos.FullRowSelect = true;
            listViewPagamentos.GridLines = true;
            listViewPagamentos.Columns.Clear();
            listViewPagamentos.Columns.Add("Vencimento", 80);
            listViewPagamentos.Columns.Add("Valor", 70);
            listViewPagamentos.Columns.Add("Status", 80);
            listViewPagamentos.Columns.Add("Data Pagto", 80);
        }

        private void CarregarDadosIniciais()
        {
            CarregarUnidades(comboBox3);       // Aba 1
            CarregarUnidades(cboUnidadeEditar); // Aba 3
            CarregarPlanos();                  // Aba 2
            CarregarAlunosCombo();             // Aba 2
            CarregarListaAlunosTab3("");       // Aba 3 (Lista lateral)
        }

        // Atualiza listas quando o usuário troca de aba
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage2) CarregarAlunosCombo();
            if (tabControl1.SelectedTab == tabPage3) CarregarListaAlunosTab3(textBox5.Text);
        }

        // =============================================================
        // MÉTODOS AUXILIARES DE CARREGAMENTO (Combos)
        // =============================================================
        private void CarregarUnidades(ComboBox combo)
        {
            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT UnidadeID, NomeUnidade FROM UNIDADES WHERE Status='A'", cn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    combo.DataSource = dt;
                    combo.DisplayMember = "NomeUnidade";
                    combo.ValueMember = "UnidadeID";
                    combo.SelectedIndex = -1;
                }
            }
            catch { }
        }

        private void CarregarAlunosCombo()
        {
            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    string sql = "SELECT AlunoID, Nome FROM ALUNOS ORDER BY Nome";
                    SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    comboBox1.DataSource = dt; // Combo da Aba 2
                    comboBox1.DisplayMember = "Nome";
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
                    comboBox2.DataSource = dt;
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
            // Validações com MaskedTextBox
            string cpfLimpo = textBox2.Text.Replace(",", "").Replace("-", "").Trim(); // Remove formatação
            if (!DateTime.TryParse(textBox4.Text, out DateTime dataNasc))
            {
                MessageBox.Show("Data de Nascimento inválida.");
                return;
            }
            if (comboBox3.SelectedValue == null)
            {
                MessageBox.Show("Selecione uma Unidade.");
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
                        cmd.Parameters.AddWithValue("@Nome", textBox1.Text);
                        cmd.Parameters.AddWithValue("@CPF", cpfLimpo);
                        cmd.Parameters.AddWithValue("@Email", textBox3.Text);
                        cmd.Parameters.AddWithValue("@DataNascimento", dataNasc);
                        cmd.Parameters.AddWithValue("@UnidadeID", comboBox3.SelectedValue);

                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Aluno cadastrado!");
                // Limpar campos
                textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear(); comboBox3.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        // =============================================================
        // ABA 2: ASSINATURA & GERAÇÃO DE PAGAMENTOS
        // =============================================================
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { } // Placeholder

        private void btnCadastrarAssinatura_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null || comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Selecione Aluno e Plano.");
                return;
            }

            int alunoId = Convert.ToInt32(comboBox1.SelectedValue);
            int planoId = Convert.ToInt32(comboBox2.SelectedValue);
            DateTime dataInicio = dateTimePicker1.Value;

            using (SqlConnection cn = Banco.ObterConexao())
            {
                cn.Open();
                SqlTransaction transacao = cn.BeginTransaction();

                try
                {
                    // 1. Detalhes do Plano
                    decimal valorMensal = 0;
                    int duracaoMeses = 0;
                    using (SqlCommand cmd = new SqlCommand("SELECT ValorMensal, DuracaoMeses FROM PLANOS WHERE PlanoID = @id", cn, transacao))
                    {
                        cmd.Parameters.AddWithValue("@id", planoId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                valorMensal = Convert.ToDecimal(reader["ValorMensal"]);
                                duracaoMeses = Convert.ToInt32(reader["DuracaoMeses"]);
                            }
                        }
                    }

                    DateTime dataFim = dataInicio.AddMonths(duracaoMeses);

                    // 2. Criar Assinatura
                    string sqlAss = "INSERT INTO ASSINATURAS (AlunoID_FK, PlanoID_FK, DataInicio, DataVencimento) VALUES (@aluno, @plano, @inicio, @fim); SELECT SCOPE_IDENTITY();";
                    int assID;
                    using (SqlCommand cmd = new SqlCommand(sqlAss, cn, transacao))
                    {
                        cmd.Parameters.AddWithValue("@aluno", alunoId);
                        cmd.Parameters.AddWithValue("@plano", planoId);
                        cmd.Parameters.AddWithValue("@inicio", dataInicio);
                        cmd.Parameters.AddWithValue("@fim", dataFim);
                        assID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 3. Gerar Pagamentos
                    string sqlPag = "INSERT INTO PAGAMENTOS (AssinaturaID_FK, ValorNominal, DataVencimento, Status) VALUES (@ass, @valor, @venc, 'P')";
                    for (int i = 0; i < duracaoMeses; i++)
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlPag, cn, transacao))
                        {
                            cmd.Parameters.AddWithValue("@ass", assID);
                            cmd.Parameters.AddWithValue("@valor", valorMensal);
                            cmd.Parameters.AddWithValue("@venc", dataInicio.AddMonths(i + 1));
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transacao.Commit();
                    MessageBox.Show("Assinatura realizada com sucesso!");
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        // =============================================================
        // ABA 3: DETALHES E EDIÇÃO
        // =============================================================
        private void CarregarListaAlunosTab3(string busca)
        {
            listView1.Items.Clear();
            using (SqlConnection cn = Banco.ObterConexao())
            {
                cn.Open();
                string sql = "SELECT AlunoID, Nome FROM ALUNOS WHERE Nome LIKE @nome ORDER BY Nome";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@nome", "%" + busca + "%");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListViewItem item = new ListViewItem(reader["Nome"].ToString());
                            item.Tag = reader["AlunoID"];
                            listView1.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            _alunoIdEdicao = (int)listView1.SelectedItems[0].Tag;
            PreencherDetalhes(_alunoIdEdicao);
        }

        private void PreencherDetalhes(int id)
        {
            using (SqlConnection cn = Banco.ObterConexao())
            {
                cn.Open();

                // 1. Dados Pessoais
                string sqlPessoal = "SELECT Nome, CPF, Email, UnidadeID_FK FROM ALUNOS WHERE AlunoID = @id";
                using (SqlCommand cmd = new SqlCommand(sqlPessoal, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            txtNome.Text = r["Nome"].ToString();
                            txtCPF.Text = r["CPF"].ToString();
                            txtEmail.Text = r["Email"].ToString();
                            if (r["UnidadeID_FK"] != DBNull.Value) cboUnidadeEditar.SelectedValue = r["UnidadeID_FK"];
                        }
                    }
                }

                // 2. Plano
                string sqlPlano = @"SELECT TOP 1 P.NomePlano, A.DataVencimento 
                                    FROM ASSINATURAS A JOIN PLANOS P ON A.PlanoID_FK = P.PlanoID 
                                    WHERE A.AlunoID_FK = @id ORDER BY A.DataVencimento DESC";
                using (SqlCommand cmd = new SqlCommand(sqlPlano, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            lblPlano.Text = "Plano: " + r["NomePlano"].ToString();
                            DateTime vcto = Convert.ToDateTime(r["DataVencimento"]);
                            lblVencimentoContrato.Text = "Vence: " + vcto.ToShortDateString();

                            if (vcto < DateTime.Today)
                            {
                                lblStatusContrato.Text = "VENCIDO";
                                lblStatusContrato.ForeColor = Color.Red;
                            }
                            else
                            {
                                lblStatusContrato.Text = "ATIVO";
                                lblStatusContrato.ForeColor = Color.Green;
                            }
                        }
                        else
                        {
                            lblPlano.Text = "Sem Plano";
                            lblVencimentoContrato.Text = "---";
                            lblStatusContrato.Text = "";
                        }
                    }
                }

                // 3. Financeiro
                listViewPagamentos.Items.Clear();
                // SQL adaptado para considerar se tem coluna DataPagamento ou não (vamos assumir que você criou)
                // Se não criou a coluna ainda, remova "P.DataPagamento" do SELECT abaixo
                string sqlFin = @"SELECT P.DataVencimento, P.ValorNominal, P.Status 
                                  FROM PAGAMENTOS P 
                                  JOIN ASSINATURAS A ON P.AssinaturaID_FK = A.AssinaturaID 
                                  WHERE A.AlunoID_FK = @id ORDER BY P.DataVencimento DESC";

                using (SqlCommand cmd = new SqlCommand(sqlFin, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            DateTime vcto = Convert.ToDateTime(r["DataVencimento"]);
                            decimal valor = Convert.ToDecimal(r["ValorNominal"]);
                            string status = r["Status"].ToString();

                            ListViewItem item = new ListViewItem(vcto.ToShortDateString());
                            item.SubItems.Add("R$ " + valor.ToString("F2"));

                            string descStatus = status == "C" ? "Pago" : (status == "G" ? "Atrasado" : "Pendente");
                            item.SubItems.Add(descStatus);
                            // Se tiver DataPagamento, adicione aqui o SubItem 4

                            if (status == "C") item.ForeColor = Color.Green;
                            else if (status == "G" || (status == "P" && vcto < DateTime.Today)) item.ForeColor = Color.Red;

                            listViewPagamentos.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (_alunoIdEdicao == 0) return;
            try
            {
                using (SqlConnection cn = Banco.ObterConexao())
                {
                    cn.Open();
                    string sql = "UPDATE ALUNOS SET Nome=@n, CPF=@c, Email=@e, UnidadeID_FK=@u WHERE AlunoID=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@n", txtNome.Text);
                        cmd.Parameters.AddWithValue("@c", txtCPF.Text); // Envia formatado ou limpo, depende do banco
                        cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@u", cboUnidadeEditar.SelectedValue);
                        cmd.Parameters.AddWithValue("@id", _alunoIdEdicao);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Dados atualizados!");
                CarregarListaAlunosTab3(textBox5.Text); // Atualiza lista
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }
}