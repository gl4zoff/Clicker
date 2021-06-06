using UnityEngine;
using Structs;

public class BuildingsGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int GridSize = new Vector2Int(10, 10);
    [SerializeField] private GameObject panel;
    [SerializeField] private Game game;
    
    private Building flyingBuilding;
    private Camera mainCamera;

    private void Start()
    {
        if(game.grid == null)
            game.grid = new bool[GridSize.x, GridSize.y];

        mainCamera = Camera.main;
    }

    public void StartPlacingBuilding(Building buildingPrefab)
    {
        switch (buildingPrefab.rType)
        {
            case ResourceType.Rock:
                if (game.rock - buildingPrefab.costBuild < 0)
                    return;
                else
                    game.rock -= buildingPrefab.costBuild;
                break;
            case ResourceType.Iron:
                if (game.iron - buildingPrefab.costBuild < 0)
                    return;
                else
                    game.iron -= buildingPrefab.costBuild;
                break;
            case ResourceType.Wood:
                if (game.wood - buildingPrefab.costBuild < 0)
                    return;
                else
                    game.wood -= buildingPrefab.costBuild;
                break;
        }
        if (flyingBuilding != null)
        {
            Destroy(flyingBuilding.gameObject);
        }

        flyingBuilding = Instantiate(buildingPrefab);
        panel.SetActive(false);
    }

    private void Update()
    {
        if (flyingBuilding != null)
        {
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                bool available = true;

                if (x < 0 || x > GridSize.x - flyingBuilding.Size.x) available = false;
                if (y < 0 || y > GridSize.y - flyingBuilding.Size.y) available = false;

                if (available && IsPlaceTaken(x, y)) available = false;

                flyingBuilding.transform.position = new Vector3(x, 0, y);
                flyingBuilding.SetTransparent(available);

                if (available && Input.GetMouseButtonDown(0))
                {
                    PlaceFlyingBuilding(x, y);
                }
            }
        }
    }

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
            {
                if (game.grid[placeX + x, placeY + y]) return true;
            }
        }
        return false;
    }

    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
            {
                game.grid[placeX + x, placeY + y] = true;
            }
        }

        flyingBuilding.SetNormal();
        flyingBuilding.home = new Vector3(flyingBuilding.transform.position.x, 0.15f, flyingBuilding.transform.position.z - 0.45f);
        game.Buildings.Add(flyingBuilding);
        flyingBuilding = null;
    }
}
