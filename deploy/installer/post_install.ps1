# returns the package version of the package being installed
function Get-PackageVersion {
    $md5 = New-Object -TypeName System.Security.Cryptography.MD5CryptoServiceProvider
    $utf8 = New-Object -TypeName System.Text.UTF8Encoding
    $hash = [System.BitConverter]::ToString($md5.ComputeHash($utf8.GetBytes($env:Cirrus_Package_Version)))
    return $hash -replace '-', ''
}

# returns the path to the business user interface config file
function Get-BusinessUserInterfaceConfigFile {
    return [System.IO.Path]::Combine($env:Cirrus_BusinessUserInterface_DIR, 'config.json')
}

# loads the content of a json config file
function Load-Config {
    param(
        [Parameter(Mandatory = $TRUE, Position = 1)]
        [string]$path
    )

    if (!(Test-Path $path)) {
        New-Item $path -ItemType File
    }

    return Get-Content $path -Raw | ConvertFrom-Json
}

# saves the content in a json config file
function Save-Config {
    param(
        [Parameter(Mandatory = $TRUE, Position = 1)]
        [string]$path,
        [Parameter(Mandatory = $TRUE, Position = 2)]
        $content
    )

    $content | ConvertTo-Json -Compress | Set-Content $path
}

# adds or replaces a configured user interface module
function AddOrReplace-UiModule {
    param(
        [Parameter(Mandatory = $TRUE, Position = 1)]
        $content,
        [Parameter(Mandatory = $TRUE)]
        [string]$moduleName, # the unique name of the module (= name of the node / es6 module)
        [Parameter(Mandatory = $TRUE)]
        [string]$path, # the path to the module relative to the ui folder
        [switch]$doNotAppendVersion # specifies whether the package version should be added to the path or not
    )

    if (-not $content.UiModules) {
        $content | Add-Member UiModules $(New-Object System.Collections.ArrayList)
    }

    $existing = $content.UiModules | ? { $_.Name -eq $moduleName } | Select -First 1

    if (-not $existing) {
        $existing = @{}
        $content.UiModules += $existing
    }

    if (-not $doNotAppendVersion.IsPresent) {
        $path = "$($path)?v=$(Get-PackageVersion)"
    }

    $existing.name = $moduleName
    $existing.path = $path
}

$configFile = Get-BusinessUserInterfaceConfigFile
$content = Load-Config $configFile

AddOrReplace-UiModule $content -moduleName '@cirrus/module.catch-em-all.ui' -path 'cirrus.module.catch-em-all.ui.umd.min.js'

Save-Config $configFile $content
