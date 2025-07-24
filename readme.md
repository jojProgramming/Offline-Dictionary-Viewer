# Offline Dictionary
 by JOJProgramming

### What is it?
![dictionary](https://github.com/user-attachments/assets/ea0bc67a-2a1f-406b-908c-58160ecf1aca)
Offline Dictionary provides an interface to query the dictionary.
The application allows the offline viewing of SQLite 3 databases in the 
style laid out in ```OfflineDictionary/DB_ER_Diagram.png```  
This may be useful in offline environments.  
  
You can download a free dataset from [Wiktionary](https://dumps.wikimedia.org/enwiki/) and process it to meet 
this specification.

### Why?
The primary aim of this project was to experiment with Avalonia and SQLite and DotNet libraries
that enable this.

### Can I use this application?
While the app is usable on supported platforms, it is not reasonably 
suited to production use. However:
 + The project has been made somewhat modular, and could reasonably be expanded for multiplatform support, 
but does not use some modern patterns such as dependency injection, as
this was not relevant to the learning exercise. 
 + Additionally, the project has low test coverage, which would 
need to be expanded to improve stability in production environments.
 + The project is licenced under GPL 3, so you can make the required improvements
to make the application production ready.
