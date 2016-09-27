The Evaluation Engine project is made up of four repositories:

**Evaluation Engine Web UI:**
<repo url here>

**Evaluation Engine Service Broker:**
<repo url here>

**Evaluation Engine Console Application:**
<repo url here>

**Evaluation Engine Statistical Component:**
<repo url here>

The Evaluation Engine Web UI project is the front end website for running reports. It communicates with it's own SQL Server database for controlling user accounts, hashed password histories and display options. This is included in this repo via SQL Source Control, in the 'Database' folder. There is also another database included in this project, in the 'WebRServerMessages' folder, where student IDs are uploaded to run reports and where results are written to by the statistical component to later be retrieved by the website. The data warehouse, where the student data (with student IDs replaced by study IDs) exist is included in the folder DataWarehouse. Finally, there is a Postgres database called 'Crosswalk' which maps hashed student IDs to study IDs.

The Service Broker project is used to communicate between the website database and the console application (mostly for hashing student IDs), as well as to send alert emails about reports finishing or erroring out. 

The Console Application is used for two purposes. First, it hashes student IDs when new data is recieved, to create a study ID that can be freely shared without revealing personally identifiable information. Second, it is used to hash student IDs when they are submitted via the website, so those study IDs can be transmitted and used in calcuations by the Statistical Component. 

Finally, the Statistical Component is where the actual calculations occur. It is deployed to its own R server, with tasks managed by a Gearman instance.

### Setup:
* If you haven't already, check out the Evaluation Engine Web UI project and create the various databases included with the project.

* Check out this repo and update the App.config file to add your AES256 key and Key64 values, which will be shared between the various applications comprising Evaluation Engine. Also update the connection strings to point to the crosswalk database. In order for tests to work correctly, also update EvaluationEngineConsole.Test.config.

* This applciation will live on a Windows machine which can connect to the crosswalk database, but not the other databases. The Service Broker application will handle communication via the WebRServerMessages database.