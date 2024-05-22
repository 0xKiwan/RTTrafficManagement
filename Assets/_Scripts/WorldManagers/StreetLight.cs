using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.WorldManagers
{
    public class StreetLight : MonoBehaviour
    {
        private Light light; // The light component of the street light.
        private WorldManagers.TimeManager timeManager;

        // Start is called before the first frame update
        void Start()
        {
            // Get the light child.
            light = GetComponentInChildren<Light>();

            // Check we found the light.
            if (light != null)
            {
                // Disable the light.
                light.enabled = false;
            }

            // Get the TimeManager component.
            timeManager = GameObject.Find("TimeManager").GetComponent<WorldManagers.TimeManager>();
        }

        // Update is called once per frame
        void Update()
        {
            // Ensure we got the light component.
            if (light == null)
            {
                // Get the light child.
                light = GetComponentInChildren<Light>();

                // Check if we found the light.
                if (light != null)
                {
                    light.enabled = false;
                }

                // Return. We don't need to do anything else.
                return;
            }

            // Ensure we got the timeManager component.
            if (timeManager == null)
            {
                // Get the TimeManager component.
                timeManager = GameObject.Find("TimeManager").GetComponent<WorldManagers.TimeManager>();

                // Return. We don't need to do anything else.
                return;
            }

            // Check if it is currently daytime.
            if (timeManager.IsDaytime() && light.enabled == true)
            {
                // Disable the light.
                light.enabled = false;

                // Return. We don't need to do anything else.
                return;
            }

            // Enable the light.
            light.enabled = true;
        }
    }
}
