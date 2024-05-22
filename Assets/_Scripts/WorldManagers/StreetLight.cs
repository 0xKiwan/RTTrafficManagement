using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.WorldManagers
{
    public class StreetLight : MonoBehaviour
    {
        private Light light;

        void Start()
        {
            light = transform.GetChild(0).GetComponent<Light>();
        }

        /**
         * Turns on the light
         */
        public void TurnOn()
        {
            // Turn on the light
            light.enabled = true;
        }

        /**
         * Turns off the light
         */
        public void TurnOff()
        {
            // Turn off the light
            light.enabled = false;
        }

        /**
         * Used to check if the light is on
         */
        public bool IsOn()
        {
            // Return the enabled state of the light
            return light.enabled;
        }
    }
}