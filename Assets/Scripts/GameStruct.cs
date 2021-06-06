using System.Collections.Generic;
using UnityEngine;

namespace Structs
{
    [System.Serializable]
    public struct GameStruct
    {
        public List<BuildingStruct> buildings;
        public bool[] grid;
        public float money;
        public int rock, wood, iron, gold;
    }
}