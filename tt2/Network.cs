using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tt2
{
    public class Network
    {
        public List<Vertex> vertices { get; set; }

        public void CalculateFactors()
        {
            foreach(Vertex v in vertices)
            {
                int index = v.index;
                v.indegreeCentralityValue = CentralityIn(index);
                v.betweenessCentralityValue = BetweenessCentrality(index);
                v.influenceRangeValue = InfluenceRange(index);
                v.outdegreeCentralityValue = CentralityOut(index);
                v.closenessCentralityValue = ClosenessCentrality(index);
            }
        }

        public int CountAllPaths()
        {
            int allPaths = 0;
            int k = vertices.Count();
            for(int i = 0; i< k; i++)
            {
                for(int j = 0; j<k; j++)
                {
                    int paths = CountPaths(vertices[i].index, vertices[j].index);
                    allPaths += paths;
                }
            }
            return allPaths;
        }

        public bool DoesShortestPathContain(int source, int destination, int target)
        {
            bool result = false;

            return result;
        }

        public void CreateVertex2(int v, List<int> Edg)
        {
            Vertex newVert = new Vertex();
            newVert.index = v;
            newVert.edges = Edg;
            newVert.distance = Int32.MaxValue;
            int index = vertices.FindIndex(vert => vert.index == v);
            if (index >= 0)
                vertices.RemoveAt(index);
            vertices.Add(newVert);
            
        }

        public float BetweenessCentrality(int index)
        {
            float result = 0;
            int pathsV = 0;
            int k = vertices.Count();
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    int source = vertices[i].index;
                    int destination = vertices[j].index;
                    if(source != index && destination != index)
                    {
                        int paths = CountShortestPathsContaining(source, destination, index);
                        pathsV += paths;
                    }

                }
            }
            float pathsv = (float)pathsV;
            float allpaths = (float)CountAllPaths();
            result = pathsv / allpaths;
            return result;
        }

        public float ClosenessCentrality(int index)
        {
            float result = 0;
            for(int i = 0; i<vertices.Count; i++)
            {
                float a = CountDistance(index, vertices[i].index);
                if (a != 0 && a !=Int32.MaxValue)
                    result += (1/a);
            }
            result /= 10;
            return result;
        }

        public int InfluenceRange(int index)
        {
            int result = 0;
            foreach(Vertex v in vertices)
            {
                int a = CountDistance(index, v.index);
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
                v.paren = null;
            }
            Vertex vert1 = vertices.Find(vert => vert.index == index1);
            Vertex vert2 = vertices.Find(vert => vert.index == index2);
            vert1.distance = 0;
            Queue<Vertex> q = new Queue<Vertex>();
            q.Enqueue(vert1);
            while (q.Count != 0)
            {
                Vertex current = q.Dequeue();
                foreach (int neighbIndex in current.edges)
                {
                    Vertex neighb = vertices.Find(vert => vert.index == neighbIndex);
                    if (neighb.distance == Int32.MaxValue)
                    {
                        neighb.distance = current.distance + 1;
                        neighb.paren = current;
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
            if( a + b == c)
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
                v.paren = null;
                v.numberOfPaths = 0;
            }
            if (index1 == index2)
                return 0;
            else
            {
                Vertex vert1 = vertices.Find(vert => vert.index == index1);
                Vertex vert2 = vertices.Find(vert => vert.index == index2);
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
                        Vertex neighb = vertices.Find(vert => vert.index == neighbIndex);
                        if (neighb.distance == Int32.MaxValue)
                        {
                            neighb.distance = current.distance + 1;
                            neighb.numberOfPaths = current.numberOfPaths;
                            neighb.paren = current;
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
            foreach(Vertex v in vertices)
            {
                switch(whichCase)
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
                        result += v.betweenessCentralityValue;
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
                    vertexIndex = v.index;
                }

            }
            if (flag)
            {
                result.Add(vertexIndex);
                result.Add(resultCentrality);
            }
            foreach(Vertex v in vertices)
            {
                if(v.indegreeCentralityValue == resultCentrality && v.index != vertexIndex)
                {
                    result.Add(v.index);
                    result.Add(v.indegreeCentralityValue);
                }
            }
            return result;
        }

        public List<string> ShowVertices()
        {
            List<string> list = new List<string>();
            foreach(Vertex v in vertices)
            {
                list.Add(v.index.ToString());
            }
            return list;
        }

        public int CentralityIn(int index)
        {
            int result = 0;
            foreach(Vertex v in vertices)
            {
                if(v.edges.Contains(index))
                {
                    result += 1;
                }
            }
            return result;
        }

        public int CentralityOut(int index)
        {
            int result = 0;
            Vertex vert1 = vertices.Find(vert => vert.index == index);
            result = vert1.edges.Count;
            return result;
        }

        public double Density()
        {
            double result = 0;
            foreach(Vertex ver in vertices)
            {
                result += ver.edges.Count;
            }
            double n = vertices.Count;
            result /= (n * (n - 1));
            return result;
        }
        }
        public class Vertex
        {
            public int index { get; set; }
            //public int weight { get; set; }
            public List<int> edges;
            public int distance { get; set; }
            public Vertex paren;
            public bool visited = false;
            public int numberOfPaths { get; set; }
            public int indegreeCentralityValue { get; set; }
            public int outdegreeCentralityValue { get; set; }
            public float betweenessCentralityValue { get; set; }
            public int influenceRangeValue { get; set; }
            public float closenessCentralityValue { get; set; }
            
        }
    }


