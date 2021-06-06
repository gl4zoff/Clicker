using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Structs;
using System;
using TMPro;

public class Game : MonoBehaviour
{
    [HideInInspector] public List<BuildingStruct> buildings;
    [HideInInspector] public List<Building> Buildings;

    public bool[,] grid;
    public float money = 0;
    public int rock = 0, wood = 0, iron = 0, gold = 0;

    [SerializeField] private string pathJson;
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private GameObject infoPanel, storagePanel;
    [SerializeField] private TextMeshProUGUI storageText, infoText;


    private Building currentBuilding;

    private void Save()
    {
        buildings.Clear();
        foreach(Building b in Buildings)
        {
            b.AddToSaveStruct();
        }
        GameStruct game = new GameStruct
        {
            buildings = this.buildings,
            money = this.money,
            gold = this.gold,
            iron = this.iron,
            rock = this.rock,
            wood = this.wood,
            grid = ConvertArray(this.grid)
        };
        string json = JsonUtility.ToJson(game, true);
        try
        {
            File.WriteAllText(pathJson, json);
        }
        catch(Exception e)
        {
            Debug.Log($"Че-то пошло не так: {e.Message}");
        }
    }
    private void Load()
    {
        if (File.Exists(pathJson))
        {
            try
            {
                string json = File.ReadAllText(pathJson);

                GameStruct game = JsonUtility.FromJson<GameStruct>(json);
                grid = ConvertArray(game.grid); money = game.money; rock = game.rock; wood = game.wood; iron = game.iron; gold = game.gold; buildings = game.buildings;
                foreach (BuildingStruct building in buildings)
                {
                    GameObject house = Instantiate(prefabs[(int)building.type], new Vector3(building.x, building.y, building.z), Quaternion.Euler(0,0,0));
                    Building b = house.GetComponent<Building>();
                    b.x = building.x;
                    b.y = building.y;
                    b.z = building.z;
                    b.moneyPerSecond = building.moneyPerSecond;
                    b.resourcePerMinute = building.resourcePerMinute;
                    b.lvl = building.lvl;
                    b.countResources = building.countResources;
                    b.countWorkers = building.countWorkers;
                    b.nextTakeStorage = building.nextTakeStorage;
                    b.nextTakeResource = building.nextTakeResource;
                    b.nextTakeMoney = building.nextTakeMoney;
                    b.money = building.money;
                    b.gold = building.gold;
                    b.home = new Vector3(b.x,b.y,b.z-0.45f);
                    b.costMoney = building.costMoney;
                    b.costLvl = building.costLvl;
                    b.costWorker = building.costWorker;
                    Buildings.Add(b);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Че-то пошло не так: {e.Message}");
            }
        }
    }
    private bool[] ConvertArray(bool[,] array)
    {
        int i, j, k = 0;
        bool[] newArray = new bool[100];
        for(i = 0; i < 10; i++)
        {
            for(j = 0; j < 10; j++)
            {
                newArray[k] = array[i, j];
                k++;
            }
        }
        return newArray;
    }
    private bool[,] ConvertArray(bool[] array)
    {
        int i, j, k = 0;
        bool[,] newArray = new bool[10,10];
        for (i = 0; i < 10; i++)
        {
            for (j = 0; j < 10; j++)
            {
                newArray[i,j] = array[k];
                k++;
            }
        }
        return newArray;
    }

    private void Awake()
    {
        Load();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Worker")
        {
            Worker worker = other.GetComponent<Worker>();
            switch (worker.building.rType)
            {
                case ResourceType.Rock:
                    rock += worker.building.countResources;                    
                    break;
                case ResourceType.Wood:
                    wood += worker.building.countResources;
                    break;
                case ResourceType.Iron:
                    iron += worker.building.countResources;
                    break;
            }
            money += worker.building.money;
            gold += worker.building.gold;
            worker.building.countResources = 0;
            worker.building.money = 0;
            worker.building.gold = 0;
            worker.isReturning = true;
            worker.GoToHome();
        }
    }
    private void OnMouseUp()
    {
        storagePanel.SetActive(true);
        storageText.text =
$@"Деньги: {money.ToString("F2")}
Камень: {rock}
Железо: {iron}
Дерево: {wood}
Золото: {gold}";
    }

    public void OnBuildingClick(Building building)
    {
        currentBuilding = building;
        string resName = "";
        switch (currentBuilding.rType)
        {
            case ResourceType.Rock:
                resName = "Камень";
                break;
            case ResourceType.Iron:
                resName = "Железо";
                break;
            case ResourceType.Wood:
                resName = "Дерево";
                break;
        }
        infoPanel.SetActive(true);
        infoText.text =
$@"{resName}: {currentBuilding.countResources}
Деньги: {currentBuilding.money.ToString("F2")}
Золото: {currentBuilding.gold}
Уровень: {currentBuilding.lvl}
Кол-во рабочих: {currentBuilding.countWorkers}
Деньги/сек: {(currentBuilding.moneyPerSecond * currentBuilding.countWorkers).ToString("F2")}
Рес/мин: {(currentBuilding.resourcePerMinute * currentBuilding.countWorkers).ToString("F2")}
До Отправки на склад: {(int)currentBuilding.nextTakeStorage}sec";
    }
    public void UpgradeLvl()
    {
        if (currentBuilding != null)
        {
            currentBuilding.UpgradeLvl();
        }
    }
    public void UpgradeMoney()
    {
        if (currentBuilding != null)
        {
            currentBuilding.UpgradeMoney();
        }
    }
    public void UpgradeWorker()
    {
        if (currentBuilding != null)
        {
            currentBuilding.UpgradeWorker();
        }
    }
    public void Exit()
    {
        infoPanel.SetActive(false);
        storagePanel.SetActive(false);
    }
}