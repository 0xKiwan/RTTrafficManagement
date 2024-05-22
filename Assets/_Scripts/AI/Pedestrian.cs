using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TrafficSim.Player;

namespace TrafficSim.AI
{
    public class Pedestrian : MonoBehaviour
    {
        public GameObject pedestrianPrefab;                     // The pedestrian prefab.
        public Pathfinding.PedestrianPathfinding pathfinding;   // The pedestrian pathfinding.
        public Pathfinding.AStarBase aStar;                     // The A* pathfinding base algorithm.
        public GameManager gameManagerRef;                      // A reference to the game manager.
        public ProceduralEngine.MapGrid mapGrid;                // The map grid.
        public List<Pathfinding.RoadNode> path;                 // The path the pedestrian will walk.
        public Pathfinding.RoadNode nextNode;                   // The next node in the path.
        public int pathIndex = 0;                               // The index of the current node in the path.
        public float walkSpeed = 0.3f;                          // The speed at which the pedestrian walks.
        public bool paused = false;                             // Whether or not the pedestrian is paused.
        private Camera mainCamera;                              // The main camera.
        private bool isWalkingForward = true;                   // Whether or not the pedestrian is walking forward.
        private CameraSwitcher cameraSwitcher;                  // The camera switcher script.

        /**
         * Start is called before the first frame update.
         */
        public void Start()
        {
            // Get the main camera.
            mainCamera = Camera.main;

            // Get the "CameraManager" object.
            GameObject cameraManager = GameObject.Find("CameraManager");

            // Get the "CameraSwitcher" script.
            cameraSwitcher = cameraManager.GetComponent<CameraSwitcher>();

            // Get the map grid.
            mapGrid = gameManagerRef.mapGenerator.mapGrid;

            // Get all of the road cells.
            List<ProceduralEngine.MapGridCell> roadCells = mapGrid.GetAllRoadCells();

            // Ensure map generation was successful.
            if (roadCells.Count < 2)
            {
                // Destroy this object & return.
                Destroy(gameObject);
                return;
            }

            // Find a start, and end location for the pedestrian's path.
            ProceduralEngine.MapGridCell startCell = roadCells[Random.Range(0, roadCells.Count)];
            ProceduralEngine.MapGridCell endCell = roadCells[Random.Range(0, roadCells.Count)];

            // Perform A* pathfinding between the start and end cells.
            aStar = new Pathfinding.AStarBase(mapGrid, startCell, endCell);
            List<ProceduralEngine.MapGridCell> basePath = aStar.FindPath();
            
            // Convert the base A* pathfinding into a valid pedestrian node path.
            pathfinding = new Pathfinding.PedestrianPathfinding(basePath);
            path = pathfinding.GenerateRandomPath();

            // Check everything was successful.
            if (path == null || path.Count == 0)
            {
                // Destroy this object & return.
                Destroy(gameObject);
                return;
            }

            // Get the first node in the path.
            Transform startNode = path[0].transform;

            // Set the pedestrian's position to the start node.
            transform.position = new Vector3(startNode.position.x, startNode.position.y, startNode.position.z);

            // Rotate the pedestrian to face the direction of the next node.
            RotateTowardsNextNode();

            // Start a coroutine to walk the path, avoiding blocking the main thread with updates.
            StartCoroutine(WalkPath());
        }

        /**
         * Used to rotate the pedestrian towards the next node in the path.
         */
        private void RotateTowardsNextNode()
        {
            // Check if we are at the end of the path.
            if (pathIndex >= path.Count - 1) return;

            // Get the start node.
            Pathfinding.RoadNode startNode = path[pathIndex];

            // Get the next node.
            nextNode = path[pathIndex + 1];

            // Calculate the direction.
            Vector3 direction = nextNode.position - startNode.position;

            // Normalize the direction.
            direction.Normalize();

            // Get the angle between the direction and the forward vector.
            float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);

            // Rotate the pedestrian to face the direction of the next node.
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        /**
         * Used to check if the pedestrian is visible to the main camera.
         * Pedestrians don't have colliders, so we need to check if they are within the camera's frustum.
         */
        private bool IsVisibleToMainCamera()
        {
            // Combined frustum and clipping plane checks.
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(transform.position);
            return screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height && screenPoint.z > mainCamera.nearClipPlane;
        }

        /**
         * A coroutine to walk the pedestrian along the path.
         */
        private IEnumerator WalkPath()
        {
            // Loop forever.
            while (true)
            {
                // Check if the pedestrian is paused.
                if (paused)
                {
                    // If the pedestrian IS paused, wait for a frame.
                    yield return null;
                    continue;
                }

                // Get the distance between the pedestrian and the main camera.
                if (cameraSwitcher.cameras[cameraSwitcher.currentCameraIndex] == mainCamera && !IsVisibleToMainCamera())
                {
                    // Wait until the pedestrian is closer to the camera.
                    yield return new WaitForSeconds(0.001f);
                    continue;
                }

                // Check if we have a valid path.
                if (path == null || path.Count == 0)
                {
                    Destroy(gameObject);
                    yield break;
                }

                // Check if we have reached the end of the path.
                if (pathIndex >= path.Count - 1)
                {
                    // Reverse the path.
                    isWalkingForward = !isWalkingForward;
                    pathIndex = 0;
                    path.Reverse();
                    RotateTowardsNextNode();
                    yield return new WaitForSeconds(1f);
                    continue;
                }

                // Check if we are entering an intersection.
                if (nextNode.intersectionParent != null)
                {

                    // Check if the intersection is active. and the next node is a crosswalk.
                    if (nextNode.intersectionParent.intersectionActive && nextNode.type == Pathfinding.RoadNode.NodeType.CROSSWALK)
                    {
                        // Check if a request to cross has been made.
                        if (!nextNode.intersectionParent.requestToCross)
                        {
                            // Request to cross the intersection.
                            nextNode.intersectionParent.RequestToCross();
                        }

                        // Yield, intersectionActive will become false when the light changes and this code block will be skipped.
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    }
                }



                if (nextNode.pedestrianShouldWait && Vector3.Distance(transform.position, path[pathIndex].position) < 0.1f)
                {
                    // Wait for a frame.
                    yield return new WaitForSeconds(1f);
                    continue;
                }

                // Check the distance between the pedestrian and the next node.
                while (Vector3.Distance(transform.position, nextNode.position) >= 0.1f)
                {
                    // Move the pedestrian towards the next node.
                    transform.position = Vector3.MoveTowards(transform.position, nextNode.position, walkSpeed * Time.deltaTime);
                    yield return null;
                }

                // Move to the next node.
                pathIndex++;

                // Rotate towards the next node.
                RotateTowardsNextNode();

                // Wait for a frame.
                yield return null;
            }
        }
    }
}