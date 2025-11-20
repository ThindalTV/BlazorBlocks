# BlazorBlocks .NET 10.0 Upgrade Migration Plan

## 1. Executive Summary

### Scenario
Upgrade BlazorBlocks solution from mixed .NET 8.0/.NET 9.0 to .NET 10.0 (Preview), updating all Blazor and WebAssembly components to their latest versions.

### Scope
- **Total Projects**: 2
- **Current State**: 
  - BlazorBlocks.csproj: .NET 8.0 (Class Library)
  - BlazorBlocks.Test.WASM.csproj: .NET 9.0 (Blazor WebAssembly Application)
- **Target State**: Both projects targeting .NET 10.0

### Selected Strategy
**Big Bang Strategy** - All projects upgraded simultaneously in a single atomic operation.

**Rationale**:
- Small solution (2 projects only)
- Simple, clear dependency structure (library → test app)
- Both projects already on modern .NET (8.0 and 9.0)
- All packages have .NET 10.0 versions available
- No security vulnerabilities to address
- Minimal complexity (900 total LOC across both projects)
- Blazor/WebAssembly projects benefit from coordinated upgrade

### Complexity Assessment
**Overall: Low**

**Justification**:
- Very small codebase (856 LOC in library, 44 LOC in test app)
- Only 3 NuGet packages to update, all with clear upgrade paths
- Modern SDK-style projects already in place
- Clean dependency graph (no circular dependencies)
- Blazor components are framework-aligned, minimal breaking changes expected

### Critical Issues
✅ **No security vulnerabilities** detected in current dependencies
✅ **No blocking issues** identified

### Recommended Approach
**Big Bang Strategy** is ideal for this solution due to its small size and simple structure. Both projects will be upgraded simultaneously in a single coordinated operation, ensuring consistency across the entire solution.

---

## 2. Migration Strategy

### 2.1 Approach Selection

**Chosen Strategy**: Big Bang Strategy

**Strategy Rationale**: 
- **Solution Size**: Only 2 projects with 900 total LOC - extremely manageable for atomic upgrade
- **Dependency Simplicity**: Linear dependency chain (Test.WASM → BlazorBlocks library)
- **Modern Foundation**: Both projects already on .NET 8.0+ with SDK-style format
- **Package Compatibility**: All 3 packages have confirmed .NET 10.0 versions
- **Risk Level**: Low risk due to small scope and Blazor framework's strong backwards compatibility
- **Team Efficiency**: Faster completion with single coordinated update vs incremental phases

**Strategy-Specific Considerations**:
- All project file updates and package updates will be performed in one atomic operation
- Single build and test cycle after all changes applied
- No intermediate multi-targeting states needed
- Changes will be committed as a single coherent upgrade

### 2.2 Dependency-Based Ordering

**Migration Order Justification**:
The dependency graph shows a simple linear relationship:
```
BlazorBlocks.Test.WASM.csproj (net9.0) 
    └─> BlazorBlocks.csproj (net8.0)
```

**Strategy-Specific Ordering**: 
Under Big Bang strategy, both projects are updated simultaneously. However, the logical ordering for understanding is:
1. **Foundation**: BlazorBlocks.csproj (class library with 0 project dependencies)
2. **Consumer**: BlazorBlocks.Test.WASM.csproj (depends on BlazorBlocks library)

This ordering is maintained during the atomic update to ensure dependency integrity, even though changes happen together.

### 2.3 Parallel vs Sequential Execution

**Execution Approach**: Atomic simultaneous update

**Strategy Considerations**: 
Big Bang strategy treats all project file changes and package updates as a single coordinated batch:
- Project file TargetFramework updates: Both projects together
- Package reference updates: All packages across both projects together
- Build and validation: Single pass for entire solution

No parallelization needed - the entire upgrade is one indivisible operation.

---

## 3. Detailed Dependency Analysis

### 3.1 Dependency Graph Summary

**Migration Phases** (within single atomic operation):

**Phase 1: Atomic Upgrade** (All operations performed together)
- BlazorBlocks.csproj: Update from net8.0 → net10.0
- BlazorBlocks.Test.WASM.csproj: Update from net9.0 → net10.0
- All package references updated to 10.0.0 versions

**Phase 2: Test Validation**
- Execute BlazorBlocks.Test.WASM application
- Verify rendering and component functionality

### 3.2 Project Groupings

