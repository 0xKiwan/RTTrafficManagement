using System.Collections;
using System.Collections.Generic;
using TrafficSim.WorldManagers;
using UnityEngine;

namespace TrafficSim.Pathfinding
{
    public class RoadNode : MonoBehaviour
    {
        /**
         * An enum representing the node type. I.e. sidewalk, crosswalk, whatever else I come up with.
         */
        public enum NodeType
        {
            SIDEWALK,
            CROSSWALK
        }

        // A list of nodes this node can connect to within a road cell.
        public List<RoadNode> connectedNodes;

        // A* pathfinding variables
        public Vector3 pos; // Position of the node
        public NodeType type;   // The type of node
        public bool lazyConnected; // Whether or not this node is connnected by distance alone
        public bool cornerStart;   // Whether or not this node is the start of a corner
        public bool pedestrianShouldWait = false; // Whether or not a pedestrian should wait at this node for the light to change
        public Intersection intersectionParent;  // If the node belongs to an intersection, this will not be null.

        // Getter for position
        public Vector3 position
        {
            get { return transform.position; }
        }


        // Start is called before the first frame update
        void Start()
        {
            // Store the position of the node.
            pos = transform.position;

            // Stop updating the node, no node needs to be updated.
            this.enabled = false;
        }
    }
}
