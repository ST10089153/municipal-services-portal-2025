using System;
using System.Collections.Generic;
using System.Linq;
namespace MunicipalServicesReporting.DataStructures
{
    /// <summary>
    /// Graph implementation for modeling service request dependencies
    /// Uses adjacency list representation
    /// </summary>
    public class ServiceRequestGraph
    {
        private Dictionary<string, List<GraphEdge>> adjacencyList;
        private Dictionary<string, ServiceRequest> vertices;
        public int VertexCount => vertices.Count;
        public int EdgeCount { get; private set; }
        public class GraphEdge
        {
            public string DestinationId { get; set; }
            public int Weight { get; set; } // Represents dependency strength or days
            public GraphEdge(string destinationId, int weight)
            {
                DestinationId = destinationId;
                Weight = weight;
            }
        }
        public ServiceRequestGraph()
        {
            adjacencyList = new Dictionary<string, List<GraphEdge>>();
            vertices = new Dictionary<string, ServiceRequest>();
            EdgeCount = 0;
        }
        /// <summary>
        /// Add a vertex (service request) to the graph
        /// </summary>
        public void AddVertex(ServiceRequest request)
        {
            if (!vertices.ContainsKey(request.RequestId))
            {
                vertices[request.RequestId] = request;
                adjacencyList[request.RequestId] = new List<GraphEdge>();
            }
        }
        /// <summary>
        /// Add an edge (dependency) between two requests
        /// </summary>
        public void AddEdge(string sourceId, string destinationId, int weight)
        {
            if (!vertices.ContainsKey(sourceId) || !vertices.ContainsKey(destinationId))
            {
                throw new ArgumentException("Both vertices must exist in the graph");
            }
            adjacencyList[sourceId].Add(new GraphEdge(destinationId, weight));
            EdgeCount++;
        }
        /// <summary>
        /// Breadth-First Search traversal from a starting vertex
        /// Used to find all related requests
        /// Time Complexity: O(V + E)
        /// </summary>
        public List<ServiceRequest> BFS(string startId)
        {
            if (!vertices.ContainsKey(startId))
            {
                return new List<ServiceRequest>();
            }
            List<ServiceRequest> result = new List<ServiceRequest>();
            HashSet<string> visited = new HashSet<string>();
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(startId);
            visited.Add(startId);
            while (queue.Count > 0)
            {
                string currentId = queue.Dequeue();
                result.Add(vertices[currentId]);
                foreach (var edge in adjacencyList[currentId])
                {
                    if (!visited.Contains(edge.DestinationId))
                    {
                        visited.Add(edge.DestinationId);
                        queue.Enqueue(edge.DestinationId);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Depth-First Search traversal from a starting vertex
        /// Used for dependency chain analysis
        /// Time Complexity: O(V + E)
        /// </summary>
        public List<ServiceRequest> DFS(string startId)
        {
            if (!vertices.ContainsKey(startId))
            {
                return new List<ServiceRequest>();
            }
            List<ServiceRequest> result = new List<ServiceRequest>();
            HashSet<string> visited = new HashSet<string>();
            DFSRecursive(startId, visited, result);
            return result;
        }
        private void DFSRecursive(string currentId, HashSet<string> visited, List<ServiceRequest>
        result)
        {
            visited.Add(currentId);
            result.Add(vertices[currentId]);
            foreach (var edge in adjacencyList[currentId])
            {
                if (!visited.Contains(edge.DestinationId))
                {
                    DFSRecursive(edge.DestinationId, visited, result);
                }
            }
        }
        /// <summary>
        /// Find shortest path between two requests using Dijkstra's algorithm
        /// Time Complexity: O((V + E) log V)
        /// </summary>
        public List<string> FindShortestPath(string startId, string endId)
        {
            if (!vertices.ContainsKey(startId) || !vertices.ContainsKey(endId))
            {
                return new List<string>();
            }
            Dictionary<string, int> distances = new Dictionary<string, int>();
            Dictionary<string, string> previous = new Dictionary<string, string>();
            HashSet<string> unvisited = new HashSet<string>(vertices.Keys);
            foreach (var vertex in vertices.Keys)
            {
                distances[vertex] = int.MaxValue;
            }
            distances[startId] = 0;
            while (unvisited.Count > 0)
            {
                string current = unvisited.OrderBy(v => distances[v]).First();
                unvisited.Remove(current);
                if (current == endId) break;
                if (distances[current] == int.MaxValue) break;
                foreach (var edge in adjacencyList[current])
                {
                    if (!unvisited.Contains(edge.DestinationId)) continue;
                    int altDistance = distances[current] + edge.Weight;
                    if (altDistance < distances[edge.DestinationId])
                    {
                        distances[edge.DestinationId] = altDistance;
                        previous[edge.DestinationId] = current;
                    }
                }
            }
            // Reconstruct path
            List<string> path = new List<string>();
            string currentNode = endId;
            if (!previous.ContainsKey(currentNode) && currentNode != startId)
            {
                return new List<string>(); // No path found
            }
            while (currentNode != null)
            {
                path.Insert(0, currentNode);
                if (currentNode == startId) break;
                currentNode = previous.ContainsKey(currentNode) ? previous[currentNode] : null;
            }
            return path;
        }
        /// <summary>
        /// Kruskal's Algorithm for Minimum Spanning Tree
        /// Used to find optimal request processing order
        /// Time Complexity: O(E log E)
        /// </summary>
        public List<GraphEdge> GetMinimumSpanningTree()
        {
            List<GraphEdge> mst = new List<GraphEdge>();
            List<EdgeWithSource> allEdges = new List<EdgeWithSource>();
            // Collect all edges
            foreach (var vertex in adjacencyList.Keys)
            {
                foreach (var edge in adjacencyList[vertex])
                {
                    allEdges.Add(new EdgeWithSource(vertex, edge.DestinationId, edge.Weight));
                }
            }
            // Sort edges by weight
            allEdges.Sort((a, b) => a.Weight.CompareTo(b.Weight));
            // Union-Find data structure
            DisjointSet disjointSet = new DisjointSet(vertices.Keys);
            foreach (var edge in allEdges)
            {
                string root1 = disjointSet.Find(edge.SourceId);
                string root2 = disjointSet.Find(edge.DestinationId);
                if (root1 != root2)
                {
                    mst.Add(new GraphEdge(edge.DestinationId, edge.Weight));
                    disjointSet.Union(root1, root2);
                }
                if (mst.Count == vertices.Count - 1)
                {
                    break; // MST complete
                }
            }
            return mst;
        }
        /// <summary>
        /// Get all vertices (service requests)
        /// </summary>
        public List<ServiceRequest> GetAllVertices()
        {
            return new List<ServiceRequest>(vertices.Values);
        }
        /// <summary>
        /// Get adjacent requests (dependencies)
        /// </summary>
        public List<ServiceRequest> GetAdjacentRequests(string requestId)
        {
            if (!adjacencyList.ContainsKey(requestId))
            {
                return new List<ServiceRequest>();
            }
            List<ServiceRequest> adjacent = new List<ServiceRequest>();
            foreach (var edge in adjacencyList[requestId])
            {
                if (vertices.ContainsKey(edge.DestinationId))
                {
                    adjacent.Add(vertices[edge.DestinationId]);
                }
            }
            return adjacent;
        }
        /// <summary>
        /// Check if graph has cycles using DFS
        /// </summary>
        public bool HasCycle()
        {
            HashSet<string> visited = new HashSet<string>();
            HashSet<string> recursionStack = new HashSet<string>();
            foreach (var vertex in vertices.Keys)
            {
                if (!visited.Contains(vertex))
                {
                    if (HasCycleDFS(vertex, visited, recursionStack))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool HasCycleDFS(string vertex, HashSet<string> visited, HashSet<string>
        recursionStack)
        {
            visited.Add(vertex);
            recursionStack.Add(vertex);
            foreach (var edge in adjacencyList[vertex])
            {
                if (!visited.Contains(edge.DestinationId))
                {
                    if (HasCycleDFS(edge.DestinationId, visited, recursionStack))
                    {
                        return true;
                    }
                }
                else if (recursionStack.Contains(edge.DestinationId))
                {
                    return true;
                }
            }
            recursionStack.Remove(vertex);
            return false;
        }
        // Helper classes
        private class EdgeWithSource
        {
            public string SourceId { get; set; }
            public string DestinationId { get; set; }
            public int Weight { get; set; }
            public EdgeWithSource(string sourceId, string destinationId, int weight)
            {
                SourceId = sourceId;
                DestinationId = destinationId;
                Weight = weight;
            }
        }
        private class DisjointSet
        {
            private Dictionary<string, string> parent;
            private Dictionary<string, int> rank;
            public DisjointSet(IEnumerable<string> elements)
            {
                parent = new Dictionary<string, string>();
                rank = new Dictionary<string, int>();
                foreach (var element in elements)
                {
                    parent[element] = element;
                    rank[element] = 0;
                }
            }
            public string Find(string element)
            {
                if (parent[element] != element)
                {
                    parent[element] = Find(parent[element]); // Path compression
                }
                return parent[element];
            }
            public void Union(string element1, string element2)
            {
                string root1 = Find(element1);
                string root2 = Find(element2);
                if (root1 != root2)
                {
                    // Union by rank
                    if (rank[root1] < rank[root2])
                    {
                        parent[root1] = root2;
                    }
                    else if (rank[root1] > rank[root2])
                    {
                        parent[root2] = root1;
                    }
                    else
                    {
                        parent[root2] = root1;
                        rank[root1]++;
                    }
                }
            }
        }
    }
}