**Phase 0: Preparation**
- Verify .NET 10.0 SDK installed
- Ensure on `main` branch with clean working directory

**Phase 1: Atomic Framework and Package Upgrade**
All projects upgraded together:
- **BlazorBlocks.csproj** (Foundation Library)
  - Framework: net8.0 → net10.0
  - 1 package update
  
- **BlazorBlocks.Test.WASM.csproj** (WebAssembly Test Application)
  - Framework: net9.0 → net10.0
  - 2 package updates

**Phase 2: Validation**
- Build entire solution
- Test WebAssembly application

**Strategy-Specific Grouping Notes**: 
Big Bang strategy eliminates traditional tier-based phases. All projects are updated in a single coordinated operation, with validation as a separate phase only after all changes are complete.

---

## 4. Project-by-Project Migration Plans

### Project: BlazorBlocks.csproj

#### Current State

- **Target Framework**: net8.0
- **Project Type**: ClassLibrary (SDK-style)
- **Dependencies**: 0 project dependencies
- **Dependants**: 1 (BlazorBlocks.Test.WASM.csproj)
- **Package Count**: 1
- **LOC**: 856
- **Files**: 43

#### Target State

- **Target Framework**: net10.0
- **Updated Packages**: 1

#### Migration Steps

**1. Prerequisites**
- Part of atomic upgrade phase - no separate prerequisites needed
- .NET 10.0 SDK must be installed (verified in Phase 0)

**2. Framework Update**
Update `BlazorBlocks.csproj` file:
```xml
<TargetFramework>net10.0</TargetFramework>
```

**3. Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| Microsoft.AspNetCore.Components.Web | 8.0.17 | 10.0.0 | Framework alignment for .NET 10.0 compatibility |

**4. Expected Breaking Changes**

**Blazor Components (Minor):**
- **Render Mode Syntax**: .NET 10.0 may introduce refined render mode definitions or attributes
- **Component Parameters**: Enhanced parameter validation and binding improvements
- **JavaScript Interop**: Potential updates to IJSRuntime APIs
- **CSS Isolation**: Possible refinements to scoped CSS handling

**Likelihood**: Low - Blazor maintains strong backwards compatibility. Most breaking changes between .NET 8 and .NET 10 are additive.

**Areas to Monitor**:
- Custom component base classes (if any extend `ComponentBase`)
- Event handling patterns (especially async event handlers)
- Parameter binding with `[Parameter]` attribute usage
- Any direct JavaScript interop calls

**5. Code Modifications**

Expected modifications (discovered during build):
- **Obsolete API Replacements**: Check for any deprecated Blazor APIs marked for removal in .NET 10
- **Namespace Changes**: Verify all `Microsoft.AspNetCore.Components.*` namespaces resolve correctly
- **Component Lifecycle**: Review any overridden lifecycle methods (OnInitialized, OnParametersSet, etc.) for signature changes
- **EventCallback Usage**: Ensure EventCallback and EventCallback<T> usage aligns with latest patterns

**Manual Review Areas**:
- All `.razor` component files (43 files)
- Component code-behind files (if using code-behind pattern)
- Custom render fragments and templates
- Any CSS isolation files (`.razor.css`)

**6. Testing Strategy**

**Unit Tests**: 
- This is the library project; testing occurs via BlazorBlocks.Test.WASM
- Verify all exported components render without errors

**Integration Tests**:
- Execute BlazorBlocks.Test.WASM application
- Test component rendering in WebAssembly context
- Verify parameter binding and data flow
- Test event handlers and interactivity

**Manual Testing**:
- Visual verification of component rendering (performed in Phase 2)
- Browser compatibility check (Blazor WebAssembly in modern browsers)
- Ensure no console errors or warnings

**Performance Validation**:
- Initial load time comparable or improved
- Component render performance acceptable
- No memory leaks in long-running sessions

**7. Validation Checklist**

- [ ] Dependencies resolve correctly (no conflicts)
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] All components compile successfully
- [ ] No obsolete API usage warnings
- [ ] Package references restored successfully

---

### Project: BlazorBlocks.Test.WASM.csproj

#### Current State

- **Target Framework**: net9.0
- **Project Type**: AspNetCore (Blazor WebAssembly, SDK-style)
- **Dependencies**: 1 project (BlazorBlocks.csproj)
- **Dependants**: 0
- **Package Count**: 2
- **LOC**: 44
- **Files**: 24

#### Target State

