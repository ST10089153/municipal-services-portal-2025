using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesReporting.DataStructures;

namespace MunicipalServicesReporting
{
    /// <summary>
    /// Manages service requests using multiple advanced data structures
    /// Demonstrates practical application of trees, heaps, and graphs
    /// </summary>
    public class ServiceRequestManager
    {
        // BST for fast ID-based lookups
        private BinarySearchTree<string, ServiceRequest> requestBST;

        // AVL Tree for balanced priority-based access
        private AVLTree<int, List<ServiceRequest>> priorityAVL;

        // Red-Black Tree for date-based indexing
        private RedBlackTree<DateTime, List<ServiceRequest>> dateRBTree;

        // Min Heap for priority queue processing
        private MinHeap<PriorityItem<ServiceRequest>> priorityHeap;

        // Graph for dependency management
        private ServiceRequestGraph dependencyGraph;

        // Additional tracking
        private Dictionary<string, ServiceRequest> allRequests;
        private int nextPriorityValue;

        public int TotalRequests => allRequests.Count;

        public ServiceRequestManager()
        {
            requestBST = new BinarySearchTree<string, ServiceRequest>();
            priorityAVL = new AVLTree<int, List<ServiceRequest>>();
            dateRBTree = new RedBlackTree<DateTime, List<ServiceRequest>>();
            priorityHeap = new MinHeap<PriorityItem<ServiceRequest>>();
            dependencyGraph = new ServiceRequestGraph();
            allRequests = new Dictionary<string, ServiceRequest>();
            nextPriorityValue = 1;

            InitializeSampleData();
        }

        /// <summary>
        /// Initialize with sample service requests
        /// </summary>
        private void InitializeSampleData()
        {
            List<ServiceRequest> sampleRequests = new List<ServiceRequest>
            {
                new ServiceRequest("SR001", "Pothole on Main Street", "Roads & Infrastructure",
                    "Large pothole causing traffic issues", "123 Main St",
                    DateTime.Now.AddDays(-10), RequestStatus.InProgress, Priority.High, "Roads Department")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(3), CitizenImpactScore = 150 },

                new ServiceRequest("SR002", "Street Light Not Working", "Public Safety",
                    "Street light out for 3 days", "456 Oak Ave",
                    DateTime.Now.AddDays(-7), RequestStatus.UnderReview, Priority.Medium, "Electrical Department")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(5), CitizenImpactScore = 45 },

                new ServiceRequest("SR003", "Water Leak", "Water & Electricity",
                    "Major water leak affecting multiple homes", "789 Pine Rd",
                    DateTime.Now.AddDays(-5), RequestStatus.InProgress, Priority.Critical, "Water Services")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(1), CitizenImpactScore = 200 },

                new ServiceRequest("SR004", "Graffiti Removal", "Parks & Recreation",
                    "Graffiti on community center wall", "Community Center",
                    DateTime.Now.AddDays(-15), RequestStatus.Completed, Priority.Low, "Parks Department")
                    { ActualCompletionDate = DateTime.Now.AddDays(-2), CitizenImpactScore = 20 },

