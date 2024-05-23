


using UnityEngine;
using System.Collections.Generic;
using TrafficSim.Pathfinding;

namespace TrafficSim.ProceduralEngine
{
    /**
     * Class used to actually generate the map.
     */
    public class MapGenerator : MonoBehaviour {
    
        [SerializeField] private int mapWidth;                  // The width of the map. Set in the inspector.
        [SerializeField] private int mapHeight;                 // The height of the map. Set in the inspector.
        [SerializeField] private int zoneSize;                  // The size of the zone. Set in the inspector.
        public MapGrid mapGrid;                                 // The map grid. Stores actual information about the map.
        [SerializeField] private GameManager gameManager;    // Reference to the GameManager object.

        /**
         * Called by the GameManager when the game starts.
         */
        public void Initialize()
        {
            // Initialize the mapGrid object.
            mapGrid.Initialize(mapWidth, mapHeight);

            // Get the GameManager object.
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        /**
         * Used to generate the map in different phases.
         */
        public void GenerateMap()
        {

            // Generate the first pass of the map.
            GenerateTerrain();

            // Assign neighbors to each cell. This will be called again after the road layout is generated.
            AssignNeighbors();

            // Construct a RoadFixer object.
            RoadFixer roadFixer = new RoadFixer(mapGrid);

            // Fix road orientation.
            roadFixer.FixRoadOrientation();

            // Map adjacent grass cells to roads.
            roadFixer.MapAdjacentGrassCells();

            // Initialize all cells.
            mapGrid.InitializeAllCells();

            // Construct a BuildingGenerator object.
            BuildingGenerator buildingGenerator = new BuildingGenerator(mapGrid, roadFixer.GetRoundaboutCells());

            // Generate buildings.
            buildingGenerator.GenerateBuildings();

            // Setup grass terrain.
            SetupGrassSpawner();

            // Final pass to assign neighbors, since orientations have changed & buildings have been added.
            AssignNeighbors();

            // Map the roads. TODO: convert this to a function instead of a class.
            new RoadMapper(mapGrid, roadFixer);

        }

        /**
         *  Used to mark all structures as static for static batching.
         */
        public void SetAllStructuresToStatic()
        {
            // Get every single MapGridCell in the world.
            foreach (MapGridCell cell in mapGrid.cells.Values)
            {
                // Get the structureModel from the cell.
                StructureModel structureModel = cell.GetStructureModel();

                // If the structureModel is not null, set it to static.
                if (structureModel != null)
                {
                    structureModel.structure.isStatic = true;
                }
            }
        }

        /**
         * Used to setup gpu instanced grass blocks.
         */
        public void SetupGrassSpawner()
        {
            // Get the "BuildingInstanceManager" object to access the building instances.
            GameObject buildingInstanceManager = GameObject.Find("BuildingMeshInstancer");

            // Create a Grass object.
            GameObject grass = new GameObject("Grass");

            // Set the parent to the buildingInstanceManager.
            grass.transform.SetParent(buildingInstanceManager.transform);

            // Add a PrefabInstancer component to the GameObject.
            grass.AddComponent<PrefabInstancer>();

            // Get the PrefabInstancer component.
            PrefabInstancer prefabInstancer = grass.GetComponent<PrefabInstancer>();

            // Setup the grass prefab.
            prefabInstancer.SetupPrefab(gameManager.terrainPrefabs[gameManager.terrainPrefabDictionary[MapGridCellType.Grass]]);

            // Get all grass cells.
            List<MapGridCell> grassCells = mapGrid.GetAllCellsByType(MapGridCellType.Grass);

            // Loop through all grass cells and add to the prefabInstancer.
            foreach (MapGridCell cell in grassCells)
            {
                // Get the cellpos as vector3 int.
                Vector3 cellPos = cell.GetPosition();

                // Rotation 0
                Quaternion rot = Quaternion.Euler(0, 0, 0);

                // Scale 1
                Vector3 scale = new Vector3(1, 1, 1);

                // Add the matrix to the current batch.
                prefabInstancer.AddInstance(cellPos, rot, scale);
            }

            // Get all children of BuildingMeshInstancer.
            Transform[] children = buildingInstanceManager.GetComponentsInChildren<Transform>();

            // Loop through children & get the PrefabInstancer component.
            foreach (Transform child in children)
            {
                PrefabInstancer pi = child.GetComponent<PrefabInstancer>();

                // If the PrefabInstancer is not null, render the instances.
                if (pi != null)
                {
                    pi.FinalizeMatrices();
                }
            }
        }

        /**
         * Final pass to loop through the entire map grid and assign references to neighbors to each cell.
         */
        public void AssignNeighbors()
        {
            // Loop through the map grid width.
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    // Get the current cell.
                    MapGridCell currentCell = mapGrid.GetCellByPosition(new Vector3Int(x, 0, y));

                    // Get the neighboring cells (top, bottom, left, right). Assign them to currentCell. Check that neighbor positions are in bounds.
                    if (x > 0) { currentCell.leftNeighbor = mapGrid.GetCellByPosition(new Vector3Int(x - 1, 0, y)); }
                    if (x < mapWidth - 1) { currentCell.rightNeighbor = mapGrid.GetCellByPosition(new Vector3Int(x + 1, 0, y)); }
                    if (y > 0) { currentCell.bottomNeighbor = mapGrid.GetCellByPosition(new Vector3Int(x, 0, y - 1)); }
                    if (y < mapHeight - 1) { currentCell.topNeighbor = mapGrid.GetCellByPosition(new Vector3Int(x, 0, y + 1)); }
                }
            }
        }

        /**
         * First pass used to generate the actual road layout.
         */
        public void GenerateTerrain()
        {
            // Loop through the map grid width.
            for (int x = 0; x < mapWidth; x += (zoneSize + 1))
            {
                // Loop through the map grid height.
                for (int y = 0; y < mapHeight; y += (zoneSize + 1))
                {
                    // Square expansion zone. Used to remove one border face from a grid square.
                    int xExpansion = 0;
                    int yExpansion = 0;

                    // Have a 60% chance to expand the zone.
                    if (UnityEngine.Random.Range(0, 100) < 60)
                    {
                        // 50% chance of expanding the zone in the X direction.
                        if (UnityEngine.Random.Range(0, 100) < 50) { xExpansion = 1; }

                        // 50% chance of expanding the zone in the Y direction.
                        else { yExpansion = 1; }
                    }

                    // Loop through the zone height.
                    for (int x1 = 0; x1 < Mathf.Min(zoneSize + xExpansion, mapWidth - x); x1++)
                    {
                        for (int y1 = 0; y1 < Mathf.Min(zoneSize + yExpansion, mapHeight - y); y1++)
                        {
                            // Set the cell type to grass.
                            mapGrid.SetCellType(x + x1, y + y1, MapGridCellType.Grass);
                        }
                    }
                }
            }

            // Loop around the entire map grid and add a border of grass 1 cell wide.
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    // If we are on the border of the map, set the cell type to grass.
                    if (x == 0 || x == mapWidth - 1 || y == 0 || y == mapHeight - 1)
                    {
                        mapGrid.SetCellType(x, y, MapGridCellType.Grass);
                    }
                }
            }
        }

    }
}