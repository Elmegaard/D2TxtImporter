# Huge Thank-You to Elmegaard For This Code. Please buy him a coffee.
1. You need to extract two files:
  - C:\Program Files (x86)\Diablo II\ProjectD2\pd2data.mpq (data/global/excel/*) -> some_path\pd2-text-files
  - C:\Program Files (x86)\Diablo II\ProjectD2\pd2data.mpq (data/local/LNG/ENG/*) -> some_path\pd2-table-files

Here is an example run of the console variant:

```
./D2TxtImporter.console.exe \
  --excelPath "C:\Users\camer\Desktop\pd2-text-files" \
  --tablePath "C:\Users\camer\Desktop\pd2-table-files" \
  --outputPath "C:\Users\camer\Desktop\pd2-json-files" \
  --continueOnException \
  --cubeRecipeDescription
```


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
To get information use:
.\D2TxtImporter.console.exe --help

# To do
- Rework cube recipes
- Rework hard coded values (enhanced damage, skill tree names, etc.)
- Add support for all DescFunc from ItemStatCosts.txt, I do not have text files that use all of them and can't really test it all with vanilla .txt files. If a "todo" shows up in your output, please post the todo in a new issue along with how it is supposed to look so I can correctly generate the output.
- Cleanup the mess of a code this entire project is..
- Add better searching on web part
- Internet Explorer not supported, and I am not doing it either.

# Issues
If you open an issue, please provide the required .txt and .tbl files so I can debug it. As mentioned above, I only have the vanilla files for 1.13c to test with. Also attach the errorlog.txt file.

# Credits
- .tbl import functionality: https://github.com/kambala-decapitator/QTblEditor
- Ascended1962 (Creator of Diablo 2 Enriched): General help with the project
