/* =============================================================
   PROJETO ACADEMIA – SCRIPT COMPLETO
   Contém: DDL + DML + FUNCTION + TRIGGER + STORED PROCEDURE + INDICES
   ============================================================= */
   drop table ACESSOS_CATRACA
   drop table PLANOS_BENEFICIOS
   drop table BENEFICIOS
   drop table PAGAMENTOS
   drop table ASSINATURAS
   drop table PLANOS
   drop table ALUNOS
   drop table UNIDADES
   
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
/* =============================================================
   2. POPULAÇÃO DO BANCO (INSERTS)
   ============================================================= */

----------------------------------------------------
-- 1. UNIDADES
----------------------------------------------------
INSERT INTO UNIDADES (NomeUnidade, Endereco, Status) VALUES
('Unidade Centro', 'Rua Central, 100', 'A'),
('Unidade Norte', 'Avenida Norte, 200', 'A'),
('Unidade Sul', 'Rua Sul, 300', 'A'),
('Unidade Leste', 'Rua Leste, 400', 'A'),
('Unidade Oeste', 'Rua Oeste, 500', 'A'),
('Unidade Premium', 'Avenida Luxo, 600', 'A'),
('Unidade Fit', 'Rua Acadêmica, 700', 'A'),
('Unidade Strong', 'Rua Musculação, 800', 'A'),
('Unidade Power', 'Avenida Esportes, 900', 'A'),
('Unidade Prime', 'Rua Prime, 1000', 'A');

----------------------------------------------------
-- 2. ALUNOS
----------------------------------------------------
INSERT INTO ALUNOS (Nome, CPF, Email, DataNascimento, UnidadeID_FK) VALUES
('João Silva',  '12345678901', 'joao@email.com', '2000-05-10', 1),
('Maria Costa', '23456789012', 'maria@email.com', '1999-08-21', 1),
('Pedro Souza','34567890123', 'pedro@email.com', '1998-03-11', 2),
('Ana Lima',   '45678901234', 'ana@email.com', '2001-01-15', 2),
('Rafael Dias', '56789012345','rafael@email.com','2002-06-01', 3),
('Camila Rocha','67890123456','camila@email.com','1997-07-27', 3),
('Lucas Alves', '78901234567','lucas@email.com','1995-04-02', 4),
('Beatriz Melo','89012345678','bia@email.com','1996-12-19', 5),
('Julia Nunes', '90123456789','julia@email.com','2000-10-09', 6),
('Carlos Peres','01234567890','carlos@email.com','1999-02-14', 7),
('Roberto Carlos', '11122233399', 'rc@email.com', '1980-04-19', 1),
('Erasmo Carlos',  '22233344488', 'erasmo@email.com', '1982-06-05', 1),
('Wanderléa Silva','33344455577', 'wand@email.com', '1985-11-02', 2),
('Tim Maia',       '44455566666', 'tim@email.com', '1975-09-28', 2),
('Rita Lee',       '55566677755', 'rita@email.com', '1978-12-31', 3),
('Cazuza Araújo',  '66677788844', 'cazuza@email.com', '1988-04-04', 3),
('Renato Russo',   '77788899933', 'renato@email.com', '1990-03-27', 4),
('Cassia Eller',   '88899900022', 'cassia@email.com', '1992-12-10', 4),
('Chorao Santos',  '99900011111', 'chorao@email.com', '1995-04-09', 5),
('Elis Regina',    '00011122200', 'elis@email.com', '1979-03-17', 5),
('Ney Matogrosso', '12121212199', 'ney@email.com', '1976-08-01', 6),
('Gal Costa',      '23232323288', 'gal@email.com', '1980-09-26', 6),
('Maria Bethania', '34343434377', 'beth@email.com', '1978-06-18', 7),
('Caetano Veloso', '45454545466', 'cae@email.com', '1977-08-07', 7),
('Gilberto Gil',   '56565656555', 'gil@email.com', '1977-06-26', 8),
('Djavan Silva',   '67676767644', 'djavan@email.com', '1981-01-27', 8),
('Marisa Monte',   '78787878733', 'marisa@email.com', '1987-07-01', 9),
('Ivete Sangalo',  '89898989822', 'ivete@email.com', '1982-05-27', 9),
('Claudia Leitte', '90909090911', 'claudia@email.com', '1985-07-10', 10),
('Lulu Santos',    '01010101000', 'lulu@email.com', '1983-05-04', 10);

----------------------------------------------------
-- 3. PLANOS
----------------------------------------------------
INSERT INTO PLANOS (NomePlano, ValorMensal, DuracaoMeses) VALUES
('Básico', 99.90, 1),
('Intermediário', 149.90, 1),
('Premium', 199.90, 1),
('Musculação', 120.00, 3),
('CrossFit', 220.00, 1),
('Funcional', 130.00, 1),
('Bike Indoor', 110.00, 1),
('Natação', 160.00, 1),
('Boxe', 140.00, 1),
('Completo', 299.90, 1);

