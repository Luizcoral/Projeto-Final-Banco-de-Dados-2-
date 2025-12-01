using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager
{
    public class Banco
    {
        public static SqlConnection ObterConexao()
        {
            // Lê a connection string nomeada em App.config (connectionStrings)
            var conn = ConfigurationManager.ConnectionStrings["AcademiaDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(conn))
            {
                // Fallback para uso via Settings (ServerName) caso connectionStrings não esteja configurada
                var server = Properties.Settings.Default.ServerName?.ToString();
                conn = $"Data Source={server};Initial Catalog=AcademiaDB;Integrated Security=True;";
            }

            return new SqlConnection(conn);
        }

        public DataTable ListarUnidades()
        {
            using (var conexao = Banco.ObterConexao())
            {
                conexao.Open();
                // Trazemos apenas as unidades Ativas ('A') para visualização comum
                string sql = "SELECT UnidadeID, NomeUnidade, Endereco FROM UNIDADES WHERE Status = 'A'"; // 

                using (var cmd = new SqlCommand(sql, conexao))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        public void SalvarUnidade(string nome, string endereco)
        {
            using (var conexao = Banco.ObterConexao())
            {
                conexao.Open();
                // O Status tem DEFAULT 'A', então não precisamos enviar se for uma nova unidade 
                string sql = "INSERT INTO UNIDADES (NomeUnidade, Endereco) VALUES (@nome, @end)";

                using (var cmd = new SqlCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@end", endereco);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public DataTable ListarPlanos()
        {
            using (var conexao = Banco.ObterConexao())
            {
                conexao.Open();
                // Seleciona colunas para exibir ao usuário
                string sql = "SELECT PlanoID, NomePlano, ValorMensal, DuracaoMeses FROM PLANOS"; // 

                using (var adapter = new SqlDataAdapter(sql, conexao))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
        public void SalvarPlano(string nome, decimal valor, int meses)
        {
            using (var conexao = Banco.ObterConexao())
            {
                conexao.Open();
                string sql = "INSERT INTO PLANOS (NomePlano, ValorMensal, DuracaoMeses) VALUES (@nome, @valor, @meses)"; // [cite: 12]

                using (var cmd = new SqlCommand(sql, conexao))
                {
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@valor", valor);
                    cmd.Parameters.AddWithValue("@meses", meses);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public DataTable ListarBeneficios()
        {
            // Importante para popular CheckboxList na tela de cadastro de planos
            using (var conexao = Banco.ObterConexao())
            {
                conexao.Open();
                string sql = "SELECT BeneficioID, NomeBeneficio FROM BENEFICIOS"; // 
                using (var adapter = new SqlDataAdapter(sql, conexao))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }




}