
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.WorldManagers
{
    public class Intersection : MonoBehaviour
    {
        // North facing traffic light(s)
        public List<TrafficLightGroup> northLights = new List<TrafficLightGroup>();

        // South facing traffic light(s)
        public List<TrafficLightGroup> southLights = new List<TrafficLightGroup>();

        // East facing traffic light(s)
        public List<TrafficLightGroup> eastLights = new List<TrafficLightGroup>();

        // West facing traffic light(s)
        public List<TrafficLightGroup> westLights = new List<TrafficLightGroup>();

        private float timer = 0f; // Used to track time for light changes

        // This list will be used to store all traffic light groups in the intersection
        private List<List<TrafficLightGroup>> allLights = new List<List<TrafficLightGroup>>();

        // The index of the currently active light group
        private int currentLightGroupIndex = 0;

        // The currently active light group.
        private List<TrafficLightGroup> currentGroup;

        // The last light group that was active.
        private List<TrafficLightGroup> lastGroup;

        // If the intersection is active, pedestrians must request to cross. Similar to uk pedestrian crossings.
        public bool intersectionActive = true;

        // Whether or not a request to cross has been made.
        public bool requestToCross = false;

        // How long to wait before changing the light.
        private float lightChangeTime = 10f;

        // How long to wait before changing from amber to any other light.
        private float amberTime = 2f;

        // How long since RequestToCross was called.
        private float requestTimer = 0f;

        // How long since all lights were made red for crossing.
        private float crossingTimer = 0f;

        // Start is called before the first frame update
        void Start()
        {
            // Only add lights lists to the allLights list if they are not empty.
            if (northLights.Count > 0) allLights.Add(northLights);
            if (southLights.Count > 0) allLights.Add(southLights);
            if (eastLights.Count > 0) allLights.Add(eastLights);
            if (westLights.Count > 0) allLights.Add(westLights);

            // Set all lights to red
            SetAllLightsRed();

            // Choose a random light group to start with
            currentGroup = allLights[0];

            // Choose a random last group
            lastGroup = allLights[1];

            // Set lastGroup to green.
            foreach (var lightGroup in lastGroup) lightGroup.SetGreen();

            // Set the current group to red.
            foreach (var lightGroup in currentGroup) lightGroup.SetRed();
        }

        // Public function for requesting to cross the intersection.
        public void RequestToCross()
        {
            // If the intersection is active, set requestToCross to true.
            if (intersectionActive) {

                // Mark requestToCross as true.
                requestToCross = true;

                // Reset the requestTimer.
                requestTimer = 0f;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Increment the timer.
            timer += Time.deltaTime;

            // Increment requestTimer.
            requestTimer += Time.deltaTime;

            // Increment crossingTimer.
            crossingTimer += Time.deltaTime;

            // If requestToCross is true, and 5 seconds have passed since the request was made, stage to red and skip the rest of the update.
            if (requestToCross && requestTimer >= 5f)
            {
                // Set all lights to red.
                SetAllLightsRed();

                // Reset requestToCross.
                requestToCross = false;

                // Reset crossingTimer.
                crossingTimer = 0f;

                // If the current light group index is greater than or equal to the allLights count, reset the index to 0.
                if (currentLightGroupIndex >= allLights.Count) currentLightGroupIndex = 0;

                // Skip the rest of the update.
                return;
            }

            // Give pedestrians 5 seconds to cross the intersection.
            if (crossingTimer < 5f)
            {
                // Skip & return.
                return;
            }

            // Check if currentGroup has changed.
            if (currentGroup != allLights[currentLightGroupIndex])
            {
                // Update lastGroup to currentGroup.
                lastGroup = currentGroup;

                // Update currentGroup to the new currentGroup.
                currentGroup = allLights[currentLightGroupIndex];
            }

            // If the timer is greater than the light change time, change the lights.
            if (timer >= lightChangeTime)
            {
                // Stage lastGroup to amber.
                foreach (var lightGroup in lastGroup) lightGroup.SetAmber();;

                // Stage currentGroup to amber.
                foreach (var lightGroup in currentGroup) lightGroup.SetAmber();
            }

            // Check if timer >= lightChangeTime + amberTime and staged == false
            if (timer >= (lightChangeTime + amberTime))
            {
                // Finalize lastGroup to red, and set currentGroup to green.
                foreach (var lightGroup in lastGroup) lightGroup.SetRed();
                foreach (var lightGroup in currentGroup) lightGroup.SetGreen();

                // Mark intersection as active, if ANY light is green.
                intersectionActive = true;

                // Reset the timer.
                timer = 0f;

                // Increment the current light group index.
                currentLightGroupIndex++;

                // If the current light group index is greater than or equal to the allLights count, reset the index to 0.
                if (currentLightGroupIndex >= allLights.Count) currentLightGroupIndex = 0;
            }
        }

        /**
         * Used to set all lights to red.
         */
        private void SetAllLightsRed()
        {
            // Loop through all of the lightGroup lists in allLights.
            foreach (var lightGroup in allLights)
            {
                // Loop through all of the lightGroups in the lightGroup list.
                foreach (var light in lightGroup)
                {
                    // Set the light to red.
                    light.SetRed();
                }
            }

            // Mark intersection as inactive.
            intersectionActive = false;
        }

    }
}