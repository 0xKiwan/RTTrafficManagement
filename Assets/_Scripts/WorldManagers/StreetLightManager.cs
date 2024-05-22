using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.WorldManagers
{
    public class StreetLightManager : MonoBehaviour
    {
        // A list of all street lights in the world.
        public List<StreetLight> streetLights = new List<StreetLight>();

        // The TimeManager object.
        public TimeManager timeManager;

        // Boolean to keep track of whether initialization has been done.
        private bool initialized = false;

        // Start is called before the first frame update
        void Start()
        {
            // Cache street lights once when the scene starts.
            if (!initialized)
            {
                InitializeStreetLights();
                initialized = true;
            }

            // Subscribe to the time changed event.
            timeManager.OnTimeChanged += UpdateStreetLights;
        }

        // Update the street lights based on the time of day.
        void UpdateStreetLights(bool isDaytime)
        {
            // Loop through all of the street lights
            foreach (StreetLight light in streetLights)
            {
                // If the light is not null, update it
                if (light != null)
                {
                    // If it's daytime and the light is on, turn off the light.
                    if (isDaytime && light.IsOn())
                    {
                        light.TurnOff();
                    }

                    // If it's nighttime and the light is off, turn on the light.
                    else if (!isDaytime && !light.IsOn())
                    {
                        light.TurnOn();
                    }
                }
            }
        }

        // Cache street lights in the scene.
        void InitializeStreetLights()
        {
            streetLights.AddRange(FindObjectsOfType<StreetLight>());
            // Remove null entries from the list.
            streetLights.RemoveAll(light => light == null);
        }

        // Unsubscribe from the time changed event when the object is destroyed.
        private void OnDestroy()
        {
            timeManager.OnTimeChanged -= UpdateStreetLights;
        }
    }
}