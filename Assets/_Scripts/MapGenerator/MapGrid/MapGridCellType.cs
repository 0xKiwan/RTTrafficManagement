

using UnityEngine;

namespace TrafficSim.ProceduralEngine
{
    /**
     * Enum that represents the type of a cell in the map.
     */
    public enum MapGridCellType
    {
        Grass,                      // 0
        RoadStraight,               // 1
        RoadStraightCrossing,       // 2
        RoadCorner,                 // 3
        RoadDeadEnd,                // 4
        IntersectionFourWay,        // 5
        IntersectionThreeWay,       // 6
        IntersectionRoundabout      // 7
    };
}