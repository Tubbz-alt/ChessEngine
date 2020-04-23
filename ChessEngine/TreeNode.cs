using System.Collections.Generic;

namespace ChessEngine
{
    class TreeNode<T>
    {
        public List<TreeNode<T>> Children = new List<TreeNode<T>>();

        public T Value { get; set; }
        public string Edge { get; set; }   

        public TreeNode(T nodeValue, string edgeValue)
        {
            Value = nodeValue;
            Edge = edgeValue;
        }

        public TreeNode<T> AddChild(T nodeValue, string edgeValue)
        {
            TreeNode<T> nodeItem = new TreeNode<T>(nodeValue, edgeValue);
            Children.Add(nodeItem);
            return nodeItem;
        }
    }
}
