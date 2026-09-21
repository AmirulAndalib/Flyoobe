# Optional Flyoobe Setup Actions

This directory contains the packages used to build the optional **Actions** release asset. They are deliberately kept outside Flyoobe's core feature set.

## Install

1. Download the **Actions** asset from the Flyoobe release page.
2. Extract its `Actions` folder into the application's `Data` folder.
3. Enable **Setup Actions** under **Settings > Advanced**.

The final layout should be:

```text
Flyoobe/
|- Flyoobe.exe
`- Data/
   `- Actions/
      `- <action-id>/
         |- action.ini
         `- run.ps1
```

Nothing in this folder runs automatically. Manual actions require an explicit click and confirmation. An action can participate in a recipe only when its manifest opts in with `RecipeAllowed=true` and uses the `Prepare` or `Finish` phase.

## Before running an action

- Read `action.ini` and `run.ps1`.
- Pay attention to `RequiresAdmin` and `Warning`.
- Treat actions that download changing remote code as external software.
- Keep backups before making broad system changes.

To create your own package, read the complete [Setup Actions guide](../docs/setup-actions.md).