----------------------------------------------------
-- 4. BENEFICIOS
----------------------------------------------------
INSERT INTO BENEFICIOS (NomeBeneficio, Descricao) VALUES
('Acesso Total', 'Acesso a todas as áreas da academia'),
('Personal Trainer', 'Sessões com personal'),
('Nutricionista', 'Consultas mensais'),
('Aulas Coletivas', 'Participação ilimitada'),
('Piscina', 'Acesso à piscina'),
('Sauna', 'Acesso à sauna'),
('Estacionamento', 'Vaga exclusiva'),
('Massagem', 'Sessão mensal'),
('CrossZone', 'Área de crossfit'),
('Café Fit', 'Café saudável à vontade');

----------------------------------------------------
-- 5. PLANOS_BENEFICIOS
----------------------------------------------------
INSERT INTO PLANOS_BENEFICIOS VALUES
(1,1),(1,4),
(2,1),(2,4),(2,7),
(3,1),(3,2),(3,3),(3,4),(3,5),
(10,1),(10,2),(10,3),(10,4),(10,5),(10,6),(10,7),(10,8);

----------------------------------------------------
-- 6. ASSINATURAS
----------------------------------------------------
INSERT INTO ASSINATURAS (AlunoID_FK, PlanoID_FK, DataInicio, DataVencimento) VALUES
(1,1,'2025-01-01','2025-02-01'),
(2,2,'2025-01-02','2025-02-02'),
(3,3,'2025-01-03','2025-02-03'),
(4,4,'2025-01-04','2025-04-04'),
(5,5,'2025-01-05','2025-02-05'),
(6,6,'2025-01-06','2025-02-06'),
(7,7,'2025-01-07','2025-02-07'),
(8,8,'2025-01-08','2025-02-08'),
(9,9,'2025-01-09','2025-02-09'),
(10,10,'2025-01-10','2025-02-10'),
(11, 4, '2024-01-01', '2025-12-31'), -- Ativo (Longo prazo)
(12, 1, '2025-01-01', '2025-02-01'), -- Vencido
(13, 2, '2025-02-01', '2026-02-01'), -- Ativo
(14, 3, '2025-01-15', '2026-01-15'), -- Ativo
(15, 4, '2024-06-01', '2025-06-01'), -- Ativo
(16, 5, '2025-03-01', '2025-04-01'), -- Futuro/Ativo
(17, 10, '2025-01-01', '2025-12-31'), -- Ativo (Plano Caro)
(18, 1, '2025-10-01', '2025-11-01'), -- Futuro
(19, 2, '2025-02-01', '2026-02-01'), -- Ativo
(20, 3, '2025-02-01', '2026-02-01'), -- Ativo
(21, 1, '2024-01-01', '2024-02-01'), -- Vencido (Antigo)
(22, 1, '2024-01-01', '2024-02-01'), -- Vencido (Antigo)
(23, 6, '2025-01-01', '2025-02-01'), -- Vencido
(24, 7, '2025-05-01', '2025-06-01'), -- Ativo
(25, 8, '2025-01-01', '2026-01-01'), -- Ativo
(26, 9, '2025-02-01', '2025-03-01'), -- Vencido
(27, 2, '2025-01-01', '2026-01-01'), -- Ativo
(28, 3, '2025-01-01', '2026-01-01'), -- Ativo
(29, 4, '2025-01-01', '2026-01-01'), -- Ativo
(30, 5, '2025-01-01', '2025-02-01'); -- Vencido

----------------------------------------------------
-- 7. PAGAMENTOS
----------------------------------------------------
INSERT INTO PAGAMENTOS (AssinaturaID_FK, ValorNominal, DataVencimento, Status) VALUES
(1, 99.90, '2025-02-01', 'P'),
(2, 149.90, '2025-02-02', 'P'),
(3, 199.90, '2025-02-03', 'G'),
(4, 120.00, '2025-04-04', 'G'),
(5, 220.00, '2025-02-05', 'P'),
(6, 130.00, '2025-02-06', 'P'),
(7, 110.00, '2025-02-07', 'G'),
(8, 160.00, '2025-02-08', 'P'),
(9, 140.00, '2025-02-09', 'G'),
(10,299.90, '2025-02-10','P'),
(11, 120.00, '2025-01-01', 'C'),
(11, 120.00, '2025-02-01', 'C'),
(13, 149.90, '2025-02-01', 'C'),
(14, 199.90, '2025-01-15', 'C'),
(17, 299.90, '2025-01-01', 'C'),
(25, 160.00, '2025-01-01', 'C'),
(27, 149.90, '2025-01-01', 'C'),
(11, 120.00, '2025-03-01', 'P'),
(13, 149.90, '2025-03-01', 'P'),
(14, 199.90, '2025-02-15', 'P'),
(16, 220.00, '2025-03-01', 'P'),
(24, 110.00, '2025-05-01', 'P'),
(28, 199.90, '2025-02-01', 'P'),
(12, 99.90,  '2025-01-01', 'G'),
(12, 99.90,  '2025-02-01', 'G'),
(21, 99.90,  '2024-02-01', 'G'), -- Dívida antiga
(26, 140.00, '2025-02-01', 'G'),
(30, 220.00, '2025-01-01', 'G');
go

