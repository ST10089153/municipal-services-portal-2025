using System;
using System.Collections.Generic;
namespace MunicipalServicesReporting.DataStructures
{
    /// <summary>
    /// Custom Min Heap implementation for priority-based service request processing
    /// Used to always process highest priority requests first
    /// </summary>
    public class MinHeap<T> where T : IComparable<T>
    {
        private List<T> heap;
        public int Count => heap.Count;
        public MinHeap()
        {
            heap = new List<T>();
        }
        public MinHeap(IEnumerable<T> collection)
        {
            heap = new List<T>(collection);
            BuildHeap();
        }
        /// <summary>
        /// Insert an element into the heap
        /// Time Complexity: O(log n)
        /// </summary>
        public void Insert(T item)
        {
            heap.Add(item);
            HeapifyUp(heap.Count - 1);
        }
        /// <summary>
        /// Remove and return the minimum element
        /// Time Complexity: O(log n)
        /// </summary>
        public T ExtractMin()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }
            T min = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            if (heap.Count > 0)
            {
                HeapifyDown(0);
            }
            return min;
        }
        /// <summary>
        /// Peek at the minimum element without removing it
        /// Time Complexity: O(1)
        /// </summary>
        public T PeekMin()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }
            return heap[0];
        }
        /// <summary>
        /// Build heap from existing elements
        /// Time Complexity: O(n)
        /// </summary>
        private void BuildHeap()
        {
            for (int i = heap.Count / 2 - 1; i >= 0; i--)
            {
                HeapifyDown(i);
            }
        }
        /// <summary>
        /// Move element up to maintain heap property
        /// </summary>
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (heap[index].CompareTo(heap[parentIndex]) >= 0)
                {
                    break;
                }
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }
        /// <summary>
        /// Move element down to maintain heap property
        /// </summary>
        private void HeapifyDown(int index)
        {
            int lastIndex = heap.Count - 1;
            while (index < lastIndex)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;
                if (leftChild <= lastIndex && heap[leftChild].CompareTo(heap[smallest]) < 0)
                {
                    smallest = leftChild;
                }
                if (rightChild <= lastIndex && heap[rightChild].CompareTo(heap[smallest]) < 0)
                {
                    smallest = rightChild;
                }
                if (smallest == index)
                {
                    break;
                }
                Swap(index, smallest);
                index = smallest;
            }
        }
        /// <summary>
        /// Swap two elements in the heap
        /// </summary>
        private void Swap(int i, int j)
        {
            T temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }
        /// <summary>
        /// Check if heap is empty
        /// </summary>
        public bool IsEmpty()
        {
            return heap.Count == 0;
        }
        /// <summary>
        /// Get all elements (not in sorted order)
        /// </summary>
        public List<T> GetAll()
        {
            return new List<T>(heap);
        }
        /// <summary>
        /// Clear all elements
        /// </summary>
        public void Clear()
        {
            heap.Clear();
        }
        /// <summary>
        /// Get all elements in sorted order (empties the heap)
        /// </summary>
        public List<T> GetSorted()
        {
            List<T> sorted = new List<T>();
            while (Count > 0)
            {
                sorted.Add(ExtractMin());
            }
            return sorted;
        }
    }
    /// <summary>
    /// Wrapper class for using MinHeap with custom priority
    /// </summary>
    public class PriorityItem<T> : IComparable<PriorityItem<T>>
    {
        public int Priority { get; set; }
        public T Item { get; set; }
        public PriorityItem(int priority, T item)
        {
            Priority = priority;
            Item = item;
        }
        public int CompareTo(PriorityItem<T> other)
        {
            if (other == null) return 1;
            return this.Priority.CompareTo(other.Priority);
        }
    }
}