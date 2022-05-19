using System.Diagnostics;

namespace Connect_4;

public class Connect4
{  
    private double limit_millis = 3000;
    
    public int row = 6;

    public int column = 7;

    public int[,] array;

    public int[] possible_moves;

    public int player;

    private Random rand = new Random();
    private Stopwatch watch = new Stopwatch();

    public Connect4()
    {
        array = new int[row, column];
        possible_moves = new int[column + 1];
    }

    void InitGame()
    {
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < column; j++) {
                array[i, j] = 0;
            }
        }

        player = 1;
    }

    void PrintBoard()
    {
        Console.Write("\n \n C O N N E C T 4\n \n");
        for (int i = row - 1; i >= 0; i--) {
            for (int j = 0; j < column; j++) {
                Console.Write($"{array[i, j]}  ");
            }
            Console.Write("\n");
        }
        Console.Write("\n");
    }

    // create a list of possible moves and the total amount of them and return randomly one of them
    int GetPossibleMoves(int[,] array) {
        int[] remaining_moves = new int[column];
        int amount_moves = 0;
        int random_move = 0;

        // list available columns
        for (int j = 0; j < column; j++) {
            if (array[row - 1, j] == 0) {
                possible_moves[j] = 1;
                remaining_moves[amount_moves] = j;
                amount_moves++;
            } else {
                possible_moves[j] = 0;
            }
        }

        // set the total amount of possible moves
        possible_moves[7] = amount_moves;

        // chose a random number from from the list of available columns
        if (amount_moves > 0) {
            random_move = remaining_moves[rand.Next() % amount_moves];
        }

        return random_move;
    }


    // set the move in the next available slot
    int SetMove(int[,] array, int player, int move) {
        int position_row = 0;

        // check for free slot within the selected column
        for (int i = 0; i < row; i++) {
            if (array[i, move] == 0) {
                array[i, move] = player;
                position_row = i;
                break;
            }
        }

        // return the row in which the move has been set
        return position_row;
    }


    // check for game ending
    int CheckWin(int[,] array, int player, int move, int position_row) {
        int temp_column;
        int temp_position_row;

        // vertical check
        for (int i = 0; i < row - 3; i++) {
            if (array[i, move] == player && array[i + 1, move] == player && array[i + 2, move] == player &&
                array[i + 3, move] == player) {
                return player;
            }
        }

        // horizontal check
        for (int j = 0; j < column - 3; j++) {
            if (array[position_row, j] == player && array[position_row, j + 1] == player &&
                array[position_row, j + 2] == player &&
                array[position_row, j + 3] == player) {
                return player;
            }
        }

        // diagonal down right check
        if (move > position_row) {
            temp_column = move - position_row;
            temp_position_row = 0;
        } else {
            temp_column = 0;
            temp_position_row = position_row - move;
        }
        for (; temp_position_row < row - 3 && temp_column < column - 3; temp_position_row++, temp_column++) {
            if (array[temp_position_row, temp_column] == player &&
                array[temp_position_row + 1, temp_column + 1] == player &&
                array[temp_position_row + 2, temp_column + 2] == player &&
                array[temp_position_row + 3, temp_column + 3] == player) {
                return player;
            }
        }

        // diagonal down left check
        if (((column - 1) - move) > position_row) {
            temp_column = position_row + move;
            temp_position_row = 0;
        } else {
            temp_column = (column - 1);
            temp_position_row = position_row - ((column - 1) - move);
        }
        for (; temp_position_row < row - 3 && temp_column > 2; temp_position_row++, temp_column--) {
            if (array[temp_position_row, temp_column] == player &&
                array[temp_position_row + 1, temp_column - 1] == player &&
                array[temp_position_row + 2, temp_column - 2] == player &&
                array[temp_position_row + 3, temp_column - 3] == player) {
                return player;
            }
        }

        // tie
        if (array[row - 1, 0] != 0 && array[row - 1, 1] != 0 && array[row - 1, 2] != 0 &&
            array[row - 1, 3] != 0 && array[row - 1, 4] != 0 && array[row - 1, 5] != 0 &&
            array[row - 1, 6] != 0) {
            return 3;
        }

        // return 0 if no final game state found
        return 0;
    }


    // switch the actual player to the other player
    int SwitchPlayer(int player) {
        if (player == 1) {
            player = 2;
        } else {
            player = 1;
        }

        // return the switched player
        return player;
    }


    // simulate the game with random moves until someone wins
    int Simulate(int[,] array, int player, int move, int position_row) {
        // loop to set moves till a final game state
        while (true) {
            // check if game ending occurred
            if (CheckWin(array, player, move, position_row) != 0) {
                // return result of the final game state
                return CheckWin(array, player, move, position_row);
            }

            // switch the actual player
            player = SwitchPlayer(player);

            // make a random possible move
            move = GetPossibleMoves(array);
            position_row = SetMove(array, player, move);
        }
    }


    // select the best node calculated with the uct formula
    int UctSelect(Node node, Node root) {
        double uct_result;
        double best_value = -1000;
        int selected_node = 0;

        // iterate through the child nodes of the node
        for (int i = 0; i < column; i++) {

            // check if child node exists
            if (!object.ReferenceEquals(null, node.child_nodes[i])) {

                // calculate ucb1 result
                uct_result = (double) node.child_nodes[i].wins / (double) node.child_nodes[i].visits +
                             Math.Sqrt(2) * Math.Sqrt(Math.Log((double) root.visits) / (double) node.child_nodes[i].visits);

                // save the best
                if (uct_result > best_value) {
                    best_value = uct_result;
                    selected_node = i;
                }
            }
        }

        // return the selected node
        return selected_node;
    }


    // select the node with the most visits
    int VisitsSelect(Node node) {
        double best_value = -1000;
        int selected_node = 0;

        // iterate through the child nodes of the node
        for (int i = 0; i < column; i++) {

            // check if child node exists
            if (!object.ReferenceEquals(null, node.child_nodes[i])) {

                // save the best
                if (node.child_nodes[i].visits > best_value) {
                    best_value = node.child_nodes[i].visits;
                    selected_node = i;
                }
            }
        }

        // return the selected node
        return selected_node;
    }


    // update the nodes of the tree with the result of the simulation
    void Update(int win, Node node) {
        // iterate through all nodes of the used branch
        while (true) {
            // update the node either with a win or no win, depending on whose move it would be
            if (win == node.player) {
                node.wins = node.wins + 1;
            } else if (win != 0) {
                node.wins = node.wins - 1;
            }

            // increase visit for nodes of all nodes, of both player
            node.visits++;

            // select parent node
            if (!object.ReferenceEquals(null, node.parent_node)) {
                node = node.parent_node;
            } else {
                break;
            }
        }
    }


    // start the mcts algorithm to build a search tree to find a good move
    int RunMCTS() {
        int[,] temp_array = new int[row,column];
        int temp_player;
        int move = 0;
        int position_row = 0;
        int result;
        double delay;

        // counter for the time limit
        watch.Start();

        // copy the actual array and player to a temporary array for the simulations, get a list of all available moves
        temp_player = player;
        Array.Copy(array, temp_array, array.Length);
        GetPossibleMoves(array);

        // create a root node
        Node root = new Node();
        root.SetPossibleMoves(possible_moves);
        root.player = player;

        Node node = root;

        // create tree to keep track of all nodes to erase them at the end of all simulations
        Tree tree = new Tree();

        // iterate through the search tree to select, expand, simulate and update the nodes of it
        while (true) {

            // if there is a move which is not represented as child node left, expand it as child node
            if (node.possible_moves[7] > 0) {

                // get a random move
                move = node.GetRandomMove();

                // set the move inside the copy of the field
                position_row = SetMove(temp_array, temp_player, move);

                // get the possible moves for the state of the new child node
                GetPossibleMoves(temp_array);

                // expand the node by the child node appropriate to the possible move
                node.AddChild(possible_moves, temp_player, move);

                // add this node to the tree
                tree.AddNode(node.child_nodes[move]);

                // start the simulation upon the actual move
                result = Simulate(temp_array, temp_player, move, position_row);

                // update the node and all parent nodes by the result of the simulation
                Update(result, node.child_nodes[move]);

                // copy again the original state and set the node back to root
                temp_player = player;
                Array.Copy(array, temp_array, array.Length);
                node = root;

            } else {
                // check if it is a final state, a leaf node that can not have any child nodes to select next
                if (CheckWin(temp_array, SwitchPlayer(temp_player), move, position_row) != 0) {

                    // update the node and all parent nodes by the result of this final state
                    result = CheckWin(temp_array, SwitchPlayer(temp_player), move, position_row);
                    Update(result, node);

                    // copy again the original state and set the node back to root
                    temp_player = player;
                    Array.Copy(array, temp_array, array.Length);
                    node = root;

                } else {
                    // select the child node with the best ratio calculated with the uct formula
                    move = UctSelect(node, root);

                    // set the move inside the copy of the field and switch player
                    position_row = SetMove(temp_array, temp_player, move);
                    temp_player = SwitchPlayer(temp_player);

                    // select child node as next node to be processed
                    node = node.child_nodes[move];
                }
            }

            // calculate the time spent for simulations to ensure it stops if the time is up
            delay = watch.ElapsedMilliseconds;
            if (delay > limit_millis) {
                watch.Stop();
                watch.Reset();
                break;
            }
        }

        // print the total amount of simulations from this search
        Console.WriteLine($"Total amount of simulations MCTS: {root.visits}" );
        //cout << "Total amount of simulations MCTS: " << root_pt->visits << endl;

        // get the best move of the root node, according to the final action selection criteria
        move = VisitsSelect(root);

        // delete all nodes that were created during the simulation
        tree.DeleteNodes();

        // return best move
        return move;
    }

    // start the game
    public void PlayGame() {
        int move;
        int move_print;
        int position_row;
        int win;
        char game_restart;

        // erase the game field
        InitGame();

        // print the board at the beginning of the game
        PrintBoard();

        // game play
        while (true) {

            // text to show whose players turn it is
            Console.Write($"Player {player} select your column: \n");

            // player selection
            if (player == 1) {
                while (true) {
                    // get human move
                    var input = Console.ReadLine();
                    if (!int.TryParse(input, out move))
                    {
                        Console.Write("You have entered invalid number. Please select a valid column: \n");
                    }
                    else
                    {
                        // check for legal move - if column number exist or if it is full already
                        if (move < 1 || move > column || array[row - 1, move - 1] != 0) {
                            Console.Write("Invalid column, please select a valid column:\n");
                        } else {
                            move--;
                            break;
                        }
                    }
                }
            } else {
                // run mcs/mcts algorithm to approximate the best move for the computer player
                move = RunMCTS();

                // print move
                move_print = move + 1;
                Console.WriteLine(move_print);
            }

            // set the move in the next available slot
            position_row = SetMove(array, player, move);

            // print board
            PrintBoard();

            // check for game ending
            win = CheckWin(array, player, move, position_row);

            // end game if final game state reached, otherwise switch player
            if (win != 0) {
                if (win == 3) {
                    Console.WriteLine("Tie. Nobody won.");
                } else {
                    Console.WriteLine($"Player {player} won.");
                }

                // game restart
                Console.WriteLine("\n New game? [Y/N]");
                game_restart = Console.ReadLine()[0];
                if (game_restart == 'y' || game_restart == 'Y') {
                    // erase the game field and print the empty board
                    InitGame();
				    // new game message
                    Console.Write("\n \nNEW GAME");
                    PrintBoard();
                } else {
                    Console.WriteLine("Take care, bye.");
                    break;
                }
            } else {
                // switch the actual player
                player = SwitchPlayer(player);
            }
        }
    }
}