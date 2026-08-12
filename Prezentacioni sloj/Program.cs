using BibliotekaKlasa.KlasePodatakaSP.Repozitorijumi;
using RVS_Aplikacija.Servisi;

var builder = WebApplication.CreateBuilder(args);

string konekcioniString = builder.Configuration.GetConnectionString("KonekcioniString")
    ?? throw new InvalidOperationException("Connection string 'KonekcioniString' nije definisan.");

string restApiBazniUrl = builder.Configuration["RestApi:BaziniUrl"]
    ?? throw new InvalidOperationException("'RestApi:BaziniUrl' nije definisan u appsettings.json.");

// LOGIN - direktan pristup Sloju za rad sa podacima (Način 3 - stored procedure)
builder.Services.AddScoped(_ => new KorisnikSPRepo(konekcioniString));

// KOMUNIKACIJA SA SLOJEM SERVISA (REST API) - preko HttpClient-a
builder.Services.AddHttpClient("RVS_REST_API", klijent =>
{
    klijent.BaseAddress = new Uri(restApiBazniUrl);
});

builder.Services.AddScoped<NaloziApiServis>();
builder.Services.AddScoped<KupacApiServis>();

// SESIJA - čuva podatke o prijavljenom korisniku (login)
// ============================================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opcije =>
{
    opcije.IdleTimeout = TimeSpan.FromMinutes(30);
    opcije.Cookie.HttpOnly = true;
    opcije.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Nalog/Greska");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();      // MORA biti pre UseAuthorization/MapControllerRoute
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Nalog}/{action=Prijava}/{id?}");

app.Run();
