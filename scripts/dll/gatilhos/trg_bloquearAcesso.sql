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