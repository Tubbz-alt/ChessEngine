using System.Collections.Generic;

namespace ChessEngine
{
    class TreeNode<T>
    {
        List<TreeNode<T>> Children = new List<TreeNode<T>>();

        T node { get; set; }
        string edge { get; set; }   

        public TreeNode(T nodeValue, string edgeValue)
        {
            node = nodeValue;
            edge = edgeValue;
        }

        public TreeNode<T> AddChild(T nodeValue, string edgeValue)
        {
            TreeNode<T> nodeItem = new TreeNode<T>(nodeValue, edgeValue);
            Children.Add(nodeItem);
            return nodeItem;
        }
    }
}
