using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrafficSim.ProceduralEngine
{
    /**
     * Class that generates buildings on a completed map.
     */
    class BuildingGenerator
    {

        // The map grid.
        private MapGrid mapGridRef;

        // The roundabout positions.
        private List<Vector3Int> roundaboutPositions;

        // A dictionary for meshfilters between housePrefabs index and their meshfilter.
        private Dictionary<int, List<StructureModel>> structureModels = new Dictionary<int, List<StructureModel>>();

        // Constructor for the BuildingGenerator class.
        public BuildingGenerator(MapGrid mapGridRef, List<Vector3Int> roundaboutPositions)
        {
            // Store the map grid reference.
            this.mapGridRef = mapGridRef;

            // Store the roundabout positions.
            this.roundaboutPositions = roundaboutPositions;
        }

        /**
         * Used to generate buildings on grass cells adjacent to roads.
         */
        public void GenerateBuildings()
        {
            // Get all road cells from the map.
            List<MapGridCell> roadCells = mapGridRef.GetAllRoadCells();

            // Get the GameManager object to access the house prefabs.
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            // Get the "BuildingInstanceManager" object to access the building instances.
            GameObject buildingInstanceManager = GameObject.Find("BuildingMeshInstancer");

            // Loop through all road cells.
            foreach (MapGridCell cell in roadCells)
            {
                // Loop through adjacent grass cells.
                foreach (MapGridCell grassCell in cell.adjGrassCells)
                {
                    // Skipp null cells.
                    if (grassCell == null) { continue; }

                    // Skip roundabout cells.
                    if (roundaboutPositions.Contains(cell.GetPosition())) { continue; }

                    // Get the index for the housePrefab.
                    int housePrefabIndex = Random.Range(0, gameManager.housePrefabs.Count);

                    // Get a random house prefab.
                    GameObject housePrefab = gameManager.housePrefabs[housePrefabIndex];

                    // Check if an object with the prefab name exists in the buildingInstanceManager.
                    if (buildingInstanceManager.transform.Find(housePrefab.name) == null)
                    {
                        // Create a new PrefabInstancer object with the housePrefab name.
                        GameObject prefabInstancer = new GameObject(housePrefab.name);

                        // Set the parent of the prefabInstancer to the buildingInstanceManager.
                        prefabInstancer.transform.parent = buildingInstanceManager.transform;

                        // Add the prefabInstancer to the new GameObject.
                        prefabInstancer.AddComponent<PrefabInstancer>();

                        // Get the PrefabInstancer component.
                        PrefabInstancer prefabInstancerComponent = prefabInstancer.GetComponent<PrefabInstancer>();

                        // Setup the prefab inside of the prefabInstancer.
                        prefabInstancerComponent.SetupPrefab(housePrefab);
                    }
                    else
                    {
                        // Get the object with the same name as the housePrefab.
                        PrefabInstancer prefabInstancer = buildingInstanceManager.transform.Find(housePrefab.name).gameObject.GetComponent<PrefabInstancer>(); ;

                        // Get position of the house.
                        Vector3 position = grassCell.GetPosition() + new Vector3(0, 1.0f, 0);

                        // Get the rotation of the building.
                        Quaternion rotation = GetBuildingRotation(grassCell, cell);

                        // Scale 1.0f
                        Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

                        // Add an instance of the prefab to the prefabInstancer.
                        prefabInstancer.AddInstance(position, rotation, scale);

                    }
                }
            }
        }

        /**
         * Used to set a building's rotation based on grass & road cell positions.
         */
        private Quaternion GetBuildingRotation(MapGridCell grassCell, MapGridCell roadCell)
        {
            // Get relative position of the grass cell to the road cell.
            Vector3Int relativePos = grassCell.GetPosition() - roadCell.GetPosition();

            // The rotation of the building.
            Quaternion rotation = Quaternion.identity;

            // Set the rotation of the building based on the relative position.
            rotation =  relativePos.x < 0 ? Quaternion.Euler(0, -90, 0) :
                        relativePos.x > 0 ? Quaternion.Euler(0, 90, 0) :
                        relativePos.z < 0 ? Quaternion.Euler(0, 180, 0) :
                        rotation;

            return rotation;
        }
    }
}