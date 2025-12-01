🏋️‍♀️ Modelo Físico – Banco de Dados de Academia

Este projeto apresenta o modelo físico de um banco de dados para gerenciamento de uma academia, desenvolvido com o objetivo de estruturar de forma organizada todas as informações relacionadas a unidades, alunos, planos, benefícios, assinaturas, pagamentos e acessos.
O modelo foi elaborado para garantir integridade referencial, desempenho e clareza no relacionamento entre as entidades do sistema.

A base de dados é composta por oito tabelas principais:
Unidades, Planos, Benefícios, Alunos, Planos_Benefícios, Assinaturas, Pagamentos e Acessos_Catraca.
Cada uma desempenha um papel essencial no controle das operações da academia:

Unidades, Planos e Benefícios armazenam informações básicas sobre as filiais, tipos de planos e vantagens oferecidas.

Alunos registra os dados cadastrais de cada membro e sua unidade principal.

Assinaturas e Pagamentos controlam os contratos firmados entre alunos e academia, além do histórico e status de cada pagamento.

Acessos_Catraca mantém o log de entradas e saídas dos alunos nas unidades.

Planos_Benefícios representa a relação N:N entre planos e benefícios.

Os relacionamentos foram definidos para refletir o funcionamento real de uma academia:
cada aluno pertence a uma unidade, pode possuir várias assinaturas, realizar pagamentos recorrentes e ter seus acessos registrados.
Um mesmo plano pode estar disponível em várias unidades e incluir múltiplos benefícios, garantindo flexibilidade e escalabilidade ao sistema.

A modelagem foi realizada com o uso das seguintes ferramentas:

🧩 Draw.io (diagrams.net) – criação do diagrama físico;

📊 Excel – documentação detalhada das tabelas e atributos;

💾 SQL Server – referência para os tipos de dados, índices e constraints.

Este modelo físico serve como base sólida para a implementação do banco de dados relacional da academia, oferecendo suporte ao desenvolvimento de sistemas de gestão com controle eficiente de cadastros, contratos, finanças e acessos.

## Configuração e execução da interface C# (GymManager)

- **Pré-requisitos:**
	- SQL Server (local ou remoto) com permissões para criar banco de dados.
	- Visual Studio (Windows) com suporte a projetos Windows Forms (.NET Framework 4.8).

- **Criar o banco de dados:**
	1. Abra o arquivo `scripts/query_main.sql` no SQL Server Management Studio (SSMS) ou outro cliente.
	2. Ajuste o nome do servidor se necessário (o script cria o banco `AcademiaDB`).
	3. Execute o script para criar tabelas, procedures e popular os dados de exemplo.

- **Configurar string de conexão:**
	1. Abra `crud/GymManager/App.config` e localize a seção `connectionStrings`.
	2. Atualize `Data Source` para o nome/instância do seu SQL Server (ex: `localhost`, `\\.\\SQLEXPRESS`, ou `MEUSERVIDOR`).
	3. Se você usar autenticação SQL, substitua `Integrated Security=True` por `User ID=seu_user;Password=sua_senha;`.

- **Executar a aplicação:**
	1. Abra `crud/GymManager/GymManager.sln` no Visual Studio.
	2. Compile e execute (F5). A interface de Recepção abre por padrão.

- **Notas:**
	- O arquivo `crud/GymManager/Banco.cs` lê a connection string nomeada `AcademiaDB` em `App.config`. Se ausente, ele utiliza a configuração `ServerName` em `Properties.Settings` como fallback.
	- Scripts SQL adicionais e documentação estão na pasta `scripts/`.
