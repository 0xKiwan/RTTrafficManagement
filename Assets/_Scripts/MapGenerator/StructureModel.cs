using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace TrafficSim.ProceduralEngine
{
    /**
     * Wrapper class for the GameObject of each cell on the map.
     * Can contain terrain, buildings, etc.
     */
    public class StructureModel : MonoBehaviour {
        
        float yHeight;                  // The y height of the structure.
        public GameObject structure;    // The structure GameObject.
        public Mesh mesh;               // The mesh of the structure, for mesh combining.

        /**
         * Used to create a new structure model.
         */
        public void CreateModel(GameObject model)
        {
            // Instantiate the model as a child of the cell.
            structure = Instantiate(model, transform);

            // Store the yHeight of the structure.
            yHeight = model.transform.position.y;
        }

        /**
         * Used to swap the model of the structure.
         */
        public void SwapModel(GameObject model)
        {
            // Loop through all children of the structure.
            foreach (Transform child in transform)
            {
                // Destroy the child.
                Destroy(child.gameObject);
            }

            // Instantiate the model as a child of the cell.
            var structure = Instantiate(model, transform);

            // Update the yHeight of the structure.
            structure.transform.localPosition = new Vector3(0, yHeight, 0);
        }

        /**
         * Used to destroy the structure.
         */
        public void DestroyStructure()
        {
            // Destroy the structure.
            Destroy(gameObject);
        }

        /**
         * Used to set the rotation of the structure.
         */
        public void SetRotation(Quaternion rotation)
        {
            // Set the rotation of the structure.
            transform.rotation = rotation;
        }

        /**
         * Used to set the color of the structure. Updates all materials & children.
         */
        public void SetColor(Color color)
        {
            // Loop through all children of the structure.
            foreach (Transform child in transform)
            {
                // Get the renderer of the child.
                Renderer renderer = child.GetComponent<Renderer>();

                // Loop through all materials of the renderer.
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    // Set the color of the material.
                    renderer.materials[i].color = color;
                }
            }
        }

    }
}