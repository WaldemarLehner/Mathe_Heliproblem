using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Heliproblem
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point(double _X,double _Y)
        {
            X = _X;
            Y = _Y;

        }
        public Point()
        {
            X = 0;
            Y = 0;
        }
        public PointF ReturnPointF()
        {
            PointF retobj = new PointF
            {
                X = (float)X,
                Y = (float)Y
            };
            return retobj;
        }
    }
}
