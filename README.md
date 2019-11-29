# D2TxtImporter

# Guide
To function correctly you need the following .tbl files from the following .mpq files (they can be named however you like):
- d2data.mpq
  - string.tbl
- patch_d2.mpq
  - patchstring.tbl
- d2exp.mpq
  - patchstring.tbl
  - expansionstring.tbl
## Client Application
- Excel Directory: Absolute path to your .txt files
- Table Directory: Absolute path to your .tbl files
- Output Directory: The directory you want your output to go to
## Console Application
.\D2TxtImporter.console.exe "<Absolute path to your .txt files>" "<Absolute path to your .tbl files>" "< The Directory you want your output to go to>"

# Credits
- .tbl import functionality: https://github.com/kambala-decapitator/QTblEditor
- Ascended1962 (Creator of Diablo 2 Enriched): General help with the project
