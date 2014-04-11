PADI Project - PADI-DSTM
 ========================
|      README FILE       |
|       Grupo  16        |
|                        |
| Samuel Coelho,   69350 | 
| Marcos Germano,  67921 |
| Rafael Baltazar, 70116 |
|                        |
 ========================

This file contains a brief explanation on how to start the servers in the PADI-DSTM project.

CONTENTS:
1. How to start the master server
2. How to start the data servers
3. How to work with the client interface

---------------------------------

1. HOW TO START THE MASTER SERVER

- Open the project solution (.sln) on Visual Studio (if needed)
- (Re)Build the solution (if needed)
- Run the MASTER-SERVER project (with or without debug mode)
- When the console application opens up, insert the desired PORT for the master server
NOTE: The master server is always working under the localhost ip/hostname

2. HOW TO START THE DATA SERVERS

- Open the project solution (.sln) on Visual Studio (if needed)
- (Re)Build the solution (if needed)
- Run the DATA-SERVER project (with our without debug mode)
- When the console application opens up, insert the SERVER PORT, then the MASTER PORT and the MASTER HOSTNAME (localhost in this case)

3. HOW TO WORK WITH THE CLIENT INTERFACE

- The client interface (ClientForms project) provides all the PADI-DSTM interface methods through the click of buttons.
- First click the 'connect' button to connect to the master server
- Click anything else to create / access / read or write on PadInts and even Fail/Freeze/Recover any data server.

 ===========
|    END    |
 ===========
