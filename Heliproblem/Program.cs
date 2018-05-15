using System;
using System.IO;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Heliproblem
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Datapoint> datapoints = new List<Datapoint>();
            
            var points = new Point[] { new Point(), new Point(), new Point() };
            var cfg = new Config();
            
            if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data.cfg"))
            {
                StreamReader stream = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "data.cfg");
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    string[] line_s = line.Split(';');
                    //Console.WriteLine(s[0] + "\n" + s[1] + "\n" + s[2] + "\n-----------");
                    datapoints.Add(new Datapoint(line_s[0], float.Parse(line_s[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(line_s[2], CultureInfo.InvariantCulture.NumberFormat),Int32.Parse(line_s[3])));
                    
                }
                if (isConfigPresent())
                {
                    StreamReader cfgReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "config.cfg");
                    string cfg_line;
                    while((cfg_line=cfgReader.ReadLine()) != null)
                    {
                        string[] line_p = cfg_line.Split(':');
                        //Console.WriteLine(cfg_line);
                        switch (line_p[0])
                        {
                            case "precision":
                                cfg.precision = Int32.Parse(line_p[1]);
                                break;
                            case "yMin":
                                cfg.MinY = Int32.Parse(line_p[1]);
                                break;
                            case "yMax":
                                cfg.MaxY = Int32.Parse(line_p[1]);
                                break;
                            case "xMin":
                                cfg.MinX = Int32.Parse(line_p[1]);
                                break;
                            case "xMax":
                                cfg.MaxX = Int32.Parse(line_p[1]);
                                break;
                        }
                    }
                    //if not set in cfg, declare min and max bounds
                    if(cfg.MaxX == -1 ||cfg.MinX == -1 || cfg.MinY == -1 ||cfg.MaxY == -1)
                    {
                        bool lfMaxX, lfMinX, lfMaxY, lfMinY;
                        lfMaxX = lfMinX = lfMaxY = lfMinY = false;
                        if(cfg.MaxX == -1) { lfMaxX = true; }
                        if(cfg.MinX == -1) { lfMinX = true; cfg.MinX = float.MaxValue; }
                        if(cfg.MaxY == -1) { lfMaxY = true; }
                        if(cfg.MinY == -1) { lfMinY = true; cfg.MinY = float.MaxValue; }
                        foreach(Datapoint datapoint in datapoints)
                        {
                            if (lfMaxX && datapoint.PosX > cfg.MaxX) { cfg.MaxX = datapoint.PosX; }
                            if (lfMaxY && datapoint.PosY > cfg.MaxY) { cfg.MaxY = datapoint.PosY; }
                            if (lfMinX && datapoint.PosX < cfg.MinX) { cfg.MinX = datapoint.PosX; }
                            if (lfMinY && datapoint.PosX < cfg.MinY) { cfg.MinY = datapoint.PosY; }

                        }
                    }

                    String stmt = String.Empty;
                    foreach (Datapoint datapoint in datapoints)
                    {
                        stmt += datapoint.PosX + "\n" + datapoint.PosY + "\n" + datapoint.Location + "\n----------------\n";
                    }
                    Console.WriteLine(stmt);
                    Console.WriteLine(datapoints.Count + " Einträge.");
                    Console.WriteLine("Beliebige Taste zum Fortfahren drücken.");
                    Console.ReadKey();
                    Console.Clear();
                    runcalc();


                    
                    Console.ReadKey();
                }
               
            }
            else
            {
                Console.WriteLine("Fehler: Die Datei \"" + AppDomain.CurrentDomain.BaseDirectory + "data.cfg\" ist nicht vorhanden.");
                isConfigPresent();
                Console.ReadKey();
            }


            bool isConfigPresent()
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "config.cfg"))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Fehler: Die Datei \"" + AppDomain.CurrentDomain.BaseDirectory + "config.cfg\" ist nicht vorhanden.");
                    return false;
                }
            }
            void runcalc()
            {
                double record = double.MaxValue;
                var hundreth =  (long)(Math.Pow(cfg.precision,points.Length*2)/100)-1;
                Point[] record_obj = new Point[3];
                int count = 0;
                int percentdone = 0;
                long cumcount = 0;
                long endcount = (long)Math.Pow(cfg.precision, points.Length * 2);
                for(points[0].X = 0; points[0].X < cfg.precision; points[0].X++)
                {
                    for(points[0].Y = 0; points[0].Y < cfg.precision; points[0].Y++)
                    {
                        for(points[1].X = 0; points[1].X < cfg.precision; points[1].X++)
                        {
                            for(points[1].Y = 0; points[1].Y < cfg.precision; points[1].Y++)
                            {
                                for(points[2].X = 0; points[2].X < cfg.precision; points[2].X++)
                                {
                                    for(points[2].Y = 0; points[2].Y < cfg.precision; points[2].Y++)
                                    {
                                        
                                        if(count > hundreth)
                                        {
                                            cumcount += count;
                                                count = 0;
                                            percentdone++;
                                            Console.Clear();
                                            Console.WriteLine(percentdone + "%");
                                        }
                                        double retval = getCalculation();
                                        if(retval < record)
                                        {
                                            record = retval;
                                            record_obj = convertPointArray(points);
                                        }
                                        count++;

                                    }
                                }
                            }
                        }
                    }
                }
                Console.Clear();
                Console.WriteLine("Berechnung fertig...\n\n\n");
                Console.WriteLine("Bester Punktwert (geringer = besser) :\n" + record);
                Console.WriteLine("Die Positionoptima bei einer Ungenauigkeit von \n +/- " + Math.Round((cfg.MaxX - cfg.MinX) / cfg.precision / 2, 2) + " LE für X bzw. \n +/- " + Math.Round((cfg.MaxY - cfg.MinY) / cfg.precision / 2 , 2) + " LE für Y.");
                int i = 0;
                foreach(Point p in record_obj)
                {
                    i++;
                    Console.WriteLine("P" + i + ".X = " + p.X +"\nP"+i+".Y = " + p.Y +"\n\n");

                }

                var exportimg = new ExportImage(record_obj, datapoints.ToArray());
                Console.ReadKey();

            }
            double getCalculation()
            {
                double score = 0;
                foreach(Datapoint datapoint in datapoints)
                {
                    // Get lowest Val from each of variable points. Add that Val to Score
                    double local_score = double.MaxValue;
                    foreach(Point p in points)
                    {
                       double local_score2 =  getDistP(p, datapoint.PosX, datapoint.PosY);
                        if(local_score2 < local_score)
                        {
                            local_score = local_score2;
                        }
                    }
                    score += local_score * datapoint.Weight; 
                }
                return score;
            }
            double getDistP(Point p1,float x2,float y2)
            {
                // var deltaX = (cfg.MinX + (p1.X / cfg.precision) * (cfg.MaxX - cfg.MinX)) - (cfg.MinX + (x2 / cfg.precision) * (cfg.MaxX - cfg.MinX));
                // var deltaY = (cfg.MinY + (p1.Y / cfg.precision) * (cfg.MaxY - cfg.MinY)) - (cfg.MinY + (y2 / cfg.precision) * (cfg.MaxY - cfg.MinY));
                var deltaX = (cfg.MinX + (p1.X / cfg.precision) * (cfg.MaxX - cfg.MinX)) - x2;
                var deltaY = (cfg.MinY + (p1.Y / cfg.precision) * (cfg.MaxY - cfg.MinY)) - y2;

                return Math.Sqrt(deltaX*deltaX+deltaY*deltaY);
            }
            double getDist(float x1,float y1,float x2,float y2)
            {
                //var deltaX = (cfg.MinX + (x1 / cfg.precision) * (cfg.MaxX - cfg.MinX)) - (cfg.MinX + (x2 / cfg.precision) * (cfg.MaxX - cfg.MinX));
                //var deltaY = (cfg.MinY + (y1 / cfg.precision) * (cfg.MaxY - cfg.MinY)) - (cfg.MinY + (y2 / cfg.precision) * (cfg.MaxY - cfg.MinY));
                var deltaX = (cfg.MinX + (x1 / cfg.precision) * (cfg.MaxX - cfg.MinX)) - x2;
                var deltaY = (cfg.MinY + (y1 / cfg.precision) * (cfg.MaxY - cfg.MinY)) - y2;

                return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);//
            }
            Point convertPoint(Point p)
            {
                var x = (cfg.MinX + (p.X / cfg.precision) * (cfg.MaxX - cfg.MinX));
                var y = (cfg.MinY + (p.Y / cfg.precision) * (cfg.MaxY - cfg.MinY));
                return new Point(x, y);
            }
            Point[] convertPointArray(Point[] p_arr)
            {
                var count = 0;
                Point[] returnpoints = new Point[p_arr.Length];
                foreach(Point p in p_arr)
                {
                    var x = (cfg.MinX + (p.X / cfg.precision) * (cfg.MaxX - cfg.MinX));
                    var y = (cfg.MinY + (p.Y / cfg.precision) * (cfg.MaxY - cfg.MinY));
                    returnpoints[count] = new Point(x, y);
                    count++;
                }
                return returnpoints;
            }
            

            

        }
     
    }  
}