SET DATEFORMAT ymd;
----------------------------------------------------
-- 8. ACESSOS_CATRACA
----------------------------------------------------
INSERT INTO ACESSOS_CATRACA (AlunoID_FK, UnidadeID_FK, DataHoraAcesso, TipoAcesso) VALUES
-- Manhã
(1, 1, '2025-11-20 06:10:00', 'E'), (3, 2, '2025-11-20 06:15:00', 'E'),
(5, 3, '2025-11-20 06:20:00', 'E'), (7, 4, '2025-11-20 06:30:00', 'E'),
(11, 1, '2025-11-20 07:05:00', 'E'), (13, 2, '2025-11-20 07:10:00', 'E'),
(15, 3, '2025-11-20 07:15:00', 'E'), (17, 4, '2025-11-20 07:20:00', 'E'),
(25, 8, '2025-11-20 08:00:00', 'E'), (27, 9, '2025-11-20 08:30:00', 'E'),

-- Almoço
(2, 1, '2025-11-20 12:10:00', 'E'), (4, 2, '2025-11-20 12:15:00', 'E'),
(6, 3, '2025-11-20 12:30:00', 'E'), (14, 7, '2025-11-20 12:45:00', 'E'),

-- Noite
(8, 5, '2025-11-20 18:05:00', 'E'), (9, 5, '2025-11-20 18:10:00', 'E'),
(10, 6, '2025-11-20 18:15:00', 'E'), 
(14, 7, '2025-11-20 18:30:00', 'E'), (16, 7, '2025-11-20 18:40:00', 'E'),
(18, 8, '2025-11-20 19:00:00', 'E'), (19, 8, '2025-11-20 19:05:00', 'E'),
(20, 9, '2025-11-20 19:10:00', 'E'), (1, 1, '2025-11-20 19:15:00', 'E'),
(11, 1, '2025-11-20 19:20:00', 'E'), (13, 2, '2025-11-20 19:25:00', 'E'),
(15, 3, '2025-11-20 19:30:00', 'E'), (17, 4, '2025-11-20 19:35:00', 'E'),
(23, 6, '2025-11-20 19:40:00', 'E'), (24, 7, '2025-11-20 19:50:00', 'E'),
(28, 9, '2025-11-20 20:00:00', 'E'), (29, 10,'2025-11-20 20:10:00', 'E');

/* =============================================================
   3. CRIAÇÃO DOS ÍNDICES
   ============================================================= */

CREATE INDEX idx_alunos_unidade ON ALUNOS(UnidadeID_FK);
CREATE INDEX idx_assinaturas_aluno ON ASSINATURAS(AlunoID_FK);
CREATE INDEX idx_pagamentos_assinatura ON PAGAMENTOS(AssinaturaID_FK);
CREATE INDEX idx_catraca_aluno ON ACESSOS_CATRACA(AlunoID_FK);


/* =============================================================
   4. FUNCTION (Validar idade mínima)
   ============================================================= */

CREATE FUNCTION fn_CalcularIdade(@DataNasc DATE)
RETURNS INT
AS
BEGIN
    RETURN DATEDIFF(YEAR, @DataNasc, GETDATE());
END;
GO


/* =============================================================
   5. TRIGGER (Bloqueia alunos com pagamento atrasado)
   ============================================================= */

CREATE TRIGGER trg_BloquearAcesso
ON ACESSOS_CATRACA
AFTER INSERT
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM PAGAMENTOS P
        JOIN ASSINATURAS A ON A.AssinaturaID = P.AssinaturaID_FK
        JOIN INSERTED I ON I.AlunoID_FK = A.AlunoID_FK
        WHERE P.Status = 'G'
    )
    BEGIN
        RAISERROR ('Acesso bloqueado: pagamento em atraso.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO


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


/* =============================================================
   7. SELECTS DE TESTE
   ============================================================= */

-- Mostrar todos os alunos com suas unidades
SELECT A.Nome, U.NomeUnidade
FROM ALUNOS A
JOIN UNIDADES U ON A.UnidadeID_FK = U.UnidadeID;

-- Ver assinaturas completas
SELECT * FROM ASSINATURAS;

-- Ver pagamentos em atraso
SELECT * FROM PAGAMENTOS WHERE Status = 'G';

-- Ver benefícios por plano
SELECT P.NomePlano, B.NomeBeneficio
FROM PLANOS_BENEFICIOS PB
JOIN PLANOS P ON PB.PlanoID_FK = P.PlanoID
JOIN BENEFICIOS B ON PB.BeneficioID_FK = B.BeneficioID;

-- Testar function
SELECT dbo.fn_CalcularIdade('2000-05-10') AS Idade;

-- Testar procedure
-- EXEC sp_CadastrarAluno 'Novo Cliente', '11122233344', 'novo@email.com', '2002-01-01', 1;

