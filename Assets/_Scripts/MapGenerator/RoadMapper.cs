using System.Collections.Generic;
using TMPro;
using TrafficSim.WorldManagers;
using Unity.VisualScripting;
using UnityEngine;

namespace TrafficSim.ProceduralEngine
{
    /**
     * This class is used to map every single road cell in the map,
     * and create a street network.
     */
    /**
     * This class is used to map every single road cell in the map,
     * and create a street network.
     */
    public class RoadMapper
    {

        // Reference to the map grid.
        private MapGrid mapGridRef;

        // Reference to the road fixer.
        private RoadFixer roadFixerRef;

        // A set of cells we have visited while mapping.
        private HashSet<MapGridCell> processedCells = new HashSet<MapGridCell>();

        // A list of roads we have found.
        public List<Road> roads = new List<Road>();

        // The GameManager object.
        private GameManager gameManagerRef;

        // Lists of common elements for street names
        private string[] roadTypes = {
            "Street", "Road", "Lane", "Avenue", "Drive", "Way", "Court", "Terrace", "Close",
            "Alley", "Arcade", "Boulevard", "Broadway", "Bypass", "Circle", "Circus", "Concourse", "Crescent", "Driveway",
            "Esplanade", "Gardens", "Grove", "Highway", "Lane", "Meadow", "Mews", "Parade", "Parkway", "Passage",
            "Path", "Place", "Plaza", "Promenade", "Quay", "Ridge", "Row", "Square", "Street", "Terrace",
            "Trail", "Valley", "Viaduct", "Vista", "Walk", "Wharf", "Wood", "Wynd", "Close", "Crescent",
            "Dale", "Dell", "Downs", "End", "Fairway", "Fields", "Gate", "Gardens", "Glen", "Green",
            "Heights", "Hollow", "Knoll", "Mead", "Mount", "Orchard", "Park", "Pike", "Place", "Plains",
            "Rise", "Shore", "Springs", "Stead", "Terrace", "Track", "Trail", "Vale", "View", "Village",
            "Walks", "Way", "Woodlands", "Yard", "Avenue", "Boulevard", "Court", "Cross", "Drive", "Falls",
            "Grove", "Highlands", "Inlet", "Junction", "Loop", "Pass", "Point", "Ridge", "Road", "Run",
            "Stream", "Summit", "Turn", "Valley", "Vista", "Woods"
        };

        private string[] landmarks = {
            "Park", "Gardens", "Square", "Green", "Common", "Hill", "Meadow", "View", "Crescent", "Place",
            "Vale", "Heath", "Mews", "Path", "Row", "Chase", "Rise", "Gate", "Wharf",
            "Bridge", "Fields", "Valley", "Bank", "Pond", "Lake", "Farm", "Barn", "Mill", "Market",
            "Harbour", "Dock", "Pier", "Grove", "Terrace", "Court", "Alley", "Arcade", "Boulevard", "Broadway",
            "Bypass", "Circle", "Circus", "Concourse", "Cove", "Dale", "Dell", "Downs", "Esplanade", "Fairway",
            "Falls", "Fells", "Gardens", "Glade", "Glen", "Heights", "Hollow", "Inlet", "Island", "Knoll",
            "Landing", "Meadow", "Mount", "Orchard", "Parade", "Parkway", "Passage", "Pike", "Plains", "Promenade",
            "Quay", "Ridge", "Rise", "Shore", "Springs", "Stead", "Summit", "Track", "Trail", "Valley",
            "Village", "Walks", "Woods", "Yard", "Bay", "Borough", "Bluffs", "Cape", "Cliff", "Crest",
            "Crossing", "Drive", "Falls", "Gateway", "Harbor", "Highlands", "Junction", "Key", "Landing", "Loop",
            "Oasis", "Overlook", "Pass", "Pines", "Point", "Ridge", "River", "Run", "Shore", "Sound",
            "Strand", "Summit", "Terrace", "Trace", "Vista", "Woods"
        };

        private string[] prefixes = {
            "Upper", "Lower", "East", "West", "North", "South", "Old", "New", "Great", "Little",
            "High", "Low", "Central", "Middle", "Inner", "Outer", "Broad", "Long", "Short", "Deep",
            "Shallow", "Wide", "Narrow", "Sunny", "Dark", "Bright", "Quiet", "Busy", "Golden", "Silver",
            "Royal", "Regal", "Majestic", "Grand", "Humble", "Merry", "Peaceful", "Tranquil", "Happy", "Friendly",
            "Rustic", "Urban", "Suburban", "Coastal", "Inland", "Western", "Eastern", "Northern", "Southern", "Ancient",
            "Modern", "Historic", "Famous", "Notable", "Secluded", "Hidden", "Open", "Sunny", "Shady", "Windy",
            "Foggy", "Snowy", "Rainy", "Stormy", "Calm", "Breezy", "Quiet", "Noisy", "Vibrant", "Lively",
            "Sleepy", "Silent", "Hushed", "Echoing", "Glorious", "Radiant", "Glimmering", "Sparkling", "Glistening", "Frosty",
            "Golden", "Crimson", "Azure", "Emerald", "Amber", "Ivory", "Onyx", "Pearl", "Ruby", "Sapphire",
            "Diamond", "Crystal", "Opal", "Turquoise", "Jade", "Amethyst", "Garnet", "Topaz", "Citrine", "Obsidian"
        };


