# C4rl1t0sRefactor

Extensión para **Visual Studio 2022** (escritorio) que recrea el menú
**"Refactor This"** de ReSharper: pulsas **`Ctrl + Shift + R`** sobre tu código y
aparece una lista con los refactors más habituales. Eliges uno y la extensión lanza
el comando de refactorización **nativo de Visual Studio** correspondiente.

## ¿Para qué sirve?

Visual Studio ya trae buenos refactors, pero están repartidos por menús contextuales,
el submenú *Refactorizar*, la bombilla (`Ctrl + .`) y atajos sueltos difíciles de
recordar. ReSharper resolvía eso con **un único atajo** (`Ctrl + Shift + R`) que abre
un menú corto y numerado con todo lo importante en un sitio.

Este proyecto reproduce esa idea **sin necesidad de ReSharper**:

- **Un solo punto de entrada.** `Ctrl + Shift + R` en cualquier archivo C#/VB.
- **Menú rápido y navegable.** Flechas + Enter, doble clic, o teclas `1`–`9`.
- **Delega en Visual Studio.** No reimplementa refactorizaciones: invoca los comandos
  reales (`Refactor.Rename`, `Refactor.ExtractMethod`, …), así que el comportamiento,
  las vistas previas y el deshacer son exactamente los de VS.
- **Consciente del contexto.** Los refactors que no aplican en la posición del cursor
  aparecen atenuados.

## Refactors incluidos

| # | Menú | Comando de Visual Studio | Equivalente ReSharper |
|---|------|--------------------------|-----------------------|
| 1 | Rename (renombrar) | `Refactor.Rename` | `Refactor.Rename` |
| 2 | Extract Method (extraer método) | `Refactor.ExtractMethod` | `Refactor.ExtractMethod` |
| 3 | Extract Interface (extraer interfaz) | `Refactor.ExtractInterface` | `Refactor.ExtractInterface` |
| 4 | Extract Base Class (extraer clase base) | `Refactor.ExtractBaseClass` *(fallback: Quick Actions)* | `Refactor.ExtractSuperclass` |
| 5 | Encapsulate Field (encapsular campo) | `Refactor.EncapsulateField` | `Refactor.EncapsulateField` |
| 6 | Change Signature (cambiar firma) | `Refactor.ChangeSignature` *(fallback: Quick Actions)* | `Refactor.ChangeSignature` |
| 7 | Remove Parameters (quitar parámetros) | `Refactor.RemoveParameters` | `Refactor.ChangeSignature` |
| 8 | Reorder Parameters (reordenar parámetros) | `Refactor.ReorderParameters` | `Refactor.ChangeSignature` |
| 9 | Promote Local to Parameter (introducir parámetro) | `Refactor.PromoteLocalVariableToParameter` | `Refactor.IntroduceParameter` |
| 10 | **Move Type to File / Move to Namespace** | Quick Actions (`Ctrl + .`) | `Refactor.MoveType` |
| 11 | Inline / Introduce Variable / Introduce Field | Quick Actions (`Ctrl + .`) | `Refactor.Inline` / `Refactor.IntroduceVariable` |
| 12 | Quick Actions & Refactorings | `EditorContextMenus.CodeWindow.QuickActionsForPosition` | — |

### Sobre "Move Type"

ReSharper tiene una acción propia `Refactor.MoveType`. **Visual Studio no expone un
comando independiente** para eso: "Mover el tipo a *Archivo*.cs" y "Mover a espacio de
nombres…" viven dentro de *Quick Actions* (la bombilla, `Ctrl + .`). Por eso el ítem 10
abre Quick Actions en la posición del cursor, donde esas opciones aparecen directamente.
Los ítems 4 y 6 intentan primero el comando nativo y, si tu versión de VS no lo tuviera,
caen también a Quick Actions.

## Requisitos

- Windows + **Visual Studio 2022 (17.x)**
- Carga de trabajo **"Desarrollo de extensiones de Visual Studio"** (aporta el VS SDK)

## Compilar el `.vsix`

### Opción A — script

```powershell
cd C:\Code\RefactorThisMenu
./build.ps1
```

Genera `src\C4rl1t0sRefactor\bin\Release\C4rl1t0sRefactor.vsix`.

### Opción B — Visual Studio

