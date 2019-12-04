# D2TxtImporter
This program is used to process all the .txt files of your mod and generate documentation for it.

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=HW6L5XFFAFZ5J&source=url) Buy me a cup of coffee if you feel like it. I spend a significant amount of time on this project.

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
- Add support for all DescFunc from ItemStatCosts.txt, I do not have text files that use all of them and can't really test it all with vanilla .txt files. If a "todo" shows up in your output, please post the todo in a new issue along with how it is supposed to look so I can correctly generate the output.
- Cleanup the mess of a code this entire project is..
- Add better exception handling so the exceptions actually makes sense
- Add better searching on web part
- Internet Explorer not supported, and I am not doing it either.

# Issues
If you open an issue, please provide the required .txt and .tbl files so I can debug it. As mentioned above, I only have the vanilla files for 1.13c to test with.

# Credits
- .tbl import functionality: https://github.com/kambala-decapitator/QTblEditor
- Ascended1962 (Creator of Diablo 2 Enriched): General help with the project
