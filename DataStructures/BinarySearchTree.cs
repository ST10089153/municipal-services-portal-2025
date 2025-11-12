using System;
using System.Collections.Generic;
namespace MunicipalServicesReporting.DataStructures
{
    /// <summary>
    /// Custom Binary Search Tree implementation for efficient service request searching
    /// Used for searching requests by RequestId in O(log n) time
    /// </summary>
    public class BinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class BSTNode
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public BSTNode Left { get; set; }
            public BSTNode Right { get; set; }
            public BSTNode(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Left = null;
                Right = null;
            }
        }
        private BSTNode root;
        public int Count { get; private set; }
        public BinarySearchTree()
        {
            root = null;
            Count = 0;
        }
        /// <summary>
        /// Insert a key-value pair into the BST
        /// Time Complexity: O(log n) average, O(n) worst case
        /// </summary>
        public void Insert(TKey key, TValue value)
        {
            root = InsertRecursive(root, key, value);
            Count++;
        }
        private BSTNode InsertRecursive(BSTNode node, TKey key, TValue value)
        {
            if (node == null)
            {
                return new BSTNode(key, value);
            }
            int comparison = key.CompareTo(node.Key);
            if (comparison < 0)
            {
                node.Left = InsertRecursive(node.Left, key, value);
            }
            else if (comparison > 0)
            {
                node.Right = InsertRecursive(node.Right, key, value);
            }
            else
            {
                // Update existing value
                node.Value = value;
            }
            return node;
        }
        /// <summary>
        /// Search for a value by key
        /// Time Complexity: O(log n) average
        /// </summary>
        public TValue Search(TKey key)
        {
            BSTNode result = SearchRecursive(root, key);
            if (result == null)
            {
                throw new KeyNotFoundException($"Key {key} not found in BST");
            }
            return result.Value;
        }
        private BSTNode SearchRecursive(BSTNode node, TKey key)
        {
            if (node == null)
            {
                return null;
            }
            int comparison = key.CompareTo(node.Key);
            if (comparison == 0)
            {
                return node;
            }
            else if (comparison < 0)
            {
                return SearchRecursive(node.Left, key);
            }
            else
            {
                return SearchRecursive(node.Right, key);
            }
        }
        /// <summary>
        /// Check if a key exists in the BST
        /// </summary>
        public bool Contains(TKey key)
        {
            return SearchRecursive(root, key) != null;
        }
        /// <summary>
        /// Get all key-value pairs in sorted order (in-order traversal)
        /// </summary>
        public List<KeyValuePair<TKey, TValue>> GetAllInOrder()
        {
            List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>();
            InOrderTraversal(root, result);
            return result;
        }
        private void InOrderTraversal(BSTNode node, List<KeyValuePair<TKey, TValue>> result)
        {
            if (node == null) return;
            InOrderTraversal(node.Left, result);
            result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
            InOrderTraversal(node.Right, result);
        }
        /// <summary>
        /// Get the minimum key in the BST
        /// </summary>
        public TKey GetMin()
        {
            if (root == null)
                throw new InvalidOperationException("Tree is empty");
            BSTNode current = root;
            while (current.Left != null)
            {
                current = current.Left;
            }
            return current.Key;
        }
        /// <summary>
        /// Get the maximum key in the BST
        /// </summary>
        public TKey GetMax()
        {
            if (root == null)
                throw new InvalidOperationException("Tree is empty");
            BSTNode current = root;
            while (current.Right != null)
            {
                current = current.Right;
            }
            return current.Key;
        }
        /// <summary>
        /// Clear all nodes from the tree
        /// </summary>
        public void Clear()
        {
            root = null;
            Count = 0;
        }
    }
}