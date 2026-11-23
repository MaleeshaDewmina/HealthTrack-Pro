# HealthTrack Pro — Post-Viva Improvements

This update follows the `v1.0-coursework-final` coursework tag. It does **not** change that tag.

## Changes included

- **Notifications:** New notification dropdown with an authenticated, user-scoped MVC endpoint; recent notifications, unread badge, mark-as-read and mark-all-read controls.
- **Administrator reports:** Optional From/To date filters, print-friendly layout, and CSV export compatible with Excel. CSV export is not a native `.xlsx` workbook.
- **Client dashboard:** Load the active workout and daily calorie information from existing services and health records.
- **AI Coach:** Larger chat area and simplified, less-nested layout.
- **Appointments:** Refined Client appointment date/time input.
- **Trainer:** Corrected workout-plan creation button display.

## Before merging

- Rebuild the solution with Visual Studio and restore packages if prompted.
- Run the existing 21 automated tests; do **not** assume they passed until rerun.
- Sign in as Client, Trainer, and Administrator and check the appropriate pages.
- Verify the notification dropdown's GET and anti-forgery-protected POST actions.
- Verify CSV export and date filters with meaningful dates; check print preview.
- Verify the updated dashboard shows correct metrics, the AI chat scrolls, appointment inputs work, and the Trainer workout-plan form renders.
- Check that local secrets, user databases, photos, and backup directories are **not** part of the Git diff.

## Scope

This post-viva set contains 14 source files (10 modified and 4 new), extracted from the local working project ZIP. It intentionally does not contain the local SQL Server database, build outputs, user photos or patch backups.
