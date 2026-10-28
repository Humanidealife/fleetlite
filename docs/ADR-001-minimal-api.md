# ADR-001 — Minimal API pour les points d'entrée de FleetLite.Api

- **Statut** : Accepté
- **Date** : AAAA-MM-JJ
- **Décideur** : Prénom Nom

## Contexte

`FleetLite.Api` expose en HTTP le domaine de FleetLite (véhicules, coûts, TCO, alertes)
au front Angular et à des partenaires. ASP.NET Core 10 propose deux styles pour écrire un
point d'entrée, c'est-à-dire une route HTTP et le code qui y répond :

| Style | Principe | Exemple |
|---|---|---|
| Minimal API | La route est reliée directement à une fonction. | `app.MapGet("/vehicules/{id:guid}", LireVehicule);` |
| Contrôleur | Une classe dérivée de `ControllerBase` ; chaque méthode publique est un point d'entrée, routé par un attribut. | `[HttpGet("{id:guid}")] public ActionResult<VehiculeDto> Lire(Guid id)` |

Le style se choisit avant de créer le projet : `dotnet new webapi` produit des minimal API,
`dotnet new webapi --use-controllers` des contrôleurs.

## Décision

Les points d'entrée sont écrits en **minimal API**, selon trois règles :

1. **Une classe statique par ressource** dans `Endpoints/` (par exemple `VehiculesEndpoints`),
   montée sous `app.MapGroup("/api/v1")`. `Program.cs` ne contient aucun gestionnaire.
2. **Chaque gestionnaire est une méthode statique nommée, `internal`, documentée par `///`**,
   qui renvoie des `TypedResults`.
3. **Un contrôleur n'est ajouté que** pour l'un des quatre besoins pour lesquels Microsoft
   conseille d'envisager les contrôleurs : extensibilité de la liaison de modèle
   (`IModelBinder`), validation par `IModelValidator`, *application parts*, OData. Chaque
   ajout fait l'objet d'un nouvel ADR.

## Options écartées

| Option | Raison |
|---|---|
| Contrôleurs uniquement | Microsoft recommande les minimal API pour un nouveau projet, et aucun des quatre besoins de la règle 3 ne concerne FleetLite. |
| Minimal API sans aucune exception | Si l'un de ces quatre besoins apparaissait, il faudrait le couvrir par une solution sur mesure. |

## Justification

- Microsoft : « For new projects, we recommend using Minimal APIs ».
- Les besoins de FleetLite, actuels et prévus, sont tous couverts en minimal API : JSON,
  validation, pagination, réception de lots — et même de fichiers, puisque `IFormFile` s'y
  lie en .NET 10.
- Règle 2 : les `///` d'une lambda ne sont pas repris dans le document OpenAPI ; ceux d'une
  méthode `private` non plus, car le générateur de .NET 10 ne lit que les méthodes `public`,
  `internal` ou `protected internal`.

## Conséquences

- **Positives** : `Program.cs` reste court ; le document OpenAPI reprend les `///` et les
  réponses possibles de chaque gestionnaire.
- **Négatives** : les règles 1 et 2 tiennent par la revue de code, pas par le framework ;
  un projet de tests n'appellera les gestionnaires que si `FleetLite.Api` le déclare dans
  `InternalsVisibleTo`.

## À réexaminer si

- l'un des quatre besoins de la règle 3 apparaît ;
- Microsoft change sa recommandation.

## Sources

- [APIs overview](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/apis?view=aspnetcore-10.0) — Microsoft Learn : recommandation, et besoins pour lesquels envisager les contrôleurs.
- [Parameter binding in Minimal API apps](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-10.0) — Microsoft Learn : liaison de `IFormFile`.
- [What's new in ASP.NET Core 10.0](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0?view=aspnetcore-10.0) — Microsoft Learn : commentaires XML non repris sur les lambdas.
- [AssemblyTypeSymbolsVisitor.cs](https://raw.githubusercontent.com/dotnet/aspnetcore/release/10.0/src/OpenApi/gen/Helpers/AssemblyTypeSymbolsVisitor.cs) — code source d'ASP.NET Core 10