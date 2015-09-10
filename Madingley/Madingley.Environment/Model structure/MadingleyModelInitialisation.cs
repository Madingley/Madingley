using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Microsoft.Research.Science.Data;

namespace Madingley
{
    /// <summary>
    /// Initialization information for Madingley model simulations
    /// </summary>
    public static class MadingleyModelInitialisation
    {
        /// <summary>
        /// Reads the initalization file to get information for the set of simulations to be run
        /// </summary>
        /// <param name="simulationInitialisationFilename">The name of the initialization file with information on the simulations to be run</param>
        /// <param name="definitionsFilename">Definitions file name</param>
        /// <param name="environmentDataRoot">The path to folder which contains the data inputs</param>
        /// <param name="inputPath">The path to folder which contains the model inputs</param>
        public static Tuple<Madingley.Common.Environment, SortedList<string, EnviroData>> Load(
            string simulationInitialisationFilename,
            string definitionsFilename,
            string environmentDataRoot,
            string inputPath,
            bool readData)
        {
            // Write to console
            Console.WriteLine("Initializing model...\n");

            // Construct file names
            var simulationInitialisationFileName = System.IO.Path.Combine(inputPath, simulationInitialisationFilename);
            var definitionsFileName = System.IO.Path.Combine(inputPath, definitionsFilename);

            string SimulationFileString = "msds:csv?file=" + simulationInitialisationFileName + "&openMode=readOnly";
            string DefinitionsFileString = "msds:csv?file=" + definitionsFileName + "&openMode=readOnly";

            var e = new Madingley.Common.Environment();

            e.FileNames.Add(simulationInitialisationFileName);
            e.FileNames.Add(definitionsFileName);

            var EnviroStack = new SortedList<string, EnviroData>();

            // Read in the simulation data
            DataSet InternalData = DataSet.Open(SimulationFileString);

            // Get the names of parameters in the initialization file
            var VarParameters = InternalData.Variables[1].GetData();

            // Get the values for the parameters
            var VarValues = InternalData.Variables[0].GetData();

            // Loop over the parameters
            for (int row = 0; row < VarParameters.Length; row++)
            {
                // Switch based on the name of the parameter, and write the value to the appropriate field
                switch (VarParameters.GetValue(row).ToString().ToLower())
                {
                    case "grid cell size":
                        e.CellSize = Convert.ToDouble(VarValues.GetValue(row));
                        break;
                    case "bottom latitude":
                        e.BottomLatitude = Convert.ToDouble(VarValues.GetValue(row));
                        break;
                    case "top latitude":
                        e.TopLatitude = Convert.ToDouble(VarValues.GetValue(row));
                        break;
                    case "leftmost longitude":
                        e.LeftmostLongitude = Convert.ToDouble(VarValues.GetValue(row));
                        break;
                    case "rightmost longitude":
                        e.RightmostLongitude = Convert.ToDouble(VarValues.GetValue(row));
                        break;
                    case "specific location file":
                        if (VarValues.GetValue(row).ToString() != "")
                        {
                            e.SpecificLocations = true;
                            var specificLocationsFile = VarValues.GetValue(row).ToString();
                            var fileName = System.IO.Path.Combine(inputPath, "Initial Model State Setup", specificLocationsFile);
                            e.FileNames.Add(fileName);

                            ReadSpecificLocations(e, fileName);
                        }
                        break;
                }
            }

            InternalData.Dispose();

            // Read in the definitions data
            InternalData = DataSet.Open(DefinitionsFileString);

            // Get the names of parameters in the initialization file
            VarParameters = InternalData.Variables[1].GetData();

            // Get the values for the parameters
            VarValues = InternalData.Variables[0].GetData();

            // Loop over the parameters
            for (int row = 0; row < VarParameters.Length; row++)
            {
                // Switch based on the name of the parameter, and write the value to the appropriate field
                switch (VarParameters.GetValue(row).ToString().ToLower())
                {
                    case "environmental data file":
                        {
                            // Read environmental data layers
                            var environmentalLayerFile = VarValues.GetValue(row).ToString();
                            var fileName = System.IO.Path.Combine(inputPath, "Environmental Data Layer List", environmentalLayerFile);
                            e.FileNames.Add(fileName);

                            EnviroStack = ReadEnvironmentalLayers(e, fileName, environmentDataRoot, readData);
                        }
                        break;
                }
            }

            InternalData.Dispose();

            return Tuple.Create(e, EnviroStack);
        }

