using UnityEngine;

namespace Structs
{
    [System.Serializable]
    public struct BuildingStruct
    {
        public float x,y,z;
        public BuildingType type;
        public float moneyPerSecond, nextTakeMoney, nextTakeResource, nextTakeStorage, money;
        public int lvl, countWorkers, countResources, resourcePerMinute, gold, costLvl, costWorker, costMoney;
    }
}
