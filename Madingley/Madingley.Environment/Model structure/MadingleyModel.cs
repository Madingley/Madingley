using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley
{
    /// <summary>
    /// The ecosystem model
    /// </summary>
    public static class MadingleyModel
    {
        /// <summary>
        /// Initializes the ecosystem model
        /// </summary>
        /// <param name="mmi">An instance of the model initialisation class</param> 
        public static void Load(Tuple<Madingley.Common.Environment, SortedList<string, EnviroData>> mmi, bool readData)
        {
            var e = mmi.Item1;

            if (e.SpecificLocations == false)
            {
                var CellList = new List<Tuple<int, int>>();

                var NumLatCells = (uint)((e.TopLatitude - e.BottomLatitude) / e.CellSize);
                var NumLonCells = (uint)((e.RightmostLongitude - e.LeftmostLongitude) / e.CellSize);

                // Loop over all cells in the model
                for (int latitudeIndex = 0; latitudeIndex < NumLatCells; latitudeIndex++)
                {
                    for (int longitudeIndex = 0; longitudeIndex < NumLonCells; longitudeIndex++)
                    {
                        // Add the vector to the list of all active grid cells
                        CellList.Add(Tuple.Create(latitudeIndex, longitudeIndex));
                    }
                }

                e.FocusCells = CellList;
            }

            if (readData)
            {
                var cellList = e.FocusCells.Select(a => new UInt32[] { (uint)a.Item1, (uint)a.Item2 }).ToList();

                var EcosystemModelGrid = new ModelGrid((float)e.BottomLatitude, (float)e.LeftmostLongitude, (float)e.TopLatitude, (float)e.RightmostLongitude,
                    (float)e.CellSize, (float)e.CellSize, cellList, mmi.Item2,
                    e.SpecificLocations);

                Func<Tuple<int, int>, IDictionary<string, double[]>> convertCellEnvironment =
                    cell =>
                    {
                        var env = EcosystemModelGrid.GetCellEnvironment((uint)cell.Item1, (uint)cell.Item2);

                        return env.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
                    };

                e.CellEnvironment = e.FocusCells.Select(convertCellEnvironment).ToList();
            }
        }
    }
}