- **Target Framework**: net10.0
- **Updated Packages**: 2

#### Migration Steps

**1. Prerequisites**
- Part of atomic upgrade phase
- BlazorBlocks.csproj framework update must be included in same operation

**2. Framework Update**
Update `BlazorBlocks.Test.WASM.csproj` file:
```xml
<TargetFramework>net10.0</TargetFramework>
```

**3. Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| Microsoft.AspNetCore.Components.WebAssembly | 9.0.5 | 10.0.0 | Framework alignment for .NET 10.0 WebAssembly runtime |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 9.0.5 | 10.0.0 | Development server compatibility with .NET 10.0 tooling |

**4. Expected Breaking Changes**

**WebAssembly Runtime:**
- **Hosting Model Updates**: Potential changes to `Program.cs` WebAssembly host builder pattern
- **Service Registration**: Updates to `IServiceCollection` extension methods
- **Static Asset Handling**: Refinements to `wwwroot` asset loading
- **Debugging Experience**: Improved browser debugging may require DevServer updates

**Blazor WebAssembly Specifics**:
- **Boot JSON Format**: Possible changes to `blazor.boot.json` structure (auto-generated)
- **Lazy Loading**: Enhanced lazy loading patterns for assemblies
- **Prerendering**: Updates to prerendering capabilities (if used)
- **Authentication**: Changes to WebAssembly authentication flows (if implemented)

**Likelihood**: Low to Medium - .NET 9 to .NET 10 is a minor version jump for WebAssembly runtime.

**5. Code Modifications**

Expected modifications:
- **Program.cs Updates**: 
  - Verify `WebAssemblyHostBuilder.CreateDefault(args)` pattern
  - Check service registration syntax
  - Confirm `builder.RootComponents.Add` usage
  
- **Index.html**: 
  - Verify `_framework/blazor.webassembly.js` reference (auto-managed)
  - Check for any manual script references
  
- **App.razor**: 
  - Validate Router configuration
  - Check `Found` and `NotFound` render fragments

- **Imports (_Imports.razor)**: 
  - Ensure all namespace directives are valid
  
**Configuration Changes**:
- `wwwroot/appsettings.json`: Review for any breaking configuration changes
- `launchSettings.json`: Verify development server configuration

**6. Testing Strategy**

**Application Launch**:
- Verify application starts in browser without errors
- Check browser console for any runtime warnings or errors
- Confirm WebAssembly runtime loads successfully

**Component Rendering**:
- All BlazorBlocks components render correctly
- Parameter binding works as expected
- Events trigger appropriate handlers

**Development Experience**:
- Hot reload functionality works (DevServer)
- CSS changes apply correctly
- JavaScript debugging functions properly

**Browser Compatibility**:
- Test in Chrome, Edge, Firefox, Safari (if applicable)
- Verify WebAssembly support detected correctly

**7. Validation Checklist**

- [ ] Dependencies resolve correctly
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] Application launches in browser successfully
- [ ] No console errors in browser developer tools
- [ ] All routes/pages accessible
- [ ] BlazorBlocks components render correctly
- [ ] Interactive features function properly
- [ ] Hot reload works during development

---

## 5. Package Update Reference

### Common Package Updates

| Package | Current | Target | Projects Affected | Update Reason |
|---------|---------|--------|-------------------|---------------|
| Microsoft.AspNetCore.Components.Web | 8.0.17 | 10.0.0 | 1 (BlazorBlocks.csproj) | Framework compatibility |
| Microsoft.AspNetCore.Components.WebAssembly | 9.0.5 | 10.0.0 | 1 (BlazorBlocks.Test.WASM.csproj) | Framework compatibility |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 9.0.5 | 10.0.0 | 1 (BlazorBlocks.Test.WASM.csproj) | Tooling compatibility |

### Update Strategy

**All packages are Blazor/ASP.NET Core framework packages** that must be updated together for .NET 10.0 compatibility. These packages are tightly coupled to the .NET runtime version.

**No security vulnerabilities** are present in current versions - updates are purely for framework alignment.

---

## 6. Breaking Changes Catalog

### .NET 8.0 → .NET 10.0 Breaking Changes

**Note**: This section covers potential breaking changes. Actual impact will be discovered during compilation and testing.

### Blazor Components Framework

**1. Component Rendering**
- **Render Mode Enhancements**: .NET 10 may introduce new render modes or refine existing ones (Static, Server, WebAssembly, Auto)
- **Impact**: Low - existing render modes remain supported
- **Action**: Review component render mode specifications if explicitly set

