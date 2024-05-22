using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.PropHandlers
{
    public class StreetLight : MonoBehaviour
    {
        private Light light; // The light component of the street light.
        private WorldManagers.TimeManager timeManager;

        // Start is called before the first frame update
        void Start()
        {
            // Get the first child.
            Transform child = transform.GetChild(0);

            // Get the light from the child.
            light = child.GetComponent<Light>();

            // Disable the light.
            light.enabled = false;

            // Get the TimeManager component.
            timeManager = GameObject.Find("TimeManager").GetComponent<WorldManagers.TimeManager>();
        }

        // Update is called once per frame
        void Update()
        {
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
