# Changelog

## Flyoobe 3.01 — Native controls, Setup Actions and a little Windows 7 nostalgia

Flyoobe 3.0 may have been only a day old when this work started, but the release itself was the result of a much longer rebuild. I am unusually motivated to keep pushing it because the new foundation already surpasses the previous version by miles.

This is still the same project that grew out of Flyby11—just with a clearer idea of what it wants to be. Flyby11 helped Windows get onto a PC; Flyoobe now helps the PC feel like yours afterwards.

The corresponding refactored source will be published after the remaining cleanup. A source dump is easy. A source tree another human can navigate is the part worth finishing.

### Setup Actions

- **[Added]** Support for local PowerShell-based Setup Actions.
- **[Added]** Descriptions, selectable commands and optional action choices.
- **[Added]** Live script output, because a frozen window is not a progress indicator—it is a trust exercise.
- **[Added]** Recipe-safe actions that run during explicit `Prepare` or `Finish` phases.
- **[Added]** An action importer and direct access to the local Actions folder.
- **[Added]** Documentation for writing, reviewing and sharing actions.
- **[Changed]** Actions stay isolated from the core application and can be disabled completely.
- **[Changed]** The old Flyoobe Extensions model has been retired in favour of one smaller execution model.

In other words: I added a door without rebuilding the entire house around everyone who might eventually walk through it.

### A more familiar interface

- **[Changed]** The application name is now written consistently as **Flyoobe**.
- **[Changed]** Added an Explorer-style navigation bar with Back, Forward, breadcrumbs and a shared page search.
- **[Changed]** Replaced the dense overview table with simple expandable native groups.
- **[Changed]** Matching recommendations stay collapsed while items needing attention remain visible.
- **[Changed]** Added familiar blue page headings, white content areas and native grey command footers.
- **[Changed]** Added a subtle Windows 7-inspired blue-to-green accent.
- **[Changed]** Simplified the browser page into a lighter vertical workflow.
- **[Changed]** Kept the interface based on standard Windows controls—no custom skin pretending to be an operating system.

The older I get, the more I appreciate old software. Apparently nostalgia is just usability after it has passed a sufficiently long code review.

### Fixes and refinement

- **[Fixed]** Improved TreeView and breadcrumb spacing at high DPI.
- **[Fixed]** Added native scrollbars where pages can become smaller than their content.
- **[Fixed]** Prevented clipped controls on narrower Settings and Browser pages.
- **[Fixed]** Removed duplicate Back buttons and inconsistent navigation paths.
- **[Fixed]** Reworked recipe review mode so it follows the native interface instead of introducing a separate yellow visual language.
- **[Fixed]** Consolidated page search behind a small `ISearchableView` contract.

### Installing the optional Actions asset

1. Download the **Actions** asset from the release page.
2. Extract the included `Actions` folder into Flyoobe's `Data` folder.
3. Open **Settings > Advanced** and enable **Setup Actions**.

The expected result is `Data\Actions\<action-id>\action.ini`. Nothing runs automatically after extraction.
