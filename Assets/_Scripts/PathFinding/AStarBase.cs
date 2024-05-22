using System.Collections;
using System.Collections.Generic;
using TrafficSim.ProceduralEngine;
using UnityEngine;

namespace TrafficSim.Pathfinding
{
    public class AStarBase
    {
        private MapGrid mapGridRef; // The map grid
        private MapGridCell start; // The start position
        private MapGridCell end; // The end position

        // Initialize the A* algorithm
        public AStarBase(MapGrid grid, MapGridCell start, MapGridCell end)
        {
            // Set the map grid reference
            mapGridRef = grid;

            // Set the start and end positions
            this.start = start;
            this.end = end;
        }

        // Find the path
        public List<MapGridCell> FindPath()
        {
            // Initialize the open and closed lists
            List<MapGridCell> openList = new List<MapGridCell>();
            HashSet<MapGridCell> closedList = new HashSet<MapGridCell>();

            // Add the start position to the open list
            openList.Add(start);

            // Initialize the costs for the start node
            start.gCost = 0;
            start.hCost = GetDistance(start, end);
            start.parent = null;

            // Loop until the open list is empty
            while (openList.Count > 0)
            {
                // Sort the open list by fCost
                openList.Sort((a, b) => a.fCost.CompareTo(b.fCost));

                // Get the current cell
                MapGridCell currentCell = openList[0];

                // Remove the current cell from the open list
                openList.Remove(currentCell);

                // If the goal has been reached, reconstruct the path and return it
                if (currentCell == end)
                {
                    return RetracePath(start, end);
                }

                // Add the current cell to the closed list
                closedList.Add(currentCell);

                // Check each neighbor of the current cell
                foreach (MapGridCell neighbor in mapGridRef.GetNeighboringCells(currentCell))
                {
                    // Skip non-road cells or already processed cells
                    if (!neighbor.IsRoadCell() || closedList.Contains(neighbor)) continue;

                    // Work out a tentative gCost
                    float tentativeGCost = currentCell.gCost + GetDistance(currentCell, neighbor);

                    // Find openNode in openList
                    bool isInOpenList = openList.Contains(neighbor);

                    // Check if this path to the neighbor is better than any previous one
                    if (!isInOpenList || tentativeGCost < neighbor.gCost)
                    {
                        neighbor.gCost = tentativeGCost;
                        neighbor.hCost = GetDistance(neighbor, end);
                        neighbor.parent = currentCell;

                        if (!isInOpenList)
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            // Return an empty list if no path is found
            return new List<MapGridCell>();
        }

        // Retrace the path
        private List<MapGridCell> RetracePath(MapGridCell start, MapGridCell end)
        {
            // Initialize the path
            List<MapGridCell> path = new List<MapGridCell>();

            // Set the current cell to the end cell
            MapGridCell currentCell = end;

            // Loop until the current cell is the start cell
            while (currentCell != start)
            {
                // Add the current cell to the path
                path.Add(currentCell);

                // Set the current cell to the parent of the current cell
                currentCell = currentCell.parent;
            }

            // Add the start cell to the path
            path.Add(start);

            // Reverse the path
            path.Reverse();

            // Return the path
            return path;
        }

        // Calculate the manhattan distance between two cells
        private static int GetDistance(MapGridCell a, MapGridCell b)
        {
            // Get positions of the cells
            Vector3Int posA = a.GetPosition();
            Vector3Int posB = b.GetPosition();

            // Calculate the distance between the cells
            int dstX = Mathf.Abs(posA.x - posB.x);
            int dstY = Mathf.Abs(posA.y - posB.y);

            // Return the distance
            return dstX + dstY;
        }
    }
}