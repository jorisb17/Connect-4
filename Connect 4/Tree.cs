namespace Connect_4;

public class Tree
{
   public int amount_nodes = 0;

   public List<Node> nodes = new List<Node>();

   public void AddNode(Node node)
   {
      nodes.Add(node);
      amount_nodes++;
   }

   public void DeleteNodes()
   {
      nodes.Clear();
   }
}