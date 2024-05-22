using System.Collections;
using System.Collections.Generic;
using TrafficSim.ProceduralEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

/**
 * This has to be some of the worst code I've ever written. I'm sorry.
 */

namespace TrafficSim.Pathfinding
{
    public class PedestrianPathfinding
    {
        List<MapGridCell> origPath; // The original path


        /**
         * Constructor for the PedestrianPathfinding class
         */
        public PedestrianPathfinding(List<MapGridCell> origPath)
        {
            // Store the original path
            this.origPath = origPath;
        }

        /**
         * Find a random node within the MapGridCell.
         * This will be the pedestrian's spawn point.
         */
        public RoadNode FindRandomPedestrianNode(MapGridCell cell)
        {
            // Get the RoadHelper from the cell.
            RoadHandler roadHelper = cell.GetRoadHandler();

            // Return null if the RoadHandler is null.
            if (roadHelper == null) return null;

            // Get a random node from the RoadHandler.
            return roadHelper.pedestrianNodes[Random.Range(0, roadHelper.pedestrianNodes.Count)];
        }

        /**
         * Used to generate a random path given the origPath between all of the nodes inside of the road cells
         */
        public List<RoadNode> GenerateRandomPath()
        {
            // Check we have a valid path.
            if (origPath.Count == 0) return new List<RoadNode>();

            // Get a random roadNode to start the path.
            RoadNode startNode = FindRandomPedestrianNode(origPath[0]);

            // Return null if the startNode is null.
            if (startNode == null) return null;

            // Initialize the path list.
            List<RoadNode> nodePath = new List<RoadNode>() { startNode };

            // Loop through each cell in the original path.
            for (int i = 0; i < origPath.Count; i++)
            {
                // Get the cell at this index.
                MapGridCell cell = origPath[i];

                // This will store the closest node to the last node in the path.
                RoadNode closestNode = null;

                // This will store the distance to the closest node.
                float closestDistance = float.MaxValue;

                // Get the RoadHandler from the current cell.
                RoadHandler roadHelper = cell.GetRoadHandler();

                // Loop through each node in the RoadHandler. Bias towards sidewalk nodes, but choose crossing nodes if we have to.
                foreach (RoadNode node in roadHelper.pedestrianNodes)
                {
                    // Get the distance between the last node in the path and this node.
                    float distance = Mathf.Abs(Vector3.Distance(node.position, nodePath[nodePath.Count - 1].position));

                    // If this node is closer than the last closest node and it is a sidewalk node, update the closest node.
                    if (distance < closestDistance && node.type == RoadNode.NodeType.SIDEWALK)
                    {
                        closestNode     = node;
                        closestDistance = distance;
                    }
                }

                // Mark this node as lazily connected.
                closestNode.lazyConnected = true;

                // Add the closest node to the path.
                nodePath.Add(closestNode);

                // If we're at the end of the path, break.
                if (i == origPath.Count - 1) break;

                // Get the next cell in the path.
                MapGridCell nextCell = origPath[i+1];

                // Calculate the direction between this cell and the next cell.
                Vector3 direction = nextCell.GetPosition() - cell.GetPosition();

                // Normalize the direction.
                direction.Normalize();

                // Check if the current cell is a corner.
                if (roadHelper.isCorner)
                {
                    // Find the closest node within the corner, to the previous node.
                    RoadNode cornerStart = null;

                    // Will store the distance to the closest node.
                    float cornerDistance = float.MaxValue;

                    // Loop through each node in the corner.
                    foreach (RoadNode node in roadHelper.pedestrianNodes)
                    {
                        // Get the distance between the node and the last node in the path.
                        float distance = Mathf.Abs(Vector3.Distance(node.position, nodePath[nodePath.Count - 1].position));

                        // If this node is closer than the last closest node, update the closest node.
                        if (distance < cornerDistance)
                        {
                            cornerStart = node;
                            cornerDistance = distance;
                        }
                    }

                    // Mark the cornerStart as a corner start.
                    cornerStart.cornerStart = true;

                    // Add to the path.
                    nodePath.Add(cornerStart);

                    // Store the current node as cornerStart.
                    RoadNode cornerEnd = cornerStart;

                    // Maximum number of loops to traverse a corner.
                    int maxLoops = 20;
                    int curLoops = 0;

                    // Loop through connected nodes, and update cornerEnd with each found connected node that isn't already in the path.
                    while (cornerEnd.connectedNodes.Count > 0)
                    {
                        // Break if we've reached 20 loops.
                        if (curLoops > maxLoops) break;

                        // Loop through cornerEnd's connectedNodes.
                        foreach (RoadNode node in cornerEnd.connectedNodes)
                        {
                            // Check if this node is already in the path.   
                            if (nodePath.Contains(node)) continue;

                            // Store this node in the path.
                            nodePath.Add(node);

                            // Update cornerEnd.
                            cornerEnd = node;
                        }

                        // Increment the loop counter.
                        curLoops++;
                    }
                }

                else
                {
                    // Find connections from the last node in the path.
                    foreach (RoadNode node in nodePath[nodePath.Count - 1].connectedNodes)
                    {
                        // Get the direction from the last node to this node.
                        Vector3 nodeDirection = node.position - nodePath[nodePath.Count - 1].position;

                        // Normalize the direction.
                        nodeDirection.Normalize();

                        // If the directions are the same, add this node to the path.
                        if (Vector3.Dot(direction, nodeDirection) > 0.9f)
                        {
                            nodePath.Add(node);
                        }
                    }
                }
            }

            return nodePath;
        }
    }
}