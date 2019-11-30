# D2TxtImporter
This program is used to process all the .txt files of your mod and generate documentation for it.

# Guide
To function correctly you need the following .tbl files (they can be named however you like, as long as they have the .tbl extension):
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
Mainly used for debugging, not really updated.  
.\D2TxtImporter.console.exe "<Absolute path to your .txt files>" "<Absolute path to your .tbl files>" "< The Directory you want your output to go to>"

# To do
- Rework cube recipes
- Rework hard coded values (enhanced damage, skill tree names, etc.)
- Add support for all DescFunc from ItemStatCosts.txt, I do not have text files that use all of them and can't really test it all with vanilla .txt files
- Cleanup the mess of a code this entire project is..
- Add more output formats sql, json, etc?

# Credits
- .tbl import functionality: https://github.com/kambala-decapitator/QTblEditor
- Ascended1962 (Creator of Diablo 2 Enriched): General help with the project
