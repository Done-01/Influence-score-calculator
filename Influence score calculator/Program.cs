class Program
{
    static void Main(string[] args)
    {
        string unweightedFilepath = "Files/unweighted_network.csv";
        string weightedFilepath = "Files/weighted_network.csv";

        Dictionary<string, List<string>> unweightedGraph = ImportUnweightedGraph(unweightedFilepath);
        Dictionary<string, List<(string, double)>> weightedGraph = ImportWeightedGraph(weightedFilepath);
        
        Dictionary<string, double> unweightedResults = BreadthFirstSearch(unweightedGraph, "Diana");
        Dictionary<string, double> weightedResults = DijkstrasAlgorithm(weightedGraph, "B");

        PrintSolution(unweightedResults);
        PrintSolution(weightedResults);
    }

    static Dictionary<string, List<string>> ImportUnweightedGraph(string path)
    {
        Dictionary<string, List<string>> graph = new Dictionary<string, List<string>>();

        foreach (string line in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Trim().Split(',');
            string node = values[0].Trim();
            string connectedNode = values[1].Trim();

            if (!graph.ContainsKey(node))
                graph[node] = new List<string>();

            if (!graph.ContainsKey(connectedNode))
                graph[connectedNode] = new List<string>();

            graph[node].Add(connectedNode);
            graph[connectedNode].Add(node);
        }

        return graph;
    }

    static Dictionary<string, List<(string, double)>> ImportWeightedGraph(string path)
    {
        Dictionary<string, List<(string, double)>> graph = new Dictionary<string, List<(string, double)>>();
        string[] importedArray = File.ReadAllLines(path);

        for (int i = 1; i < importedArray.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(importedArray[i])) continue;

            string[] values = importedArray[i].Trim().Split(',');
            string node = values[0].Trim();

            if (!double.TryParse(values[2], out double distance)) continue;

            string connectedNode = values[1].Trim();

            if (!graph.ContainsKey(node))
                graph[node] = new List<(string, double)>();

            if (!graph.ContainsKey(connectedNode))
                graph[connectedNode] = new List<(string, double)>();

            graph[node].Add((connectedNode, distance));
            graph[connectedNode].Add((node, distance));
        }

        return graph;
    }

    static Dictionary<string, double> BreadthFirstSearch(Dictionary<string, List<string>> graph, string startingNode)
    {
        Dictionary<string, double> distances = new Dictionary<string, double>();
        Queue<string> queue = new Queue<string>();

        if (!graph.ContainsKey(startingNode))
        {
            Console.WriteLine("Name not present in graph");
            return distances;
        }

        queue.Enqueue(startingNode);
        distances[startingNode] = 0;

        while (queue.Count > 0)
        {
            string current = queue.Dequeue();
            double distance = distances[current] + 1;

            foreach (string connection in graph[current])
            {
                if (distances.ContainsKey(connection)) continue;
                distances.Add(connection, distance);
                queue.Enqueue(connection);
            }
        }

        return distances;
    }

    static Dictionary<string, double> DijkstrasAlgorithm(Dictionary<string, List<(string, double)>> graph, string nodeName)
    {
        Dictionary<string, double> distances = new Dictionary<string, double>();
        HashSet<string> visited = new HashSet<string>();
        PriorityQueue<string, double> queue = new PriorityQueue<string, double>();

        if (!graph.ContainsKey(nodeName))
        {
            Console.WriteLine("Name not present in graph");
            return distances;
        }

        queue.Enqueue(nodeName, 0);
        distances[nodeName] = 0;

        while (queue.Count > 0)
        {
            string currentNodeName = queue.Dequeue();

            if (visited.Contains(currentNodeName)) continue;

            visited.Add(currentNodeName);
            double distanceFromStart = distances[currentNodeName];
            Console.WriteLine($"\n{currentNodeName} is {distanceFromStart} from start");

            foreach (var neighbour in graph[currentNodeName])
            {
                double newDistance = distanceFromStart + neighbour.Item2;
                Console.WriteLine($"{neighbour.Item1} is {newDistance} from {currentNodeName}");

                if (!distances.ContainsKey(neighbour.Item1))
                {
                    distances.Add(neighbour.Item1, newDistance);
                    queue.Enqueue(neighbour.Item1, newDistance);
                }
                else if (newDistance < distances[neighbour.Item1])
                {
                    distances[neighbour.Item1] = newDistance;
                    queue.Enqueue(neighbour.Item1, newDistance);
                }
            }
        }

        return distances;
    }

    static void PrintSolution(Dictionary<string,double> results)
    {
        double totalDistance = 0;
        double neighbours = results.Count;

        foreach(var result in results)
        {
            totalDistance = totalDistance + result.Value;
        }

       double influenceScore = (neighbours - 1) / totalDistance;

       Console.WriteLine($"{Math.Round(influenceScore,2)}");
    } 
}