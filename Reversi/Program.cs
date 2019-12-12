using System;


namespace Reversi
{
    class Program
    {
        public static void Main(string[] args)
        {
            Terminal terminalInstance = new Terminal();

            while(true) {
                String[] playerNames = terminalInstance.playerNames();                   // Request Player Names
                int currentPlayerIndex = 0;
                int otherPlayerIndex = 1;

                Board boardInstance = new Board();
                boardInstance.initializeBoard();                                         //Create Board
                boardInstance.printBoard();

                int[] rowColumn = new int[2];

                while (true)
                {                                                                        //implement error checking
                    terminalInstance.getMove(rowColumn, currentPlayerIndex);             //get move

                    if (boardInstance.validateMove(rowColumn[0], rowColumn[1], currentPlayerIndex, otherPlayerIndex))
                    {       //Error check move
                        boardInstance.updateBoard(rowColumn[0], rowColumn[1], currentPlayerIndex, otherPlayerIndex);          //Update Board
                        boardInstance.printBoard();
                    }
                    else
                    {
                        System.Console.WriteLine("Error, no valid position found.");
                        continue;
                    }

                    if (boardInstance.hasValidMove(otherPlayerIndex, currentPlayerIndex) == true)
                    {      //If other player has valid move, switch players & continue
                        currentPlayerIndex = (currentPlayerIndex + 1) % 2;
                        otherPlayerIndex = (otherPlayerIndex + 1) % 2;
                        continue;
                    }
                    else if (boardInstance.hasValidMove(currentPlayerIndex, otherPlayerIndex) == true)
                    {    //If current player has valid move
                        System.Console.WriteLine("No valid move for " + playerNames[otherPlayerIndex] + "; turn is skipped.");
                        continue;
                    }
                    else
                    {
                        System.Console.WriteLine("No remaining moves. Game Over.");
                        break;
                    }
                }

                boardInstance.gameOver(playerNames);

                if (!terminalInstance.playAgain())
                {
                    break;
                }
            } //End game loop
        }
    }

    // *********************************************************************************************
    // *********************************************************************************************

    class Terminal
    {
        String[] players = { "Player1", "Player2" };

        public String[] playerNames()
        {
            // Output board, and retreive playerNames names.
            System.Console.WriteLine("\nWelcome to a game of Reversi/Othello. Please enter player names.");
            System.Console.Write("Player 1: ");

            players[0] = System.Console.ReadLine();
            //players[0] = var.Parse(System.Console.ReadLine());
            System.Console.Write("Player 2: ");
            players[1] = System.Console.ReadLine();

            System.Console.Write(players[0] + " is X.\n");
            System.Console.Write(players[1] + " is O.");

            System.Console.WriteLine();
            return players;
        }

        private bool isValidNumber(char number)
        {
            return number >= '1' && number <= '8';
        }

        private bool isValidCharacter(char letter)
        {
            letter = char.ToLower(letter);
            return letter >= 'a' && letter <= 'h';
        }

        public void getMove(int[] rowColumn, int currentPlayerIndex)
        {
            String stringInput;

            int row = 0;
            int column = 0;

            while (true)
            {
                System.Console.WriteLine(players[currentPlayerIndex] + "'s turn. Enter a board position:"); //" Represented as a Letter/Number combination.");
                stringInput = System.Console.ReadLine();

                if (stringInput.Length != 2)                  // Check for approprate length
                {               
                    stringInput = "00";                       // Force invalid answer, divert to error message
                }

                char a = stringInput[0];
                char b = stringInput[1];

                if ((isValidNumber(a) && isValidCharacter(b)) || (isValidCharacter(a) && isValidNumber(b)))
                {
                    if (isValidCharacter(b))
                    {
                        row = a - '1';
                        column = char.ToLower(b) - 'a';
                        break;
                    }
                    else if (isValidCharacter(a))
                    {
                        row = b - '1';
                        column = char.ToLower(a) - 'a';
                        break;
                    }
                    else
                    {
                        System.Console.WriteLine("Error, getMove failed.");
                        break;
                    }
                }
                else
                {
                    System.Console.WriteLine("Invalid input.  Enter a single letter (A-H) and number (1-8) combination.");
                    System.Console.WriteLine("For example: 1A.");
                }
            }
            rowColumn[0] = row;
            rowColumn[1] = column;
        }

        public bool playAgain()
        {
            System.Console.Write("Would you like to play again? If so, type 'yes': \n");
            String response = System.Console.ReadLine().ToLower();

            return response.Equals("yes");
        }
    }

    // *********************************************************************************************
    // *********************************************************************************************

    class Board
    {
        Char [,] grid = new char[8,8];                  // = {1, 2, 3, 4, 5, 6, 7, 8};
        static Char [] playerSymbol = { 'X', 'O' };     //symbols for player 1, player 2

        public void initializeBoard() {
            // Board board setup
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    grid[i,j] = '-';
                }
            }

            // Default Placement
            grid[3,3] = grid[4,4] = 'X';
            grid[3,4] = grid[4,3] = 'O';

