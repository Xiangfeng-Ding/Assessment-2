# Balanced Three-Day GitHub Upload Instructions

This folder is one of the three balanced upload batches for the Unity project. Upload the *contents* of each day folder into the same GitHub repository root. Do not upload the outer day folder itself.

After all three days are uploaded, the repository root should contain `Assets/`, `Packages/`, `ProjectSettings/`, `.gitignore`, and this README. Do not upload `Library/`, `Logs/`, `UserSettings/`, `.vs/`, `obj/`, `*.csproj`, or `*.sln`, because these are local/generated files.

The three balanced batches were verified by merging them back together and comparing the resulting `Assets/`, `Packages/`, and `ProjectSettings/` file list against the original Unity project. The final merged project is intended to be opened from the repository root in Unity.
