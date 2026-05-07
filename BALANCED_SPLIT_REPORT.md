# Balanced Unity Project Split Report
The project was redistributed by actual file size rather than by functional directory, so each day is close to one third of the original project size. Asset files and their corresponding `.meta` files were kept together whenever possible.
| Batch | Unity payload size | Files | Purpose |
|---|---:|---:|---|
| Day1_Balanced_Upload | 561.0 MiB | 1020 | Balanced GitHub upload batch 1. |
| Day2_Balanced_Upload | 561.0 MiB | 1022 | Balanced GitHub upload batch 2. |
| Day3_Balanced_Upload | 561.0 MiB | 1016 | Balanced GitHub upload batch 3. |

## Upload Rule

Upload the contents inside each day folder into the same repository root. Do not upload the outer day folder itself. After Day 3, the merged repository should be the complete Unity project.

## Excluded Local/Generated Files

`Library/`, `Logs/`, `UserSettings/`, `.vs/`, `obj/`, `*.csproj`, and `*.sln` were intentionally excluded.
