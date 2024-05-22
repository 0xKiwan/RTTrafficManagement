using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSim.ProceduralEngine;

namespace TrafficSim
{
    /**
    * The main GameManager class.
    * Contains the main game information, prefabs, etc.
    */
    public class GameManager : MonoBehaviour {
        
        // A list of all the terrain prefabs used in the game.
        public List<GameObject> terrainPrefabs;

        // A list of all the house prefabs used in the game.
        public List<GameObject> housePrefabs;

        // A list of all the roads in the game.
        public List<Road> roadObjects;

        // The gameobject for street lamps.
        public GameObject streetLampPrefab;

        // The map generator object.
        public MapGenerator mapGenerator;

        // The road mapper object.
        public RoadMapper roadMapper;


        // A dictionary mapping MapGridCellType to the corresponding prefab index.
        public Dictionary<MapGridCellType, int> terrainPrefabDictionary = new Dictionary<MapGridCellType, int>()
        {
            {MapGridCellType.Grass, 0},
            {MapGridCellType.IntersectionFourWay, 1},
            {MapGridCellType.IntersectionThreeWay, 2},
            {MapGridCellType.IntersectionRoundabout, 3},
            {MapGridCellType.RoadCorner, 4},
            {MapGridCellType.RoadDeadEnd, 5},
            {MapGridCellType.RoadStraight, 6},
            {MapGridCellType.RoadStraightCrossing, 7}
        };

        // Start is called before the first frame update
        void Start()
        {
            // Debug.Log("House count: " + housePrefabs.Count);

            // Initialize the map generator.
            mapGenerator.Initialize();

            // Generate the map.
            mapGenerator.GenerateMap();
        }

    }
}