            //For Testing Only, uncomment or edit as needed
            //grid[5,2] = grid [2,5] = 'O';     
            //grid[4,5] = grid[4,6] = 'O';
            //grid[6,1] = grid[1,1] = grid[6,6] = grid[1,6] = 'O';
            //grid[3,4] = grid [6,6] = 'O';
        }

        public void printBoard() {
            System.Console.WriteLine();
            System.Console.WriteLine("  a b c d e f g h  ");

            for (int i = 0; i < 8; i++) {
                System.Console.Write((i + 1) + " ");

                for (int j = 0; j < 8; j++) {
                    System.Console.Write(grid[i,j] + " ");
                }
                System.Console.WriteLine(i + 1);
            }
            System.Console.WriteLine("  a b c d e f g h  \n");
        }

        public bool validateMove(int row, int column, int currentPlayerIndex, int otherPlayerIndex)
        {

            char currentPlayer = playerSymbol[currentPlayerIndex];
            char otherPlayer = playerSymbol[otherPlayerIndex];

            if (grid[row,column] != '-')
            {
                System.Console.WriteLine("Invalid placement.  Pieces may only be placed on empty locations.");
                return false;
            }

            //Down (South)
            if ((row + 1 < 7) && (grid[row + 1,column] == otherPlayer))
            {

                for (int i = 1; (grid[row + i,column] == otherPlayer) && (row + i <= 6);)
                {
                    ++i;
                    if (grid[row + i,column] == currentPlayer)
                    {
                        return true;
                    }
                }
            }
            //Up (North)
            if ((row - 1 > 0) && (grid[row - 1,column] == otherPlayer))
            {

                for (int i = 1; (grid[row - i,column] == otherPlayer) && (row - i >= 1);)
                {
                    ++i;
                    if (grid[row - i,column] == currentPlayer)
                    {
                        return true;
                    }
                }
            }
            //Right (East)
            if ((column + 1 < 7) && (grid[row,column + 1] == otherPlayer))
            {

                for (int i = 1; (grid[row,column + i] == otherPlayer) && (column + i <= 6);)
                {
                    ++i;
                    if (grid[row,column + i] == currentPlayer)
                    {
                        return true;
                    }
                }
            }
            //Left (West)
            if ((column - 1 > 0) && (grid[row,column - 1] == otherPlayer))
            {

                for (int i = 1; (grid[row,column - i] == otherPlayer) && (column - i >= 1);)
                {
                    ++i;
                    if (grid[row,column - i] == currentPlayer)
                    {
                        return true;
                    }
                }
            }
            // (South-East)
            if ((row + 1 < 7) && (column + 1 < 7) && (grid[row + 1,column + 1] == otherPlayer))
            {

                for (int i = 1; (grid[row + i,column + i] == otherPlayer) && (row + i <= 6) && (column + i <= 6);)
                {
                    ++i;
                    if (grid[row + i,column + i] == currentPlayer)
                    {
                        return true;
                    }
                }
            }
            // (North-West)
            if ((row - 1 > 0) && (column - 1 > 0) && (grid[row - 1,column - 1] == otherPlayer))
            {

                for (int i = 1; (grid[row - i,column - i] == otherPlayer) && (row - i > 0) && (column - i > 0);)
                {
                    ++i;
                    if (grid[row - i,column - i] == currentPlayer)
                    {
                        return true;
                    }
                }
            }
            // (South-West)
            if ((row + 1 < 7) && (column - 1 > 0) && (grid[row + 1,column - 1] == otherPlayer))
            {

                for (int i = 1; (grid[row + i,column - i] == otherPlayer) && (row + i <= 6) && (column - i >= 1);)
                {
                    ++i;
                    if (grid[row + i,column - i] == currentPlayer)
                    {
                        return true;
                    }
                }
            }
            // (North-East)
            if ((row - 1 > 0) && (column + 1 < 7) && (grid[row - 1,column + 1] == otherPlayer))
            {

                for (int i = 1; (grid[row - i,column + i] == otherPlayer) && (row - i >= 1) && (column + i <= 6);)
                {
                    ++i;
                    if (grid[row - i,column + i] == currentPlayer)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void updateBoard(int row, int column, int currentPlayerIndex, int otherPlayerIndex)
        {
            char currentPlayer = playerSymbol[currentPlayerIndex];
            char otherPlayer = playerSymbol[otherPlayerIndex];

            if ((row < 0) && (row > 7) && (column < 0) && (column > 7))
            {
                System.Console.WriteLine("Error in updating the board.");
                return;
            }

            grid[row,column] = currentPlayer;

            //Down (South)
            if ((row + 1 < 7) && (grid[row + 1,column] == otherPlayer))
            {

                for (int i = 1; (grid[row + i,column] == otherPlayer) && (row + i <= 6);)
                {
                    ++i;
                    if (grid[row + i,column] == currentPlayer)
                    {
                        for (int j = 1; (grid[row + j,column] == otherPlayer) && (row + j <= 6); j++)
                        {
                            grid[row + j,column] = currentPlayer;
                        }
                        break;
                    }
                }
            }
            //Up (North)
            if ((row - 1 > 0) && (grid[row - 1,column] == otherPlayer))
            {

                for (int i = 1; (grid[row - i,column] == otherPlayer) && (row - i >= 1);)
                {
                    ++i;
                    if (grid[row - i,column] == currentPlayer)
                    {
                        for (int j = 1; (grid[row - j,column] == otherPlayer) && (row - j >= 1); j++)
                        {
                            grid[row - j,column] = currentPlayer;
                        }
                        break;
                    }
                }
            }
            //Right (East)
            if ((column + 1 < 7) && (grid[row,column + 1] == otherPlayer))
            {

                for (int i = 1; (grid[row,column + i] == otherPlayer) && (column + i <= 6);)
                {
                    ++i;
                    if (grid[row,column + i] == currentPlayer)
                    {
                        for (int j = 1; (grid[row,column + j] == otherPlayer) && (column + j <= 6); j++)
                        {
                            grid[row,column + j] = currentPlayer;
                        }
                        break;
                    }
                }
            }
            //Left (West)
            if ((column - 1 > 0) && (grid[row,column - 1] == otherPlayer))
            {

                for (int i = 1; (grid[row,column - i] == otherPlayer) && (column - i >= 1);)
                {
                    ++i;
                    if (grid[row,column - i] == currentPlayer)
                    {
                        for (int j = 1; (grid[row,column - j] == otherPlayer) && (column - j >= 1); j++)
                        {
                            grid[row,column - j] = currentPlayer;
                        }
                        break;
                    }
                }
            }
            // (South-East)
            if ((row + 1 < 7) && (column + 1 < 7) && (grid[row + 1,column + 1] == otherPlayer))
            {

                for (int i = 1; (grid[row + i,column + i] == otherPlayer) && (row + i <= 6) && (column + i <= 6);)
                {
                    ++i;
                    if (grid[row + i,column + i] == currentPlayer)
                    {
                        for (int j = 1; (grid[row + j,column + j] == otherPlayer) && (row + j <= 6) && (column + j <= 6); j++)
                        {
                            grid[row + j,column + j] = currentPlayer;
                        }
                        break;
                    }
                }
            }
            // (North-West)
            if ((row - 1 > 0) && (column - 1 > 0) && (grid[row - 1,column - 1] == otherPlayer))
            {

                for (int i = 1; (grid[row - i,column - i] == otherPlayer) && (row - i > 0) && (column - i > 0);)
                {
                    ++i;
                    if (grid[row - i,column - i] == currentPlayer)
                    {
                        for (int j = 1; (grid[row - j,column - j] == otherPlayer) && (row - j <= 6) && (column - j <= 6); j++)
                        {
                            grid[row - j,column - j] = currentPlayer;
                        }
                        break;
                    }
                }
            }
            // (South-West)
            if ((row + 1 < 7) && (column - 1 > 0) && (grid[row + 1,column - 1] == otherPlayer))
            {

                for (int i = 1; (grid[row + i,column - i] == otherPlayer) && (row + i <= 6) && (column - i >= 1);)
                {
                    ++i;
                    if (grid[row + i,column - i] == currentPlayer)
                    {
                        for (int j = 1; (grid[row + j,column - j] == otherPlayer) && (row + j <= 6) && (column - j >= 1); j++)
                        {
                            grid[row + j,column - j] = currentPlayer;
                        }
                        break;
                    }
                }
            }
            // (North-East)
            if ((row - 1 > 0) && (column + 1 < 7) && (grid[row - 1,column + 1] == otherPlayer))
            {

                for (int i = 1; (grid[row - i,column + i] == otherPlayer) && (row - i >= 1) && (column + i <= 6);)
                {
                    ++i;
                    if (grid[row - i,column + i] == currentPlayer)
                    {
                        for (int j = 1; (grid[row - j,column + j] == otherPlayer) && (row - j >= 1) && (column + j <= 6); j++)
                        {
                            grid[row - j,column + j] = currentPlayer;
                        }
                        break;
                    }
                }
            }
        }

        public bool hasValidMove(int currentPlayerIndex, int otherPlayerIndex)
        {
            for (int i = 0; i < 8; i++)
            {

                for (int j = 0; j < 8; j++)
                {
                    if (grid[i,j] == '-')
                    {
                        if (validateMove(i, j, currentPlayerIndex, otherPlayerIndex))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void gameOver(String[] playerNames)
        {
            int count1 = 0;
            int count2 = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (grid[i,j] == 'X')
                    {
                        count1++;
                    }
                    if (grid[i,j] == 'O')
                    {
                        count2++;
                    }
                }
            }

            if (count1 < count2)
            {
                System.Console.WriteLine(playerNames[0] + " wins.");
            }
            if (count1 > count2)
            {
                System.Console.WriteLine(playerNames[1] + " wins.");
            }
            if (count1 == count2)
            {
                System.Console.WriteLine("The game is a tie.");
            }
            System.Console.WriteLine(playerNames[0] + " has " + count1 + " pieces.");
            System.Console.WriteLine(playerNames[1] + " has " + count2 + " pieces.");
        }
    }
}

