

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrafficSim.ProceduralEngine
{
    /**
     * Class that fixes road orientation.
     */
    public class RoadFixer
    {
        // Reference to the map grid.
        private MapGrid mapGridRef;

        // A list of positions that are within roundabouts.
        private List<Vector3Int> roundaboutPositions;

        // Constructor for the RoadFixer class.
        public RoadFixer(MapGrid grid)
        {
            // Store the reference to the map grid.
            mapGridRef = grid;

            // Initialize the roundabout positions list.
            roundaboutPositions = new List<Vector3Int>();
        }

        // Used to fix road orientation.
        public void FixRoadOrientation()
        {
            // Get all road cells from the mapGridRef.
            List<MapGridCell> roadCells = mapGridRef.GetAllRoadCells();

            // Loop through all the road cells.
            foreach (MapGridCell cell in roadCells)
            {
                // Get adjacent cell types to the current cell.
                MapGridCell[] adjacentCells = mapGridRef.GetNeighboringCells(cell);

                // Get the count of non-null adjacent cells that are road cells.
                int adjRoadCellCount = adjacentCells.Count(adjCell => adjCell != null && adjCell.IsRoadCell());

                // Debug.Log(adjRoadCellCount);

                // Check if this road should be a dead end.
                if (adjRoadCellCount == 0 || adjRoadCellCount == 1) { CreateDeadEnd(adjacentCells, cell); }

                // Check if this road should be a straight road.
                else if (adjRoadCellCount == 2)
                {
                    if (!CreateStraightRoad(adjacentCells, cell))
                    {
                        // Create a corner.
                        CreateCorner(adjacentCells, cell);
                    }
                }

                // Check if this road should be a three way intersection.
                else if (adjRoadCellCount == 3) { CreateIntersectionThreeWay(adjacentCells, cell); }

                // Otherwise, this road should be a four way intersection.
                else { CreateFourWayIntersection(cell); }
            }
        }

        // Used to map adjacent grass cells and store them into a road cell.
        public void MapAdjacentGrassCells()
        {
            // Get all road cells in the map.
            List<MapGridCell> roadCells = mapGridRef.GetAllRoadCells();

            // Loop through road cells and store adjacent terrain cells.
            foreach (MapGridCell cell in roadCells)
            {
                // Get adjacent cells to the current cell.
                MapGridCell[] adjacentCells = mapGridRef.GetNeighboringCells(cell);

                // Loop through adjacent cells & check if terrain cell.
                foreach (MapGridCell adjCell in adjacentCells)
                {
                    // Check if this cell is grass.
                    if (adjCell != null && !(adjCell.GetCellType() == MapGridCellType.Grass)) { continue; }

                    // Store the cell inside of the road cell. This is used later for placing structures.
                    cell.adjGrassCells.Add(adjCell);
                }

            }
        }

        // Used to create a dead end.
        private void CreateDeadEnd(MapGridCell[] adjacentCells, MapGridCell cell)
        {
            //
            // [0] = Left neighbour, [1] = Top neighbour, [2] = Right neighbour, [3] = Down neighbour
            //

            // Map adjacent cell indexes to rotation values.
            Dictionary<int, int> rotationMap = new Dictionary<int, int>
            {
                {0, 270},
                {1, 0},
                {2, 90},
                {3, 180}
            };

            // Loop through the rotation map.
            foreach (KeyValuePair<int, int> entry in rotationMap)
            {
                // Check if the current adjacent cell is a road cell.
                if (adjacentCells[entry.Key] != null && adjacentCells[entry.Key].IsRoadCell())
                {
                    // Modify the cell type.
                    cell.SetCellType(MapGridCellType.RoadDeadEnd);

                    // Construct a rotation quaternion.
                    Quaternion rotation = Quaternion.Euler(0, entry.Value, 0);

                    // Set the cell rotation.
                    cell.SetRotation(rotation);
                }
            }
        }

        /**
         * Used to create a straight road.
         * * 0 = left, 1 = top, 2 = right, 3 = bottom
         */
        private bool CreateStraightRoad(MapGridCell[] adjacentCells, MapGridCell cell)
        {
            // Map adjacent cell indexes to rotation values.
            Dictionary<int[], int> rotationMap = new Dictionary<int[], int>
            {
                {new int[] {0, 2}, 90},
                {new int[] {1, 3}, 0}
            };

            // Loop through the rotation map.
            foreach (KeyValuePair<int[], int> entry in rotationMap)
            {
                // Check if the two possible adjacent cells are road cells.
                if (adjacentCells[entry.Key[0]] != null && adjacentCells[entry.Key[0]].IsRoadCell() &&
                    adjacentCells[entry.Key[1]] != null && adjacentCells[entry.Key[1]].IsRoadCell())
                {
                    // Modify the cell type.
                    cell.SetCellType(MapGridCellType.RoadStraight);

                    // Construct a rotation quaternion.
                    Quaternion rotation = Quaternion.Euler(0, entry.Value, 0);

                    // Set the cell rotation.
                    cell.SetRotation(rotation);

                    // Return that we successfully created a straight road.
                    return true;
                }
            }

            // Return that we failed to create a straight road.
            return false;
        }

        /**
         * Used to create a corner.
         * * 0 = left, 1 = top, 2 = right, 3 = bottom
         */
        private void CreateCorner(MapGridCell[] adjacentCells, MapGridCell cell)
        {
            // Map adjacent cell indexes to rotation values.
            Dictionary<int[], int> rotationMap = new Dictionary<int[], int>
            {
                {new int[] {1, 2}, 0},
                {new int[] {2, 3}, 90},
                {new int[] {3, 0}, 180},
                {new int[] {0, 1}, 270}
            };

            // Loop through the rotation map.
            foreach (KeyValuePair<int[], int> entry in rotationMap)
            {
                // Check if the two possible adjacent cells are road cells.
                if (adjacentCells[entry.Key[0]] != null && adjacentCells[entry.Key[0]].IsRoadCell() &&
                    adjacentCells[entry.Key[1]] != null && adjacentCells[entry.Key[1]].IsRoadCell())
                {
                    // Modify the cell type.
                    cell.SetCellType(MapGridCellType.RoadCorner);

                    // Construct a rotation quaternion.
                    Quaternion rotation = Quaternion.Euler(0, entry.Value, 0);

                    // Set the cell rotation.
                    cell.SetRotation(rotation);
                }
            }
        }

        /**
         * Used to create a three way intersection.
         * * 0 = left, 1 = top, 2 = right, 3 = bottom
         */
        private void CreateIntersectionThreeWay(MapGridCell[] adjacentCells, MapGridCell cell)
        {
            // Check if this cell is in the roundabout positions list.
            if (roundaboutPositions.Contains(cell.GetPosition()))
            {
                // Return.
                return;
            }

            // Map adjacent cell indexes to rotation values.
            Dictionary<int[], int> rotationMap = new Dictionary<int[], int>
            {
                {new int[] {3, 0, 1}, 0},
                {new int[] {0, 1, 2}, 90},
                {new int[] {1, 2, 3}, 180},
                {new int[] {2, 3, 0}, 270},
            };

            // Loop through the rotation map.
            foreach (KeyValuePair<int[], int> entry in rotationMap)
            {
                // Check if the two possible adjacent cells are road cells.
                if (adjacentCells[entry.Key[0]] != null && adjacentCells[entry.Key[0]].IsRoadCell() &&
                    adjacentCells[entry.Key[1]] != null && adjacentCells[entry.Key[1]].IsRoadCell() &&
                    adjacentCells[entry.Key[2]] != null && adjacentCells[entry.Key[2]].IsRoadCell())
                {
                    // Modify the cell type.
                    cell.SetCellType(MapGridCellType.IntersectionThreeWay);

                    // Construct a rotation quaternion.
                    Quaternion rotation = Quaternion.Euler(0, entry.Value, 0);

                    // Set the cell rotation.
                    cell.SetRotation(rotation);
                }
            }
        }

        /**
         * Used to create a four way intersection.
         */
        private void CreateFourWayIntersection(MapGridCell cell)
        {

            // Get all cells in a 3x3 grid around the current cell.
            List<MapGridCell> surroundingCells = mapGridRef.GetSurroundingCells(cell, 1);

            // Get the count of road cells in the surrounding cells.
            int roadCellCount = surroundingCells.Count;

            // A map between surrounding cells index, cell type & rotation.
            // This is awful. I know.
            Dictionary<int, Dictionary<MapGridCellType, Quaternion>> surroundingCellMap = new Dictionary<int, Dictionary<MapGridCellType, Quaternion>>
            {
                {0, new Dictionary<MapGridCellType, Quaternion> {{MapGridCellType.RoadCorner, Quaternion.Euler(0, 0, 0)}}},                 // Bottom Left.
                {1, new Dictionary<MapGridCellType, Quaternion> {{MapGridCellType.IntersectionRoundabout, Quaternion.Euler(0, 0, 0)}}},     // Left
                {2, new Dictionary<MapGridCellType, Quaternion> {{MapGridCellType.RoadCorner, Quaternion.Euler(0, 90, 0)}}},                // Top Left
                {3, new Dictionary<MapGridCellType, Quaternion> {{MapGridCellType.IntersectionRoundabout, Quaternion.Euler(0, 270, 0)}}},   // Bottom
                {4, new Dictionary<MapGridCellType, Quaternion> {{MapGridCellType.Grass, Quaternion.Euler(0, 0, 0)}}},                      // Center
                {5, new Dictionary<MapGridCellType, Quaternion> {{MapGridCellType.IntersectionRoundabout, Quaternion.Euler(0, 90, 0)}}},    // Top
                {6, new Dictionary<MapGridCellType, Quaternion> {{MapGridCellType.RoadCorner, Quaternion.Euler(0, 270, 0)}}},               // Bottom Right
                {7, new Dictionary<MapGridCellType, Quaternion> {{MapGridCellType.IntersectionRoundabout, Quaternion.Euler(0, 180, 0)}}},   // Right
                {8, new Dictionary<MapGridCellType, Quaternion> {{MapGridCellType.RoadCorner, Quaternion.Euler(0, 180, 0)}}}                // Top Right
            };


            if (roadCellCount == 9 && Random.Range(0, 100) < 25)
            {
                // Loop through the surrounding cell map.
                for (int i = 0; i < roadCellCount; i++)
                {
                    // Get the surrounding cell info from the map.
                    Dictionary<MapGridCellType, Quaternion> surroundingCellInfo = surroundingCellMap[i];

                    // Set the surrounding cell at this index to the cell type.
                    surroundingCells[i].SetCellType(surroundingCellInfo.Keys.First());

                    // Set the rotation of the surrounding cell at this index.
                    surroundingCells[i].SetRotation(surroundingCellInfo.Values.First());

                    // Add the position of the cell to the roundabout positions list.
                    roundaboutPositions.Add(surroundingCells[i].GetPosition());
                }
            }
            // Otherwise, we can just create a four way intersection.
            else
            {
                // Set the cell type to a four way intersection.
                cell.SetCellType(MapGridCellType.IntersectionFourWay);

                // Set the cell rotation to 0.
                cell.SetRotation(Quaternion.Euler(0, 0, 0));
            }
        }

        /**
         * Used to get all roundabout cells.
         */
        public List<Vector3Int> GetRoundaboutCells()
        {
            return roundaboutPositions;
        }
    }
}