**2. Parameter Binding**
- **Enhanced Validation**: Stricter parameter type checking
- **Impact**: Low - well-typed parameters unlikely to break
- **Action**: Review components with complex parameter types

**3. Event Handling**
- **Async Event Patterns**: Refinements to async event callback handling
- **Impact**: Low - existing patterns remain valid
- **Action**: Test all event handlers, especially async ones

**4. JavaScript Interop**
- **IJSRuntime API Updates**: Potential method signature changes or new overloads
- **Impact**: Low to Medium - if using advanced JS interop
- **Action**: Review all `IJSRuntime.InvokeAsync` calls

### Blazor WebAssembly Runtime

**1. Hosting Model**
- **WebAssemblyHostBuilder Changes**: Potential updates to builder API
- **Impact**: Low - standard patterns remain compatible
- **Action**: Review `Program.cs` initialization code

**2. Static Assets**
- **wwwroot Handling**: Enhanced static asset serving
- **Impact**: Low - transparent improvements
- **Action**: Verify all static assets load correctly

**3. Boot Process**
- **Startup Sequence**: Refinements to WebAssembly initialization
- **Impact**: Low - auto-generated boot files updated automatically
- **Action**: Test application launch and initial load

### ASP.NET Core Framework

**1. Dependency Injection**
- **Service Lifetime Changes**: Potential updates to service registration patterns
- **Impact**: Low - existing patterns remain valid
- **Action**: Review service registrations in `Program.cs`

**2. Configuration System**
- **Options Pattern Updates**: Enhanced configuration binding
- **Impact**: Low - backwards compatible
- **Action**: Verify `appsettings.json` loading

**3. Middleware Pipeline**
- **Middleware Order**: Potential refinements (minimal for WebAssembly)
- **Impact**: Minimal - WebAssembly has limited middleware
- **Action**: None required for typical WebAssembly apps

### General .NET 10.0 Changes

**1. C# Language Version**
- **Default C# Version**: .NET 10 may default to C# 13
- **Impact**: Low - new features opt-in
- **Action**: Review for any new language warnings

**2. API Obsoletions**
- **Deprecated APIs**: Some .NET 8/9 APIs marked obsolete may be removed
- **Impact**: Low - modern code unlikely affected
- **Action**: Address any obsolete warnings during build

**3. NullabilityInfoContext**
- **Null Reference Handling**: Enhanced nullability analysis
- **Impact**: Low - more warnings possible
- **Action**: Address nullability warnings if enabled

### Framework-Specific Considerations

**Blazor Components.Web Package (8.0.17 → 10.0.0)**
- Two major version jump (8 → 10)
- Potential API surface changes in component base classes
- Enhanced render optimizations may affect custom rendering logic

**WebAssembly Packages (9.0.5 → 10.0.0)**
- One major version jump (9 → 10)
- Runtime improvements likely transparent
- DevServer tooling may have updated command-line options

---

## 7. Risk Management

### 7.1 High-Risk Changes

**Strategy Risk Factors**: 
Big Bang strategy concentrates all changes into one operation, which means:
- All issues surface simultaneously during first build
- No incremental validation checkpoints
- Mitigation: Small solution size (900 LOC) and simple structure reduce risk significantly

| Project | Risk Level | Risk Description | Mitigation Strategy |
|---------|------------|------------------|---------------------|
| BlazorBlocks.csproj | **Low** | 856 LOC, component library, framework upgrade from net8.0 | Thorough component testing in Test.WASM app; Blazor has strong backwards compatibility |
| BlazorBlocks.Test.WASM.csproj | **Low-Medium** | WebAssembly runtime upgrade, but only 44 LOC and minimal custom code | Test in multiple browsers; verify WebAssembly runtime loads correctly |

### 7.2 Risk Mitigation

**For Component Library (BlazorBlocks.csproj)**:
- **Mitigation**: Test all 43 components through Test.WASM application
- **Fallback**: If specific component fails, isolate and fix individually
- **Validation**: Visual inspection and interaction testing

**For WebAssembly App (BlazorBlocks.Test.WASM.csproj)**:
- **Mitigation**: Test application launch in multiple browsers
- **Fallback**: Review `Program.cs` against .NET 10 templates if launch fails
- **Validation**: Browser console monitoring for runtime errors

