using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heliproblem
{
    public class Datapoint
    {
        public string Location { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public int Weight { get; set; }
        public Datapoint(string _loc,float _x,float _y, int _weight)
        {
            Location = _loc;
            PosX = _x;
            PosY = _y;
            Weight = _weight;
        }


    }
}
