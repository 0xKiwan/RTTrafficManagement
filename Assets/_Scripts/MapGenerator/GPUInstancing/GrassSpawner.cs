using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TrafficSim;

namespace TrafficSim.ProceduralEngine
{
    public class GrassSpawner : MonoBehaviour
    {
        public int instances = 1000;                // Number of instances per batch.
        public Mesh objMesh;                        // Mesh for grass instances.
        public Material objMaterial;                // Material for grass instances.
        private List<Matrix4x4[]> batches = new List<Matrix4x4[]>(); // Batches of object matrices.

        // Reference to the game manager.
        private GameManager gameManager;

        // Generate grass objects.
        public void GenerateGrassObjects()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            List<MapGridCell> grassCells = gameManager.mapGenerator.mapGrid.GetAllCellsByType(MapGridCellType.Grass);

            List<Matrix4x4> currentBatch = new List<Matrix4x4>();
            int batchIndex = 0;

            foreach (MapGridCell cell in grassCells)
            {
                // Get the cellpos as vector3 int.
                Vector3 cellPos = cell.GetPosition();

                // Increment y by +1
                cellPos.y += 0.5f;

                // Add the matrix to the current batch.
                currentBatch.Add(Matrix4x4.TRS(cellPos, Quaternion.identity, Vector3.one));

                if (currentBatch.Count >= instances)
                {
                    batches.Add(currentBatch.ToArray());
                    currentBatch.Clear();
                    batchIndex++;
                }
            }

            // Add the last batch if it's not empty.
            if (currentBatch.Count > 0)
                batches.Add(currentBatch.ToArray());
        }

        // Update is called once per frame.
        void Update()
        {
            RenderBatches();
        }

        // Render grass batches using GPU instancing.
        private void RenderBatches()
        {
            foreach (Matrix4x4[] batch in batches)
            {
                Graphics.DrawMeshInstanced(objMesh, 0, objMaterial, batch);
            }
        }
    }
}