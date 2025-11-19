📦 Sistema de Gerenciamento de Academia – Banco de Dados

Este repositório contém o script completo de criação e povoamento de um banco de dados para um sistema de academia, incluindo:

Cadastro de unidades

Alunos

Planos

Benefícios

Assinaturas

Pagamentos

Controle de acesso por catraca

Funções

Trigger de bloqueio

Procedures

Índices


O objetivo é demonstrar uma modelagem completa em SQL Server, seguindo boas práticas e regras reais de negócio.

🏗️ Modelagem do Banco de Dados

O banco é organizado em módulos:

🧍 Alunos e Unidades

Cada aluno pertence a uma unidade

Cada unidade pode ter vários alunos

📝 Planos e Benefícios

Planos possuem vários benefícios

Benefícios se relacionam com vários planos (N:N)

💳 Assinaturas e Pagamentos

Assinatura liga um aluno a um plano

Pagamentos ligados a cada assinatura

🚪 Acessos da Catraca

Registra entrada/saída

Trigger valida situação financeira antes do acesso

📐 Modelo Entidade-Relacionamento (ER) — Resumo Visual
UNIDADES 1---N ALUNOS
ALUNOS 1---N ASSINATURAS
PLANOS 1---N ASSINATURAS
ASSINATURAS 1---N PAGAMENTOS

PLANOS N---N BENEFICIOS  (PLANOS_BENEFICIOS)

ALUNOS 1---N ACESSOS_CATRACA
UNIDADES 1---N ACESSOS_CATRACA

⚙️ Automação
🔹 Function

fn_CalcularIdade
→ Calcula idade do aluno.

🔹 Trigger

trg_BloquearAcesso
→ Impede acesso caso o aluno tenha pagamentos em atraso.

🔹 Procedure

sp_CadastrarAluno
→ Cadastra rapidamente novos alunos.