CREATE DATABASE PVC_Stolarija;
GO

USE PVC_Stolarija;
GO

-- Nezavisna tabela: Korisnik (login)
CREATE TABLE Korisnik (
    Id INT IDENTITY PRIMARY KEY,
    KorisnickoIme NVARCHAR(50) NOT NULL UNIQUE,
    Ime NVARCHAR(50) NOT NULL,
    Prezime NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NULL,
    LozinkaHash NVARCHAR(200) NOT NULL,
    LozinkaSalt NVARCHAR(200) NOT NULL,
    Uloga NVARCHAR(30) NOT NULL DEFAULT 'Referent'  -- Referent / Rukovodilac
);
GO

-- Sifarnik: Kupac
CREATE TABLE Kupac (
    IdKupac INT IDENTITY PRIMARY KEY,
    Naziv NVARCHAR(150) NOT NULL,
    Adresa NVARCHAR(200) NULL,
    Telefon NVARCHAR(30) NULL,
    Email NVARCHAR(100) NULL
);
GO

-- Glavna tabela (sustina dokumenta): ProizvodniNalog
CREATE TABLE ProizvodniNalog (
    IdNaloga INT IDENTITY PRIMARY KEY,
    BrojNaloga NVARCHAR(20) NOT NULL UNIQUE,
    IdKupac INT NOT NULL FOREIGN KEY REFERENCES Kupac(IdKupac),
    IdKorisnik INT NOT NULL FOREIGN KEY REFERENCES Korisnik(Id),
    DatumPrijema DATE NOT NULL,
    ZeljeniDatumIzrade DATE NOT NULL,
    PredlozeniDatumIzrade DATE NOT NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Na cekanju',
    UkupnaPovrsinaM2 DECIMAL(8,2) NOT NULL DEFAULT 0
);
GO

-- Detalj (master-detail): StavkaNaloga
CREATE TABLE StavkaNaloga (
    IdStavke INT IDENTITY PRIMARY KEY,
    IdNaloga INT NOT NULL FOREIGN KEY REFERENCES ProizvodniNalog(IdNaloga) ON DELETE CASCADE,
    TipElementa NVARCHAR(30) NOT NULL,       -- Prozor / Balkonska vrata / Ulazna vrata / Roletna
    SirinaMM INT NOT NULL CHECK (SirinaMM > 0),
    VisinaMM INT NOT NULL CHECK (VisinaMM > 0),
    Kolicina INT NOT NULL CHECK (Kolicina > 0),
    BojaProfila NVARCHAR(50) NULL,
    TipStakla NVARCHAR(50) NULL,
    TipOkova NVARCHAR(50) NULL,
    PovrsinaM2 AS (CAST(SirinaMM AS DECIMAL(10,2)) * VisinaMM / 1000000.0 * Kolicina) PERSISTED
);
GO

-- Evidencija dnevnog kapaciteta (koristi poslovna logika)
CREATE TABLE DnevniKapacitet (
    Datum DATE NOT NULL PRIMARY KEY,
    IskoriscenoM2 DECIMAL(8,2) NOT NULL DEFAULT 0,
    BrojNaloga INT NOT NULL DEFAULT 0
);
GO


--2. POCETNI (SEED) PODACI


INSERT INTO Kupac (Naziv, Adresa, Telefon, Email) VALUES
('Petrovic Marko', 'Zrenjanin, Kralja Aleksandra 12', '063/111-222', 'marko.petrovic@email.com'),
('Stankovic Ana',  'Novi Sad, Bulevar oslobodjenja 45', '064/222-333', 'ana.stankovic@email.com'),
('Firma Gradnja d.o.o.', 'Zrenjanin, Industrijska zona bb', '023/555-000', 'office@gradnja.rs');
('Uros Pualic', 'Indjija, Licka 36', '069/207-736', 'uros.pualic@gmail.com');
GO

-- Podrazumevana lozinka (za potrebe testiranja): "sifra123"
-- LozinkaSalt i LozinkaHash su ilustrativni - u realnoj primeni se generisu iz FunkcijeLozinke klase
INSERT INTO Korisnik (KorisnickoIme, Ime, Prezime, Email, LozinkaHash, LozinkaSalt, Uloga) VALUES
('referent1', 'Jovana', 'Jovic', 'jovana.jovic@firma.rs', 'PLACEHOLDER_HASH', 'PLACEHOLDER_SALT', 'Referent'),
('rukovodilac1', 'Nikola', 'Nikolic', 'nikola.nikolic@firma.rs', 'PLACEHOLDER_HASH', 'PLACEHOLDER_SALT', 'Rukovodilac');
GO


--3. USKLADISTENE PROCEDURE (STORED PROCEDURES)
--Koriste se u SP varijanti repozitorijuma (KorisnikSPRepo)
 

-- Dodavanje novog korisnika
CREATE OR ALTER PROCEDURE spDodajKorisnika
    @KorisnickoIme NVARCHAR(50),
    @Ime NVARCHAR(50),
    @Prezime NVARCHAR(50),
    @Email NVARCHAR(100),
    @LozinkaHash NVARCHAR(200),
    @LozinkaSalt NVARCHAR(200),
    @Uloga NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Korisnik (KorisnickoIme, Ime, Prezime, Email, LozinkaHash, LozinkaSalt, Uloga)
    VALUES (@KorisnickoIme, @Ime, @Prezime, @Email, @LozinkaHash, @LozinkaSalt, @Uloga);

    SELECT SCOPE_IDENTITY() AS NoviId;
END
GO

-- Dohvatanje korisnika po korisnickom imenu (za login)
CREATE OR ALTER PROCEDURE spDajKorisnikPoKorisnickomImenu
    @KorisnickoIme NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, KorisnickoIme, Ime, Prezime, Email, LozinkaHash, LozinkaSalt, Uloga
    FROM Korisnik
    WHERE KorisnickoIme = @KorisnickoIme;
END
GO

-- Dohvatanje korisnika po Id
CREATE OR ALTER PROCEDURE spDajKorisnikaPoId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, KorisnickoIme, Ime, Prezime, Email, LozinkaHash, LozinkaSalt, Uloga
    FROM Korisnik
    WHERE Id = @Id;
END
GO

-- Spisak svih korisnika
CREATE OR ALTER PROCEDURE spDajSveKorisnike
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, KorisnickoIme, Ime, Prezime, Email, LozinkaHash, LozinkaSalt, Uloga
    FROM Korisnik
    ORDER BY Prezime, Ime;
END
GO

-- Izmena korisnika
CREATE OR ALTER PROCEDURE spIzmeniKorisnika
    @Id INT,
    @Ime NVARCHAR(50),
    @Prezime NVARCHAR(50),
    @Email NVARCHAR(100),
    @Uloga NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Korisnik
       SET Ime = @Ime, Prezime = @Prezime, Email = @Email, Uloga = @Uloga
     WHERE Id = @Id;
END
GO

-- Brisanje korisnika
CREATE OR ALTER PROCEDURE spObrisiKorisnika
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Korisnik WHERE Id = @Id;
END
GO

PRINT 'Baza podataka PVC_Stolarija je uspesno kreirana.';
