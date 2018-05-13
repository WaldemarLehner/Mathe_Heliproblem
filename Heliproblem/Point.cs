using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heliproblem
{
    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Point(float _X,float _Y)
        {
            X = _X;
            Y = _Y;

        }
        public Point()
        {
            X = 0;
            Y = 0;
        }
    }
}
