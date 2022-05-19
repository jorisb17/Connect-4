namespace Connect_4;

public class Tree
{
   public int amount_nodes = 0;

   public Node[] nodes = new Node[400000];

   public void AddNode(Node node)
   {
      nodes[amount_nodes] = node;
      amount_nodes++;
   }

   public void DeleteNodes()
   {
      Array.Clear(nodes);
   }
}