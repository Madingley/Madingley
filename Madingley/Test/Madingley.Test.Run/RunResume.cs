using System;
using System.Linq;
using System.Threading;

using NUnit.Framework;

using Madingley.Output;
using Madingley.Test.Input;

namespace Madingley.Test.Run
{
    public class RunResume
    {
        public static T RunPauseAndResumeMadingley<T>(Data data, int pauseIterations, int maxIterations, Action<T> optionalHalfTest)
        {
            var pathToModelSetup = "Model setup for test";
            var pathToData = "Data for test";

            Madingley.Test.Common.Common.CopyDirectory(pathToModelSetup, data.PathToModelSetup);

            Madingley.Test.Common.Environment.CreateDirectories(pathToData);
            Madingley.Test.Common.Environment.CreateFiles(pathToData, data.Terrestrial);

            var configuration = Madingley.Configuration.Loader.Load(pathToModelSetup);
            var environment = Madingley.Environment.Loader.Load(pathToData, pathToModelSetup, true);

            var progress = new ProgressReporter();
            var cancellationToken = CancellationToken.None;

            var halfResult = Madingley.Model.Run<T>(null, configuration, environment, Madingley.Output.Factory.Create, progress, cancellationToken).Skip(pauseIterations - 1).Take(1).Last();

            if (optionalHalfTest != null)
            {
                optionalHalfTest(halfResult.Item2);
            }

            var result = Madingley.Model.Run<T>(halfResult.Item1, configuration, environment, Madingley.Output.Factory.Create, progress, cancellationToken).Last();

            return result.Item2;
        }

        public static void CommonTestResumable(CommonTestType commonTestType, Func<Data, int, int, Action<MadingleyModelOutputDataSets>, MadingleyModelOutputDataSets> runner)
        {
            var data = Data.LookupCommonTestTypePaths(commonTestType, null);

            var rowCount = data.RowCount;
            var pauseMonthsComplete = (int)(rowCount / 2);
            var maxIterations = (int)(rowCount / 12);
            var pauseIterations = (int)(maxIterations / 2);

            Action<MadingleyModelOutputDataSets> optionalHalfTest = halfResult => Madingley.Test.Run.Common.TestMadingleyModelOutputDataSets(commonTestType, pauseMonthsComplete, halfResult);

            var result = runner.Invoke(data, pauseIterations, maxIterations, optionalHalfTest);

            Madingley.Test.Run.Common.TestMadingleyModelOutputDataSets(commonTestType, null, result);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Cows"), Category("1 Cell"), Category("10 Years"), Category("Resumable")]
        public void Test_Madingley_Cows_1Cell_10Years_Resumable()
        {
            CommonTestResumable(CommonTestType.COWS_1_CELL_10_YEARS, RunPauseAndResumeMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Cows"), Category("4 Cells"), Category("10 Years"), Category("Resumable")]
        public void Test_Madingley_Cows_4Cells_10Years_Resumable()
        {
            CommonTestResumable(CommonTestType.COWS_4_CELLS_10_YEARS, RunPauseAndResumeMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Terrestrial"), Category("4 Cells"), Category("10 Years"), Category("Resumable")]
        public void Test_Madingley_All_4Cells_10Years_Resumable()
        {
            CommonTestResumable(CommonTestType.TERRESTRIAL_4_CELLS_10_YEARS, RunPauseAndResumeMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Marine"), Category("1 Cell"), Category("10 Years"), Category("Resumable")]
        public void Test_Madingley_Marine_1Cell_10Years_Resumable()
        {
            CommonTestResumable(CommonTestType.MARINE_1_CELL_10_YEARS, RunPauseAndResumeMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Marine"), Category("4 Cells"), Category("10 Years"), Category("Resumable")]
        public void Test_Madingley_Marine_4Cells_10Years_Resumable()
        {
            CommonTestResumable(CommonTestType.MARINE_4_CELLS_10_YEARS, RunPauseAndResumeMadingley<MadingleyModelOutputDataSets>);
        }
    }
}
