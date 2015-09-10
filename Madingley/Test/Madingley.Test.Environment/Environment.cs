using System;
using System.Linq;

using NUnit.Framework;

namespace Madingley.Test.Environment
{
    public class Environment
    {
        [Test]
        public void TestEnvironmentLoad()
        {
            var modelSetupRoot = "Model setup for tests";
            var environmentDataRoot = "Data for tests";

            Madingley.Test.Common.Environment.CreateDirectories(environmentDataRoot);
            Madingley.Test.Common.Environment.CreateFiles(environmentDataRoot, true);

            var actual = Madingley.Environment.Loader.Load(environmentDataRoot, modelSetupRoot, true);

            Assert.AreEqual(100, actual.CellEnvironment.Count());
        }
    }
}