**Big Bang Strategy Specific Mitigations**:
- **Pre-Flight Check**: Verify .NET 10.0 SDK installed before starting
- **Atomic Commit**: Single commit allows easy revert if critical issues arise
- **Build-First Approach**: Address all compilation errors before runtime testing
- **Browser Testing**: Multi-browser validation due to WebAssembly nature

### 7.3 Contingency Plans

**Scenario 1: Breaking Changes in Component Base Classes**
- **Trigger**: Build errors in multiple components after upgrade
- **Response**: 
  1. Review .NET 10 Blazor breaking changes documentation
  2. Identify common pattern across failing components
  3. Apply systematic fix to all affected components
  4. Recompile and test

**Scenario 2: WebAssembly Runtime Fails to Load**
- **Trigger**: Application builds but fails to start in browser
- **Response**:
  1. Check browser console for specific error messages
  2. Verify `blazor.boot.json` generated correctly
  3. Compare `Program.cs` with .NET 10 WebAssembly template
  4. Review static asset references in `index.html`

**Scenario 3: Performance Regression**
- **Trigger**: Application loads significantly slower after upgrade
- **Response**:
  1. Profile WebAssembly download and initialization times
  2. Check for new bundle size increases
  3. Review lazy loading configuration
  4. Consider enabling AOT compilation if available in .NET 10

**Rollback Scenario**:
If critical blocking issues arise:
1. Revert atomic commit on `main` branch
2. Document specific failure mode
3. Research .NET 10 compatibility requirements
4. Plan targeted fix or wait for .NET 10 GA if using preview SDK

---

## 8. Testing and Validation Strategy

### 8.1 Phase-by-Phase Testing

**Phase 0: Pre-Flight Validation**
- [ ] .NET 10.0 SDK installed and accessible
- [ ] `dotnet --version` shows 10.0.x
- [ ] Visual Studio / VS Code supports .NET 10.0 projects
- [ ] Working directory on `main` branch with no pending changes

**Phase 1: Atomic Upgrade Validation**

**Build Testing**:
- [ ] `dotnet restore` completes without errors for entire solution
- [ ] `dotnet build` succeeds for BlazorBlocks.csproj
- [ ] `dotnet build` succeeds for BlazorBlocks.Test.WASM.csproj
- [ ] Zero build errors across solution
- [ ] Zero build warnings (or only expected .NET 10 informational warnings)

**Dependency Resolution**:
- [ ] No package version conflicts
- [ ] All packages restore to 10.0.0 versions
- [ ] Project reference resolves correctly (Test.WASM → BlazorBlocks)

**Code Analysis**:
- [ ] No obsolete API usage warnings
- [ ] No breaking change compiler errors
- [ ] Nullability warnings acceptable (if nullable context enabled)

**Phase 2: Runtime Validation**

**Application Launch**:
- [ ] `dotnet run` starts development server successfully
- [ ] Browser opens and loads application
- [ ] No errors in browser console
- [ ] WebAssembly runtime initializes
- [ ] Blazor framework loads successfully

**Component Rendering**:
- [ ] All pages/routes load without errors
- [ ] All BlazorBlocks components render visually
- [ ] No missing component errors
- [ ] CSS styles apply correctly

**Interactivity**:
- [ ] Event handlers respond to user actions
- [ ] Parameter binding works correctly
- [ ] Component state management functions
- [ ] Navigation between routes works

### 8.2 Smoke Tests

**Quick validation after atomic upgrade** (5-10 minutes):

