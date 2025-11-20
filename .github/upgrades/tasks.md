# BlazorBlocks .NET 10.0 Upgrade Migration Plan

## Overview

Apply the Big Bang Strategy from Plan §2.1: perform an atomic upgrade of both projects to `net10.0`, update package references to `10.0.0`, restore, build and fix compilation issues, commit the atomic upgrade, then run automated smoke/validation steps. Manual UI/visual verification steps from the plan are intentionally excluded (non-automatable).

**Progress**: 2/4 tasks complete (50%) ![50%](https://progress-bar.xyz/50)

## Tasks

### [✓] TASK-001: Verify prerequisites (SDK) *(Completed: 2025-11-20 21:52)*
**References**: Plan §3.2 Phase 0: Preparation, Plan §8.1 Phase 0: Pre-Flight Validation

- [✓] (1) Run `dotnet --version` and verify it reports `10.0.x` (or a preview 10.0 SDK) (**Verify**)
- [✓] (2) If a `global.json` is present and must pin SDK, update it to 10.0.x per Plan §3.2 (only if plan requires) and verify `dotnet --version` matches (**Verify**)

---

### [✓] TASK-002: Atomic framework and package upgrade (all projects) *(Completed: 2025-11-20 21:54)*
**References**: Plan §2.3 Parallel vs Sequential Execution, Plan §3.1 Phase 1: Atomic Upgrade, Plan §5 Package Update Reference, Plan Appendix A: Project File Changes, Plan §6 Breaking Changes Catalog

- [✓] (1) Update `<TargetFramework>` to `net10.0` in both `BlazorBlocks.csproj` and `BlazorBlocks.Test.WASM.csproj` per Plan Appendix A (apply changes in a single coordinated batch)
- [✓] (2) Update package references to `10.0.0` per Plan §5 (packages: `Microsoft.AspNetCore.Components.Web`, `Microsoft.AspNetCore.Components.WebAssembly`, `Microsoft.AspNetCore.Components.WebAssembly.DevServer`) across the projects in the same atomic change
- [✓] (3) Run `dotnet restore` for the entire solution and verify all packages restore successfully (**Verify**)
- [✓] (4) Build the solution (`dotnet build`) and fix all compilation errors discovered, addressing breaking changes referenced in Plan §6 (one bounded pass: apply fixes, then rebuild)  
- [✓] (5) Solution builds with 0 errors (deterministic verification) (**Verify**)

---

### [▶] TASK-003: Create single atomic commit for Phase 1 changes
**References**: Plan §10.1 Big Bang Strategy Source Control Approach, Plan §10.3 Commit Strategy

- [▶] (1) Commit all Phase 1 changes (project files, package version updates, and any compilation fixes applied in TASK-002) in a single atomic commit with message:
      `chore: upgrade to .NET 10.0 - update all projects and packages`
- [▶] (2) Verify commit exists with the expected message (e.g., `git log -1` shows the commit) (**Verify**)

---

### [ ] TASK-004: Automated validation / smoke tests (build + dev server)
**References**: Plan §8 Testing and Validation Strategy, Plan §8.2 Smoke Tests

- [ ] (1) Run full release build smoke commands:
      `dotnet clean && dotnet restore && dotnet build --configuration Release` for the solution per Plan §8.2
- [ ] (2) Verify the Release build completes with 0 errors (**Verify**)
- [ ] (3) Start the Test.WASM dev server: `cd Test\BlazorBlocks.Test.WASM && dotnet run` and verify the server process starts and does not exit with an error (monitor console output for startup errors) (**Verify**)  
      - Note: Manual browser UI checks are excluded (non-automatable). If automated end-to-end tests exist, run them per Plan §8; otherwise rely on successful startup without runtime errors.
- [ ] (4) If runtime or startup errors are detected, address them per Plan §6 (Breaking Changes Catalog) and Plan §7 (Contingency). After fixes, rebuild and re-run the verification steps above until the deterministic verifications pass (bounded work: apply fixes discovered, then re-run the specific verification steps) (**Verify**)

---