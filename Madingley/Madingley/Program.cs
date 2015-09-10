using System;

namespace Madingley
{

    /// <summary>
    /// The entry point for the model
    /// <todoM>Write model output to an output file</todoM>
    /// </summary>
    class Program
    {
        /// <summary>
        /// Starts a model run or set of model runs
        /// </summary>
        static void Main()
        {
            // Write out model details to the console
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Madingley model v. 0.3333333\n");
            Console.ForegroundColor = ConsoleColor.White;

            var modelSetupRoot = "../../../Model setup";
            var environmentDataRoot = "../../../Data/Original";
            var environmentFileName = "environment.json";

            var beginTime = DateTime.Now;

            var configuration = Madingley.Configuration.Loader.Load(modelSetupRoot);

            var useCache = false;
            var cachedEnvironment = (Common.Environment)null;

            if (System.IO.File.Exists(environmentFileName))
            {
                using (var reader = new System.IO.StreamReader(environmentFileName))
                {
                    cachedEnvironment = Madingley.Serialization.Environment.Deserialize(reader);
                }

                var partialEnvironment = Madingley.Environment.Loader.Load(environmentDataRoot, modelSetupRoot, false);

                useCache = cachedEnvironment.EqualsWithoutData(partialEnvironment);
            }

            var environment = (Common.Environment)null;

            if (useCache)
            {
                environment = cachedEnvironment;
            }
            else
            {
                environment = Madingley.Environment.Loader.Load(environmentDataRoot, modelSetupRoot, true);

                using (var writer = new System.IO.StreamWriter(environmentFileName))
                {
                    Madingley.Serialization.Environment.Serialize(environment, writer);
                }
            }

            var modelTime = DateTime.Now;

            Madingley.Model.RunTraditional(configuration, environment, Madingley.Output.Factory.Create);

            var endTime = DateTime.Now;
            var modelInterval = endTime - modelTime;

            Console.WriteLine("Model run finished");
            Console.WriteLine("Total elapsed time was {0} seconds", (endTime - beginTime).TotalSeconds);
            Console.WriteLine("Model setup time was {0} seconds", (modelTime - beginTime).TotalSeconds);
            Console.WriteLine("Model run time was {0} seconds", (endTime - modelTime).TotalSeconds);

            Console.ReadKey();
        }
    }
}