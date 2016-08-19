﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tt2
{
    public class Network
    {
        public List<Vertex> vertices { get; set; }

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
            newVert.Edges = Edg;
            newVert.Distance = Int32.MaxValue;
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

        public double ClosenessCentrality(int index)
        {
            double result = 0;
            for(int i = 0; i<vertices.Count; i++)
            {
                double a = CountDistance(index, vertices[i].index);
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
                v.Distance = Int32.MaxValue;
                v.Parent = null;
            }
            Vertex vert1 = vertices.Find(vert => vert.index == index1);
            Vertex vert2 = vertices.Find(vert => vert.index == index2);
            vert1.Distance = 0;
            Queue<Vertex> q = new Queue<Vertex>();
            q.Enqueue(vert1);
            while (q.Count != 0)
            {
                Vertex current = q.Dequeue();
                foreach (int neighbIndex in current.Edges)
                {
                    Vertex neighb = vertices.Find(vert => vert.index == neighbIndex);
                    if (neighb.Distance == Int32.MaxValue)
                    {
                        neighb.Distance = current.Distance + 1;
                        neighb.Parent = current;
                        q.Enqueue(neighb);
                    }
                }
            }
            result = vert2.Distance;
            
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
                v.Distance = Int32.MaxValue;
                v.Parent = null;
                v.numberOfPaths = 0;
            }
            if (index1 == index2)
                return 0;
            else
            {
                Vertex vert1 = vertices.Find(vert => vert.index == index1);
                Vertex vert2 = vertices.Find(vert => vert.index == index2);
                vert1.Distance = 0;
                vert1.numberOfPaths = 1;
                Queue<Vertex> q = new Queue<Vertex>();
                Dictionary<Vertex, int> path_counts = new Dictionary<Vertex, int>();
                q.Enqueue(vert1);
                while (q.Count != 0)
                {
                    Vertex current = q.Dequeue();
                    foreach (int neighbIndex in current.Edges)
                    {
                        Vertex neighb = vertices.Find(vert => vert.index == neighbIndex);
                        if (neighb.Distance == Int32.MaxValue)
                        {
                            neighb.Distance = current.Distance + 1;
                            neighb.numberOfPaths = current.numberOfPaths;
                            neighb.Parent = current;
                            q.Enqueue(neighb);
                        }
                        else
                        {
                            if (neighb.Distance == current.Distance + 1)
                            {
                                neighb.numberOfPaths += current.numberOfPaths;
                            }
                            if (neighb.Distance > current.Distance + 1)
                            {
                                neighb.Distance = current.Distance + 1;
                                neighb.numberOfPaths = current.numberOfPaths + 1;
                            }
                        }
                    }

                }
                return vert2.numberOfPaths;
            }
        }

        public float AverageCentrality()
        {
            float result = 0;
            float NumberOfVertices = vertices.Count();
            foreach(Vertex v in vertices)
            {
                result += v.Centrality();
            }
            result /= NumberOfVertices;
            return result;
        }

        public List<int> MaxCentrality() // zwraca indeks wierzchołka i jego centralność
        {
            int resultCentrality = 0;
            int vertexIndex = 0;
            List<int> result = new List<int>();
            bool flag = false;
            foreach(Vertex v in vertices)
            {
                if(v.Centrality() > resultCentrality)
                {
                    flag = true;
                    resultCentrality = v.Centrality();
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
                if(v.Centrality() == resultCentrality && v.index != vertexIndex)
                {
                    result.Add(v.index);
                    result.Add(v.Centrality());
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
                if(v.Edges.Contains(index))
                {
                    result += 1;
                }
            }
            return result;
        }
        }
        public class Vertex
        {
            public int index { get; set; }
            public int weight { get; set; }
            public List<int> Edges;
            public int Distance { get; set; }
            public Vertex Parent;
            public bool visited = false;
            public int numberOfPaths { get; set; }

            public int Centrality()
            {
                int result = Edges.Count();
                return result;
            }
        }
    }

