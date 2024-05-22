using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.Player
{
    public class CameraController : MonoBehaviour
    {
        public Camera topDownCamera;
        public Camera freeCamera;
        public MainMenu mainMenu;

        private float shiftHeldDuration = 0f;
        private Dictionary<KeyCode, Vector3> keyToDirection;

        private const float baseSpeed = 10f;
        private const float shiftMultiplier = 2.5f;
        private const float rotationSpeed = 100f;

        // Original min and max values for the camera's position.
        private Vector3 originalMin, originalMax;

        // Editable min and max values for the camera's position.
        private Vector3 minBounds, maxBounds;

        // Start is called before the first frame update
        void Start()
        {
            // Get the Main Menu object.
            mainMenu = GameObject.Find("MainMenu").GetComponent<MainMenu>();

            // Set original min and max bounds.
            originalMin = new Vector3(34f, 0f, 24.4f);
            originalMax = new Vector3(165f, 0f, 174.5f);

            // Initialize editable bounds to their original values.
            minBounds = originalMin;
            maxBounds = originalMax;

            // Initialize the dictionary mapping keycodes to directions.
            keyToDirection = new Dictionary<KeyCode, Vector3>
            {
                { KeyCode.W, Vector3.forward },     // W = forward
                { KeyCode.A, Vector3.left },        // A = left
                { KeyCode.S, Vector3.back },        // S = back
                { KeyCode.D, Vector3.right },       // D = right
                { KeyCode.Q, Vector3.up },          // Q = up
                { KeyCode.E, Vector3.down }         // E = down
            };
        }

        // Update is called once per frame
        void Update()
        {
            // Check if the main menu object was not found, and find it if necessary.
            if (mainMenu == null)
            {
                mainMenu = GameObject.Find("MainMenu").GetComponent<MainMenu>();
            }

            // Check if the menu is active.
            if (mainMenu.menuActive) return;

            // Get the current active camera.
            Camera currentCamera = Camera.current;

            // Check if the current camera is the top-down camera.
            if (currentCamera == topDownCamera)
            {
                // Handle top-down camera movement and zoom.
                HandleTopDownCamera();
            }
            // Otherwise, it is the free camera.
            else if (currentCamera == freeCamera)
            {
                // Handle free camera movement and rotation.
                HandleFreeCamera();
            }
        }

        // Handle free camera movement and rotation.
        private void HandleFreeCamera()
        {
            // Handle free camera rotation when the user holds the right mouse button.
            if (Input.GetMouseButton(1))
            {
                HandleFreeCameraRotation();
            }
            // Free the cursor when the user releases the right mouse button.
            else
            {
                // Show the cursor.
                Cursor.visible = true;

                // Unlock the cursor.
                Cursor.lockState = CursorLockMode.None;
            }

            // Handle free camera movement.
            HandleFreeCameraMovement();
        }

        // Handle free camera rotation.
        private void HandleFreeCameraRotation()
        {
            // Hide the cursor.
            Cursor.visible = false;

            // Lock cursor to the center of the screen.
            Cursor.lockState = CursorLockMode.Locked;

            // Get the current camera's rotation.
            Vector3 rotation = freeCamera.transform.rotation.eulerAngles;

            // Handle horizontal rotation.
            rotation.y += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            // Handle vertical rotation.
            rotation.x -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Set the camera's rotation.
            freeCamera.transform.rotation = Quaternion.Euler(rotation);
        }

        // Handle free camera movement.
        private void HandleFreeCameraMovement()
        {
            // Determine the speed multiplier based on shift key.
            float speed = baseSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1f);

            // Loop through each key in the dictionary.
            foreach (var keyDirection in keyToDirection)
            {
                // Check if the user is holding the key.
                if (Input.GetKey(keyDirection.Key))
                {
                    // Handle movement.
                    freeCamera.transform.Translate(keyDirection.Value * speed * Time.deltaTime, Space.Self);
                }
            }
        }

        // Handle top-down camera movement and zoom.
        private void HandleTopDownCamera()
        {
            // Handle WASD movement.
            HandleTopDownCameraMovement();

            // Handle camera zoom via scroll wheel.
            HandleTopDownCameraZoom();
        }

        // Handle top-down camera movement.
        private void HandleTopDownCameraMovement()
        {
            // Determine the speed multiplier based on shift key.
            float speed = baseSpeed * (Input.GetKey(KeyCode.LeftShift) ? Mathf.Clamp(shiftMultiplier * shiftHeldDuration * 5, 1f, 10f) : 1f);

            // Check if the user is holding shift and adjust shift held duration.
            if (Input.GetKey(KeyCode.LeftShift))
            {
                shiftHeldDuration += Time.deltaTime;
            }
            else
            {
                shiftHeldDuration = 0f;
            }

            // Get the current camera's position.
            Vector3 position = topDownCamera.transform.position;

            // Handle horizontal & vertical movement.
            position.x += Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            position.z += Input.GetAxis("Vertical") * speed * Time.deltaTime;

            // Clamp the camera's position.
            position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
            position.z = Mathf.Clamp(position.z, minBounds.z, maxBounds.z);

            // Set the camera's position.
            topDownCamera.transform.position = position;
        }

        // Handle top-down camera zoom.
        private void HandleTopDownCameraZoom()
        {
            // Determine the zoom speed multiplier based on shift key.
            float zoomSpeed = baseSpeed * (Input.GetKey(KeyCode.LeftShift) ? Mathf.Clamp(shiftMultiplier * shiftHeldDuration * 5, 1f, 10f) : 1f);

            // Adjust the orthographic size of the camera based on scroll wheel input.
            float newSize = Mathf.Clamp(topDownCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime, 10f, 25f);
            topDownCamera.orthographicSize = newSize;

            // Scale the bounds based on the new orthographic size.
            float scale = newSize / 25f;
            minBounds.x = originalMin.x * scale;
            minBounds.z = originalMin.z * scale;

            // Adjust the max bounds of the camera's position.
            maxBounds.x = originalMax.x + (originalMin.x - minBounds.x);
            maxBounds.z = originalMax.z + (originalMin.z - minBounds.z);
        }
    }
}