        /// <summary>
        /// Read in the specified locations in which to run the model
        /// </summary>
        /// <param name="environment">Environment object</param>
        /// <param name="fileName">The name of the file with specific locations information</param>
        public static void ReadSpecificLocations(Madingley.Common.Environment environment, string fileName)
        {
            var CellList = new List<Tuple<int, int>>();

            List<double> LatitudeList = new List<double>();
            List<double> LongitudeList = new List<double>();

            Console.WriteLine("Reading in specific location data");
            Console.WriteLine("");

            // construct file name
            string FileString = "msds:csv?file=" + fileName + "&openMode=readOnly";

            // Read in the data
            DataSet InternalData = DataSet.Open(FileString);

            foreach (Variable v in InternalData.Variables)
            {
                //Get the name of the variable currently referenced in the dataset
                string HeaderName = v.Name;
                //Copy the values for this variable into an array
                var TempValues = v.GetData();

                switch (HeaderName.ToLower())
                {
                    // Add the latitude and longitude values to the appropriate list
                    case "latitude":
                        for (int ii = 0; ii < TempValues.Length; ii++) LatitudeList.Add(Convert.ToDouble(TempValues.GetValue(ii).ToString()));
                        break;
                    case "longitude":
                        for (int ii = 0; ii < TempValues.Length; ii++) LongitudeList.Add(Convert.ToDouble(TempValues.GetValue(ii).ToString()));
                        break;
                    default:
                        Console.WriteLine("Variable defined in the specific location file but not processed: ", HeaderName);
                        break;
                }
            }

            InternalData.Dispose();

            // Loop over cells defined in the specific locations file
            for (int ii = 0; ii < LatitudeList.Count; ii++)
            {
                // Get the longitude and latitude indices for the current grid cell
                var latitudeIndex = (int)Math.Floor((LatitudeList.ElementAt(ii) - environment.BottomLatitude) / environment.CellSize);
                var longitudeIndex = (int)Math.Floor((LongitudeList.ElementAt(ii) - environment.LeftmostLongitude) / environment.CellSize);

                // Add these indices to the list of active cells
                CellList.Add(Tuple.Create(latitudeIndex, longitudeIndex));
            }

            environment.FocusCells = CellList;
        }

