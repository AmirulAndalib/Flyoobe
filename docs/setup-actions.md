# Writing Setup Actions for Flyoobe

Setup Actions are my small escape hatch for things that do not belong in Flyoobe's settings database.

Think of rebuilding the icon cache, creating a restore point, opening a Windows tool, or running one final post-install command. I wanted this to stay boring in the best possible way: a little PowerShell, an optional INI file, and no plugin framework hiding behind it.

Nothing runs just because an action exists. You either start it yourself or explicitly add a recipe-safe action to a recipe.

You can enable the feature under **Settings > Advanced > Setup Actions**. The Setup Actions page lets you import an action and open the local Actions folder.

## The quick way: one PowerShell file

For a small personal action, a single `.ps1` is enough:

```powershell
# Description: Clears my application's local cache.
# Author: Your name

$ErrorActionPreference = 'Stop'

try {
    Remove-Item "$env:LOCALAPPDATA\MyApp\Cache\*" -Recurse -Force
    Write-Output 'Cache cleared.'
    exit 0
}
catch {
    Write-Error $_
    exit 1
}
```

Import the script from the Setup Actions page. Flyoobe copies it into `Data\Actions` and uses the file name as its visible name. With no extra metadata it becomes a manual-only `Utility` action.

These optional headers are understood for single scripts:

```powershell
# Id: clear-my-cache
# Description: Clears my application's local cache.
# Version: 1.0
# Author: Your name
# Phase: Utility
# RequiresAdmin: false
# RecipeAllowed: false
# Warning: This closes MyApp before clearing its cache.
# Options: Clear cache; Show cache folder
```

Keep the headers within the first 40 lines and use one comment per field. If you want translations or plan to share the action, use a package instead.

## The clean way: an action package

This is the format I use for actions that ship with Flyoobe:

```text
my-action/
|- action.ini
`- run.ps1
```

Pick `action.ini` when importing it. Flyoobe validates the package and copies the whole folder into `Data\Actions\<Id>`. You can include helper files too; the script can find them through `$PSScriptRoot`.

Here is a complete `action.ini`:

```ini
[Action]
Id=default-power-plan
Name=Default power plan
Name.de=Standard-Energieplan
Description=Choose one of the built-in Windows power plans.
Description.de=Wähle einen der integrierten Windows-Energiepläne.
Version=1.0
Author=Your name
Phase=Finish
RequiresAdmin=false
RecipeAllowed=true
Script=run.ps1
```

## What the manifest fields mean

| Field | Required | What it does |
|---|---:|---|
| `Id` | Yes | Stable action ID. Use letters, numbers, dots, underscores, or hyphens. Start with a letter or number. |
| `Name` | Yes | The name shown in Flyoobe. |
| `Name.<locale>` | No | A translated name, for example `Name.de`. |
| `Description` | No | A short explanation of the action. |
| `Description.<locale>` | No | A translated description. |
| `Version` | No | Informational version. Defaults to `1.0`. |
| `Author` | No | Shown in the action details. |
| `Phase` | Yes | `Prepare`, `Finish`, or `Utility`. |
| `RequiresAdmin` | No | Set this to `true` if Flyoobe needs to be running as administrator. |
| `RecipeAllowed` | No | Allows the action to be exported to and run by recipes. Has no effect on `Utility` actions. |
| `Warning` | No | Extra confirmation text shown before a manual run. |
| `Warning.<locale>` | No | A translated warning. |
| `Script` | Yes | Relative path to a `.ps1` inside the package folder. |

The script must stay inside its package folder. Flyoobe rejects paths that try to escape it.

## Adding a dropdown

This is the only dynamic UI feature I added on purpose. Put an `Options` line near the top of the script and separate the choices with semicolons:

```powershell
# Options: Balanced; High Performance; Power Saver

param([string]$choice)

$ErrorActionPreference = 'Stop'

