using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heliproblem
{
    public class Config
    {
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }
        public int precision { get; set; }
        public Config() => MinX = MaxY = MinY = MaxX = precision = -1;
    }
}
