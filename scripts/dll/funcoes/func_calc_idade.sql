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