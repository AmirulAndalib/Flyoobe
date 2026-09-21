# Legacy projects and source status

Flyoobe has had three distinct chapters. Keeping them named correctly matters more than pretending they are one continuous codebase.

## Flyby11

Flyby11 was the original focused upgrader. Its job was to help Windows 10 machines start a Windows 11 upgrade when Microsoft's normal setup path rejected otherwise usable hardware.

The complete classic application—including its native CPU compatibility helper—is preserved under [`legacy/Flyby11`](../legacy/Flyby11). It is the origin of this repository, but it is no longer actively maintained. Windows setup behavior can change, so old binaries and methods should not be treated as current guidance.

## Flyoobe 2

Flyoobe 2 expanded the idea beyond the upgrade itself: OOBE choices, applications, debloating, tweaks and script extensions moved into one application. That generation proved the larger workflow, but it also accumulated several overlapping systems.

The `Flyoobe` source directory on the default branch currently belongs to this generation. It remains temporarily while the replacement source is cleaned up for publication.

## Flyoobe 3

Flyoobe 3 is a substantial refactor, not a cosmetic update. It uses a smaller native WinForms shell, file-based rule catalogs, explicit recipes and the new optional Setup Actions model.

Current releases already use this direction. The refactored source will replace the previous implementation once its public structure and documentation are ready.

## Repository directories during the transition

| Directory | Meaning |
|---|---|
| `Flyoobe/` | Previous Flyoobe source; awaiting replacement by the 3.x source |
| `legacy/Flyby11/` | Complete archived Flyby11 application and native CPU helper |
| `Actions/` | Optional Setup Actions packaged for Flyoobe 3 |
| `docs/` | Documentation for the current direction |

No new features should be built on the legacy projects or the retired extension system. New optional automation belongs in `Actions/` and follows [`setup-actions.md`](setup-actions.md).
