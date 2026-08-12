using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

using BibliotekaKlasa.TehnoloskeKlase;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.KlasePodatakaEF.KontekstEF;
using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;
using BibliotekaKlasa.KlasePodatakaSP.Repozitorijumi;
using PoslovnaLogika.Klase;

var builder = WebApplication.CreateBuilder(args);

string konekcioniString = builder.Configuration.GetConnectionString("KonekcioniString")
    ?? throw new InvalidOperationException("Connection string 'KonekcioniString' nije definisan u appsettings.json.");

// SLOJ ZA RAD SA PODACIMA - registracija svih 5 repozitorijuma

builder.Services.AddDbContext<AppDbContext>(opcije =>
    opcije.UseSqlServer(konekcioniString));
builder.Services.AddScoped<ProizvodniNalogRepo>();

builder.Services.AddScoped(_ => new KonekcijaKlasa(konekcioniString));

builder.Services.AddScoped<KupacRepo>();

builder.Services.AddScoped<DnevniKapacitetRepo>();

builder.Services.AddScoped(_ => new KorisnikSPRepo(konekcioniString));

// SLOJ POSLOVNE LOGIKE
builder.Services.AddScoped<OgranicenjeKapaciteta>();
builder.Services.AddScoped<ProizvodniNalogLogika>();

// SLOJ SERVISA - kontroleri, JSON, Swagger, CORS
builder.Services.AddControllers()
    .AddJsonOptions(opcije =>
    {
        opcije.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opcije.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opcije =>
{
    opcije.AddPolicy("DozvoliMvcAplikaciju", politika =>
    {
        politika.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DozvoliMvcAplikaciju");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();