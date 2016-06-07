using System.Collections.Generic;
using HiveCity.Server.Framework;
using HiveCity.Server.Region.Models;

namespace HiveCity.Server.World.Models
{
    public class RegionInfo : IServerData
    {
        public List<Position> Boundary { get; set; }

        // whether a position is inside a polygon
        public bool Contains(Position pos)
        {
            int windingNumber = 0;  // per segment to check if one side or the other
         
            // TODO: this maybe wrong
            // loop through all edges of polygon
            for (int i = 0; i < Boundary.Count - 1; i++)
            {
                // look at edge of boundery[i] to boundery[i+1]
                // whether inside or outside of the polygon
                if (Boundary[i].Z <= pos.Z)
                {
                    if (Boundary[i + 1].Z > pos.Z) // upward crossing
                    {
                        if (IsLeft(Boundary[i], Boundary[i + 1], pos) > 0) // inside of edge
                        {
                            windingNumber++;
                        }
                    }
                    else
                    {
                        if (Boundary[i + 1].Z <= pos.Z) // Downward crossing
                        {
                            if (IsLeft(Boundary[i], Boundary[i + 1], pos) < 0) // outside of edge
                            {
                                windingNumber--;
                            }
                        }
                    }
                }
            }

            return windingNumber != 0;
        }

        // take a position and see if it is to the left or the right of a line segment
        // defined by p1 and p2 
        public float IsLeft(Position p0, Position p1, Position p2)
        {
            // distance away from line segment of the point
            return ((p1.X - p0.X)*(p2.Z - p0.Z) - (p2.X - p0.X)*(p1.Z - p0.Z));
        }
    }
}
