using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Windows;
using System.Windows.Automation.Peers;
using Newtonsoft.Json;

namespace tt2
{
    public class Network
    {
        public string name { get; set; }

        public int id { get; set; }

        public List<DateTime> datesOfUpdates { get; set; }

        public List<Vertex> vertices { get; set; }

        public List<Link> links { get; set; }

        public int serviceType { get; set; }

        public NetworkFactors factors { get; set; }

        public void CalculateFactors()
        {
            foreach (Vertex v in vertices)
            {
                int index = v.id;
                v.indegreeCentralityValue = CentralityIn(index);
                v.betweennessCentralityValue = BetweennessCentrality(index);
                v.influenceRangeValue = InfluenceRange(index);
                v.outdegreeCentralityValue = CentralityOut(index);
                v.closenessCentralityValue = ClosenessCentrality(index);
            }
        }

        public int CountAllPaths()
        {
            int allPaths = 0;
            int k = vertices.Count();
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    int paths = CountPaths(vertices[i].id, vertices[j].id);
                    allPaths += paths;
                }
            }
            return allPaths;
        }

        public int allPaths { get; set; }

        public bool DoesShortestPathContain(int source, int destination, int target)
        {
            bool result = false;

            return result;
        }

        public void CreateVertex2(int v, List<int> Edg)
        {
            Vertex newVert = new Vertex();
            newVert.id = v;
            newVert.edges = Edg;
            newVert.distance = Int32.MaxValue;
            int index = vertices.FindIndex(vert => vert.id == v);
            if (index >= 0)
                vertices.RemoveAt(index);
            vertices.Add(newVert);

        }

        public float BetweennessCentrality(int index)
        {
            float result = 0;
            int pathsV = 0;
            int k = vertices.Count();
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    int source = vertices[i].id;
                    int destination = vertices[j].id;
                    if (source != index && destination != index)
                    {
                        int paths = CountShortestPathsContaining(source, destination, index);
                        pathsV += paths;
                    }

                }
            }
            float pathsv = (float)pathsV;
            if (allPaths == default(int))
                allPaths = CountAllPaths();
            result = pathsv/* / (float)allPaths*/;
            return result;
        }

        public void BetweennessCentrality2()
        {
            foreach(Vertex v in vertices)
            {
                v.betweennessCentralityValue = 0;
            }
            foreach(Vertex s in vertices)
            {
                Stack<Vertex> S = new Stack<Vertex>();
                var len = vertices.Count;
                Dictionary<int, List<Vertex>> P = new Dictionary<int, List<Vertex>>();
                foreach (Vertex v in vertices)
                {
                    List<Vertex> p = new List<Vertex>();
                    P.Add(v.id, p);
                }
                Dictionary<int, float> sigma = new Dictionary<int, float>();
                Dictionary<int, float> d = new Dictionary<int, float>();
                foreach (Vertex t in vertices)
                {
                    if (t == s)
                    {
                        sigma.Add(t.id, 1);
                        d.Add(t.id, 0);
                    }
                    else
                    {
                        sigma.Add(t.id, 0);
                        d.Add(t.id, -1);
                    }
                }
                Queue<Vertex> Q = new Queue<Vertex>();
                Q.Enqueue(s);
                while (Q.Count != 0)
                {
                    var v = Q.Dequeue();
                    S.Push(v);
                    foreach (int w in v.edges)
                    {
                        if (d[w] < 0)
                        {
                            var ww = vertices.Find(vert => vert.id == w);
                            Q.Enqueue(ww);
                            d[w] = d[v.id] + 1;
                        }
                        if (d[w] == d[v.id] + 1)
                        {
                            sigma[w] += sigma[v.id];
                            P[w].Add(v);
                        }
                    }
                }
                Dictionary<int, float> delta = new Dictionary<int, float>();
                foreach(Vertex v in vertices)
                    delta.Add(v.id, 0);
                while (S.Count != 0)
                {
                    var w = S.Pop();
                    foreach (Vertex v in P[w.id])
                    {
                        delta[v.id] += (sigma[v.id]/sigma[w.id])*(1 + delta[w.id]);
                        if (w != s)
                            w.betweennessCentralityValue += delta[w.id];
                    }

                }
            }


        }
        public float ClosenessCentrality(int index)
        {
            float result = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                float a = CountDistance(index, vertices[i].id);
                if (a != 0 && a != Int32.MaxValue)
                    result += (1 / a);
            }
            result /= 10;
            return result;
        }

        public int InfluenceRange(int index)
        {
            int result = 0;
            foreach (Vertex v in vertices)
            {
                int a;
                if (index == v.id)
                    a = 0;
                else a = CountDistance(index, v.id);
                if (a != 0 && a != Int32.MaxValue)
                    result++;
            }
            return result;
        }

        public int CountDistance(int index1, int index2)
        {
            int result = Int32.MaxValue;
            foreach (Vertex v in vertices)
            {
                v.distance = Int32.MaxValue;
                v.parent = null;
            }
            Vertex vert1 = vertices.Find(vert => vert.id == index1);
            Vertex vert2 = vertices.Find(vert => vert.id == index2);
            vert1.distance = 0;
            Queue<Vertex> q = new Queue<Vertex>();
            q.Enqueue(vert1);
            while (q.Count != 0)
            {
                Vertex current = q.Dequeue();
                foreach (int neighbIndex in current.edges)
                {
                    Vertex neighb = vertices.Find(vert => vert.id == neighbIndex);
                    if (neighb.distance == Int32.MaxValue)
                    {
                        neighb.distance = current.distance + 1;
                        neighb.parent = current;
                        q.Enqueue(neighb);
                    }
                }
            }
            result = vert2.distance;

            return result;
        }

        //public void DFS(int source, int destination, int target) // do znajdywania ścieżek z danym wierzchołkiem
        //{
        //    Vertex vertex = vertices.Find(vert => vert.index == source);
        //    vertex.visited = true;
        //    List<int> edges2 = vertex.Edges;
        //    if (edges2.Contains(target))
        //    {

        //    }

        //    foreach (int i in vertex.Edges)
        //    {
        //        Vertex neighb = vertices.Find(vert => vert.index == i);
        //        if (neighb.visited == false)
        //        {
        //            DFS(neighb.index, target);
        //        }
        //    }
        //}

        public int CountShortestPathsContaining(int sourc, int dest, int cont)
        {
            int result = 0;
            int a = CountDistance(sourc, cont);
            int b = CountDistance(cont, dest);
            int c = CountDistance(sourc, dest);
            if (a + b == c)
            {
                result = CountPaths(sourc, cont) * CountPaths(cont, dest);
            }

            return result;
        }

        public int CountPaths(int index1, int index2)
        {

            foreach (Vertex v in vertices)
            {
                v.distance = Int32.MaxValue;
                v.parent = null;
                v.numberOfPaths = 0;
            }
            if (index1 == index2)
                return 0;
            else
            {
                Vertex vert1 = vertices.Find(vert => vert.id == index1);
                Vertex vert2 = vertices.Find(vert => vert.id == index2);
                vert1.distance = 0;
                vert1.numberOfPaths = 1;
                Queue<Vertex> q = new Queue<Vertex>();
                Dictionary<Vertex, int> path_counts = new Dictionary<Vertex, int>();
                q.Enqueue(vert1);
                while (q.Count != 0)
                {
                    Vertex current = q.Dequeue();
                    foreach (int neighbIndex in current.edges)
                    {
                        Vertex neighb = vertices.Find(vert => vert.id == neighbIndex);
                        if (neighb.distance == Int32.MaxValue)
                        {
                            neighb.distance = current.distance + 1;
                            neighb.numberOfPaths = current.numberOfPaths;
                            neighb.parent = current;
                            q.Enqueue(neighb);
                        }
                        else
                        {
                            if (neighb.distance == current.distance + 1)
                            {
                                neighb.numberOfPaths += current.numberOfPaths;
                            }
                            if (neighb.distance > current.distance + 1)
                            {
                                neighb.distance = current.distance + 1;
                                neighb.numberOfPaths = current.numberOfPaths + 1;
                            }
                        }
                    }

                }
                return vert2.numberOfPaths;
            }
        }

        public float AverageFactor(int whichCase)
        {
            float result = 0;
            float NumberOfVertices = vertices.Count();
            foreach (Vertex v in vertices)
            {
                switch (whichCase)
                {
                    case 1:
                        result += v.indegreeCentralityValue;
                        break;
                    case 2:
                        result += v.outdegreeCentralityValue;
                        break;
                    case 3:
                        result += v.closenessCentralityValue;
                        break;
                    case 4:
                        result += v.betweennessCentralityValue;
                        break;
                    case 5:
                        result += v.influenceRangeValue;
                        break;
                }

            }
            result /= NumberOfVertices;
            return result;
        }

        public List<int> MaxCentrality(int whichCase) //przerobić jakoś żeby rozszerzyć na inne wskaźniki
        {
            int resultCentrality = 0;
            int vertexIndex = 0;
            List<int> result = new List<int>();
            bool flag = false;

            foreach (Vertex v in vertices)
            {
                if (v.indegreeCentralityValue > resultCentrality)
                {
                    flag = true;
                    resultCentrality = v.indegreeCentralityValue;
                    vertexIndex = v.id;
                }

            }
            if (flag)
            {
                result.Add(vertexIndex);
                result.Add(resultCentrality);
            }
            foreach (Vertex v in vertices)
            {
                if (v.indegreeCentralityValue == resultCentrality && v.id != vertexIndex)
                {
                    result.Add(v.id);
                    result.Add(v.indegreeCentralityValue);
                }
            }
            return result;
        }

        public List<string> ShowVertices()
        {
            List<string> list = new List<string>();
            foreach (Vertex v in vertices)
            {
                list.Add(v.id.ToString());
            }
            return list;
        }

        public int CentralityIn(int index)
        {
            int result = 0;
            foreach (Vertex v in vertices)
            {
                if (v.edges.Contains(index))
                {
                    result += 1;
                }
            }
            return result;
        }

        public int CentralityOut(int index)
        {
            int result = 0;
            Vertex vert1 = vertices.Find(vert => vert.id == index);
            result = vert1.edges.Count;
            return result;
        }

        public double Density()
        {
            double result = 0;
            foreach (Vertex ver in vertices)
            {
                result += ver.edges.Count;
            }
            double n = vertices.Count;
            result /= (n * (n - 1));
            return result;
        }

        public string WriteToJson()
        {
            List<Link> links = new List<Link>();
            foreach (Vertex v in vertices)
            {
                foreach (int e in v.edges)
                {
                    Link link = new Link();
                    link.source = v.id;
                    link.target = e;
                    links.Add(link);
                }

            }
            NetworkFactors factors = new NetworkFactors();
            factors.density = Density();
            factors.avInCen = AverageFactor(1);
            factors.avOutCen = AverageFactor(2);
            factors.avCloCen = AverageFactor(3);
            factors.avBetCen = AverageFactor(4);
            factors.avInfRan = AverageFactor(5);
            var json = JsonConvert.SerializeObject(new
            {
                vertices = vertices,
                links = links,
                factors = factors
            });
            return json;

        }
    }

    public class Vertex
    {
        public int id { get; set; }

        public string identifier { get; set; }

        public List<int> edges;

        [JsonIgnore]
        public int distance { get; set; }
        [JsonIgnore]
        public Vertex parent;
        [JsonIgnore]
        public bool visited = false;
        [JsonIgnore]
        public int numberOfPaths { get; set; }

        public string name { get; set; }

        public int indegreeCentralityValue { get; set; }
        public int outdegreeCentralityValue { get; set; }
        public float betweennessCentralityValue { get; set; }
        public int influenceRangeValue { get; set; }
        public float closenessCentralityValue { get; set; }

    }

    public class Link
    {
        public int source { get; set; }
        public int target { get; set; }
    }

    public class NetworkFactors
    {
        public string status { get; set; }
        public float avInCen { get; set; }
        public float avOutCen { get; set; }
        public float avInfRan { get; set; }
        public float avBetCen { get; set; }
        public float avCloCen { get; set; }
        public double density { get; set; }
    }
}