 ======================
|   <b>PADI-DSTM</b>   |
 ======================

 
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

<b> CONTENTS: </b>
- How to start the master server
- How to start the data servers
- How to work with the client interface

---------------------------------

<b> HOW TO START THE MASTER SERVER </b>

- Open the project solution (.sln) on Visual Studio (if needed)
- (Re)Build the solution (if needed)
- Run the MASTER-SERVER project (with or without debug mode)
- When the console application opens up, insert the desired PORT for the master server
NOTE: The master server is always working under the localhost ip/hostname

<b> HOW TO START THE DATA SERVERS </b>

- Open the project solution (.sln) on Visual Studio (if needed)
- (Re)Build the solution (if needed)
- Run the DATA-SERVER project (with our without debug mode)
- When the console application opens up, insert the SERVER PORT, then the MASTER PORT and the MASTER HOSTNAME (localhost in this case)
- You can run as many data servers as you want, you just need to specify different ports for each one (if they are running in the same machine)

<b> HOW TO WORK WITH THE CLIENT INTERFACE </b>

- The client interface (ClientForms project) provides all the PADI-DSTM interface methods through the click of buttons.
- First click the 'connect' button to connect to the master server
- Click anything else to create / access / read or write on PadInts and even Fail/Freeze/Recover any data server.
- You can also run as many clients as you want.

 ===========
|    END    |
 ===========
