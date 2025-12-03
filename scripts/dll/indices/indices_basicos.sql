/* =============================================================
   3. CRIAÇÃO DOS ÍNDICES
   ============================================================= */

CREATE INDEX idx_alunos_unidade ON ALUNOS(UnidadeID_FK);
CREATE INDEX idx_assinaturas_aluno ON ASSINATURAS(AlunoID_FK);
CREATE INDEX idx_pagamentos_assinatura ON PAGAMENTOS(AssinaturaID_FK);
CREATE INDEX idx_catraca_aluno ON ACESSOS_CATRACA(AlunoID_FK);
