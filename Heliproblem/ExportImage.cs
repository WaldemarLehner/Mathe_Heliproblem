using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;


namespace Heliproblem
{
    public class ExportImage
    {
        public int width, height;
        public string path;
        Point origin = new Point();
        Datapoint[] dataset;
        Point[] points;
        Bitmap bitmap;
        Graphics graphics;
        

        public ExportImage(Point[] _points,Datapoint[] _dataset,string _path)
        {
            points = _points;
            dataset = _dataset;
            path = _path;
            
            GetImgDimensions();
            Generate();
        }

        public ExportImage(Point[] _points, Datapoint[] _dataset)
        {
            points = _points;
            dataset = _dataset;
            path = AppDomain.CurrentDomain.BaseDirectory + "img/" + DateTime.UtcNow.ToString("dd-MM-yyyy \\@ hh-mm-ss") + ".png";

            GetImgDimensions();
            Generate();
           
        }

        void GetImgDimensions()
        {
            float minX, maxX, minY, maxY;
            minX = minY = float.MaxValue;
            maxX = maxY = float.MinValue;
            foreach(Datapoint x in dataset)
            {
                var p = x.ReturnPointF();
                if(p.X < minX) { minX = p.X; }
                if(p.X > maxX) { maxX = p.X; }
                if(p.Y < minY) { minY = p.Y; }
                if(p.Y > maxY) { maxY = p.Y; }
            }
            origin.X = minX - minX % 10;
            origin.Y = minY - minY % 10;
            width = (RoundUp(maxX) - (int)origin.X) * 10 + 20; //20 = margin // 1LE = 5px
            height = (RoundUp(maxY) - (int)origin.Y) * 10 + 20;

            // Offset margin
            origin.X += 10;
            origin.Y += 10;
        }

        int RoundUp(float val)
        {
            if(val % 10 == 0) { return (int)val; }
            int val_int = (int)val;
            return (10 - val_int % 10) + val_int;
        }

        void Generate()
        {
            bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            var dictionary = new Dictionary<Datapoint,Point>();
            foreach(Datapoint dp in dataset)
            {
                Point lowestScore = new Point() ;
                float score = float.MaxValue;
                foreach(Point p in points)
                {
                    if(GetDistPointDataset(p,dp) < score)
                    {
                        lowestScore = p;
                        score = GetDistPointDataset(p, dp);
                    }
            
                }
                dictionary.Add(dp, lowestScore);

            }
            using(graphics = Graphics.FromImage(bitmap))
            {
                //Fill w/ White
                graphics.Clear(Color.White);
                //Draw Connecting Lines
                foreach (KeyValuePair<Datapoint, Point> kvp in dictionary)
                {
                    DrawLinePointDataset(kvp.Value.ReturnPointF(), kvp.Key.ReturnPointF(), Color.LightGray, .5F);
                }
                //Draw Datapoints
                foreach (Datapoint dp in dataset)
                {

                    
                    DrawPointAndText(dp.ReturnPointF(), 10, 10, dp.Location, Color.Crimson, Color.Gray);
                }
                // Draw Var Points
                foreach(Point p in points)
                {
                    DrawPoint(p.ReturnPointF(), 10, Color.DarkBlue);
                }
            }
               
            //Save
            String pathWithOutFile = String.Empty;
            int index = path.LastIndexOf("/");
            if (index > 0)
            {
                pathWithOutFile = path.Substring(0, index);
            }

            if (!Directory.Exists(pathWithOutFile))
            {
                Directory.CreateDirectory(pathWithOutFile);
            }
            //bitmap = new Bitmap(width, height, graphics);
           // Bitmap savebitmap = new Bitmap(bitmap);
           // bitmap.Dispose();
            bitmap.Save(path);
            System.Diagnostics.Process.Start(path);
        }



        void DrawLinePointDataset(PointF _p,PointF _datapoint,Color c,float w)
        {
            using (var pen = new Pen(c, w))
            {
                graphics.DrawLine(pen, ConvertX(_p.X), ConvertY(_p.Y), ConvertX(_datapoint.X), ConvertY(_datapoint.Y));
            }
        }

        void DrawPointAndText(PointF point, int radius, int fontsize, string text,Color c,Color textcolor)
        {
            graphics.DrawString(text, new Font("Arial", fontsize), new SolidBrush(textcolor), new PointF(ConvertX(point.X) + 15,  ConvertY(point.Y) - 10));
            graphics.FillEllipse(new SolidBrush(c), ConvertX(point.X) - (radius / 2), ConvertY(point.Y) - (radius / 2), radius, radius);
            
        }
        void DrawPoint(PointF point, int radius,Color c)
        {
            graphics.FillEllipse(new SolidBrush(c), ConvertX(point.X) - (radius / 2), ConvertY(point.Y) - (radius / 2), radius, radius);
        }


        int ConvertX(float _x)
        {
            return (int)((_x * 10) + origin.X);
        }
        int ConvertY(float _y)
        {
            return (int)(bitmap.Height - ((_y * 10) + origin.Y));
        }

        float GetDistPointDataset(Point p,Datapoint dp)
        {
            var dX = dp.PosX - p.X;
            var dY = dp.PosY - p.Y;
            return (float)Math.Sqrt(dX * dX + dY * dY);
        }







    }
}