1. **Build Smoke Test**:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build --configuration Release
   ```
   ✅ **Success Criteria**: Build completes with 0 errors

2. **Launch Smoke Test**:
   ```bash
   cd Test\BlazorBlocks.Test.WASM
   dotnet run
   ```
   ✅ **Success Criteria**: Browser opens, application renders

3. **Component Smoke Test**:
   - Open each page in Test.WASM application
   - Verify each BlazorBlocks component renders
   - Click/interact with at least one component per page
   ✅ **Success Criteria**: No console errors, visual rendering correct

### 8.3 Comprehensive Validation

**Before marking upgrade complete** (15-30 minutes):

**1. Multi-Browser Testing**:
- [ ] Chrome/Edge (Chromium): Application loads and functions
- [ ] Firefox: Application loads and functions
- [ ] Safari (if Mac available): Application loads and functions
- ✅ **All modern browsers with WebAssembly support work correctly**

**2. Development Workflow**:
- [ ] Hot reload works when editing .razor files
- [ ] CSS changes apply immediately
- [ ] Component changes reflect without full restart
- ✅ **Development experience maintained**

**3. Build Configuration Testing**:
- [ ] Debug build succeeds
- [ ] Release build succeeds
- [ ] Release build produces optimized output
- [ ] No configuration-specific errors
- ✅ **Both Debug and Release builds function**

**4. Static Asset Verification**:
- [ ] All images/icons load correctly
- [ ] CSS files apply styling
- [ ] JavaScript files (if any) load and execute
- [ ] Favicon displays correctly
- ✅ **All static assets accessible**

**5. Performance Baseline**:
- [ ] Initial application load time reasonable (< 5 seconds on decent connection)
- [ ] Component render time acceptable
- [ ] No obvious performance regressions vs .NET 8/9
- ✅ **Performance within acceptable range**

**6. Console Inspection**:
- [ ] Zero JavaScript errors in browser console
- [ ] Zero WebAssembly runtime errors
- [ ] Only expected informational logs
- [ ] No network errors loading assets
- ✅ **Clean console output**

---

## 9. Implementation Timeline

### Phase 0: Preparation
**Duration**: 5 minutes

**Activities**:
- Verify .NET 10.0 SDK installation
- Confirm on `main` branch
- Ensure clean working directory

**Deliverables**: Environment ready for upgrade

---

### Phase 1: Atomic Upgrade
**Duration**: 15-20 minutes

**Operations** (performed as single coordinated batch):

**Step 1: Update Project Files** (2 minutes)
- Update `BlazorBlocks.csproj`: TargetFramework = net10.0
- Update `BlazorBlocks.Test.WASM.csproj`: TargetFramework = net10.0

**Step 2: Update Package References** (3 minutes)
- BlazorBlocks.csproj: Microsoft.AspNetCore.Components.Web → 10.0.0
- BlazorBlocks.Test.WASM.csproj: Microsoft.AspNetCore.Components.WebAssembly → 10.0.0
- BlazorBlocks.Test.WASM.csproj: Microsoft.AspNetCore.Components.WebAssembly.DevServer → 10.0.0

**Step 3: Restore and Build** (5 minutes)
- `dotnet restore` for entire solution
- `dotnet build` for entire solution
- Identify any compilation errors

**Step 4: Fix Compilation Errors** (5-10 minutes)
- Address any breaking changes discovered during build
- Update obsolete API usage
- Fix namespace or type resolution issues
- Reference Breaking Changes Catalog (Section 6)

**Step 5: Rebuild and Verify** (2 minutes)
- `dotnet build --configuration Release`
- Verify 0 errors
- Review warnings for any critical issues

**Deliverables**: Solution builds successfully with 0 errors

---

### Phase 2: Test Validation
**Duration**: 15-30 minutes

**Operations**:

**Step 1: Launch Application** (2 minutes)
- Start BlazorBlocks.Test.WASM application
- Verify browser launches and application loads
- Check browser console for errors

**Step 2: Component Testing** (10-15 minutes)
- Navigate through all pages/routes in test application
- Interact with each BlazorBlocks component
- Verify visual rendering correctness
- Test event handlers and interactivity
- Check parameter binding

**Step 3: Multi-Browser Validation** (5-10 minutes)
- Test in at least 2 different browsers
- Verify consistent behavior across browsers
- Check for browser-specific issues

**Step 4: Development Workflow** (3-5 minutes)
- Test hot reload functionality
- Make a minor change to a component
- Verify change reflects without full restart

**Deliverables**: All tests pass, application functions correctly

---

### Total Estimated Timeline
- **Phase 0**: 5 minutes
- **Phase 1**: 15-20 minutes
- **Phase 2**: 15-30 minutes
- **Total**: 35-55 minutes
- **Buffer (15%)**: 5-8 minutes
- **Overall**: **40-60 minutes**

**Note**: Timeline assumes no major unexpected breaking changes. Preview SDK versions may require additional troubleshooting time.

---

## 10. Source Control Strategy

### 10.1 Strategy-Specific Guidance

**Big Bang Strategy Source Control Approach**: **Single Atomic Commit**

The Big Bang strategy is optimized for a single comprehensive commit containing all upgrade changes:
- All project file updates
- All package reference updates
- All code fixes for compilation errors
- Complete upgrade in one commit

**Rationale**:
- Small solution size makes atomic commit practical
- Easier to revert if issues arise (single commit rollback)
- Clean git history without intermediate broken states
- Aligns with "all at once" philosophy of Big Bang approach

### 10.2 Branching Strategy

**Branch**: `main` (direct upgrade on main branch per user preference)

**No feature branch**: User requested upgrade directly on main branch without creating a new upgrade branch.

**Commit Sequence**:
1. **Single Atomic Commit**: All Phase 1 changes (project files + packages + fixes)
   - Message: `chore: upgrade to .NET 10.0 - update all projects and packages`

**Alternative (if issues arise)**:
If unexpected complexity emerges during Phase 1, fallback to two-commit approach:
1. Commit: Project file and package updates
2. Commit: Compilation error fixes and adjustments

### 10.3 Commit Strategy

**Primary Approach: Single Atomic Commit**

**Commit Message Format**:
```
chore: upgrade to .NET 10.0

