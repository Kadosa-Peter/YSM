using System.Collections.Generic;
using Ysm.Models;

namespace Ysm.Assets
{
    public static class Move
    {
        public static Node Root;

        // 0 = channel 1 = category
        public static Dictionary<string, int> Nodes;

        static Move()
        {
            Nodes = new Dictionary<string, int>();
        }

        public static bool NotEmpty => Nodes.Count > 0;

        public static void Clear()
        {
            foreach (KeyValuePair<string, int> kvp in Nodes)
            {
                Node node = Root.Find(kvp.Key);

                if (node != null)
                    node.Fade = false;
            }

            Nodes.Clear();
        }

        public static void Extract(List<Node> selectedNodes)
        {
            foreach (Node node in selectedNodes)
            {
                Nodes.Remove(node.Id);
            }
        }

        public static void Add(List<Node> selectedNodes)
        {
            foreach (Node node in selectedNodes)
            {
                node.Fade = true;

                if (node.NodeType == NodeType.Channel)
                {
                    Nodes.Add(node.Id, 0);
                }
                else
                {
                    Nodes.Add(node.Id, 1);
                }
            }
        }

        public static bool Contains(string id)
        {
            if (Nodes.Count == 0)
                return false;

            return Nodes.ContainsKey(id);
        }
    }
}