try {
    switch ($choice) {
        'Balanced'         { powercfg.exe -setactive SCHEME_BALANCED }
        'High Performance' { powercfg.exe -setactive SCHEME_MIN }
        'Power Saver'      { powercfg.exe -setactive SCHEME_MAX }
        default            { throw "Unknown option: $choice" }
    }

    if ($LASTEXITCODE -ne 0) { throw "powercfg failed with exit code $LASTEXITCODE." }
    Write-Output "Selected: $choice"
    exit 0
}
catch {
    Write-Error $_
    exit 1
}
```

Flyoobe passes the selected text as the first positional argument. The parameter name is up to you, so both of these work:

```powershell
param([string]$choice)
```

```powershell
param([string]$Option)
```

Option text is written to recipes. Treat it like a stable ID. If you rename an option, a recipe containing the old text is invalid. That is intentional; the new Setup Actions core does not guess what old values might have meant.

## Prepare, Finish, or Utility?

| Phase | Run manually | Run from a recipe |
|---|---:|---|
| `Prepare` | Yes | Before Flyoobe applies the other recipe changes |
| `Finish` | Yes | After Flyoobe applies the other recipe changes |
| `Utility` | Yes | Never; this is manual-only |

A recipe action needs both a recipe phase and explicit permission:

```ini
Phase=Finish
RecipeAllowed=true
```

Recipes only store the action ID and, if there is a dropdown, the selected option. They never contain PowerShell code or a local script path. This also means the same action must already be installed on the PC that imports the recipe.

Please only mark an action as recipe-safe if it is predictable, non-interactive, and safe to run as part of a confirmed batch.

## Live output and real errors

Flyoobe starts Windows PowerShell with `-NoProfile`, `-NonInteractive`, `-ExecutionPolicy Bypass`, and `-File`. Standard output and standard error appear live on the action page.

Use `Write-Output` for useful progress. Avoid filling the log with noise. More importantly, return a non-zero exit code when something fails. Printing a red-looking sentence is not enough; Flyoobe can only trust the process result.

This is the pattern I recommend:

```powershell
$ErrorActionPreference = 'Stop'

try {
    Write-Output 'Starting...'
    # Do the work here.
    Write-Output 'Finished.'
    exit 0
}
catch {
    Write-Error $_
    exit 1
}
```

When you call a native `.exe`, check `$LASTEXITCODE` yourself. A failed native command does not automatically make PowerShell fail.

## A few rules I would stick to

- Keep the script small enough that another person can actually review it.
- Do not hide commands in encoded strings.
- Do not download and execute changing remote code unless that is the whole point of the action. If it is, say so clearly in `Warning`.
- Only set `RequiresAdmin=true` when it is really needed. Flyoobe does not silently elevate scripts.
- Keep interactive tools, launchers, and user-driven utilities out of recipes.
- Quote paths and use `$PSScriptRoot` for files shipped inside a package.
- Use `throw` or `exit 1` for failure paths. Do not rely on a scary-looking output message.

## What I deliberately left out

The new core has one execution model. There are no external console modes, special log hosts, arbitrary input boxes, category filters, or magic suffixes such as `(console)` and `(silent)`.

The only script-driven UI metadata is `# Options:`. That keeps the code small and makes an action much easier to understand, review, export, and remove again.

## If something does not work

**The package does not appear**

- Check that `Id`, `Name`, `Phase`, and `Script` exist in `action.ini`.
- Make sure `Script` points to a real `.ps1` inside the package folder.
- Make sure Setup Actions are enabled under **Settings > Advanced**.

**The dropdown does not appear**

- Put `# Options:` within the first 40 lines.
- Separate choices with semicolons.

**Flyoobe says the action succeeded after an error**

- End the failure path with `exit 1`.
- Check `$LASTEXITCODE` after calling native programs.

**A recipe cannot find the action**

- Install the same action ID on that PC first.
- Keep the option text unchanged.
- Check that the phase is `Prepare` or `Finish` and `RecipeAllowed=true`.
