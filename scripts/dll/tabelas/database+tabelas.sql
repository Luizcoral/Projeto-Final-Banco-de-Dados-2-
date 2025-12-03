
----------------------------------------------------
-- 0. CRIAÇÃO DO BANCO
----------------------------------------------------
IF DB_ID('AcademiaDB') IS NOT NULL
    DROP DATABASE AcademiaDB;
GO

CREATE DATABASE AcademiaDB;
GO
USE AcademiaDB;
GO

/* =============================================================
   1. CRIAÇÃO DAS TABELAS
   ============================================================= */

----------------------------------------------------
-- TABELA: UNIDADES
----------------------------------------------------
CREATE TABLE UNIDADES (
    UnidadeID INT IDENTITY(1,1) PRIMARY KEY,
    NomeUnidade VARCHAR(100) NOT NULL,
    Endereco VARCHAR(150),
    Status CHAR(1) DEFAULT 'A'
);

----------------------------------------------------
-- TABELA: ALUNOS
----------------------------------------------------
CREATE TABLE ALUNOS (
    AlunoID INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(100),
    CPF VARCHAR(11) UNIQUE,
    Email VARCHAR(100),
    DataNascimento DATE,
    UnidadeID_FK INT,
    FOREIGN KEY (UnidadeID_FK) REFERENCES UNIDADES(UnidadeID)
);

----------------------------------------------------
-- TABELA: PLANOS
----------------------------------------------------
CREATE TABLE PLANOS (
    PlanoID INT IDENTITY(1,1) PRIMARY KEY,
    NomePlano VARCHAR(50),
    ValorMensal DECIMAL(10,2),
    DuracaoMeses INT
);

----------------------------------------------------
-- TABELA: BENEFICIOS
----------------------------------------------------
CREATE TABLE BENEFICIOS (
    BeneficioID INT IDENTITY(1,1) PRIMARY KEY,
    NomeBeneficio VARCHAR(100),
    Descricao VARCHAR(200)
);

----------------------------------------------------
-- TABELA: PLANOS_BENEFICIOS (N:N)
----------------------------------------------------
CREATE TABLE PLANOS_BENEFICIOS (
    PlanoID_FK INT,
    BeneficioID_FK INT,
    PRIMARY KEY (PlanoID_FK, BeneficioID_FK),
    FOREIGN KEY (PlanoID_FK) REFERENCES PLANOS(PlanoID),
    FOREIGN KEY (BeneficioID_FK) REFERENCES BENEFICIOS(BeneficioID)
);

----------------------------------------------------
-- TABELA: ASSINATURAS
----------------------------------------------------
CREATE TABLE ASSINATURAS (
    AssinaturaID INT IDENTITY(1,1) PRIMARY KEY,
    AlunoID_FK INT,
    PlanoID_FK INT,
    DataInicio DATE,
    DataVencimento DATE,
    FOREIGN KEY (AlunoID_FK) REFERENCES ALUNOS(AlunoID),
    FOREIGN KEY (PlanoID_FK) REFERENCES PLANOS(PlanoID)
);

----------------------------------------------------
-- TABELA: PAGAMENTOS
----------------------------------------------------
CREATE TABLE PAGAMENTOS (
    PagamentoID INT IDENTITY(1,1) PRIMARY KEY,
    AssinaturaID_FK INT,
    ValorNominal DECIMAL(10,2),
    DataVencimento DATE,
    Status CHAR(1), -- P=Pago, G=Em atraso
    FOREIGN KEY (AssinaturaID_FK) REFERENCES ASSINATURAS(AssinaturaID)
);

----------------------------------------------------
-- TABELA: ACESSOS_CATRACA
----------------------------------------------------
CREATE TABLE ACESSOS_CATRACA (
    AcessoID INT IDENTITY(1,1) PRIMARY KEY,
    AlunoID_FK INT,
    UnidadeID_FK INT,
    DataHoraAcesso DATETIME DEFAULT GETDATE(),
    TipoAcesso CHAR(1), -- E=Entrada, S=Saída
    FOREIGN KEY (AlunoID_FK) REFERENCES ALUNOS(AlunoID),
    FOREIGN KEY (UnidadeID_FK) REFERENCES UNIDADES(UnidadeID)
);

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PAGAMENTOS' AND COLUMN_NAME = 'DataPagamento')
BEGIN
    ALTER TABLE PAGAMENTOS
    ADD DataPagamento DATETIME NULL;
END