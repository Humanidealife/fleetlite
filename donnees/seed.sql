USE FleetLite;
GO

DELETE FROM dbo.RelevesKilometriques;
DELETE FROM dbo.Couts;
DELETE FROM dbo.Contrats;
DELETE FROM dbo.Vehicules;
GO

DECLARE @v1 UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @v2 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @v3 UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';

INSERT INTO dbo.Vehicules (Id, Immatriculation, Marque, Modele, DateMiseEnCirculation, Motorisation)
VALUES (@v1, N'EF-123-GH', N'Peugeot', N'308',     '2024-03-15', 30),
       (@v2, N'BX-884-KD', N'Renault', N'Clio',    '2022-10-05', 10),
       (@v3, N'CV-207-QM', N'Ford',    N'Transit', '2023-09-10', 20);

INSERT INTO dbo.Contrats (VehiculeId, Loueur, DateDebut, DureeMois, LoyerMensuel, KilometrageContractuel)
VALUES (@v1, N'Loueur Alpha', '2024-04-01', 48, 420.00, 120000),
       (@v2, N'Loueur Beta',  '2022-10-20', 48, 310.00, 100000),
       (@v3, N'Loueur Alpha', '2023-09-25', 36, 545.00,  90000);

INSERT INTO dbo.Couts (Id, VehiculeId, DateCout, Nature, Montant)
VALUES (NEWID(), @v1, '2026-08-12', 10, 3220.45),
       (NEWID(), @v1, '2026-05-20', 20,  890.00),
       (NEWID(), @v1, '2025-11-03', 30,  640.00),
       (NEWID(), @v1, '2026-01-15', 40, 1150.00),
       (NEWID(), @v2, '2026-07-09', 10, 2480.30),
       (NEWID(), @v2, '2026-03-22', 20,  705.00),
       (NEWID(), @v2, '2025-06-14', 30,  480.00),
       (NEWID(), @v2, '2026-02-01', 40,  455.30),
       (NEWID(), @v3, '2026-08-28', 10, 5310.75),
       (NEWID(), @v3, '2026-04-11', 20, 1240.00),
       (NEWID(), @v3, '2025-12-02', 30,  620.00),
       (NEWID(), @v3, '2026-06-30', 50,  309.50);

INSERT INTO dbo.RelevesKilometriques (Id, VehiculeId, DateReleve, Kilometrage)
VALUES (NEWID(), @v1, '2026-02-10', 52000),
       (NEWID(), @v1, '2026-08-31', 78500),
       (NEWID(), @v2, '2026-03-05', 71400),
       (NEWID(), @v2, '2026-08-31', 88000),
       (NEWID(), @v3, '2026-01-20', 74100),
       (NEWID(), @v3, '2026-08-31', 98500);
GO