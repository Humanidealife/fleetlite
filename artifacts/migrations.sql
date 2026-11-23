IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261122203043_SchemaInitial'
)
BEGIN
    CREATE TABLE [Vehicules] (
        [Id] uniqueidentifier NOT NULL,
        [Immatriculation] nvarchar(15) NOT NULL,
        [Marque] nvarchar(60) NOT NULL,
        [Modele] nvarchar(60) NOT NULL,
        [DateMiseEnCirculation] date NOT NULL,
        [Motorisation] tinyint NOT NULL,
        CONSTRAINT [PK_Vehicules] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Vehicules_Motorisation] CHECK ([Motorisation] IN (10, 20, 30, 40, 50))
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261122203043_SchemaInitial'
)
BEGIN
    CREATE TABLE [Contrats] (
        [VehiculeId] uniqueidentifier NOT NULL,
        [Loueur] nvarchar(80) NOT NULL,
        [DateDebut] date NOT NULL,
        [DureeMois] smallint NOT NULL,
        [LoyerMensuel] decimal(10,2) NOT NULL,
        [KilometrageContractuel] int NOT NULL,
        [DateFin] AS DATEADD(MONTH, [DureeMois], [DateDebut]) PERSISTED,
        CONSTRAINT [PK_Contrats] PRIMARY KEY ([VehiculeId]),
        CONSTRAINT [CK_Contrats_DureeMois] CHECK ([DureeMois] BETWEEN 1 AND 120),
        CONSTRAINT [CK_Contrats_Loyer] CHECK ([LoyerMensuel] > 0),
        CONSTRAINT [FK_Contrats_Vehicules_VehiculeId] FOREIGN KEY ([VehiculeId]) REFERENCES [Vehicules] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261122203043_SchemaInitial'
)
BEGIN
    CREATE TABLE [Couts] (
        [Id] uniqueidentifier NOT NULL,
        [VehiculeId] uniqueidentifier NOT NULL,
        [DateCout] date NOT NULL,
        [Nature] tinyint NOT NULL,
        [Montant] decimal(12,2) NOT NULL,
        CONSTRAINT [PK_Couts] PRIMARY KEY NONCLUSTERED ([Id]),
        CONSTRAINT [CK_Couts_Montant] CHECK ([Montant] > 0),
        CONSTRAINT [CK_Couts_Nature] CHECK ([Nature] IN (10, 20, 30, 40, 50)),
        CONSTRAINT [FK_Couts_Vehicules_VehiculeId] FOREIGN KEY ([VehiculeId]) REFERENCES [Vehicules] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261122203043_SchemaInitial'
)
BEGIN
    CREATE TABLE [RelevesKilometriques] (
        [Id] uniqueidentifier NOT NULL,
        [VehiculeId] uniqueidentifier NOT NULL,
        [DateReleve] date NOT NULL,
        [Kilometrage] int NOT NULL,
        CONSTRAINT [PK_RelevesKilometriques] PRIMARY KEY NONCLUSTERED ([Id]),
        CONSTRAINT [CK_Releves_Kilometrage] CHECK ([Kilometrage] >= 0),
        CONSTRAINT [FK_RelevesKilometriques_Vehicules_VehiculeId] FOREIGN KEY ([VehiculeId]) REFERENCES [Vehicules] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261122203043_SchemaInitial'
)
BEGIN
    CREATE INDEX [IX_Contrats_DateFin] ON [Contrats] ([DateFin]) INCLUDE ([Loueur], [KilometrageContractuel]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261122203043_SchemaInitial'
)
BEGIN
    CREATE UNIQUE CLUSTERED INDEX [IX_Couts_VehiculeId_DateCout_Id] ON [Couts] ([VehiculeId], [DateCout], [Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261122203043_SchemaInitial'
)
BEGIN
    CREATE UNIQUE CLUSTERED INDEX [IX_RelevesKilometriques_VehiculeId_DateReleve_Id] ON [RelevesKilometriques] ([VehiculeId], [DateReleve] DESC, [Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261122203043_SchemaInitial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vehicules_Immatriculation] ON [Vehicules] ([Immatriculation]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261122203043_SchemaInitial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261122203043_SchemaInitial', N'10.0.15');
END;

COMMIT;
GO

