using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.Pathfinding
{
    public class RoadHandler : MonoBehaviour
    {
        // A list of all the pedestrian nodes for this specific road cell.
        public List<RoadNode> pedestrianNodes;

        // Determines if the road type is a corner or not.
        public bool isCorner;

        // Corner threshold for turning on a corner.
        float approximateThresholdCorner = 0.3f;

        // A reference to this road's intersection.
        public WorldManagers.Intersection intersectionRef;

        // Start is called before the first frame update
        void Start()
        {
            // Disable updates for the RoadHandler. They are un-neccessary.
            this.enabled = false;
        }

        /**
         * Used to get the nearest node to a given position.
         */
        public RoadNode GetNearestNode(Vector3 position)
        {
            // Check if this marker is a corner.
            if (isCorner)
            {
                // Loop through pedestrian markers.
                foreach (RoadNode node in pedestrianNodes)
                {
                    // Get the direction between the marker position and the given position.
                    Vector3 direction = node.pos - position;

                    // Normalize the direction.
                    direction.Normalize();

                    // Check if the direction is within the threshold.
                    if (Mathf.Abs(direction.x) < approximateThresholdCorner && Mathf.Abs(direction.z) < approximateThresholdCorner)
                    {
                        // Return the node.
                        return node;
                    }
                }

                // Return null.
                return null;
            }

            //
            // If the road is not a corner, return the nearest node.
            //

            // Will store the closest node.
            RoadNode closestNode = null;

            // Will store the distance to the closest node.
            float closestDistance = float.MaxValue;

            // Loop through each node in the pedestrian nodes.
            foreach (RoadNode node in pedestrianNodes)
            {
                // Get the distance between the node and the given position.
                float distance = Mathf.Abs(Vector3.Distance(node.pos, position));

                // If this node is closer than the last closest node, update the closest node.
                if (distance < closestDistance)
                {
                    closestNode = node;
                    closestDistance = distance;
                }
            }

            // Return the closest node.
            return closestNode;
        }
    }
}