using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace Map
{
    class Program
    {
        static void Main(string[] args)
        {
            double minLat = 0;
            double maxLat = 0;
            double minLon = 0;
            double maxLon = 0;
            int i = 0;
            string id, roadType = "";
            List<node> allNodes = new List<node>();
            List<Road> allRoads = new List<Road>();
            List<string> wayNodes = new List<string>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("map.osm");
            XmlNode bounds = xDoc.SelectSingleNode("//osm/bounds");
            minLat = Convert.ToDouble(bounds.Attributes["minlat"].Value);
            maxLat = Convert.ToDouble(bounds.Attributes["maxlat"].Value);
            minLon = Convert.ToDouble(bounds.Attributes["minlon"].Value);
            maxLon = Convert.ToDouble(bounds.Attributes["maxlon"].Value);
            XmlNodeList allNodeTags = xDoc.SelectNodes("//osm/node");
            XmlNodeList wayNodeTags = xDoc.SelectNodes("//osm/way");
            XmlNodeList child;
            foreach (XmlNode xNode in allNodeTags)
            {
                allNodes.Add(new node(xNode.Attributes["id"].Value, distance(minLat, minLon, minLat, Convert.ToDouble(xNode.Attributes["lon"].Value)), distance(minLat, minLon, Convert.ToDouble(xNode.Attributes["lat"].Value), minLon)));
            }
            foreach(XmlNode n in wayNodeTags)
            {
                child = n.SelectNodes(".//nd");
                foreach(XmlNode a in child)
                {
                    wayNodes.Add(a.Attributes["ref"].Value);
                }
                child = n.SelectNodes(".//tag");
                foreach(XmlNode b in child)
                {
                    if (b.Attributes["k"].Value=="highway"&&b.Attributes["v"].Value!="footway")
                    {
                        roadType = b.Attributes["v"].Value;
                        Console.WriteLine(roadType);
                        allRoads.Add(new Road(n.Attributes["id"].Value, allNodes, wayNodes, roadType));
                    }
                }
                wayNodes.Clear();
            }
            foreach(Road r in allRoads)
            {
                Console.WriteLine(r.id + " " + r.roadType);
            }

            
        }
        public static double distance(double lat1, double lon1, double lat2, double lon2)
        {
            double dist = 0;
            double rad = 6371000;
            double a = (Math.PI * lat1) / 180.0;
            double b = (Math.PI * lat2) / 180.0;
            double c = ((lat2 - lat1) * Math.PI) / 180.0;
            double d = ((lon2 - lon1) * Math.PI) / 180.0;
            double e = (Math.Sin(c / 2) * Math.Sin(c / 2)) + (Math.Cos(a) * Math.Cos(b) * Math.Sin(d / 2) * Math.Sin(d / 2));
            double f = 2 * Math.Atan2(Math.Sqrt(e), Math.Sqrt(1 - e));
            dist = rad * f;
            return dist;
        }
    }


        public class Car
        {
            private double speed, xPosInit, yPosInit, time, spacing, length, xPosFin, yPosFin;
            public Car(double xPosInit,double yPosInit,double spacing,double length,double speed)
            {
                this.xPosInit = xPosInit;
                this.yPosInit = yPosInit;
                this.spacing = spacing;
                this.length = length;
                this.speed = speed;
            }

        }
        public class Road
        {
            private List<link> allLinks = new List<link>();
            public List<point> allPoints = new List<point>();
            public string id, roadType;
            private struct link
            {
                public double slope, x1, x2, y1, y2;
                public link(double x1, double x2, double y1, double y2)
                {
                    this.x1 = x1;
                    this.x2 = x2;
                    this.y1 = y1;
                    this.y2 = y2;
                    this.slope = (y2 - y1) / (x2 - x1);
                }
            }
            public struct point
            {
                public double x, y;
                public point(double x, double y)
                {
                    this.x = x;
                    this.y = y;
                }
            }
            public Road()
            {

            }
            public Road(string id, List<node> allNodes, List<string> wayNodes, string roadType)
            {
                this.id = id;
                this.roadType = roadType;
                for(int i=0;i<wayNodes.Count;i++)
                {
                    for(int j=0; j<allNodes.Count; j++)
                    {
                        if (allNodes[j].id==wayNodes[i])
                        {
                            allPoints.Add(new point(allNodes[j].x, allNodes[j].y));
                        }
                    }
                }
                for(int k=0;k<allPoints.Count-1;k++)
                {
                    allLinks.Add(new link(allPoints[k].x, allPoints[k].y, allPoints[k + 1].x, allPoints[k + 1].y));
                }
            }
            public bool IsValidPosition(double xpos, double ypos)
            {
                foreach (link l in allLinks)
                {
                    if ((l.slope) * (xpos - l.x1) + l.y1 == ypos && l.x1 - ((ypos - l.y1) / (l.slope)) == xpos)
                        return true;
                    else
                        return false;
                }
                return false;
            }
    }
        
        public struct node
        {
            public string id;
            public double x;
            public double y;
            public node(string id, double x, double y)
            {
                this.id = id;
                this.x = x;
                this.y = y;
            }
        }
    }

