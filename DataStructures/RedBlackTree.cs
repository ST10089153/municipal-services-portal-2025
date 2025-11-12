using System;
using System.Collections.Generic;
namespace MunicipalServicesReporting.DataStructures
{
    /// <summary>
    /// Custom Red-Black Tree implementation for balanced date-based indexing
    /// Used for efficient date-range queries of service requests
    /// </summary>
    public class RedBlackTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private enum NodeColor { Red, Black }
        private class RBNode
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public RBNode Left { get; set; }
            public RBNode Right { get; set; }
            public RBNode Parent { get; set; }
            public NodeColor Color { get; set; }
            public RBNode(TKey key, TValue value, NodeColor color = NodeColor.Red)
            {
                Key = key;
                Value = value;
                Color = color;
            }
        }
        private RBNode root;
        private readonly RBNode NIL; // Sentinel node
        public int Count { get; private set; }
        public RedBlackTree()
        {
            NIL = new RBNode(default(TKey), default(TValue), NodeColor.Black);
            root = NIL;
            Count = 0;
        }
        /// <summary>
        /// Insert a key-value pair into the Red-Black Tree
        /// Time Complexity: O(log n) guaranteed
        /// </summary>
        public void Insert(TKey key, TValue value)
        {
            RBNode newNode = new RBNode(key, value, NodeColor.Red);
            newNode.Left = NIL;
            newNode.Right = NIL;
            newNode.Parent = NIL;
            RBNode parent = NIL;
            RBNode current = root;
            // BST insertion
            while (current != NIL)
            {
                parent = current;
                int comparison = key.CompareTo(current.Key);
                if (comparison < 0)
                {
                    current = current.Left;
                }
                else if (comparison > 0)
                {
                    current = current.Right;
                }
                else
                {
                    // Update existing value
                    current.Value = value;
                    return;
                }
            }
            newNode.Parent = parent;
            if (parent == NIL)
            {
                root = newNode;
            }
            else if (key.CompareTo(parent.Key) < 0)
            {
                parent.Left = newNode;
            }
            else
            {
                parent.Right = newNode;
            }
            Count++;
            // Fix Red-Black Tree properties
            InsertFixup(newNode);
        }
        private void InsertFixup(RBNode node)
        {
            while (node.Parent.Color == NodeColor.Red)
            {
                if (node.Parent == node.Parent.Parent.Left)
                {
                    RBNode uncle = node.Parent.Parent.Right;
                    if (uncle.Color == NodeColor.Red)
                    {
                        // Case 1: Uncle is red
                        node.Parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        node.Parent.Parent.Color = NodeColor.Red;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Right)
                        {
                            // Case 2: Node is right child
                            node = node.Parent;
                            LeftRotate(node);
                        }
                        // Case 3: Node is left child
                        node.Parent.Color = NodeColor.Black;
                        node.Parent.Parent.Color = NodeColor.Red;
                        RightRotate(node.Parent.Parent);
                    }
                }
                else
                {
                    RBNode uncle = node.Parent.Parent.Left;
                    if (uncle.Color == NodeColor.Red)
                    {
                        node.Parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        node.Parent.Parent.Color = NodeColor.Red;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Left)
                        {
                            node = node.Parent;
                            RightRotate(node);
                        }
                        node.Parent.Color = NodeColor.Black;
                        node.Parent.Parent.Color = NodeColor.Red;
                        LeftRotate(node.Parent.Parent);
                    }
                }
            }
            root.Color = NodeColor.Black;
        }
        private void LeftRotate(RBNode x)
        {
            RBNode y = x.Right;
            x.Right = y.Left;
            if (y.Left != NIL)
            {
                y.Left.Parent = x;
            }
            y.Parent = x.Parent;
            if (x.Parent == NIL)
            {
                root = y;
            }
            else if (x == x.Parent.Left)
            {
                x.Parent.Left = y;
            }
            else
            {
                x.Parent.Right = y;
            }
            y.Left = x;
            x.Parent = y;
        }
        private void RightRotate(RBNode y)
        {
            RBNode x = y.Left;
            y.Left = x.Right;
            if (x.Right != NIL)
            {
                x.Right.Parent = y;
            }
            x.Parent = y.Parent;
            if (y.Parent == NIL)
            {
                root = x;
            }
            else if (y == y.Parent.Right)
            {
                y.Parent.Right = x;
            }
            else
            {
                y.Parent.Left = x;
            }
            x.Right = y;
            y.Parent = x;
        }
        /// <summary>
        /// Search for a value by key
        /// </summary>
        public TValue Search(TKey key)
        {
            RBNode result = SearchRecursive(root, key);
            if (result == NIL)
            {
                throw new KeyNotFoundException($"Key {key} not found");
            }
            return result.Value;
        }
        private RBNode SearchRecursive(RBNode node, TKey key)
        {
            if (node == NIL)
            {
                return NIL;
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
        /// Check if key exists
        /// </summary>
        public bool Contains(TKey key)
        {
            return SearchRecursive(root, key) != NIL;
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
        private void InOrderTraversal(RBNode node, List<KeyValuePair<TKey, TValue>> result)
        {
            if (node == NIL) return;
            InOrderTraversal(node.Left, result);
            result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
            InOrderTraversal(node.Right, result);
        }
        /// <summary>
        /// Get all keys in a range [minKey, maxKey]
        /// </summary>
        public List<TValue> GetRange(TKey minKey, TKey maxKey)
        {
            List<TValue> result = new List<TValue>();
            RangeSearch(root, minKey, maxKey, result);
            return result;
        }
        private void RangeSearch(RBNode node, TKey minKey, TKey maxKey, List<TValue>
        result)
        {
            if (node == NIL) return;
            int compareMin = node.Key.CompareTo(minKey);
            int compareMax = node.Key.CompareTo(maxKey);
            if (compareMin > 0)
            {
                RangeSearch(node.Left, minKey, maxKey, result);
            }
            if (compareMin >= 0 && compareMax <= 0)
            {
                result.Add(node.Value);
            }
            if (compareMax < 0)
            {
                RangeSearch(node.Right, minKey, maxKey, result);
            }
        }
        /// <summary>
        /// Clear the tree
        /// </summary>
        public void Clear()
        {
            root = NIL;
            Count = 0;
        }
    }
}