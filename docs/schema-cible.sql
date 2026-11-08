CREATE TABLE dbo.Vehicules
(
    Id                    UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Vehicules PRIMARY KEY CLUSTERED,
    Immatriculation       NVARCHAR(15)  NOT NULL,
    Marque                NVARCHAR(60)  NOT NULL,
    Modele                NVARCHAR(60)  NOT NULL,
    DateMiseEnCirculation DATE          NOT NULL,
    Motorisation          TINYINT       NOT NULL,
    CONSTRAINT UQ_Vehicules_Immatriculation UNIQUE (Immatriculation),
    -- 0 (NonRenseignee) exclu : une motorisation inconnue n'entre pas en base.
    CONSTRAINT CK_Vehicules_Motorisation CHECK (Motorisation IN (10, 20, 30, 40, 50))
);
GO

CREATE TABLE dbo.Contrats
(
    VehiculeId             UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Contrats PRIMARY KEY CLUSTERED,
    Loueur                 NVARCHAR(80)  NOT NULL,
    DateDebut              DATE          NOT NULL,
    DureeMois              SMALLINT      NOT NULL,
    LoyerMensuel           DECIMAL(10,2) NOT NULL,
    KilometrageContractuel INT           NOT NULL,
    DateFin AS DATEADD(MONTH, DureeMois, DateDebut) PERSISTED,
    CONSTRAINT FK_Contrats_Vehicules FOREIGN KEY (VehiculeId)
        REFERENCES dbo.Vehicules (Id),
    CONSTRAINT CK_Contrats_DureeMois CHECK (DureeMois BETWEEN 1 AND 120),
    CONSTRAINT CK_Contrats_Loyer     CHECK (LoyerMensuel > 0)
);
GO

CREATE INDEX IX_Contrats_DateFin
    ON dbo.Contrats (DateFin)
    INCLUDE (Loueur, KilometrageContractuel);
GO

CREATE TABLE dbo.Couts
(
    Id         UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Couts PRIMARY KEY NONCLUSTERED,
    VehiculeId UNIQUEIDENTIFIER NOT NULL,
    DateCout   DATE             NOT NULL,
    Nature     TINYINT          NOT NULL,
    Montant    DECIMAL(12,2)    NOT NULL,
    CONSTRAINT FK_Couts_Vehicules FOREIGN KEY (VehiculeId)
        REFERENCES dbo.Vehicules (Id),
    CONSTRAINT CK_Couts_Montant CHECK (Montant > 0),
    CONSTRAINT CK_Couts_Nature  CHECK (Nature IN (10, 20, 30, 40, 50))
);
GO

-- L'ordre physique suit le chemin de lecture : « les coûts d'un véhicule, par date ».
-- Id en dernière position rend la clé unique et évite l'uniqueifier de 4 octets.
CREATE UNIQUE CLUSTERED INDEX IX_Couts_Vehicule_Date
    ON dbo.Couts (VehiculeId, DateCout, Id);
GO

CREATE TABLE dbo.RelevesKilometriques
(
    Id          UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Releves PRIMARY KEY NONCLUSTERED,
    VehiculeId  UNIQUEIDENTIFIER NOT NULL,
    DateReleve  DATE             NOT NULL,
    Kilometrage INT              NOT NULL,
    CONSTRAINT FK_Releves_Vehicules FOREIGN KEY (VehiculeId)
        REFERENCES dbo.Vehicules (Id),
    CONSTRAINT CK_Releves_Kilometrage CHECK (Kilometrage >= 0)
);
GO

-- DESC sur la date : le dernier relevé d'un véhicule est en tête de sa plage.
CREATE UNIQUE CLUSTERED INDEX IX_Releves_Vehicule_Date
    ON dbo.RelevesKilometriques (VehiculeId, DateReleve DESC, Id);
GO