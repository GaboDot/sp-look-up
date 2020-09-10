![alt text](https://github.com/g4b0-88/SPLookUp/blob/master/screenshots/Screenshot_1.png?raw=true "SearchBox")
## SPLookUp
Dekstop app for looking into SQL Server SP (Stored Procedure) whether executes or calls another(s) SP's, also gets tables involved in the searched SP.

This app was developed with the intention when you don't have any knowledge about the database and the SP involved in it, so you just pick a random SP, put it in the LookUp app and will give you all the SP's inside and also the tables involved in it.

### How it Works?
- Connect to the database
- Takes the SP you pasted in the app
- Extracts the text of it (SP_HELPTEXT)
- Analyzes line per line to find any EXEC instruction inside
- Split the line founded to get only the SP Name
- Add it into an object ... and here we have recursion!

For each SP inside another repeat the process until there's no SP to extract.

### Highlights
- The process runs on background (thread)
- Exludes SQL Keywords (at least a ton of them)
- Excludes some weird words I found inside my SP's (this section could be adapted to your needs)
- The result is shown as Json Treeview
- Count all SP's it founded from the original SP
- Measures process time
- Exports Excel (thanks to [Aspose.Cells](https://docs.aspose.com/cells/net/))
- Exports Json File

### Screenshots
![alt text](https://github.com/g4b0-88/SPLookUp/blob/master/screenshots/Screenshot_1.png?raw=true "SearchBox")
![alt text](https://github.com/g4b0-88/SPLookUp/blob/master/screenshots/Screenshot_2.png?raw=true "JsonViewer")
![alt text](https://github.com/g4b0-88/SPLookUp/blob/master/screenshots/Screenshot_3.png?raw=true "ExportedFiles")

### Final Words
- I used Aspose Cells as it exports directly JSON structure, however it has some bugs like mixing up one object inside another, any help here would be appreciated.
- If you know any other library to export Json to a readable Excel, I would appreciate you share it with the repo.

Thanks!
