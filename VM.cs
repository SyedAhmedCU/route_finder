using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace route_finder
{
    internal class VM : INotifyPropertyChanged
    {
        #region Property Changed
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region singleton
        private static VM vm;
        public static VM Instance { get { vm ??= new VM(); return vm; } }
        #endregion
        #region properties
        const int FIRESTATION_LOCATION = 1;
        const string BREAK_BETWEEN_CASES = "0 0";
        public Dictionary<int, List<int>> graph = new();
        public List<List<int>> routeDFS = new();
        public int endNode;

        private string? inputFileName;
        public string? InputFileName
        {
            get { return inputFileName; }
            set { inputFileName = value; NotifyPropertyChanged(); }
        }

        private string? inputText;
        public string? InputText
        {
            get { return inputText; }
            set { inputText = value; NotifyPropertyChanged(); }
        }
        private string? outputText;
        public string? OutputText
        {
            get { return outputText; }
            set { outputText = value; NotifyPropertyChanged(); }
        }
        #endregion
        public void init()
        {
            if (InputText == string.Empty || InputText == null)
            {
                MessageBox.Show("Please Add the Input File");
                InputFile();
            }
            else
            {
                //convert the input text to a string[]
                //create int[][] from street corner(node) pairs and parse int
                //find test cases and perform depth first search
                ConvertInput();
            }
        }
        public void ConvertInput()
        {
            string[] lines = InputText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int[][] input = new int[lines.Length][];

            //start with test case 1
            int testCase = 1;
            // first index for current test case
            int firstIndex = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (BREAK_BETWEEN_CASES == lines[i])
                {
                    int lastIndex = i;
                    NodeGraph(testCase, input, firstIndex, lastIndex);
                    // for next case 
                    testCase++;
                    firstIndex = i + 1;
                }
                else
                {
                    input[i] = Array.ConvertAll(lines[i].Split(new char[] { ' ' }), int.Parse);
                }
            }
        }
        /// <summary>
        /// This method NodeGraph creates a dictionary of list with nodes as key and value as a list of their adjacent nodes 
        /// Once the graph created,it starts a depth first search to find the routes for current case
        /// </summary>
        /// <param name="testCase"></param>
        /// <param name="input"></param>
        /// <param name="firstIndex"></param>
        /// <param name="lastIndex"></param>
        public void NodeGraph(int testCase, int[][] input, int firstIndex, int lastIndex)
        {
            Dictionary<int, List<int>> graphDict = new();

            for (int i = firstIndex + 1; i < lastIndex; i++)
            {
                int u = input[i][0];
                int v = input[i][1];
                graphDict = AddToGraphDict(graphDict, u, v);
                graphDict = AddToGraphDict(graphDict, v, u);
            }
            int stationLocation = FIRESTATION_LOCATION;
            int fireLocation = input[firstIndex][0];
            //pass the value to global scope to use it in DepthFirstSearch method
            graph = graphDict;
            endNode = fireLocation;
            // reset the DFS output
            routeDFS = new();
            //Use DFS which is a recursive algorithm
            List<List<int>> routeList = DepthFirstSearch(stationLocation, new List<int>() { stationLocation });
            int numOfRoutes = routeList.Count();

            //show output
            #region Output
            #region Show Node Graph
            //show the node graph 
            //string graphString = Newtonsoft.Json.JsonConvert.SerializeObject(graph);
            //string routeListString = Newtonsoft.Json.JsonConvert.SerializeObject(routeList);
            //OutputText += $"\n Case {testCase} Graph: Node with Adjacent Nodes: {graphString} \n " +
            //    $"Available Routes: {routeListString} \n";
            #endregion
            OutputText += $"\nCase {testCase}:\n ";
            for (int i = 0; i < numOfRoutes; i++)
            {
                OutputText += $"{string.Join(" ", routeList[i])} \n ";
            }
            OutputText += $"There are {numOfRoutes} routes from the fire station {stationLocation} " +
                $"to the fire location at street corner {fireLocation}. \n ";
            #endregion
        }
        /// <summary>
        /// This method adds number (node) pairs into a dictionary of list.
        /// All the adjacent nodes are added to a list of values and then added to its corresponding key  
        /// </summary>
        /// <param name="graphDict"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Returns a dictionary of list after adding the number pair to the dictionary</returns>
        public Dictionary<int, List<int>> AddToGraphDict(Dictionary<int, List<int>> graphDict, int x, int y)
        {
            List<int> valueList = new();
            if (!graphDict.ContainsKey(x))
            {
                valueList.Add(y);
                graphDict.Add(x, valueList);
            }
            else
            {
                graphDict[x].Add(y);
            }
            return graphDict;
        }
        public List<List<int>> DepthFirstSearch(int node, List<int> path)
        {
            if (node == endNode)
            {
                routeDFS.Add(path);
            }
            else
            {
                //recursion
                foreach (int adjacentNodes in graph[node])
                {
                    if (!path.Contains(adjacentNodes))
                    {
                        List<int> newPath = new(path);
                        newPath.Add(adjacentNodes);
                        DepthFirstSearch(adjacentNodes, newPath);
                    }
                }
            }
            return routeDFS;
        }
        /// <summary>
        /// Launch OpenFileDialog by calling ShowDialog method.
        /// Get the selected file name and display in a TextBox.
        /// Load content of file in InputText
        /// </summary>
        public void InputFile()
        {
            OpenFileDialog openFileDialog = new();
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                InputFileName = openFileDialog.FileName;
                InputText = File.ReadAllText(openFileDialog.FileName);
            }
        }
        public void SaveFoundRoute()
        {
            string currentRun = $"New run starting at {DateTime.Now:MMMM dd, yyyy HH:mm:ss.fff}{Environment.NewLine}";
            string? SaveTxtfile = $"{currentRun}\n" +
                $"{InputText}\n\n" +
                $"{OutputText}";
            SaveFileDialog saveFileDialog = new();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, SaveTxtfile);
            }
        }
        public void ClearOutput()
        {
            OutputText = string.Empty;
            graph = new();
            routeDFS = new();
            InputText = string.Empty;
            InputFileName = string.Empty;
        }
    }
}