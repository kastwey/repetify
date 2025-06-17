use RepetifyDb;
BEGIN TRANSACTION;

DECLARE @UserId UNIQUEIDENTIFIER;
DECLARE @DeckId UNIQUEIDENTIFIER;

-- Obtener el primer usuario (puedes adaptar el criterio si lo necesitas)
SELECT TOP 1 @UserId = Id FROM Users;

-- Si no hay usuario, salimos
IF @UserId IS NULL
BEGIN
    PRINT 'No hay usuarios en la tabla Users.';
    ROLLBACK TRANSACTION;
    RETURN;
END

-- Comprobar si ya existe el mazo
IF NOT EXISTS (SELECT 1 FROM Decks WHERE Name = 'Negocios inglés' AND UserId = @UserId)
BEGIN
    SET @DeckId = NEWID();

    INSERT INTO Decks (Id, Name, Description, UserId, OriginalLanguage, TranslatedLanguage)
    VALUES (@DeckId, 'Negocios inglés', 'Vocabulario de negocios en inglés', @UserId, 'Español', 'Inglés');

    DECLARE @NextReviewDate DATETIME2 = DATEADD(DAY, 1, GETUTCDATE());
    DECLARE @PreviousCorrectReview DATETIME2 = CAST('0001-01-01T00:00:00.0000000' AS datetime2);

    -- Insertar tarjetas
    INSERT INTO Cards (Id, DeckId, OriginalWord, TranslatedWord, CorrectReviewStreak, NextReviewDate, PreviousCorrectReview)
    VALUES
    (NEWID(), @DeckId, 'Empresa', 'Company', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Negocio', 'Business', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Contrato', 'Contract', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Cliente', 'Client', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Proveedor', 'Supplier', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Factura', 'Invoice', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Presupuesto', 'Budget', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Inversión', 'Investment', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Acción', 'Share', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Socio', 'Partner', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Capital', 'Capital', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Mercado', 'Market', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Ventas', 'Sales', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Marketing', 'Marketing', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Publicidad', 'Advertising', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Campaña', 'Campaign', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Estrategia', 'Strategy', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Reunión', 'Meeting', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Proyecto', 'Project', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Informe', 'Report', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Asesor', 'Consultant', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Auditoría', 'Audit', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Balance', 'Balance Sheet', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Contabilidad', 'Accounting', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Interés', 'Interest', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Préstamo', 'Loan', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Impuesto', 'Tax', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Gasto', 'Expense', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Ganancia', 'Profit', 0, @NextReviewDate, @PreviousCorrectReview),
    (NEWID(), @DeckId, 'Pérdida', 'Loss', 0, @NextReviewDate, @PreviousCorrectReview);
END
ELSE
BEGIN
    PRINT 'El mazo "Negocios inglés" ya existe para este usuario.';
END

COMMIT TRANSACTION;
