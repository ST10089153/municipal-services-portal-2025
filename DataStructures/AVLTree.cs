using System;
using System.Collections.Generic;
namespace MunicipalServicesReporting.DataStructures
{
    /// <summary>
    /// Custom AVL Tree (Self-Balancing Binary Search Tree) implementation
    /// Used for maintaining balanced request data sorted by priority
    /// Guarantees O(log n) operations even in worst case
    /// </summary>
    public class AVLTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class AVLNode
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public AVLNode Left { get; set; }
            public AVLNode Right { get; set; }
            public int Height { get; set; }
            public AVLNode(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Height = 1;
            }
        }
        private AVLNode root;
        public int Count { get; private set; }
        public AVLTree()
        {
            root = null;
            Count = 0;
        }
        /// <summary>
        /// Get height of a node
        /// </summary>
        private int GetHeight(AVLNode node)
        {
            return node == null ? 0 : node.Height;
        }
        /// <summary>
        /// Get balance factor of a node
        /// </summary>
        private int GetBalance(AVLNode node)
        {
            return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
        }
        /// <summary>
        /// Right rotation
        /// </summary>
        private AVLNode RotateRight(AVLNode y)
        {
            AVLNode x = y.Left;
            AVLNode T2 = x.Right;
            // Perform rotation
            x.Right = y;
            y.Left = T2;
            // Update heights
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            return x;
        }
        /// <summary>
        /// Left rotation
        /// </summary>
        private AVLNode RotateLeft(AVLNode x)
        {
            AVLNode y = x.Right;
            AVLNode T2 = y.Left;
            // Perform rotation
            y.Left = x;
            x.Right = T2;
            // Update heights
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            return y;
        }
        /// <summary>
        /// Insert a key-value pair and maintain AVL balance
        /// Time Complexity: O(log n) guaranteed
        /// </summary>
        public void Insert(TKey key, TValue value)
        {
            root = InsertRecursive(root, key, value);
            Count++;
        }
        private AVLNode InsertRecursive(AVLNode node, TKey key, TValue value)
        {
            // 1. Perform normal BST insertion
            if (node == null)
            {
                return new AVLNode(key, value);
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
                // Duplicate key, update value
                node.Value = value;
                return node;
            }
            // 2. Update height of current node
            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
            // 3. Get balance factor
            int balance = GetBalance(node);
            // 4. Balance the tree if needed
            // Left Left Case
            if (balance > 1 && key.CompareTo(node.Left.Key) < 0)
            {
                return RotateRight(node);
            }
            // Right Right Case
            if (balance < -1 && key.CompareTo(node.Right.Key) > 0)
            {
                return RotateLeft(node);
            }
            // Left Right Case
            if (balance > 1 && key.CompareTo(node.Left.Key) > 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }
            // Right Left Case
            if (balance < -1 && key.CompareTo(node.Right.Key) < 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }
            return node;
        }
        /// <summary>
        /// Search for a value by key
        /// Time Complexity: O(log n) guaranteed
        /// </summary>
        public TValue Search(TKey key)
        {
            AVLNode result = SearchRecursive(root, key);
            if (result == null)
            {
                throw new KeyNotFoundException($"Key {key} not found in AVL Tree");
            }
            return result.Value;
        }
        private AVLNode SearchRecursive(AVLNode node, TKey key)
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
        /// Check if a key exists
        /// </summary>
        public bool Contains(TKey key)
        {
            return SearchRecursive(root, key) != null;
        }
        /// <summary>
        /// Get all elements in sorted order
        /// </summary>
        public List<KeyValuePair<TKey, TValue>> GetAllInOrder()
        {
            List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>();
            InOrderTraversal(root, result);
            return result;
        }
        private void InOrderTraversal(AVLNode node, List<KeyValuePair<TKey, TValue>> result)
        {
            if (node == null) return;
            InOrderTraversal(node.Left, result);
            result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
            InOrderTraversal(node.Right, result);
        }
        /// <summary>
        /// Get height of the tree
        /// </summary>
        public int GetTreeHeight()
        {
            return GetHeight(root);
        }
        /// <summary>
        /// Clear the tree
        /// </summary>
        public void Clear()
        {
            root = null;
            Count = 0;
        }
    }
}