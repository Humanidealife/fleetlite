using FleetLite.Api.Donnees;
using FleetLite.Api.Endpoints;
using FleetLite.Domaine;

var builder = WebApplication.CreateBuilder(args);

// ───────────────── Services (avant Build) ─────────────────
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

const string PolitiqueFront = "front";
builder.Services.AddCors(options =>
    options.AddPolicy(PolitiqueFront, politique => politique
        .WithOrigins("http://localhost:4200")   // le serveur de développement Angular plus tard
        .AllowAnyHeader()
        .AllowAnyMethod()));

// Horloge : figée si la configuration fournit une date, horloge système sinon.
builder.Services.AddSingleton<IHorloge>(_ =>
    DateOnly.TryParseExact(
        builder.Configuration["FleetLite:DateDeReference"], "yyyy-MM-dd", out DateOnly date)
        ? new HorlogeFigee(date)
        : new HorlogeSysteme());

builder.Services.AddSingleton<IDepotFlotte, DepotFlotteMemoire>();
builder.Services.AddSingleton<CalculTco>();
builder.Services.AddSingleton<MoteurAlertes>();

var app = builder.Build();

// ───────────────── Pipeline (après Build) ─────────────────
app.UseExceptionHandler();     // le plus externe : voit les exceptions de tout le reste
app.UseStatusCodePages();      // donne un corps ProblemDetails aux erreurs sans corps

if (app.Environment.IsDevelopment())
    app.MapOpenApi();          // document sur /openapi/v1.json

app.UseHttpsRedirection();
app.UseCors(PolitiqueFront);   // AVANT toute authentification

// ───────────────── Points d'entrée ─────────────────
app.MapHealthChecks("/sante");

app.MapGroup("/api/v1")
   .WithTags("Flotte")
   .MapVehicules();

app.Run();