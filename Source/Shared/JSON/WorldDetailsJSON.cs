using System;
using System.Collections.Generic;
namespace Shared
{
    [Serializable]
    public class WorldDetailsJSON
    {
        public string worldStepMode;

        public string seedString;
        public int persistentRandomValue;
        public string planetCoverage;
        public string rainfall;
        public string temperature;
        public string population;
        public string pollution;
        public List<string> factions = new List<string>();

        // key - Deflate Label
        // value - World Deflate
        public Dictionary<string,string> deflateDictionary = new Dictionary<string,string>();

        // string of the world Objects class (for settlements and their locations)
        public string WorldObjects = "";
    }
}
