using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.Player
{
    public class CameraSwitcher : MonoBehaviour
    {
        public Camera[] cameras;                // The list of usable cameras.
        public int currentCameraIndex = 0;     // The index of the current camera.

        // Start is called before the first frame update
        void Start()
        {
            // Ensure only the first camera is active at startup.
            for (int i = 1; i < cameras.Length; i++)
            {
                cameras[i].gameObject.SetActive(false);
            }

            // Ensure the first camera is active and set as current.
            if (cameras.Length > 0)
            {
                cameras[0].gameObject.SetActive(true);
                Camera.SetupCurrent(cameras[0]);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Check if the user presses "C" to switch cameras.
            if (Input.GetKeyDown(KeyCode.C))
            {
                SwitchCamera();
            }
        }

        // Switch to the next camera in the list.
        private void SwitchCamera()
        {
            // Disable the current camera.
            cameras[currentCameraIndex].gameObject.SetActive(false);

            // Move to the next camera in the list.
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

            // Enable the new camera.
            cameras[currentCameraIndex].gameObject.SetActive(true);

            // Set Camera.current to the new camera.
            Camera.SetupCurrent(cameras[currentCameraIndex]);
        }
    }
}