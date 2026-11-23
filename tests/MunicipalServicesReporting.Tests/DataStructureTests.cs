using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesReporting;
using MunicipalServicesReporting.DataStructures;
using Xunit;

namespace MunicipalServicesReporting.Tests
{
    public class BinarySearchTreeTests
    {
        [Fact]
        public void Search_ReturnsValueForInsertedKey()
        {
            var bst = new BinarySearchTree<string, int>();
            bst.Insert("SR005", 5);
            bst.Insert("SR001", 1);
            bst.Insert("SR009", 9);

            Assert.Equal(1, bst.Search("SR001"));
            Assert.Equal(9, bst.Search("SR009"));
            Assert.True(bst.Contains("SR005"));
        }

        [Fact]
        public void Search_MissingKey_Throws()
        {
            var bst = new BinarySearchTree<string, int>();
            bst.Insert("SR001", 1);

            Assert.Throws<KeyNotFoundException>(() => bst.Search("SR999"));
            Assert.False(bst.Contains("SR999"));
        }

        [Fact]
        public void InOrderTraversal_ReturnsKeysSorted()
        {
            var bst = new BinarySearchTree<int, string>();
            foreach (var k in new[] { 50, 30, 70, 20, 40, 60, 80 }) bst.Insert(k, k.ToString());

            Assert.Equal(new[] { 20, 30, 40, 50, 60, 70, 80 }, bst.GetAllInOrder().Select(kv => kv.Key));
            Assert.Equal(20, bst.GetMin());
            Assert.Equal(80, bst.GetMax());
        }
    }

    public class AVLTreeTests
    {
        [Fact]
        public void SortedInserts_StayBalanced()
        {
            // Inserting sorted keys into a plain BST gives a height of n; an AVL tree must stay O(log n)
            var avl = new AVLTree<int, int>();
            const int n = 1023;
            for (int i = 1; i <= n; i++) avl.Insert(i, i);

            double maxAvlHeight = 1.44 * Math.Log(n + 2, 2);
            Assert.True(avl.GetTreeHeight() <= maxAvlHeight, $"height {avl.GetTreeHeight()} exceeds AVL bound {maxAvlHeight:F1}");
            Assert.Equal(Enumerable.Range(1, n), avl.GetAllInOrder().Select(kv => kv.Key));
        }

        [Fact]
        public void Search_FindsEveryKeyAfterRotations()
        {
            var avl = new AVLTree<int, string>();
            foreach (var k in new[] { 30, 20, 10, 25, 40, 50, 5, 4, 3 }) avl.Insert(k, "v" + k);

            foreach (var k in new[] { 30, 20, 10, 25, 40, 50, 5, 4, 3 })
                Assert.Equal("v" + k, avl.Search(k));
            Assert.False(avl.Contains(99));
        }
    }

    public class RedBlackTreeTests
    {
        [Fact]
        public void InOrderTraversal_IsSortedAfterManyInserts()
        {
            var rb = new RedBlackTree<int, int>();
            var rng = new Random(42);
            var keys = Enumerable.Range(0, 500).Select(_ => rng.Next(10000)).Distinct().ToList();
            foreach (var k in keys) rb.Insert(k, k);

            Assert.Equal(keys.OrderBy(k => k), rb.GetAllInOrder().Select(kv => kv.Key));
            Assert.Equal(keys.Count, rb.Count);
        }

        [Fact]
        public void GetRange_ReturnsOnlyKeysInsideRangeInclusive()
        {
            var rb = new RedBlackTree<DateTime, string>();
            var start = new DateTime(2025, 11, 1);
            for (int d = 0; d < 30; d++) rb.Insert(start.AddDays(d), "day" + d);

            var result = rb.GetRange(start.AddDays(10), start.AddDays(14));

            Assert.Equal(new[] { "day10", "day11", "day12", "day13", "day14" }, result);
        }
    }

    public class MinHeapTests
    {
        [Fact]
        public void ExtractMin_ReturnsItemsInAscendingOrder()
        {
            var rng = new Random(7);
            var values = Enumerable.Range(0, 200).Select(_ => rng.Next(-1000, 1000)).ToList();
            var heap = new MinHeap<int>();
            foreach (var v in values) heap.Insert(v);

            var extracted = new List<int>();
            while (!heap.IsEmpty()) extracted.Add(heap.ExtractMin());

            Assert.Equal(values.OrderBy(v => v), extracted);
        }

        [Fact]
        public void BuildHeapConstructor_ProducesValidHeap()
        {
            var heap = new MinHeap<int>(new[] { 9, 4, 7, 1, 8, 2 });
            Assert.Equal(1, heap.PeekMin());
            Assert.Equal(new[] { 1, 2, 4, 7, 8, 9 }, heap.GetSorted());
        }

        [Fact]
        public void ExtractMin_OnEmptyHeap_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => new MinHeap<int>().ExtractMin());
        }
    }

    public class ServiceRequestGraphTests
    {
        private static ServiceRequestGraph BuildGraph()
        {
            var graph = new ServiceRequestGraph();
            foreach (var id in new[] { "A", "B", "C", "D", "E" })
                graph.AddVertex(new ServiceRequest(id, id, "Cat", "", "", DateTime.Today, RequestStatus.Submitted, Priority.Low, "Dept"));

            graph.AddEdge("A", "B", 4);
            graph.AddEdge("A", "C", 1);
            graph.AddEdge("C", "B", 1);
            graph.AddEdge("B", "D", 2);
            return graph;
        }

        [Fact]
        public void BFS_VisitsReachableVerticesLevelByLevel()
        {
            var ids = BuildGraph().BFS("A").Select(r => r.RequestId).ToList();
            Assert.Equal(new[] { "A", "B", "C", "D" }, ids); // E is unreachable
        }

        [Fact]
        public void DFS_FollowsEachBranchToTheEnd()
        {
            var ids = BuildGraph().DFS("A").Select(r => r.RequestId).ToList();
            Assert.Equal(new[] { "A", "B", "D", "C" }, ids);
        }

        [Fact]
        public void FindShortestPath_PrefersLowerTotalWeight()
        {
            // A->B directly costs 4; A->C->B costs 2
            Assert.Equal(new[] { "A", "C", "B", "D" }, BuildGraph().FindShortestPath("A", "D"));
            Assert.Empty(BuildGraph().FindShortestPath("A", "E"));
        }

        [Fact]
        public void MinimumSpanningTree_SkipsEdgesThatWouldFormACycle()
        {
            var mst = BuildGraph().GetMinimumSpanningTree();
            Assert.Equal(3, mst.Count);              // A-C, C-B, B-D (A-B would close a cycle)
            Assert.Equal(4, mst.Sum(e => e.Weight)); // 1 + 1 + 2
        }

        [Fact]
        public void HasCycle_DetectsDirectedCycle()
        {
            var graph = BuildGraph();
            Assert.False(graph.HasCycle());

            graph.AddEdge("D", "A", 1);
            Assert.True(graph.HasCycle());
        }
    }
}