        // Constructor for the RoadMapper
        public RoadMapper(MapGrid grid, RoadFixer roadFixer)
        {
            mapGridRef = grid;
            roadFixerRef = roadFixer;
            gameManagerRef = GameObject.Find("GameManager").GetComponent<GameManager>();

            // Identify the roads in the map.
            IdentifyRoads(false);
            IdentifyRoads(true);
            //IdentifyVerticalRoads();

            // Remove roundabouts from the roads.
            RemoveRoundabouts();

            // Log how many roads we have found.
            Debug.Log("Found " + roads.Count + " roads.");

            // Place the road names on the map.
            PlaceRoadNames();

            // Get the StreetLightManager object.
            StreetLightManager streetLightManager = GameObject.Find("StreetLightManager").GetComponent<StreetLightManager>();

            // Loop through all of the roads.
            foreach (Road road in roads)
            {
                // Get the cells in the road.
                List<MapGridCell> cells = road.GetCells();

                for (int i = 0; i < cells.Count; i++)
                {
                    // Get the cell.
                    MapGridCell cell = cells[i];

                    // Only do StraightRoad cells.
                    if (cell.GetCellType() != MapGridCellType.RoadStraight) continue;

                    // Get the position of the cell.
                    Vector3 pos = cell.GetPosition() + new Vector3(0, 1, 0);

                    // Rotation, will be altered based on cell orientation.
                    Vector3 rot = new Vector3(0, 0, 0);

                    // If the road is horizontal
                    if (!road.isVertical)
                    {
                        // Switch sides of the cell every 2 cells.
                        if (i % 2 == 0)
                        {
                            // Set position
                            pos = new Vector3(pos.x, pos.y, pos.z + 0.56f);

                            // Rotate y -90.
                            rot.y = -90;
                        }
                        else
                        {
                            // Set position
                            pos = new Vector3(pos.x, pos.y, pos.z - 0.56f);

                            // Rotate y +90.
                            rot.y = 90;
                        }
                    }
                    // If the road is vertical
                    else
                    {
                        // Switch sides of the cell every 2 cells.
                        if (i % 2 == 0)
                        {
                            // Set position
                            pos = new Vector3(pos.x + 0.56f, pos.y, pos.z);
                        }
                        else
                        {
                            // Set position
                            pos = new Vector3(pos.x - 0.56f, pos.y, pos.z);

                            // Rotate y 180
                            rot.y = 180;
                        }
                    }

                    // Spawn the lamp post with the computed position and rotation.
                    GameObject post = SpawnLampPost(pos, Quaternion.identity);

                    // Add a StreetLight script to the lamp post.
                    post.AddComponent<StreetLight>();

                    // Set scale to 0.8x
                    post.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                    // Apply the vector3 rotation to the post.
                    post.transform.eulerAngles = rot;

                    // Store the StreetLight component.
                    StreetLight streetLight = post.GetComponent<StreetLight>();
                    streetLightManager.streetLights.Add(streetLight);


                }
            }
        }

        // Used to spawn a street lamp at a given position.
        private GameObject SpawnLampPost(Vector3 pos, Quaternion rot)
        {
            // Instantiate a new lamp post at the given position.
            return GameObject.Instantiate(gameManagerRef.streetLampPrefab, pos, rot);
        }

        // Function to generate a road name
        private string GenerateRoadName()
        {
            // Will store the name of the road
            string name = "";

            // Add a prefix 30% of the time
            if (Random.value < 0.3f)
            {
                // Generate a random prefix
                name += prefixes[Random.Range(0, prefixes.Length)] + " ";
            }

            // Generate the rest of the road name
            name += landmarks[Random.Range(0, landmarks.Length)] + " " + roadTypes[Random.Range(0, roadTypes.Length)];

            // Return the generated road name
            return name;
        }

