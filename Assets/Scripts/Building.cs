using UnityEngine;
using Structs;

public class Building : MonoBehaviour
{
    [SerializeField] private Renderer MainRenderer;
    [SerializeField] private GameObject worker;
    private Game game;

    [HideInInspector] public float x, y, z; // Это нужно для сериализации
    public Vector2Int Size = Vector2Int.one;
    public Vector3 home;
    public BuildingType bType;
    public ResourceType rType;
    public float moneyPerSecond, nextTakeMoney, nextTakeResource, nextTakeStorage, money;
    public int lvl, countWorkers, countResources, resourcePerMinute, gold, costLvl, costWorker, costMoney, costBuild;

    public void SetTransparent(bool available)
    {
        if (available)
        {
            MainRenderer.material.color = Color.green;
        }
        else
        {
            MainRenderer.material.color = Color.red;
        }
    }

    public void SetNormal()
    {
        MainRenderer.material.color = Color.white;
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
    }

    public void AddToSaveStruct()
    {
        game.buildings.Add(new BuildingStruct
        {
            x = this.x,
            y = this.y,
            z = this.z,
            type = this.bType,
            moneyPerSecond = this.moneyPerSecond,
            lvl = this.lvl,
            countWorkers = this.countWorkers,
            countResources = this.countResources,
            resourcePerMinute = this.resourcePerMinute,
            money = this.money,
            gold = this.gold,
            nextTakeMoney = this.nextTakeMoney,
            nextTakeResource = this.nextTakeResource,
            nextTakeStorage = this.nextTakeStorage,
            costLvl = this.costLvl,
            costWorker = this.costWorker,
            costMoney = this.costMoney
        });
    }
    public void UpgradeLvl()
    {
        if (game.gold - costLvl > 0)
        {
            game.gold -= costLvl;
            moneyPerSecond *= 3.5f;
            resourcePerMinute *= 3;
            costLvl *= 3;
            game.OnBuildingClick(this);
        }
    }
    public void UpgradeMoney()
    {
        if (game.money - costMoney > 0)
        {
            game.money -= costMoney;
            moneyPerSecond += 0.01f;
            costMoney++;
            game.OnBuildingClick(this);
        }
    }
    public void UpgradeWorker()
    {
        if (game.money - costWorker > 0)
        {
            game.money -= costWorker;
            countWorkers++;
            costWorker *= 2;
            game.OnBuildingClick(this);
        }
    }
    private void OnMouseUp()
    {
        game.OnBuildingClick(this);
    }

    private void Start()
    {
        game = GameObject.Find("Storage").GetComponent<Game>();
    }

    private void Update()
    {
        if (nextTakeMoney - Time.deltaTime > 0)
            nextTakeMoney -= Time.deltaTime;
        else
        {
            nextTakeMoney = 1;
            money += moneyPerSecond * countWorkers;
        }

        if (nextTakeResource - Time.deltaTime > 0)
            nextTakeResource -= Time.deltaTime;
        else
        {
            nextTakeResource = 60;
            countResources += resourcePerMinute * countWorkers;
            if (Random.Range(0, 100) < 10)
                gold++;
        }

        if (nextTakeStorage - Time.deltaTime > 0)
            nextTakeStorage -= Time.deltaTime;
        else
        {
            nextTakeStorage = 180;
            GameObject workerObject = Instantiate(worker, home, Quaternion.Euler(0,0,0));
            Worker w = workerObject.GetComponent<Worker>();
            w.home = home;
            w.building = this;
            w.GoToStorage();
        }
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                if ((x + y) % 2 == 0) Gizmos.color = new Color(0.88f, 0f, 1f, 0.3f);
                else Gizmos.color = new Color(1f, 0.68f, 0f, 0.3f);

                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, 0.1f, 1));
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Worker")
        {
            if (other.GetComponent<Worker>().isReturning)
                Destroy(other.gameObject);
        }
    }
}

public enum BuildingType
{
    House1 = 0, House2 = 1, House3 = 2
}
public enum ResourceType
{
    Rock = 0, Wood = 1, Iron = 2
}