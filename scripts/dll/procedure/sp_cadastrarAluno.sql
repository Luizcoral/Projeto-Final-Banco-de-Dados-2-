/* =============================================================
   6. STORED PROCEDURE (Cadastrar novo aluno)
   ============================================================= */

CREATE PROCEDURE sp_CadastrarAluno
    @Nome VARCHAR(100),
    @CPF VARCHAR(11),
    @Email VARCHAR(100),
    @DataNascimento DATE,
    @UnidadeID INT
AS
BEGIN
    INSERT INTO ALUNOS (Nome, CPF, Email, DataNascimento, UnidadeID_FK)
    VALUES (@Nome, @CPF, @Email, @DataNascimento, @UnidadeID);
END;
GO