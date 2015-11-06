Prototype refactored C# Madingley model
------------------------------------

<not under active development>

This is the data, configuration and source code for the Madingley Model [http://www.madingleymodel.org/](http://www.madingleymodel.org/).

What's New
----------

The code has been developed from the original. It has been split into 5 modules, representing:
  * common data structures
  * configuration
  * environment
  * the model itself
  * output formatting

Other changes include:
  * Configuration files are now in separate folders
  * Previously hard-coded ecological process parameters are now stored in a configuration file
  * More impacts
  * Revised dispersal methods
  * Some unit tests representing small studies, 1 or 4 cells for 1 or 10 years
  * Cross platform library for manipulating netCDF, CSV and TSV files

External Libraries
------------------

The Madingley Model consumes and produces netCDF files for geospatial data [http://www.unidata.ucar.edu/software/netcdf/](http://www.unidata.ucar.edu/software/netcdf/),
and also standard CSV (comma-separated variables) and TSV (tab-separated variables) files.
This solution includes a set of projects called **SDSLite** to manipulate these file types;
it is a subset of **Scientific DataSet** [http://research.microsoft.com/en-us/projects/sds/](http://research.microsoft.com/en-us/projects/sds/) and [https://sds.codeplex.com/](https://sds.codeplex.com/).

SDSLite requires a platform dependent library available from [http://www.unidata.ucar.edu/software/netcdf/docs/getting_and_building_netcdf.html](http://www.unidata.ucar.edu/software/netcdf/docs/getting_and_building_netcdf.html).

### Windows

For Windows go to [http://www.unidata.ucar.edu/software/netcdf/docs/winbin.html](http://www.unidata.ucar.edu/software/netcdf/docs/winbin.html) and download the version of netCDF4 (without DAP) corresponding to your machine, either 32 or 64 bit. These are
currently: [http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-32.exe](http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-32.exe) or [http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-64.exe](http://www.unidata.ucar.edu/downloads/netcdf/ftp/netCDF4.3.3.1-NC4-64.exe)
When you install this library select the option to add its location to your system PATH, so that SDSLite can find it.

### Linux

For Linux install pre-built netCDF-C libraries. For example on Ubuntu:

`sudo apt-get install libnetcdf-dev`

### MacOS

Use homebrew [http://brew.sh/](http://brew.sh/) to install netcdf:

  brew install netcdf

Compilation
-----------

### Windows

Use Visual Studio to build the source files, the Community Edition of Visual Studio should be sufficient [https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx).
The model requires additional packages that are included in the packages directory, it is not necessary to use NuGet.

  * DynamicInterop.0.7.4 - to bind from C# to the netCDF library;
  * Newtonsoft.Json.7.0.1 - to store and load configuration, environment and run time state to and from json files;
  * NUnit.2.6.4 - for running the unit tests.

### Linux

For Linux, MonoDevelop [http://www.monodevelop.com/](http://www.monodevelop.com/) is able to build the solution and it can run under Mono [http://www.mono-project.com/](http://www.mono-project.com/).
See the installation instructions at [http://www.mono-project.com/docs/getting-started/install/linux/](http://www.mono-project.com/docs/getting-started/install/linux/).

### MacOS

For MacOS, Mono is also available - but unfortunately only in 32bit mode as a package. See here: [http://www.mono-project.com/docs/compiling-mono/mac/](http://www.mono-project.com/docs/compiling-mono/mac/).
64bit mode is required to run the Madingley Model.

Model Data
----------

Input data comes from either static netCDF files or **FetchClimate** [http://research.microsoft.com/en-us/projects/fetchclimate/](http://research.microsoft.com/en-us/projects/fetchclimate/).
Because the data is too large to be included in github directly, it is available as a zip file as part of the release. See the file Data/README_DATA.md for more details.
An internet connection to access FetchClimate to get additional climate data.

Model Configuration
-------------------

This solution contains sufficient configuration (in the "Model setup" folder) files to run the model, it is set to run a single cell for one year.

### FileLocationParameters.csv

This file identifies other file names to use for different aspects of the configuration.

Parameter | Type | Comment
--------- | ---- | -------
Mass Bin Filename | string | Only used for output
Environmental Data File | string | File containing environment layers (in "Environmental Data Layer List" folder)
Cohort Functional Group Definitions File | string | File containing cohort functional group definitions (in "Ecological Definition Files" folder)
Stock Functional Group Definitions File | string | File containing stock functional group definitions (in "Ecological Definition Files" folder)
Ecological parameters file | string | File containing Ecological parameters (in "Ecological Definition Files" folder)

Example:

```
Parameter,Value
Mass Bin Filename,MassBinDefinitions.csv
Environmental Data File,EnvironmentalDataLayers.csv
Cohort Functional Group Definitions File,CohortFunctionalGroupDefinitions.csv
Stock Functional Group Definitions File,StockFunctionalGroupDefinitions.csv
Ecological parameters file,EcologicalParameters.csv
```

### SimulationControlParameters.csv

The file specifies most of the changeable information needed by the model to run. It should be modified depending on the experimental purpose of the simulation, along with its counterpart the ‘Scenarios’ file.

Parameter | Type | Comment
--------- | ---- | -------
Timestep Units | "Day"/"Month"/"Year" | The temporal resolution at which the model runs; the length of time one round of calculations represents. (The model has been calibrated primary for a timestep of "month").
Length of simulation (years) | number | The length of time the simulation represents; the timescale of the model outputs.
Burn-in (years) | number | Apply impacts after this many years
Impact duration (years) | number | Apply impacts for this many years
Recovery duration (years) | number | Not used
Bottom Latitude | number | The southernmost boundary of the grid to be modelled. Input unit is decimal degrees.
Top Latitude | number | The northernmost boundary of the grid to be modelled. Input unit is decimal degrees.
Leftmost Longitude | number | The westernmost boundary of the grid to be modelled. Input unit is decimal degrees.
Rightmost Longitude | number | The easternmost boundary of the grid to be modelled. Input unit is decimal degrees.
Grid Cell Size | number | The length of one side of the square cells which will make up the model grid. A smaller value means a higher spatial resolution, and more cells within the grid. For example, a 1x1 degree grid with cell size 0.1 will be a 100 cell grid; with cell size 0.05 it will be a 400 cell grid. Decreasing cell size makes the model considerably more computationally expensive.
Specific Location File | string/blank | File name to use to run model at specific grid cells, leave blank to run for whole grid.
Plankton size threshold | number | Specifies the maximum size a single plankton individual can reach before it becomes motile. Smaller than this threshold, plankton move purely by advection. Larger than this threshold and they are assumed to move through diffusive dispersion (swimming).
Draw Randomly | "yes"/"no" | Distributes the initial stock and cohort seeding randomly across the grid, and randomises the order in which cohorts act within a timestep introducing a degree of stochasticity to the simulation (generally left as yes).
Extinction Threshold | number | The number of individuals remaining in a cohort for it to be considered extant.
Maximum Number Of Cohorts | number | The maximum number of cohorts allowed within a single cell, after which the model merges the most similar cohorts into single larger approximations. Necessary for computational reasons: increasing the maximum number of cohorts reduces uncertainty and increases the power of the model at the cost of increased computational expense.
Run Cells In Parallel | "yes"/"no" | Assigns different cells to different processor cores when running a simulation.
Run Simulations In Parallel | "yes"/"no" | Assigns different simulations to different processor cores when running multiple simulations (either different scenarios or repeats of the same).
Run Single Realm | "marine"/"terrestrial"/blank | Prevents the model from running both marine and terrestrial biomes in the same simulation. Input as ‘marine’ or ‘terrestrial’ for only marine or terrestrial realm respectively, leave blank to run both realms.
Impact Cell Index | ranges/blank | This accepts a list of ranges separated with ";". Each range may be either a single number or a pair NN-MM, for example: ```2;3;5-7;13```
Dispersal only | "yes"/"no" | Runs only the dispersal aspects of the model, ignoring ecological processes (rarely used for experimentation).
Dispersal only type | "diffusion"/"advection"/"responsive"/blank | Specifies the method of dispersal allowed when ‘Dispersal Only’ is set to ‘yes’ (rarely used).

Example:

```
Parameter,Value
Timestep Units,Month
Length of simulation (years),10
Burn-in (years),0
Impact duration (years),0
Recovery duration (years),0
Bottom Latitude,46
Top Latitude,49
Leftmost Longitude,2
Rightmost Longitude,5
Grid Cell Size,1
Specific Location File,
Plankton size threshold,0.01
Draw Randomly,no
Extinction Threshold,1
Maximum Number Of Cohorts,1000
Run Cells In Parallel,no
Run Simulations In Parallel,no
Run Single Realm,
Impact Cell Index,
Dispersal only,no
Dispersal only type,
```

### Ecological Definition Files/CohortFuctionalGroupDefinitions.csv

Defines the cohort specific functional groups to be initialised in the model.

Column | Comment
------ | -------
DEFINITION_Heterotroph/Autotroph | Defines whether this functional group is heterotrophic or autotrophic - cohorts are all "heterotroph" currently
DEFINITION_Nutrition source | The feeding mode of organisms in this functional group, values can be: "Herbivore", "Omnivore" or "Carnivore"
DEFINITION_Diet | allspecial for filter-feeding baleen whales only, blank for all else.
DEFINITION_Realm | The realm in which the functional group is found, values are "Marine" or "Terrestrial"
DEFINITION_Mobility | What is the mobility of the organisms in each functional group: currently permissible values "planktonic" or "mobile"
DEFINITION_Reproductive strategy | Is the functional group "Iteroparous" or "Semelparous"
DEFINITION_Endo/Ectotherm | Metabolic pathway: "Ectotherm" or "Endotherm"
PROPERTY_Herbivory assimilation | The proportion of plant matter ingested that is assimilated by organisms in each functional group (unitless)
PROPERTY_Carnivory assimilation | The proportion of heterotroph prey biomass ingested that is assimilated by organisms in each functional group (unitless)
PROPERTY_Proportion suitable time active | The proportion of the environmentally suitable time in each timestep that organisms are active (unitless)
PROPERTY_Minimum mass | The minimum mass for any organism in each functional group (g)
PROPERTY_Maximum mass | The maximum mass for any organism in each functional group (g)
PROPERTY_Initial number of GridCellCohorts | The number of cohorts initialised into the model at the start of each simulation
NOTES_group description | A description of what each functional group represents

Example:

```
DEFINITION_Heterotroph/Autotroph,DEFINITION_Nutrition source,DEFINITION_Diet,DEFINITION_Realm,DEFINITION_Mobility,DEFINITION_Reproductive strategy,DEFINITION_Endo/Ectotherm,PROPERTY_Herbivory assimilation,PROPERTY_Carnivory assimilation,PROPERTY_Proportion suitable time active,PROPERTY_Minimum mass,PROPERTY_Maximum mass,PROPERTY_Initial number of GridCellCohorts,NOTES_group description
Heterotroph,Omnivore,AllSpecial,Marine,Mobile,iteroparity,Endotherm,0,0.8,0.5,10000,150000000,100,Baleen whales
Heterotroph,Carnivore,All,Marine,Mobile,iteroparity,Endotherm,0,0.8,0.5,100,50000000,100,Carnivorous endotherms
Heterotroph,Carnivore,All,Marine,Mobile,iteroparity,Ectotherm,0,0.8,0.5,0.0001,2000000,100,Carnivorous ectotherms (inc eating zooplankton)
Heterotroph,Carnivore,All,Marine,Mobile,semelparity,Ectotherm,0,0.8,0.5,0.0001,2000000,100,Carnivorous ectotherms (inc eating zooplankton)
Heterotroph,Herbivore,Planktivore,Marine,Mobile,iteroparity,Ectotherm,0.7,0,0.5,0.0001,10000,100,"Herbivorous ectos (inc e.g. krill, assumed larger fishes)"
Heterotroph,Herbivore,Planktivore,Marine,Mobile,semelparity,Ectotherm,0.7,0,0.5,0.0001,10000,100,"Herbivorous ectos (inc e.g. krill, assumed larger fishes)"
Heterotroph,Omnivore,All,Marine,Mobile,iteroparity,Ectotherm,0.6,0.64,0.5,0.00001,100000,100,Omnivorous ectos
Heterotroph,Omnivore,All,Marine,Mobile,semelparity,Ectotherm,0.6,0.64,0.5,0.00001,100000,100,Omnivorous ectos
Heterotroph,Herbivore,Planktivore,Marine,Planktonic,iteroparity,Ectotherm,0.7,0,0.5,0.00001,0.1,100,Obligate zooplankton
Heterotroph,Herbivore,Planktivore,Marine,Planktonic,semelparity,Ectotherm,0.7,0,0.5,0.00001,0.1,100,Obligate zooplankton
Heterotroph,Herbivore,All,Terrestrial,Mobile,iteroparity,Endotherm,0.5,0,0.5,1.5,5000000,112,None
Heterotroph,Carnivore,All,Terrestrial,Mobile,iteroparity,Endotherm,0,0.8,0.5,3,700000,112,None
Heterotroph,Omnivore,All,Terrestrial,Mobile,iteroparity,Endotherm,0.4,0.64,0.5,3,1500000,112,None
Heterotroph,Herbivore,All,Terrestrial,Mobile,semelparity,Ectotherm,0.5,0,0.5,0.0004,1000,112,None
Heterotroph,Carnivore,All,Terrestrial,Mobile,semelparity,Ectotherm,0,0.8,0.5,0.0008,2000,112,None
Heterotroph,Omnivore,All,Terrestrial,Mobile,semelparity,Ectotherm,0.4,0.64,0.5,0.0004,2000,112,None
Heterotroph,Herbivore,All,Terrestrial,Mobile,iteroparity,Ectotherm,0.5,0,0.5,1,300000,112,None
Heterotroph,Carnivore,All,Terrestrial,Mobile,iteroparity,Ectotherm,0,0.8,0.5,1.5,2000000,112,None
Heterotroph,Omnivore,All,Terrestrial,Mobile,iteroparity,Ectotherm,0.4,0.64,0.5,1.5,55000,112,None
```

### Ecological Definition Files/StockFuctionalGroupDefinitions.csv

Defines stock specific functional groups to be initialised in the model.

Column | Comment
------ | -------
DEFINITION_Heterotroph/Autotroph | Defines whether this functional group is heterotrophic or autotrophic - stocks are all "autotroph" currently
DEFINITION_Nutrition source | Where the organisms obtain their nutrition from: currently "photosynthesis" for all stocks
DEFINITION_Diet | Not used
DEFINITION_Realm | The realm in which the functional group is found, values are "Marine" or "Terrestrial"
DEFINITION_Mobility | What is the mobility of the organisms in each functional group: currently permissible values "planktonic" or "sessile"
DEFINITION_Leaf strategy | The leaf strategy for functional groups - current values are "deciduous" or "evergreen" and apply to terrestrial stocks only
PROPERTY_Herbivory assimilation | Not used
PROPERTY_Carnivory assimilation | Not used
PROPERTY_Proportion herbivory | Not used
PROPERTY_Individual mass | The mass of an individual organism in each functional group (currently only applied to phytoplantkton)

Example:

```
DEFINITION_Heterotroph/Autotroph,DEFINITION_Nutrition source,DEFINITION_Diet,DEFINITION_Realm,DEFINITION_Mobility,DEFINITION_Leaf strategy,PROPERTY_Herbivory assimilation,PROPERTY_Carnivory assimilation,PROPERTY_Proportion herbivory,PROPERTY_Individual mass
Autotroph,Photosynthesis,,Marine,Planktonic,na,,,,0.0001
Autotroph,Photosynthesis,,Terrestrial,Sessile,Deciduous,,,,0
Autotroph,Photosynthesis,,Terrestrial,Sessile,Evergreen,,,,0
```

### Ecological Definition Files/EcologicalParameters.csv

Parameters for ecological processes.

### Environmental Data Layer List/EnvironmentalDataLayers.csv

Lists the exogenous environmental datasets to be used in the model.

Column | Type | Comment
------ | ---- | -------
Source | "Local"/"FetchClimate" | Data set is a local file or a call to the FetchClimate service
Folder | string/blank | The  folder in which the dataset is stored (relative to the directory in which the model executable is running) ("Local" only)
Filename | string/blank | The filename of the dataset ("Local" only)
Extension | string/blank | The file-extension of the dataset ("Local" only)
Dataset Name | string | The name of the variable inside the data file that is to be used
Filetype | "nc"/"csv" | The file-type of the data set file "nc" for NetCDF, "csv" for comma-separated values
Internal Layer Name | string | The name with which the environmental data will be referred to inside the model
Static | "Y" | Is the environmental layer to be used as a constant (e.g. an annual average) or does it vary through time? (Currently only "Y")
Resolution | "year"/"month" | The temporal resolution of the data to be imported
Units | string | The units of the environmental variable

Example:

```
Source,Folder,Filename,Extension,Dataset Name,Filetype,Internal Layer Name,Static,Resolution,Units
Local,,LandSeaMask,.nc,land_sea_mask,nc,LandSeaMask,Y,year,
Local,Land,hanpp_2005,.nc,HANPP,nc,HANPP,Y,year,gC/m2/year
Local,Land,AvailableWaterCapacity,.nc,AWC,nc,AWC,Y,year,mm
Local,Land,NPP,.nc,NPP,nc,LandNPP,Y,month,gC/m2/day
Local,Ocean,averaged_u_50y_top100m_monthly,.nc,uVel,nc,uVel,Y,month,cm/s
Local,Ocean,averaged_v_50y_top100m_monthly,.nc,vVel,nc,vVel,Y,month,cm/s
FetchClimate,,,,temperature,,Temperature,Y,month,degC
FetchClimate,,,,land_dtr,,LandDTR,Y,month,degC
FetchClimate,,,,temperature_ocean,,OceanTemp,Y,month,degC
FetchClimate,,,,precipitation,,Precipitation,Y,month,degC
FetchClimate,,,,frost,,FrostDays,Y,month,degC
```

### Initial Model State Setup/Scenarios.csv

Is one of the files to be edited depending on the purpose of the experiment being run. This is where the scenarios to be simulated by the model are defined, and forms the counterpart to the ‘Ecosystem Model Initialisation’ file. Currently, it is used to define impact scenarios and repeated simulations.

Column | Comment
------ | -------
label | Label this scenario
npp | NPP settings, "no 0.0 0" for no NPP impacts.
temperature | Temperature settings
harvesting | Harvesting settings
simulation number | Number of simulations to run. ‘0’ will mean that scenario is not run, while ‘10’ means the scenario will be repeated ten times. Each repeat will be different due to the randomness built into the initial model seeding (this can be turned off in the Initialisation File). Each repeated simulation has its own unique output file, which is denoted by an index number in the output filename.

Example:

```
label,npp,temperature,harvesting,simulation number
NI,no 0.0 0,no 0.0,no 0.0,1
```

### Initial Model State Setup/SpecificLocations.csv

If a file name is specified in SimulationControlParameters.csv for "Specific Location File", this is a list of cell centre coordinates of specific grid cells for which model simulations are to be performed.

Column | Comment
------ | -------
Latitude | The latitude of the cell centre (Decimal degrees)
Longitude | The longitude of the cell centre (Decimal degrees)

Example:

```
Latitude,Longitude
48,3
```

Running the Model
-----------------

The Madingley project is a simple command line application that loads the configuration, environment data, passes them to the model and stores the output.
Select this project as the start-up project and run it to run the model.

### LICENCE

You can find license details in Licence.txt file provided with this project or online at https://github.com/Madingley/Madingley/blob/master/Licence.txt.
