# C4rl1t0sRefactor

A **keyboard-shortcut tool for refactoring** in **Visual Studio 2022** (desktop).
Press **`Ctrl + Shift + R`** anywhere in your code and a short, numbered menu of the
most common refactorings pops up. Pick one and the extension runs the matching
**built-in Visual Studio refactoring command**.

## Why

Visual Studio ships solid refactorings, but they are scattered across context menus,
the *Refactor* submenu, the light bulb (`Ctrl + .`) and individual shortcuts that are
hard to remember. This extension puts the important ones behind **a single shortcut**
and a compact, keyboard-driven menu.

- **One entry point.** `Ctrl + Shift + R` in any C#/VB file.
- **Fast, navigable menu.** Arrow keys + Enter, double-click, or number keys `1`–`9`.
- **Delegates to Visual Studio.** It does not reimplement refactorings: it invokes the
  real commands (`Refactor.Rename`, `Refactor.ExtractMethod`, …), so the behavior,
  previews and undo are exactly those of Visual Studio.
- **Context aware.** Refactorings that don't apply at the caret position are greyed out.

## Refactorings included

| # | Menu entry | Visual Studio command |
|---|------------|-----------------------|
| 1 | Rename | `Refactor.Rename` |
| 2 | Extract Method | `Refactor.ExtractMethod` |
| 3 | Extract Interface | `Refactor.ExtractInterface` |
| 4 | Extract Base Class | `Refactor.ExtractBaseClass` *(fallback: Quick Actions)* |
| 5 | Encapsulate Field | `Refactor.EncapsulateField` |
| 6 | Change Signature | `Refactor.ChangeSignature` *(fallback: Quick Actions)* |
| 7 | Remove Parameters | `Refactor.RemoveParameters` |
| 8 | Reorder Parameters | `Refactor.ReorderParameters` |
| 9 | Promote Local to Parameter | `Refactor.PromoteLocalVariableToParameter` |
| 10 | Move Type to File / Move to Namespace | Quick Actions (`Ctrl + .`) |
| 11 | Inline / Introduce Variable / Introduce Field | Quick Actions (`Ctrl + .`) |
| 12 | Quick Actions & Refactorings | `EditorContextMenus.CodeWindow.QuickActionsForPosition` |

### About "Move Type"

Visual Studio has no standalone command for moving a type: "Move type to *File*.cs" and
"Move to namespace…" live inside *Quick Actions* (the light bulb, `Ctrl + .`). Entry 10
opens Quick Actions at the caret, where those options appear directly. Entries 4 and 6
try the native command first and fall back to Quick Actions if your Visual Studio
version doesn't expose it.

## Requirements

- Windows + **Visual Studio 2022 (17.x)**
- The **"Visual Studio extension development"** workload (provides the VS SDK)

## Build the `.vsix`

### Option A — script

```powershell
cd C:\Code\RefactorThisMenu
./build.ps1
```

Produces `src\C4rl1t0sRefactor\bin\Release\C4rl1t0sRefactor.vsix`.

### Option B — Visual Studio

1. Open `C4rl1t0sRefactor.sln`.
2. Set configuration to **Release** → **Build → Build Solution**.
3. The `.vsix` lands in `src\C4rl1t0sRefactor\bin\Release\`.
4. `F5` launches an experimental VS instance with the extension loaded for testing.

> ⚠️ `dotnet build` does **not** produce the `.vsix`: the VSSDK targets that package the
> extension only exist in the full `MSBuild.exe` that ships with Visual Studio. Use
> `build.ps1` (it locates that MSBuild via `vswhere`) or build from Visual Studio.

## Install

- Double-click `C4rl1t0sRefactor.vsix`, **or**
- `"%VSINSTALLDIR%\Common7\IDE\VSIXInstaller.exe" C4rl1t0sRefactor.vsix`

Restart Visual Studio. To remove it: **Extensions → Manage Extensions**.

### Updating

Bump the `Version` attribute of `<Identity>` in `source.extension.vsixmanifest`
(`1.0.1` → `1.0.2` → …) while keeping the same `Id`. When you install the new `.vsix`,
VSIX Installer detects it as an **update**.

## Usage

1. Put the caret on a symbol or code selection (C#/VB).
2. Press **`Ctrl + Shift + R`**.
3. Choose a refactoring. It's also available under **Edit → C4rl1t0sRefactor…**

### If `Ctrl + Shift + R` does nothing

The shortcut may already be bound in your keyboard profile. To rebind it:

1. **Tools → Options → Environment → Keyboard**.
2. In "Show commands containing" type `Edit.C4rl1t0sRefactor`.
3. Click into "Press shortcut keys" and press `Ctrl + Shift + R`.
4. "Use new shortcut in": **Global** → **Assign** → **OK**.

## Customizing the list

Edit `src/C4rl1t0sRefactor/RefactorDefinition.cs` → `RefactorCatalog.Build()`.
Each entry takes one or more DTE command names: they are tried in order and the first
one that exists and is available runs. Rebuild and reinstall.

## How it works

```
Ctrl+Shift+R  ──►  C4rl1t0sRefactorPackage (AsyncPackage, loaded on demand)
                        │
                        ▼
              C4rl1t0sRefactorCommand.Execute
                        │  queries DTE.Commands[...].IsAvailable for each refactoring
                        ▼
              RefactorMenuWindow (WPF, themed like VS)   ◄─ user picks an entry
                        │
                        ▼
              DTE.ExecuteCommand("Refactor.Xxx")         ─► native VS refactoring
```

- **Keyboard binding and menu entry:** `C4rl1t0sRefactorPackage.vsct`.
- **No third-party dependencies:** VS SDK + WPF only.

## Repository layout

```
├─ C4rl1t0sRefactor.sln
├─ build.ps1                        Builds and locates the .vsix
├─ LICENSE.txt                      MIT
└─ src/C4rl1t0sRefactor/
   ├─ C4rl1t0sRefactor.csproj       VSIX project (classic VSSDK format, .NET Framework 4.7.2)
   ├─ Properties/AssemblyInfo.cs
   ├─ source.extension.vsixmanifest
   ├─ C4rl1t0sRefactorPackage.vsct  Command + Ctrl+Shift+R binding + Edit-menu entry
   ├─ C4rl1t0sRefactorPackage.cs    AsyncPackage
   ├─ C4rl1t0sRefactorCommand.cs    Handler: shows the menu and runs the chosen command
   ├─ RefactorDefinition.cs         Refactoring catalog (edit it here)
   ├─ RefactorMenuWindow.xaml       Pop-up menu window
   └─ RefactorMenuWindow.xaml.cs
```

## License

MIT — see [`LICENSE.txt`](LICENSE.txt).

---

> This targets **desktop Visual Studio**. For **VS Code** the model is different
> (`package.json` + command API, no VSSDK).
