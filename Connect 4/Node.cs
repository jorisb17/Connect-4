namespace Connect_4;

public class Node
{
    public static int column = 7;

    public Node parent_node = null;

    public Node[] child_nodes = new Node[column];

    public int visits = 0;

    public int wins = 0;

    public int player;
    
    public int[] possible_moves = new int[column + 1];

    public void SetPossibleMoves(int[] possible_moves)
    {
        this.possible_moves = possible_moves;
    }

    public void AddChild(int[] possible_moves, int player, int move)
    {
        Node child;
        child = new Node();
        child.SetPossibleMoves(possible_moves);

        child_nodes[move] = child;

        child.parent_node = this;
    }

    public int GetRandomMove()
    {
        int[] remaining_moves = new int[column];
        int amount_moves = 0;
        int random_move;

        for (int i = 0; i < column; i++)
        {
            if (possible_moves[i] == 1)
            {
                remaining_moves[amount_moves] = i;
                amount_moves++;
            }
        }

        Random rand = new Random();
        random_move = remaining_moves[rand.Next() % amount_moves];
            
        possible_moves[random_move] = 0;
        possible_moves[7]--;

        return random_move;
    }
}