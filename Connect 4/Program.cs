using System.Diagnostics;

namespace Connect_4;

public class Program
{
    
    static void Main(string[] args)
    {
        Connect4 game = new Connect4();
        game.PlayGame();
    }
}