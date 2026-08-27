#requires -version 5
# Compila la extension y genera src\C4rl1t0sRefactor\bin\Release\C4rl1t0sRefactor.vsix
# IMPORTANTE: hay que usar el MSBuild.exe de Visual Studio (no 'dotnet build'),
# porque los targets del VSSDK que empaquetan el .vsix solo existen en el MSBuild completo.

$ErrorActionPreference = 'Stop'

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhere)) {
    throw "No se encontro vswhere.exe. Instala Visual Studio 2022 con la carga de trabajo 'Desarrollo de extensiones de Visual Studio'."
}

$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' |
    Select-Object -First 1

if (-not $msbuild) {
    throw "No se encontro MSBuild.exe. Instala en Visual Studio 2022 la carga de trabajo 'Desarrollo de extensiones de Visual Studio'."
}

Write-Host "MSBuild: $msbuild"

$project = Join-Path $PSScriptRoot 'src\C4rl1t0sRefactor\C4rl1t0sRefactor.csproj'

& $msbuild $project /t:Rebuild /restore /p:Configuration=Release /p:DeployExtension=false /v:minimal
if ($LASTEXITCODE -ne 0) { throw "La compilacion fallo (codigo $LASTEXITCODE)." }

$vsix = Join-Path $PSScriptRoot 'src\C4rl1t0sRefactor\bin\Release\C4rl1t0sRefactor.vsix'
if (Test-Path $vsix) {
    Write-Host ""
    Write-Host "VSIX generado:" -ForegroundColor Green
    Write-Host "  $vsix" -ForegroundColor Green
    Write-Host ""
    Write-Host "Instalalo con doble clic, o:  VSIXInstaller.exe `"$vsix`""
}
else {
    throw "La compilacion termino pero no se encontro el .vsix en $vsix"
}
