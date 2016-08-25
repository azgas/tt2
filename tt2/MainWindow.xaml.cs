using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tt2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Network siec1 = new Network();

        private void newVertex_Click(object sender, RoutedEventArgs e)
        {
            if (vertexUserInput.Text != "")
            {
                char[] splittingChars = { ' ', ','};
                string[] inp = vertexUserInput.Text.Split(splittingChars);
                List<int> edges = new List<int>();
                int v = 0;
                Int32.TryParse(inp[0], out v);
                for (int i = 1; i<inp.Count();i++ )
                {
                    int ed = 0;
                    Int32.TryParse(inp[i], out ed);
                    edges.Add(ed);

                }

                if(v!=0)
                    siec1.CreateVertex2(v, edges);
                UpdateForm(siec1.vertices);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            siec1.vertices = new List<Vertex>();
            UpdateForm(siec1.vertices);
        }
        public void UpdateForm(List<Vertex> wierzcholki)
        {
            string result ="";
            for(int f = 0; f<siec1.vertices.Count && f<10; f++)
            {
                Vertex w = siec1.vertices[f];
                //string Edg = "";
                result += "\nw: " + w.index;
                //foreach (int i in w.Edges)
                //{
                //    Edg += " " + i.ToString();
                //}
                //result += ", k: " + Edg;
            }
            textWierzcholki.Text = result;
            
        }

        //public void CreateVertex2(int v, List<int> Edg)
        //{
        //    Vertex newVert = new Vertex();
        //    newVert.index = v;
        //    newVert.Edges = Edg;
        //    int index = siec1.vertices.FindIndex(vert => vert.index == v);
        //    if (index >= 0)
        //        siec1.vertices.RemoveAt(index);
        //    siec1.vertices.Add(newVert);
        //    UpdateForm(siec1.vertices);
        //}


        private void wybierzWierchCentralnosc_DropDownOpened(object sender, EventArgs e)
        {
            //wybierzWierchCentralnosc.Items.Clear();
            //List<Vertex> sorted = siec1.vertices.OrderBy(v => v.index).ToList();
            //foreach (Vertex v in sorted)
            //    wybierzWierchCentralnosc.Items.Add(v.index);
           
        }

        private void wybierzWierchCentralnosc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(wybierzWierchCentralnosc.SelectedIndex != -1)
            {   string index = e.AddedItems[0].ToString();
                int vertexInt = Int32.Parse(index);
                Vertex vertex = siec1.vertices.Find(vert => vert.index == vertexInt);
                centralityResult.Text = vertex.Centrality().ToString();
                closenessCentralityResult.Text = siec1.ClosenessCentrality(vertexInt).ToString();
                influenceRangeResult.Text = siec1.InfluenceRange(vertexInt).ToString();
                betweenessCentralityResult.Text = siec1.BetweenessCentrality(vertexInt).ToString();
                CentralityInResult.Text = siec1.CentralityIn(vertexInt).ToString();
                //double centralityAll = 0;
                //double betwCentralityAll = 0;
                //double closCentralityAll = 0;
                //double inflRangeAll = 0;
                //double centrInAll = 0;
                //foreach(Vertex v in siec1.vertices)
                //{
                //    centralityAll += v.Centrality();
                //    betwCentralityAll += siec1.BetweenessCentrality(v.index);
                //    closCentralityAll += siec1.ClosenessCentrality(v.index);
                //    inflRangeAll += siec1.InfluenceRange(v.index);
                //    centrInAll += siec1.CentralityIn(v.index);

                //}
                //int all = siec1.vertices.Count;
                //centralityAll /= all;
                //betwCentralityAll /= all;
                //closCentralityAll /= all;
                //inflRangeAll /= all;
                //centrInAll /= all;
                //centralityResult_Av.Text = centralityAll.ToString();
                //closenessCentralityResult_Av.Text = closCentralityAll.ToString();
                //betweenessCentralityResult_Av.Text = betwCentralityAll.ToString();
                //influenceRangeResult_Av.Text = inflRangeAll.ToString();
                //CentralityInResult_Av.Text = centrInAll.ToString();
            }
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog op = new OpenFileDialog();
            op.InitialDirectory = "c:\\";
            op.Filter = "pliki txt (*.txt)|*.txt";
            op.FilterIndex = 2;
            op.RestoreDirectory = true;
            bool? userClickedOK = op.ShowDialog();
            if (userClickedOK == true)
            {
                try
                {
                    siec1.vertices.Clear();
                    StreamReader file = new StreamReader(op.FileName);
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] words = line.Split('\t');
                        int index = 0;
                        int neighb;
                        Int32.TryParse(words[0], out index);
                        Int32.TryParse(words[1], out neighb);
                        bool exists = false;
                        bool secondExists = false;
                        foreach(Vertex v in siec1.vertices)
                        {
                            if(v.index == index)
                            {
                                exists = true;
                                v.edges.Add(neighb);
                            }
                            else if (v.index == neighb)
                            {
                                secondExists = true;
                                if (ifUndirected.IsChecked == true)
                                    v.edges.Add(index);
                            }
                                
                        }
                        if(!exists)
                        {
                            List<int> edges = new List<int>() { neighb };
                            siec1.CreateVertex2(index, edges);
                        }
                        if (!secondExists)
                            if (ifUndirected.IsChecked == true)
                            {
                                List<int> edg = new List<int>() { index };
                                siec1.CreateVertex2(neighb, edg);
                            }
                            else
                                siec1.CreateVertex2(neighb, new List<int>());
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                wybierzWierchCentralnosc.Items.Clear();
                List<Vertex> sorted = siec1.vertices.OrderBy(v => v.index).ToList();
                foreach (Vertex v in sorted)
                    wybierzWierchCentralnosc.Items.Add(v.index);
                textWierzcholki.Text = "Wczytano";
            }
        }

        private void countAvButton_Click(object sender, RoutedEventArgs e)
        {
            double centralityAll = 0;
            double betwCentralityAll = 0;
            double closCentralityAll = 0;
            double inflRangeAll = 0;
            double centrInAll = 0;
            foreach (Vertex v in siec1.vertices)
            {
                centralityAll += v.Centrality();
                betwCentralityAll += siec1.BetweenessCentrality(v.index);
                closCentralityAll += siec1.ClosenessCentrality(v.index);
                inflRangeAll += siec1.InfluenceRange(v.index);
                centrInAll += siec1.CentralityIn(v.index);

            }
            int all = siec1.vertices.Count;
            centralityAll /= all;
            betwCentralityAll /= all;
            closCentralityAll /= all;
            inflRangeAll /= all;
            centrInAll /= all;
            centralityResult_Av.Text = centralityAll.ToString();
            closenessCentralityResult_Av.Text = closCentralityAll.ToString();
            betweenessCentralityResult_Av.Text = betwCentralityAll.ToString();
            influenceRangeResult_Av.Text = inflRangeAll.ToString();
            CentralityInResult_Av.Text = centrInAll.ToString();
            DensityResult.Text = siec1.Density().ToString();
        }
    }
}
