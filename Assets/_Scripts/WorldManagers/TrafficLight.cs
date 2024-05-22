
using UnityEngine;

namespace TrafficSim.WorldManagers
{
    // This class represents a traffic light in-world.
    public class TrafficLight : MonoBehaviour
    {
        // A list of traffic light groups within this traffic light object.
        public TrafficLightGroup[] trafficLightGroups;
    }
}