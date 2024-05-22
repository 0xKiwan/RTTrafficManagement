

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.ProceduralEngine
{
    /**
     * Class that represents a grid in the map.
     * Contains information about cell type, cell position, cell rotation, and a reference to the
     * game object that is in that cell.
     */
    public class MapGridCell : MonoBehaviour {
        private Vector3Int position;                                        // The coordinates of the cell in the grid.
        private Quaternion rotation;                                        // The rotation of the cell.
        private MapGridCellType cellType;                                   // The type of the cell.
        private StructureModel structureModel;                              // The structure belonging to the cell.
        private StructureModel building;                                    // The building attached to the cell.
        public List<MapGridCell> adjGrassCells;                             // The adjacent grass cells.
        private bool hasBuilding    = false;                                // Whether or not the cell has a building.
        public MapGridCell topNeighbor     = null;                          // The top neighbor of the cell.
        public MapGridCell bottomNeighbor  = null;                          // The bottom neighbor of the cell.
        public MapGridCell leftNeighbor    = null;                          // The left neighbor of the cell.
        public MapGridCell rightNeighbor   = null;                          // The right neighbor of the cell.

        // Pathfinding variables.
        public float gCost = 0;                                             // The cost to move from the starting node to a given node.
        public float hCost = 0;                                             // The cost to move from the given node to the end node.
        public float fCost { get { return gCost + hCost; } }                // The total cost of the node.
        public MapGridCell parent = null;                                   // The parent of the node.

        // A list of road cell types.
        List<MapGridCellType> roadCellTypes = new List<MapGridCellType>
        {
            MapGridCellType.RoadStraight,
            MapGridCellType.RoadCorner,
            MapGridCellType.RoadStraightCrossing,
            MapGridCellType.RoadDeadEnd,
            MapGridCellType.IntersectionFourWay,
            MapGridCellType.IntersectionThreeWay,
            MapGridCellType.IntersectionRoundabout
        };

        /**
         * Constructor for the MapGridCell class.
         */
        public MapGridCell(MapGridCellType cellType, Vector3Int pos)
        {
            // Store passed variables.
            this.cellType       = cellType;
            this.position       = pos;

            // Initialize adjGrassCells.
            adjGrassCells = new List<MapGridCell>();
        }

        /**
         * Used to get the position of the cell.
         */
        public Vector3Int GetPosition() { return position; }
        /**
         * Used to get the type of the cell.
         */
        public MapGridCellType GetCellType() { return cellType; }

        /**
         * Used to get the structure of the cell.
         */
        public StructureModel GetStructureModel() { return structureModel; }

        /**
         * Used to get the RoadHandler of the cell.
         */
        public Pathfinding.RoadHandler GetRoadHandler()
        {
            // Get the structure model of the cell.
            StructureModel structureModel = this.GetStructureModel();

            // Get the first child of the structure model.
            Transform child = structureModel.transform.GetChild(0);

            // Check the child exists.
            if (child == null) return null;

            // Get the RoadHandler component of the child.
            Pathfinding.RoadHandler roadHandler = child.GetComponentInChildren<Pathfinding.RoadHandler>();

            // Check the roadHandler exists.
            if (roadHandler == null) return null;

            // Return the roadHandler.
            return roadHandler;
        }

        /**
         * Used to get the building of the cell.
         */
        public StructureModel GetBuilding() { return building; }

        /**
         * Used to set the building of the cell.
         */
        public void SetBuilding(StructureModel building) { this.building = building; }

        /**
         * Used to construct a building on this cell given a prefab.
         */
        public StructureModel ConstructBuilding(GameObject prefab)
        {
            // If we already have a building, we are trying to place on a corner. Destroy the old building.
            if (hasBuilding) 
            { 
                // Destroy the building.
                building.DestroyStructure();

                // Return, we don't want buildings on corners.
                return null;
            }

            // Get the GameManager.
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            // Create a GameObject with the name "MapGridCell" + the prefab name.
            GameObject structure = new GameObject("Building");

            // Set the parent to the mapGrid.
            structure.transform.SetParent(gameManager.mapGenerator.mapGrid.transform);

            // Set the position of the structure + y1
            structure.transform.position = position + new Vector3(0, 1, 0);

            // Add a StructureModel component to the GameObject.
            this.building = structure.AddComponent<StructureModel>();

            // Create the model of the structure.
            this.building.CreateModel(prefab);

            // Rotate the structure.
            this.building.SetRotation(rotation); 

            // Set hasBuilding to true.
            hasBuilding = true;

            // Return the structuremodel for the building.
            return this.building;
        }

        /**
         * Used to set the cell's type.
         */
        public void SetCellType(MapGridCellType cellType) 
        { 
            // Set the cell type.
            this.cellType = cellType;
        }

        /**
         * Used to update the prefab based on MapGridCellType. Access global GameObjectManager for prefabs.
         */
        public void UpdatePrefab()
        {
            // Get the global GameManager object.
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            // Get the prefab index from the prefab dictionary.
            GameObject prefab = gameManager.terrainPrefabs[gameManager.terrainPrefabDictionary[cellType]];

            // Update the existing structure model.
            structureModel.SwapModel(prefab);
        }

        /**
         * Used to create a structure at the cell's position.
         */
        public void InitializeStructure()
        {
            // Get the GameManager.
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            // Create a GameObject with the name "MapGridCell".
            GameObject structure = new GameObject("MapGridCell");

            // Set the parent to the mapGrid.
            structure.transform.SetParent(gameManager.mapGenerator.mapGrid.transform);

            // Set the position of the structure. structureModel + y1
            structure.transform.position = position;

            // Add a StructureModel component to the GameObject.
            this.structureModel = structure.AddComponent<StructureModel>();

            // Get the prefab from the cellType.
            GameObject prefabObject = gameManager.terrainPrefabs[gameManager.terrainPrefabDictionary[cellType]];

            // Do not create model if this is a grass cell. We can destroy the object in the scene and return.
            if (cellType == MapGridCellType.Grass)
            {
                Destroy(structure);
            }

            // Create the model of the structure.
            this.structureModel.CreateModel(prefabObject);

            // Rotate the structure.
            this.structureModel.SetRotation(rotation);
        }

        /**
         * Used to check if a cell type is a road cell.
         */
        public bool IsRoadCell()
        {

            // Check if the cell's type is in the list of road cell types.
            return roadCellTypes.Contains(GetCellType());
        }

        /**
         * Used to check if a cell type is a grass cell.
         */
        public bool IsGrassCell()
        {
            // Check if the cell's type is grass.
            return GetCellType() == MapGridCellType.Grass;
        }

        /**
         * Used to set a cell's rotation.
         */
        public void SetRotation(Quaternion rotation) { this.rotation = rotation; }

        // Override == to check position.
        public static bool operator ==(MapGridCell a, MapGridCell b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b)) { return true; }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) { return false; }

            // Return true if the fields match.
            return a.position == b.position;
        }

        // Override != to check position.
        public static bool operator !=(MapGridCell a, MapGridCell b)
        {
            return !(a == b);
        }

        // Override tostring to print type & position.
        public override string ToString()
        {
            return "MapGridCell Type: " + cellType + ", Position: " + position;
        }

        // Override Object.Equals to check position.
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null) { return false; }

            // If parameter cannot be cast to Point return false.
            MapGridCell p = obj as MapGridCell;
            if ((System.Object)p == null) { return false; }

            // Return true if the fields match.
            return (position == p.position);
        }

        // Override Object.GetHashCode to check position.
        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
}