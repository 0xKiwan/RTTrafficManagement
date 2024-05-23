using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim.ProceduralEngine
{
    public class PrefabInstancer : MonoBehaviour
    {
        public GameObject prefab;
        public int instanceCount = 0;

        private Mesh mesh;
        private Material[] materials;
        private List<Vector3> positions;
        private List<Quaternion> rotations;
        private List<Vector3> scales;
        private MaterialPropertyBlock[] propertyBlocks;
        private List<int> indicesWithinRange;
        private Matrix4x4[] matrices;

        /**
         * Used to setup a given prefab for gpu instancing.
         */
        public void SetupPrefab(GameObject prefab)
        {
            // Store the prefab and instance count
            this.prefab = prefab;

            // Extract mesh and materials from the prefab
            GameObject instance = Instantiate(prefab);
            instance.SetActive(false);

            mesh = instance.GetComponentInChildren<MeshFilter>()?.sharedMesh;
            materials = instance.GetComponentInChildren<Renderer>()?.sharedMaterials;

            Destroy(instance);

            if (mesh == null || materials == null) return;

            // Initialize positions, rotations, scales, and property blocks
            positions = new List<Vector3>();
            rotations = new List<Quaternion>();
            scales = new List<Vector3>();
            propertyBlocks = new MaterialPropertyBlock[materials.Length];
            indicesWithinRange = new List<int>();

            for (int i = 0; i < materials.Length; i++)
            {
                propertyBlocks[i] = new MaterialPropertyBlock();
            }
        }

        /**
         * Used to add a prefab at a specific position, rotation, and scale.
         */
        public void AddInstance(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            // Increase the instance count
            instanceCount++;

            // Add the new instance to the positions, rotations, and scales lists
            positions.Add(position);
            rotations.Add(rotation);
            scales.Add(scale);
        }

        /**
         * Used to finalize the matrices list.
         */
        public void FinalizeMatrices()
        {
            // Prepare matrices for all instances
            matrices = new Matrix4x4[positions.Count];

            // Update the matrices array
            for (int i = 0; i < positions.Count; i++)
            {
                matrices[i] = Matrix4x4.TRS(positions[i], rotations[i], scales[i]);
            }
        }

        void Update()
        {
            if (mesh == null || materials == null || Camera.main == null)
                return;

            // Cache camera position and far clip distance
            Vector3 cameraPosition = Camera.main.transform.position;
            float squaredFarClipDistance = Camera.main.farClipPlane * Camera.main.farClipPlane;

            // Clear previous list of indices within range
            indicesWithinRange.Clear();

            // Iterate over all instances
            for (int i = 0; i < positions.Count; i++)
            {
                // Calculate the squared distance between the instance and the camera
                Vector3 offset = positions[i] - cameraPosition;
                float squaredDistanceToCamera = Vector3.Dot(offset, offset);

                // Check if the instance is within the squared far clipping plane range
                if (squaredDistanceToCamera <= squaredFarClipDistance)
                {
                    // Add the index of the instance to the list of indices within range
                    indicesWithinRange.Add(i);
                }
            }
        }

        void LateUpdate()
        {
            // Render instances within range
            if (mesh != null && materials != null && indicesWithinRange.Count > 0)
            {
                // Prepare matrices for instances within range
                Matrix4x4[] matrices = new Matrix4x4[indicesWithinRange.Count];

                // Update the matrices array for instances within range
                for (int i = 0; i < indicesWithinRange.Count; i++)
                {
                    int index = indicesWithinRange[i];
                    matrices[i] = Matrix4x4.TRS(positions[index], rotations[index], scales[index]);
                }

                // Draw instances within range in a single call
                for (int i = 0; i < materials.Length; i++)
                {
                    Graphics.DrawMeshInstanced(mesh, i, materials[i], matrices, indicesWithinRange.Count, propertyBlocks[i]);
                }
            }
        }
    }
}