        /// <summary>
        /// Reads the environmental layers listed in the specified file containing a list of environmental layers
        /// </summary>
        /// <param name="e">Environment object.</param>
        /// <param name="inputEnvironmentalLayerFileName">The name of the file containing the list of environmental layers</param>
        /// <param name="environmentDataRoot">The path to folder which contains the data inputs</param>
        public static SortedList<string, EnviroData> ReadEnvironmentalLayers(
            Madingley.Common.Environment e,
            string inputEnvironmentalLayerFileName,
            string environmentDataRoot,
            bool readData)
        {
            Console.WriteLine("Reading in environmental data:");

            // Declare lists to hold the information required to read the environmental layers
            List<string> Sources = new List<string>();
            List<string> Folders = new List<string>();
            List<string> Filenames = new List<string>();
            List<string> DatasetNames = new List<string>();
            List<string> FileTypes = new List<string>();
            List<string> LayerName = new List<string>();
            List<string> StaticLayer = new List<string>();
            List<string> Extensions = new List<string>();
            List<string> Resolutions = new List<string>();
            List<string> MethodUnits = new List<string>();

            // Variable to store the file name of the environmental data files
            string TempFilename;

            // Construct the full URI for the file  containing the list of environmental layers
            string FileString = "msds:csv?file=" + inputEnvironmentalLayerFileName + "&openMode=readOnly";

            StreamReader r_env = new StreamReader(inputEnvironmentalLayerFileName);

            var EnviroStack = new SortedList<string, EnviroData>();

            string l;
            char[] comma = ",".ToCharArray();

            string[] f;
            int col;
            // Read in the data
            l = r_env.ReadLine();
            while (!r_env.EndOfStream)
            {
                l = r_env.ReadLine();
                // Split fields by commas
                f = l.Split(comma);
                //zero the column index
                col = 0;
                // Lists of the different fields
                Sources.Add(f[col++]);
                Folders.Add(f[col++]);
                Filenames.Add(f[col++]);
                Extensions.Add(f[col++]);
                DatasetNames.Add(f[col++]);
                FileTypes.Add(f[col++]);
                LayerName.Add(f[col++]);
                StaticLayer.Add(f[col++]);
                Resolutions.Add(f[col++]);
                MethodUnits.Add(f[col++]);
            }

            for (int ii = 0; ii < MethodUnits.Count; ii++)
            {
                e.Units.Add(LayerName[ii], MethodUnits[ii]);
            }

            // Check that there are the same number of values for all parameters
            Debug.Assert(Folders.Count() == Filenames.Count() && Filenames.Count() == DatasetNames.Count() && DatasetNames.Count() == FileTypes.Count() && FileTypes.Count() == LayerName.Count(),
                "Error in Environmental Data Layer import lists - unequal number of filenames, dataset names, filetypes and datalayer names");

            // Loop over parameter values
            for (int ii = 0; ii < Filenames.Count(); ii++)
            {
                Console.Write("\r{0} Variable {1} of {2}: {3}\n", Sources[ii], ii + 1, Filenames.Count, Filenames[ii]);
                // If the layers are not static, then suffix the file name with '1' - not currently implemented
                if (StaticLayer[ii].ToLower().Equals("n"))
                {
                    Debug.Fail("This option is currently not supported");
                    Filenames[ii] = Filenames[ii] + "1";
                }
                if (Sources[ii].ToLower().Equals("local"))
                {
                    // For layers where the file format is ESRI ASCII grid, the dataset name is the same as the file name
                    if (FileTypes[ii].ToLower().Equals("esriasciigrid"))
                    {
                        DatasetNames[ii] = Filenames[ii];
                    }
                    // Generate the appropriate file name for the environmental data layer
#if true
                    TempFilename = System.IO.Path.Combine(environmentDataRoot, Folders[ii], Filenames[ii]);

                    Filenames[ii] = System.IO.Path.ChangeExtension(TempFilename, Extensions[ii]);
#else
                    if (Folders[ii].ToLower().Equals("input"))
                    {
                        TempFilename = "input/Data/" + Filenames[ii];
                    }
                    else
                    {
                        TempFilename = Folders[ii] + "/" + Filenames[ii];
                    }
                    Filenames[ii] = TempFilename + Extensions[ii];
#endif
                    // Read in and store the environmental data
                    if (File.Exists(Filenames[ii]))
                    {
                        if (readData)
                        {
                            EnviroStack.Add(LayerName[ii], new EnviroData(Filenames[ii], DatasetNames[ii], FileTypes[ii], Resolutions[ii], MethodUnits[ii]));
                        }
                        else
                        {
                            EnviroStack.Add(LayerName[ii], null);
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("ERROR: Cannot find data file: {0} ({1}), have you added the data files from https://github.com/Madingley/Madingley/releases?", Filenames[ii], System.IO.Path.GetFullPath(Filenames[ii])));
                    }
                }
                else if (Sources[ii].ToLower().Equals("fetchclimate"))
                {

                    if (!EnviroStack.ContainsKey(LayerName[ii]))
                    {
                        if (readData)
                        {
                            if (e.SpecificLocations)
                            {
                                EnviroStack.Add(LayerName[ii], new EnviroData(DatasetNames[ii], Resolutions[ii], e.BottomLatitude, e.LeftmostLongitude, e.TopLatitude, e.RightmostLongitude, e.CellSize, e.FocusCells, EnvironmentalDataSource.ANY));
                            }
                            else
                            {
                                EnviroStack.Add(LayerName[ii], new EnviroData(DatasetNames[ii], Resolutions[ii], e.BottomLatitude, e.LeftmostLongitude, e.TopLatitude, e.RightmostLongitude, e.CellSize, EnvironmentalDataSource.ANY));
                            }
                        }
                        else
                        {
                            EnviroStack.Add(LayerName[ii], null);
                        }
                    }
                }
            }

            return EnviroStack;
        }
    }
}