1. Abre `C4rl1t0sRefactor.sln`.
2. Configuración **Release** → **Compilar → Compilar solución**.
3. El `.vsix` queda en `src\C4rl1t0sRefactor\bin\Release\`.
4. `F5` abre una instancia experimental de VS con la extensión cargada para probarla.

> ⚠️ `dotnet build` **no** genera el `.vsix`: los targets del VSSDK que empaquetan la
> extensión solo existen en el `MSBuild.exe` completo de Visual Studio. Usa `build.ps1`
> (localiza ese MSBuild con `vswhere`) o compila desde el propio VS.

## Instalar

- Doble clic en `C4rl1t0sRefactor.vsix`, **o**
- `"%VSINSTALLDIR%\Common7\IDE\VSIXInstaller.exe" C4rl1t0sRefactor.vsix`

Reinicia Visual Studio. Para desinstalar: **Extensiones → Administrar extensiones**.

### Actualizar

Sube el atributo `Version` de `<Identity>` en `source.extension.vsixmanifest`
(`1.0.1` → `1.0.2` → …) manteniendo el mismo `Id`. Al instalar el nuevo `.vsix`,
VSIX Installer lo detectará como **actualización**.

## Uso

1. Coloca el cursor sobre un símbolo o selección de código C#/VB.
2. Pulsa **`Ctrl + Shift + R`**.
3. Elige el refactor. También está en el menú **Editar → C4rl1t0sRefactor…**

### Si `Ctrl + Shift + R` no responde

Ese atajo puede estar ya asignado en tu perfil de teclado (y **si tienes ReSharper
instalado, lo ocupa él** con su propio "Refactor This" — en ese caso esta extensión
sobra). Para reasignarlo:

1. **Herramientas → Opciones → Entorno → Teclado**.
2. En "Mostrar comandos que contienen" escribe `Edit.C4rl1t0sRefactor`.
3. Sitúa el cursor en "Presione las teclas de método abreviado" y pulsa `Ctrl + Shift + R`.
4. "Usar el nuevo método abreviado en": **Global** → **Asignar** → **Aceptar**.

## Personalizar la lista

Edita `src/C4rl1t0sRefactor/RefactorDefinition.cs` → `RefactorCatalog.Build()`.
Cada entrada admite varios comandos DTE: se prueban en orden y se ejecuta el primero
que exista y esté disponible. Recompila y reinstala.

## Cómo funciona por dentro

```
Ctrl+Shift+R  ──►  C4rl1t0sRefactorPackage (AsyncPackage, carga bajo demanda)
                        │
                        ▼
              C4rl1t0sRefactorCommand.Execute
                        │  consulta DTE.Commands[...].IsAvailable para cada refactor
                        ▼
              RefactorMenuWindow (WPF, temada como VS)   ◄─ el usuario elige
                        │
                        ▼
              DTE.ExecuteCommand("Refactor.Xxx")         ─► refactor nativo de VS
```

- **Enlace de teclado y entrada de menú:** `C4rl1t0sRefactorPackage.vsct`.
- **Sin dependencias de terceros:** solo VS SDK + WPF.

## Estructura del repositorio

```
├─ C4rl1t0sRefactor.sln
├─ build.ps1                        Compila y localiza el .vsix
├─ LICENSE.txt                      MIT
└─ src/C4rl1t0sRefactor/
   ├─ C4rl1t0sRefactor.csproj       Proyecto VSIX (formato clásico VSSDK, .NET Framework 4.7.2)
   ├─ Properties/AssemblyInfo.cs
   ├─ source.extension.vsixmanifest
   ├─ C4rl1t0sRefactorPackage.vsct  Comando + binding Ctrl+Shift+R + ítem en menú Editar
   ├─ C4rl1t0sRefactorPackage.cs    AsyncPackage
   ├─ C4rl1t0sRefactorCommand.cs    Handler: muestra el menú y ejecuta el comando elegido
   ├─ RefactorDefinition.cs         Catálogo de refactors (edítalo aquí)
   ├─ RefactorMenuWindow.xaml       Ventana del menú emergente
   └─ RefactorMenuWindow.xaml.cs
```

## Licencia

MIT — ver [`LICENSE.txt`](LICENSE.txt).

---

> Esto es para **Visual Studio de escritorio**. Para **VS Code** el modelo es distinto
> (`package.json` + API de comandos, sin VSSDK).
