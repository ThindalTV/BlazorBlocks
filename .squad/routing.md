# Routing Rules

## Signal → Agent

| Signal | Agent | Why |
|--------|-------|-----|
| Architecture, scope, API design, breaking changes | Holden | Lead owns structural decisions |
| Blazor components, `.razor` files, UI layout, CSS | Naomi | Frontend owns component work |
| C# services, data models, NuGet packaging, registration | Amos | Backend owns service layer |
| Tests, edge cases, quality, regression coverage | Drummer | Tester owns quality |
| Session logs, decisions merge, history updates | Scribe | Scribe owns memory |
| GitHub issues, PRs, backlog, CI | Ralph | Ralph monitors work queue |
| Well-scoped issues, implementation tasks | @copilot | Async coding agent |
| "Team" or multi-domain | Fan-out to Holden + relevant agents | Parallel work |

## Domain Map

| Domain | Owner |
|--------|-------|
| `BlazorBlocksEditor.razor` | Naomi |
| `BlazorBlocksModel.cs` | Amos |
| `BlockRegistrationService.cs` | Amos |
| `Blocks/` | Naomi + Amos (shared) |
| `Services/` | Amos |
| `Customization/` | Naomi |
| `Internals/` | Amos |
| `wwwroot/` | Naomi |
| `Tests/` | Drummer |
| `Samples/` | Naomi |
| `.csproj`, NuGet metadata | Amos |
| Architecture decisions | Holden |
