using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.WorldManagers
{
    public class Intersection : MonoBehaviour
    {
        // Traffic light groups for each direction
        public List<TrafficLightGroup> northLights = new List<TrafficLightGroup>();
        public List<TrafficLightGroup> southLights = new List<TrafficLightGroup>();
        public List<TrafficLightGroup> eastLights = new List<TrafficLightGroup>();
        public List<TrafficLightGroup> westLights = new List<TrafficLightGroup>();

        // All traffic light groups in the intersection
        private List<List<TrafficLightGroup>> allLights = new List<List<TrafficLightGroup>>();

        // Index of the currently active light group
        private int currentLightGroupIndex = 0;

        // Intersection status
        public bool intersectionActive = true;
        public bool requestToCross = false;

        // Timer variables
        private const float lightChangeTime = 10f;
        private const float amberTime = 2f;
        private float crossingTime = 5f;

        private float timer = 0f;
        private float requestTimer = 0f;
        private float crossingTimer = 0f;

        // Start is called before the first frame update
        void Start()
        {
            InitializeLights();
            SetAllLightsRed();
            SetInitialLightGroups();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTimers();
            HandleRequestToCross();
            HandleLightChanges();
        }

        // Initialize traffic light groups
        void InitializeLights()
        {
            if (northLights.Count > 0) allLights.Add(northLights);
            if (southLights.Count > 0) allLights.Add(southLights);
            if (eastLights.Count > 0) allLights.Add(eastLights);
            if (westLights.Count > 0) allLights.Add(westLights);
        }

        // Set initial light groups to green
        void SetInitialLightGroups()
        {
            foreach (var lightGroup in allLights[currentLightGroupIndex])
            {
                lightGroup.SetGreen();
            }
        }

        // Update timers
        void UpdateTimers()
        {
            // Increment timers
            timer += Time.deltaTime;
            requestTimer += Time.deltaTime;
            crossingTimer += Time.deltaTime;
        }

        // Handle pedestrian request to cross
        void HandleRequestToCross()
        {
            // If the intersection is active and a request to cross has been made
            if (intersectionActive && requestToCross && requestTimer >= crossingTime)
            {
                // Randomize the crossing request wait time.
                crossingTime = Random.Range(5f, 20f);

                // Set all lights to red and reset request variables
                SetAllLightsRed();
                requestToCross = false;
                crossingTimer = 0f;
                return;
            }
        }

        // Handle traffic light changes
        void HandleLightChanges()
        {
            if (timer >= lightChangeTime)
            {
                foreach (var lightGroup in allLights[currentLightGroupIndex])
                {
                    lightGroup.SetAmber();
                }
            }

            if (timer >= lightChangeTime + amberTime)
            {
                foreach (var lightGroup in allLights[currentLightGroupIndex])
                {
                    lightGroup.SetRed();
                }

                currentLightGroupIndex = (currentLightGroupIndex + 1) % allLights.Count;
                foreach (var lightGroup in allLights[currentLightGroupIndex])
                {
                    lightGroup.SetGreen();
                }

                intersectionActive = true;
                timer = 0f;
            }
        }

        // Set all lights to red
        private void SetAllLightsRed()
        {
            foreach (var lightGroup in allLights)
            {
                foreach (var light in lightGroup)
                {
                    light.SetRed();
                }
            }

            intersectionActive = false;
        }

        // Public function for requesting to cross the intersection
        public void RequestToCross()
        {
            if (intersectionActive)
            {
                requestToCross = true;
                requestTimer = 0f;
            }
        }
    }
}