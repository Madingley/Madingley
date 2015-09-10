using System;
using System.Linq;
using System.Threading;

using NUnit.Framework;

using Madingley.Output;
using Madingley.Test.Input;

namespace Madingley.Test.Run
{
    public class Run
    {
        public static T RunMadingley<T>(Data data)
        {
            var pathToModelSetup = @"Model setup for test";
            var pathToData = @"Data for test";

            Madingley.Test.Common.Common.CopyDirectory(pathToModelSetup, data.PathToModelSetup);

            Madingley.Test.Common.Environment.CreateDirectories(pathToData);
            Madingley.Test.Common.Environment.CreateFiles(pathToData, data.Terrestrial);

            var configuration = Madingley.Configuration.Loader.Load(pathToModelSetup);
            var environment = Madingley.Environment.Loader.Load(pathToData, pathToModelSetup, true);

            var progress = new ProgressReporter();
            var cancellationToken = CancellationToken.None;

            var result = Madingley.Model.Run<T>(null, configuration, environment, Madingley.Output.Factory.Create, progress, cancellationToken).Last();

            return result.Item2;
        }

        public static void CommonTest(CommonTestType commonTestType, Func<Data, MadingleyModelOutputDataSets> runner)
        {
            var data = Data.LookupCommonTestTypePaths(commonTestType, null);

            var result = runner.Invoke(data);

            Madingley.Test.Run.Common.TestMadingleyModelOutputDataSets(commonTestType, null, result);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Cows no NPP"), Category("1 Cell"), Category("1 Year"), Category("Not resumable")]
        public void Test_Madingley_Cows_No_NPP_1Cell_1Year()
        {
            CommonTest(CommonTestType.COWS_NO_NPP_1_CELL_1_YEAR, RunMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Cows"), Category("1 Cell"), Category("1 Year"), Category("Not resumable")]
        public void Test_Madingley_Cows_1Cell_1Year()
        {
            CommonTest(CommonTestType.COWS_1_CELL_1_YEAR, RunMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Cows"), Category("1 Cell"), Category("10 Years"), Category("Not resumable")]
        public void Test_Madingley_Cows_1Cell_10Years()
        {
            CommonTest(CommonTestType.COWS_1_CELL_10_YEARS, RunMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Cows"), Category("4 Cells"), Category("10 Years"), Category("Not resumable")]
        public void Test_Madingley_Cows_4Cells_10Years()
        {
            CommonTest(CommonTestType.COWS_4_CELLS_10_YEARS, RunMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Terrestrial"), Category("4 Cells"), Category("1 Year"), Category("Not resumable")]
        public void Test_Madingley_All_4Cells_1Year()
        {
            CommonTest(CommonTestType.TERRESTRIAL_4_CELLS_1_YEAR, RunMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Terrestrial"), Category("4 Cells"), Category("10 Years"), Category("Not resumable")]
        public void Test_Madingley_All_4Cells_10Years()
        {
            CommonTest(CommonTestType.TERRESTRIAL_4_CELLS_10_YEARS, RunMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Marine"), Category("1 Cell"), Category("1 Year"), Category("Not resumable")]
        public void Test_Madingley_Marine_1Cell_1Year()
        {
            CommonTest(CommonTestType.MARINE_1_CELL_1_YEAR, RunMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Marine"), Category("1 Cell"), Category("10 Years"), Category("Not resumable")]
        public void Test_Madingley_Marine_1Cell_10Years()
        {
            CommonTest(CommonTestType.MARINE_1_CELL_10_YEARS, RunMadingley<MadingleyModelOutputDataSets>);
        }

        [Test, Category("CI")]
        [Category("Madingley"), Category("Marine"), Category("4 Cells"), Category("10 Years"), Category("Not resumable")]
        public void Test_Madingley_Marine_4Cells_10Years()
        {
            CommonTest(CommonTestType.MARINE_4_CELLS_10_YEARS, RunMadingley<MadingleyModelOutputDataSets>);
        }
    }
}
