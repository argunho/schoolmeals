# SchoolMeals - En app för matsedlar i skolan
## Syfte med applikationen: 
Netmine vill utvärdera Blazor WebAssembly. Ett steg i utvärderingen är ett uppdrag att bygga en testapplikation som hanterar CRUD-funktionalitet. De har inga önskemål om domän eller utformning i övrigt och vill därför få uppslag på en tillämpning. 

Det förslag på domän och tillämpning är en applikation för att sammanställa skolors matsedlar. I ett första steg finns enbart ett begränsat antal matsalar, exempelvis för en mindre kommun, samt endast två typer av användare: Besökare och administratörer.

Landningssidan kan vara en samlad vy av alla skolor och aktuell veckas matsedel. Det ska därifrån gå att navigera till enskilda matsalar och se matsedlar för valda veckor.

Det finns möjlighet att logga in som administratör och då ska även funktionalitet för att skapa och editera matsedlar finnas. Det vore önskvärt om administratörer kan skapa maträtter som sedan kan kombineras ihop till en matsedel.

**Johans önskemål:** Någon form av funktionalitet för bilduppladdning.

Det finns stora möjligheter till vidareutveckling efter projektets första fas. Fler roller och kontotyper. Rekommendationer baserade på kostråd för personal. Kommentarer och betygsystem samt hantering av önskemål. Öppna api för integrering.

## Solution-struktur
ScoolMeals.App -  Blazor WebAssembly
SchoolMeals.Api - .NET Core WebApi
SchoolMeals.Shared - .NET Core Classlib