        private void PlaceRoadNames()
        {
            // Get the template for the floating text. "PlaceholderRoadName" in the scene view.
            GameObject floatingText = GameObject.Find("PlaceholderRoadName");

            // Loop through the roads.
            foreach (Road road in roads)
            {
                // Get the cells in the road.
                List<MapGridCell> cells = road.GetCells();

                // Skip if the road has no cells.
                if (cells.Count == 0) continue;

                // Get the first cell in the road.
                MapGridCell firstCell = cells[0];

                // If the road is longer than 20 cells, we can choose a random firstCell location between 0 and 15.
                if (cells.Count > 20) firstCell = cells[Random.Range(0, 15)];

                // Get the last cell in the road.
                MapGridCell lastCell = cells[cells.Count - 1];

                // Get the position of the first cell.
                Vector3 firstCellPos = firstCell.GetPosition();

                // Get the position of the last cell.
                Vector3 lastCellPos = lastCell.GetPosition();

                // Get the average position of the road.
                Vector3 averagePos = (firstCellPos + lastCellPos) / 2;

                // Based on if the road is vertical, or horizontal determine rotation.

                // Set the position of the floating text.
                GameObject instantiatedText = GameObject.Instantiate(floatingText, new Vector3(averagePos.x, 1.15f, averagePos.z), Quaternion.identity);

                // Set the text of the floating text.
                TextMeshPro roadName = instantiatedText.GetComponent<TextMeshPro>();
                roadName.text = road.GetName();

                // Set the rotation of the GameObject to (90, 0, 0)
                instantiatedText.transform.eulerAngles = new Vector3(90, 0, 0);

                // Rotate 270 Y degrees if the road is vertical.
                if (road.isVertical) instantiatedText.transform.eulerAngles = new Vector3(90, 270, 0);

                // Ensure the parent matches the floatingText's parent.
                instantiatedText.transform.SetParent(floatingText.transform.parent);

                // If the road is less than 10 cells, scale the text down to 0.5x the size.
                if (cells.Count < 10) roadName.fontSize = 0.5f * roadName.fontSize;
            }
        }

        /**
         * Used to remove all roundabout cells from the list of roads.
         */
        public void RemoveRoundabouts()
        {

            // Loop through roads.
            foreach (Road road in roads) 
            {
                // Will store a list of cells to remove.
                List<MapGridCell> cellsToRemove = new List<MapGridCell>();

                // Loop through the cells in the road.
                foreach (MapGridCell cell in road.GetCells())
                {
                    // Check if the cell is a roundabout cell.
                    if (roadFixerRef.GetRoundaboutCells().Contains(cell.GetPosition()))
                    {
                        // Add the cell to the list of cells to remove.
                        cellsToRemove.Add(cell);
                    }
                }

                // Loop through the cells to remove.
                foreach (MapGridCell cell in cellsToRemove)
                {

                    // Remove the cell from the road.
                    road.RemoveCell(cell);
                }
            }

        }

        void IdentifyRoads(bool isHorizontal)
        {
            int rows = mapGridRef.GetHeight();      // Get the number of rows in the map.
            int cols = mapGridRef.GetWidth();       // Get the number of columns in the map.

            // Determine the outer and inner loop bounds based on direction.
            int outerLoop = isHorizontal ? cols : rows;
            int innerLoop = isHorizontal ? rows : cols;

            // Outer loop (x for horizontal, z for vertical).
            for (int outer = 0; outer < outerLoop; outer++)
            {
                // Inner loop (z for horizontal, x for vertical).
                for (int inner = 0; inner < innerLoop; inner++)
                {
                    // Determine x and z based on direction.
                    int x = isHorizontal ? outer : inner;
                    int z = isHorizontal ? inner : outer;

                    // Get the cell at the current position.
                    MapGridCell cell = mapGridRef.GetCellByPosition(new Vector3Int(x, 0, z));

                    // Skip non-road cells.
                    if (!cell.IsRoadCell()) continue;

                    // Skip processed cells.
                    if (processedCells.Contains(cell)) continue;

                    // Determine the neighbor based on direction.
                    MapGridCell neighbor = isHorizontal ? cell.rightNeighbor : cell.topNeighbor;

                    // Check the cell has a neighbor in the specified direction.
                    if (neighbor != null && neighbor.IsRoadCell())
                    {
                        // We can begin scanning for a road.
                        Road road = new Road(GenerateRoadName());

                        // Store road direction.
                        road.isVertical = !isHorizontal;

                        // Add the current cell to the road.
                        road.AddCell(cell);

                        // Add the cell to the processed cells.
                        processedCells.Add(cell);

                        // Loop through the cells in the specified direction until we reach grass.
                        while (neighbor != null && neighbor.IsRoadCell())
                        {
                            // Add the cell to the road.
                            road.AddCell(neighbor);

                            // Add the cell to the processed cells.
                            processedCells.Add(neighbor);

                            // Move to the next cell.
                            cell = neighbor;
                            neighbor = isHorizontal ? cell.rightNeighbor : cell.topNeighbor;
                        }

                        // If the road has less than 5 cells, regenerate the name until it is less than 10 characters.
                        if (road.roadCells.Count <= 5)
                        {
                            // Regenerate the road name.
                            while (road.GetName().Length > 10)
                            {
                                // Generate a new road name.
                                road.SetName(GenerateRoadName());
                            }
                        }

                        // Add the road to the list of roads.
                        roads.Add(road);
                    }
                }
            }
        }
    }
}