                new ServiceRequest("SR005", "Broken Sidewalk", "Roads & Infrastructure",
                    "Cracked sidewalk posing trip hazard", "321 Elm St",
                    DateTime.Now.AddDays(-12), RequestStatus.InProgress, Priority.Medium, "Roads Department")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(7), CitizenImpactScore = 80 },

                new ServiceRequest("SR006", "Illegal Dumping", "Sanitation",
                    "Large pile of debris dumped illegally", "Industrial Area",
                    DateTime.Now.AddDays(-8), RequestStatus.UnderReview, Priority.High, "Waste Management")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(4), CitizenImpactScore = 100 },

                new ServiceRequest("SR007", "Traffic Signal Malfunction", "Public Safety",
                    "Traffic light stuck on red", "Main & 5th Intersection",
                    DateTime.Now.AddDays(-2), RequestStatus.InProgress, Priority.Critical, "Traffic Department")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(1), CitizenImpactScore = 300 },

                new ServiceRequest("SR008", "Park Bench Repair", "Parks & Recreation",
                    "Broken bench in central park", "Central Park",
                    DateTime.Now.AddDays(-20), RequestStatus.Submitted, Priority.Low, "Parks Department")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(14), CitizenImpactScore = 15 },

                new ServiceRequest("SR009", "Sewer Backup", "Water & Electricity",
                    "Sewer backing up into street", "567 Maple Dr",
                    DateTime.Now.AddDays(-4), RequestStatus.InProgress, Priority.Critical, "Water Services")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(2), CitizenImpactScore = 180 },

                new ServiceRequest("SR010", "Stop Sign Missing", "Public Safety",
                    "Stop sign knocked down", "2nd & Broadway",
                    DateTime.Now.AddDays(-6), RequestStatus.UnderReview, Priority.High, "Traffic Department")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(3), CitizenImpactScore = 120 },

                new ServiceRequest("SR011", "Playground Equipment Damaged", "Parks & Recreation",
                    "Swing set needs repair", "Riverside Park",
                    DateTime.Now.AddDays(-9), RequestStatus.Submitted, Priority.Medium, "Parks Department")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(10), CitizenImpactScore = 60 },

                new ServiceRequest("SR012", "Power Outage", "Water & Electricity",
                    "Recurring power outages in area", "Northside District",
                    DateTime.Now.AddDays(-3), RequestStatus.InProgress, Priority.Critical, "Electrical Department")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(2), CitizenImpactScore = 250 },

                new ServiceRequest("SR013", "Overgrown Vegetation", "Parks & Recreation",
                    "Bushes blocking sidewalk", "145 Cedar Ln",
                    DateTime.Now.AddDays(-18), RequestStatus.Completed, Priority.Low, "Parks Department")
                    { ActualCompletionDate = DateTime.Now.AddDays(-5), CitizenImpactScore = 25 },

                new ServiceRequest("SR014", "Road Flooding", "Roads & Infrastructure",
                    "Street floods during rain", "Low Valley Road",
                    DateTime.Now.AddDays(-11), RequestStatus.UnderReview, Priority.High, "Roads Department")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(15), CitizenImpactScore = 140 },

                new ServiceRequest("SR015", "Noise Complaint", "Other",
                    "Construction noise beyond permitted hours", "789 Commerce St",
                    DateTime.Now.AddDays(-1), RequestStatus.Submitted, Priority.Medium, "Inspections")
                    { EstimatedCompletionDate = DateTime.Now.AddDays(5), CitizenImpactScore = 30 }
            };

            // Add all requests
            foreach (var request in sampleRequests)
            {
                AddRequest(request);
            }

            // Create dependencies (graph edges)
            dependencyGraph.AddEdge("SR003", "SR001", 2); // Water leak related to pothole
            dependencyGraph.AddEdge("SR009", "SR003", 3); // Sewer backup related to water leak
            dependencyGraph.AddEdge("SR007", "SR010", 1); // Traffic signal related to stop sign
            dependencyGraph.AddEdge("SR014", "SR005", 2); // Road flooding related to sidewalk
            dependencyGraph.AddEdge("SR001", "SR005", 1); // Pothole related to sidewalk
        }

        /// <summary>
        /// Add a new service request to all data structures
        /// </summary>
        public void AddRequest(ServiceRequest request)
        {
            // Add to main dictionary
            allRequests[request.RequestId] = request;

            // BST: Fast ID lookup
            requestBST.Insert(request.RequestId, request);

            // AVL Tree: Priority-based balanced access
            int priorityKey = (int)request.PriorityLevel * 1000 + nextPriorityValue++;
            if (!priorityAVL.Contains(priorityKey))
            {
                priorityAVL.Insert(priorityKey, new List<ServiceRequest>());
            }
            var priorityList = priorityAVL.Search(priorityKey);
            priorityList.Add(request);

            // Red-Black Tree: Date-based indexing
            DateTime dateKey = request.SubmittedDate.Date;
            if (!dateRBTree.Contains(dateKey))
            {
                dateRBTree.Insert(dateKey, new List<ServiceRequest>());
            }
            var dateList = dateRBTree.Search(dateKey);
            dateList.Add(request);

            // Min Heap: Priority queue for processing
            if (request.Status != RequestStatus.Completed && request.Status != RequestStatus.Rejected)
            {
                int heapPriority = -(int)request.PriorityLevel * 100 + (int)(DateTime.Now - request.SubmittedDate).TotalDays;
                priorityHeap.Insert(new PriorityItem<ServiceRequest>(heapPriority, request));
            }

            // Graph: Add as vertex for dependency tracking
            dependencyGraph.AddVertex(request);
        }

        /// <summary>
        /// Search request by ID using BST - O(log n)
        /// </summary>
        public ServiceRequest SearchById(string requestId)
        {
            try
            {
                return requestBST.Search(requestId);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get all requests
        /// </summary>
        public List<ServiceRequest> GetAllRequests()
        {
            return new List<ServiceRequest>(allRequests.Values);
        }

        /// <summary>
        /// Get requests by status
        /// </summary>
        public List<ServiceRequest> GetRequestsByStatus(RequestStatus status)
        {
            return allRequests.Values.Where(r => r.Status == status).ToList();
        }

        /// <summary>
        /// Get requests by priority using AVL Tree
        /// </summary>
        public List<ServiceRequest> GetRequestsByPriority(Priority priority)
        {
            var allPriority = priorityAVL.GetAllInOrder();
            List<ServiceRequest> result = new List<ServiceRequest>();

            foreach (var kvp in allPriority)
            {
                foreach (var request in kvp.Value)
                {
                    if (request.PriorityLevel == priority)
                    {
                        result.Add(request);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get requests in date range using Red-Black Tree
        /// </summary>
        public List<ServiceRequest> GetRequestsInDateRange(DateTime startDate, DateTime endDate)
        {
            var requestsInRange = dateRBTree.GetRange(startDate.Date, endDate.Date);
            List<ServiceRequest> result = new List<ServiceRequest>();

            foreach (var list in requestsInRange)
            {
                result.AddRange(list);
            }

            return result;
        }

        /// <summary>
        /// Get next highest priority request using Min Heap
        /// </summary>
        public ServiceRequest GetNextPriorityRequest()
        {
            if (priorityHeap.IsEmpty())
            {
                return null;
            }

            var item = priorityHeap.ExtractMin();
            return item.Item;
        }

        /// <summary>
        /// Get related requests using Graph BFS
        /// </summary>
        public List<ServiceRequest> GetRelatedRequests(string requestId)
        {
            return dependencyGraph.BFS(requestId);
        }

        /// <summary>
        /// Get dependency chain using Graph DFS
        /// </summary>
        public List<ServiceRequest> GetDependencyChain(string requestId)
        {
            return dependencyGraph.DFS(requestId);
        }

        /// <summary>
        /// Find optimal processing order using MST
        /// </summary>
        public List<ServiceRequest> GetOptimalProcessingOrder()
        {
            var mst = dependencyGraph.GetMinimumSpanningTree();

            // Return highest priority requests in optimal order
            var allRequests = GetAllRequests()
                .Where(r => r.Status == RequestStatus.InProgress || r.Status == RequestStatus.UnderReview)
                .OrderByDescending(r => r.PriorityLevel)
                .ThenBy(r => r.SubmittedDate)
                .ToList();

            return allRequests;
        }

        /// <summary>
        /// Get statistics
        /// </summary>
        public Dictionary<string, int> GetStatistics()
        {
            return new Dictionary<string, int>
            {
                { "Total", allRequests.Count },
                { "Submitted", GetRequestsByStatus(RequestStatus.Submitted).Count },
                { "InProgress", GetRequestsByStatus(RequestStatus.InProgress).Count },
                { "Completed", GetRequestsByStatus(RequestStatus.Completed).Count },
                { "Critical", GetRequestsByPriority(Priority.Critical).Count },
                { "High", GetRequestsByPriority(Priority.High).Count },
                { "Medium", GetRequestsByPriority(Priority.Medium).Count },
                { "Low", GetRequestsByPriority(Priority.Low).Count }
            };
        }

        /// <summary>
        /// Search requests by multiple criteria
        /// </summary>
        public List<ServiceRequest> SearchRequests(string category = null, RequestStatus? status = null, Priority? priority = null)
        {
            var results = GetAllRequests();

            if (!string.IsNullOrEmpty(category) && category != "All Categories")
            {
                results = results.Where(r => r.Category == category).ToList();
            }

            if (status.HasValue)
            {
                results = results.Where(r => r.Status == status.Value).ToList();
            }

            if (priority.HasValue)
            {
                results = results.Where(r => r.PriorityLevel == priority.Value).ToList();
            }

            return results;
        }

        /// <summary>
        /// Get unique categories
        /// </summary>
        public List<string> GetAllCategories()
        {
            return allRequests.Values.Select(r => r.Category).Distinct().OrderBy(c => c).ToList();
        }
    }
}