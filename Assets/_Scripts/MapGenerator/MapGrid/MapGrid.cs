using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;


namespace TrafficSim.ProceduralEngine
{
    /**
     * Class that represents the map.
     * It is a grid of cells, each cell containing information about
     * cell type, cell position, cell rotation, and a reference to the 
     * game object that is in that cell.
     */
    public class MapGrid : MonoBehaviour
    {
        //List<MapGridCell> cells;    // The list of cells in the map.
        public Dictionary<Vector3Int, MapGridCell> cells;
        private int width;          // The width of the map.
        private int height;         // The height of the map.
        List<MapGridCell> roadCells; // Cache for GetAllRoadCells
        List<MapGridCell> roundaboutCells; // Cache for GetAllRoundaboutCells

        /**
         * Used to initialize the map grid.
         */
        public void Initialize(int width, int height)
        {
            // Initialize the list of cells.
            cells = new Dictionary<Vector3Int, MapGridCell>();

            // Loop through the map grid width.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Create a new cell.
                    MapGridCell cell = new MapGridCell(MapGridCellType.RoadStraight, new Vector3Int(x, 0, y));

                    // Initialize the cell structureModel.
                    // cell.InitializeStructure();

                    // Add the cell to the list of cells.
                    cells[cell.GetPosition()] = cell;
                }
            }

            // Store the width and height of the map.
            this.width = width;
            this.height = height;
        }

        /**
         * Used to get a cell from the map grid given its position.
         */
        public MapGridCell GetCellByPosition(Vector3Int position)
        {
            return cells[position];
        }

        /**
         * Used to get a random cell from the map grid.
         */
        public MapGridCell GetRandomCell()
        {
            // Get a random cell from the list of cells.
            return cells.Values.ElementAt(UnityEngine.Random.Range(0, cells.Count));
        }

        /**
         * Used to add a cell to the map grid.
         */
        public void AddCell(MapGridCell cell) { cells[cell.GetPosition()] = cell; }

        /**
         * Used to get the width of the map.
         */
        public int GetWidth() { return width; }

        /**
         * Used to get the height of the map.
         */
        public int GetHeight() { return height; }

        /**
         * Used to get all cells of a given type.
         */
        public List<MapGridCell> GetAllCellsByType(MapGridCellType type)
        {
            // Find all cells given the type & return them.
            return cells.Values.Where(cell => cell.GetCellType() == type).ToList();
        }

        /**
         * Used to get all road cells.
         */
        public List<MapGridCell> GetAllRoadCells()
        {
            // Check if we have already cached the road cells.
            if (roadCells != null) { return roadCells; }

            // Find all road cells.
            roadCells = cells.Values.Where(cell => cell.IsRoadCell()).ToList();

            // Return the list of road cells.
            return roadCells;
        }

        /**
         * Used to get neighboring cells of a given cell.
         * 0 = left, 1 = top, 2 = right, 3 = bottom
         */
        public MapGridCell[] GetNeighboringCells(MapGridCell cell)
        {
            // Will store the list of neighboring cells.
            MapGridCell[] neighboringCells =
            {
                cell.leftNeighbor,
                cell.topNeighbor,
                cell.rightNeighbor,
                cell.bottomNeighbor
            };

            // Return the list of neighboring cells.
            return neighboringCells;
        }

        /**
         * Used to initialize a cell at a given position.
         */
        public void InitializeCell(int x, int y)
        {
            // Initialize the cell structureModel.
            cells[new Vector3Int(x, 0, y)].InitializeStructure();
        }

        /**
         * Used to initialize all cells in the grid when filled.
         */
        public void InitializeAllCells()
        {
            // Loop through the map grid width.
            for (int x = 0; x < width; x++)
            {
                // Loop through the map grid height.
                for (int y= 0; y < height; y++)
                {
                    // Initialize the cell at this position.
                    InitializeCell(x, y);
                }
            }
        }

        /**
         * Used to set the type of a cell & update it's prefab.
         */
        public void SetCellType(int x, int y, MapGridCellType type)
        {
            // Set the cell type.
            cells[new Vector3Int(x, 0, y)].SetCellType(type);
        }

        /**
         * Used to get a list of cells surrounding a given cell in a square.
         * The size of the square is determined by the given parameter.
         */
        internal List<MapGridCell> GetSurroundingCells(MapGridCell cell, int v)
        {
            // Will store the list of surrounding cells.
            List<MapGridCell> surroundingCells = new List<MapGridCell>();

            // Get the position of the given cell.
            Vector3Int cellPosition = cell.GetPosition();

            // Loop through the surrounding cells.
            for (int x = cellPosition.x - v; x <= cellPosition.x + v; x++)
            {
                for (int y = cellPosition.z - v; y <= cellPosition.z + v; y++)
                {
                    // Check if the cell position is within the map bounds.
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        // Add the cell to the list of surrounding cells.
                        surroundingCells.Add(cells[new Vector3Int(x, 0, y)]);
                    }
                }
            }

            // Return the list of surrounding cells.
            return surroundingCells;
        }
    }
}
