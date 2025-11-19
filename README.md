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