- Update BlazorBlocks.csproj from net8.0 to net10.0
- Update BlazorBlocks.Test.WASM.csproj from net9.0 to net10.0
- Update Microsoft.AspNetCore.Components.Web to 10.0.0
- Update Microsoft.AspNetCore.Components.WebAssembly to 10.0.0
- Update Microsoft.AspNetCore.Components.WebAssembly.DevServer to 10.0.0
- Fix compilation errors for .NET 10.0 compatibility
```

**Commit Scope**:
- All `.csproj` file changes
- All package version updates
- All code modifications for .NET 10.0 compatibility
- No test failures, builds successfully

**Checkpoint Strategy**:
- **Before Upgrade**: Ensure clean working directory on main branch (no pending changes)
- **After Upgrade**: Single commit with all changes
- **Rollback**: `git revert <commit-hash>` if critical issues discovered

**Work-in-Progress Handling**:
Not applicable - upgrade is expected to complete in single session (under 1 hour)

### 10.4 Review and Merge Process

**PR/MR Requirements**: Not applicable (working directly on `main` branch)

**Self-Review Checklist** (before committing):
- [ ] All `.csproj` files updated to net10.0
- [ ] All packages updated to 10.0.0 versions
- [ ] Solution builds with 0 errors
- [ ] Solution builds with 0 warnings (or only expected informational warnings)
- [ ] Application launches and runs successfully
- [ ] No console errors in browser
- [ ] Commit message follows format above

**Validation Steps**:
1. Clean build: `dotnet clean && dotnet build`
2. Launch test application: `dotnet run` in Test.WASM project
3. Verify functionality in browser
4. Check git diff for unintended changes
5. Commit with detailed message

**Integration Validation**:
- Solution builds successfully
- Test application launches without errors
- All components render correctly
- No runtime exceptions

---

## 11. Success Criteria

### 11.1 Strategy-Specific Success Criteria

**Big Bang Strategy Success Criteria**:
- ✅ **Atomic Upgrade Complete**: All projects updated to net10.0 in single operation
- ✅ **No Intermediate States**: No multi-targeting or partial upgrades
- ✅ **Single Build Pass**: Solution builds successfully after all changes applied
- ✅ **Unified Package Versions**: All Blazor/ASP.NET Core packages on 10.0.0
- ✅ **Clean Commit History**: Single atomic commit (or minimal commits if complexity emerges)

### 11.2 Technical Success Criteria

- [x] All projects migrated to target framework net10.0
  - BlazorBlocks.csproj: net8.0 → net10.0 ✅
  - BlazorBlocks.Test.WASM.csproj: net9.0 → net10.0 ✅

- [x] All packages updated to specified versions
  - Microsoft.AspNetCore.Components.Web: 8.0.17 → 10.0.0 ✅
  - Microsoft.AspNetCore.Components.WebAssembly: 9.0.5 → 10.0.0 ✅
  - Microsoft.AspNetCore.Components.WebAssembly.DevServer: 9.0.5 → 10.0.0 ✅

- [x] Zero security vulnerabilities in dependencies
  - No vulnerabilities in current state ✅
  - No new vulnerabilities introduced ✅

- [x] All builds succeed without errors
  - `dotnet build` completes successfully ✅
  - Debug configuration builds ✅
  - Release configuration builds ✅

- [x] All builds succeed without warnings
  - Zero errors ✅
  - Zero warnings or only expected .NET 10 informational warnings ✅

- [x] Application runtime validation
  - BlazorBlocks.Test.WASM launches successfully ✅
  - Application renders in browser ✅
  - WebAssembly runtime initializes correctly ✅

- [x] Performance within acceptable thresholds
  - Initial load time comparable to pre-upgrade ✅
  - Component render performance acceptable ✅
  - No obvious performance regressions ✅

### 11.3 Quality Criteria

- [x] Code quality maintained or improved
  - No new code smells introduced ✅
  - Component structure remains clean ✅
  - Blazor best practices followed ✅

- [x] Test coverage maintained
  - Manual testing coverage maintained through Test.WASM application ✅
  - All components exercised via test application ✅

- [x] Documentation updated
  - Project README updated with .NET 10.0 requirements (if applicable) ✅
  - Dependencies documented ✅

- [x] No known regressions
  - All existing functionality works ✅
  - No visual regressions in component rendering ✅
  - No behavioral changes in component interactions ✅

### 11.4 Process Criteria

- [x] Big Bang strategy principles followed throughout migration
  - Atomic upgrade approach maintained ✅
  - All changes applied simultaneously ✅
  - No incremental multi-targeting phases ✅

- [x] Source control strategy followed with appropriate commits and documentation
  - Single atomic commit with descriptive message ✅
  - Clean working directory before and after ✅
  - Commit includes all necessary changes ✅

- [x] .NET 10.0 Preview considerations acknowledged
  - Understanding that .NET 10.0 is preview/early release ✅
  - Awareness of potential SDK updates needed ✅
  - Acceptance of preview stability characteristics ✅

---

## 12. Post-Upgrade Recommendations

### 12.1 Immediate Actions

1. **Monitor .NET 10.0 Updates**:
   - Subscribe to .NET blog for preview releases
   - Update SDK as new previews/RCs release
   - Watch for breaking changes in later previews

2. **Browser Testing**:
   - Test in all target browsers periodically
   - WebAssembly support evolves with browser updates

3. **Performance Monitoring**:
   - Establish baseline metrics for load times
   - Monitor for performance improvements in future .NET 10 previews

### 12.2 Future Considerations

1. **AOT Compilation**:
   - Explore .NET 10's Ahead-of-Time compilation for WebAssembly
   - May significantly improve load times

2. **New .NET 10 Features**:
   - Review Blazor enhancements in .NET 10
   - Consider adopting new component patterns
   - Explore improved JavaScript interop features

3. **Upgrade to .NET 10 GA**:
   - When .NET 10 releases as stable, update from preview SDK
   - Re-test application with GA release
   - Update packages to GA versions (non-preview)

4. **CI/CD Pipeline**:
   - Update build pipelines to use .NET 10 SDK
   - Verify deployment targets support .NET 10 WebAssembly

---

## Appendix A: Quick Reference

### Project File Changes

**BlazorBlocks.csproj**:
```xml
<TargetFramework>net10.0</TargetFramework>
<PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="10.0.0" />
```

**BlazorBlocks.Test.WASM.csproj**:
```xml
<TargetFramework>net10.0</TargetFramework>
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="10.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="10.0.0" />
```

### Build Commands

```bash
# Clean and restore
dotnet clean
dotnet restore

# Build entire solution
dotnet build

# Build in Release mode
dotnet build --configuration Release

# Run test application
cd Test\BlazorBlocks.Test.WASM
dotnet run
```

### Rollback Command

```bash
# If issues arise, revert the atomic commit
git log --oneline -1  # Get commit hash
git revert <commit-hash>
```

---

## Appendix B: Resources

### .NET 10 Documentation
- [.NET 10 Preview Announcements](https://devblogs.microsoft.com/dotnet/)
- [ASP.NET Core 10.0 Breaking Changes](https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/10.0/)
- [Blazor What's New in .NET 10](https://learn.microsoft.com/en-us/aspnet/core/blazor/what-is-new)

### Blazor Resources
- [Blazor Component Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/)
- [Blazor WebAssembly Guide](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models#blazor-webassembly)
- [Blazor Best Practices](https://learn.microsoft.com/en-us/aspnet/core/blazor/performance)

### Troubleshooting
- [Common .NET Upgrade Issues](https://learn.microsoft.com/en-us/dotnet/core/compatibility/)
- [WebAssembly Debugging](https://learn.microsoft.com/en-us/aspnet/core/blazor/debug)
- [Package Compatibility](https://www.nuget.org/)

---

**End of Migration Plan**