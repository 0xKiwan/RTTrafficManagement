using System.Collections;
using System.Collections.Generic;
using TrafficSim.ProceduralEngine;
using UnityEngine;

public class Road
{
    public List<MapGridCell> roadCells;     // A list of cells that make up the road.
    public string roadName;                 // The name of the road.
    public bool isVertical;                 // Whether the road is vertical or horizontal.
    public MapGridCell startCell;           // The starting position of the road.

    /**
     * Constructor for the Road object.
     */
    public Road(string roadName)
    {
        // Initialize the roadCells list.
        roadCells = new List<MapGridCell>();

        // Store the road name.
        this.roadName = roadName;
    }

    /**
     *     * Used to add a cell to the road.
     */
    public void AddCell(MapGridCell cell)
    {
        roadCells.Add(cell);
    }

    /**
     * Used to remove a cell from the road.
     */
    public void RemoveCell(MapGridCell cell)
    {
        roadCells.Remove(cell);
    }

    /**
     * Used to get the cells that make up the road.
     */
    public List<MapGridCell> GetCells()
    {
        return roadCells;
    }

    /**
     * Used to get the road name.
     */
    public string GetName()
    {
        return this.roadName;
    }

    /**
     * Used to set the road name.
     */
    public void SetName(string name)
    {
        this.roadName = name;
    }
}
