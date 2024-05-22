using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.AI
{
    public class PedestrianDirector : MonoBehaviour
    {
        // A list of pedestrians.
        public List<Pedestrian> pedestrians;

        // The pedestrian prefabs.
        public GameObject[] pedestrianPrefabs;

        // A reference to the GameManager.
        public GameManager gameManagerRef;

        // Create a pedestrian with a random path.
        public void CreatePedestrian()
        {
            // Get a random pedestrian prefab.
            GameObject pedestrianPrefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)];

            // Instantiate the pedestrian prefab.
            GameObject pedestrian = Instantiate(pedestrianPrefab, transform.position, Quaternion.identity);

            // Add the pedestrian component to the pedestrian.
            pedestrian.AddComponent<Pedestrian>();

            // Store the gamemanager reference in the pedestrian.
            pedestrian.GetComponent<Pedestrian>().gameManagerRef = gameManagerRef;

            // Set pedestrian parent to this object.
            pedestrian.transform.parent = transform;

            // Add the pedestrian to the list of pedestrians.
            pedestrians.Add(pedestrian.GetComponent<Pedestrian>());
        }

        // Start is called before the first frame update
        void Start()
        {
            // Initialize the list of pedestrians.
            pedestrians = new List<Pedestrian>();

            // Create 1000 pedestrians.
            for (int i = 0; i < 1000; i++)
            {
                // Create a pedestrian.
                CreatePedestrian();
            }
        }

    }
}
