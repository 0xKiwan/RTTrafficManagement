
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.WorldManagers
{
    // This class represents a traffic light in-world.
    public class TrafficLightGroup : MonoBehaviour
    {
        public Light redLight;
        public Light amberLight;
        public Light greenLight;
        public List<Pathfinding.RoadNode> crosswalkNodes; // Used to determine if a pedestriaan can cross the road while the light is red.


        /**
        * Used to set a light to amber.
        */
        public void SetAmber()
        {
            redLight.enabled = false;
            greenLight.enabled = false;
            amberLight.enabled = true;

            // If this light is amber, it's crosswalk nodes should make pedestrians wait.
            foreach (Pathfinding.RoadNode node in crosswalkNodes)
            {
                node.pedestrianShouldWait = true;
            }
        }

        /**
         * Used to set a light to green.
         */
        public void SetGreen()
        {
            redLight.enabled = false;
            greenLight.enabled = true;
            amberLight.enabled = false;

            // If this light is green, it's crosswalk nodes should make pedestrians wait.
            foreach (Pathfinding.RoadNode node in crosswalkNodes)
            {
                node.pedestrianShouldWait = true;
            }
        }

        /**
         * Used to set a light to red.
         */
        public void SetRed()
        {
            redLight.enabled = true;
            greenLight.enabled = false;
            amberLight.enabled = false;

            // If this light is red, it's crosswalk nodes should allow pedestrians to cross.
            foreach (Pathfinding.RoadNode node in crosswalkNodes)
            {
                node.pedestrianShouldWait = false;
            }
        }
    }
}