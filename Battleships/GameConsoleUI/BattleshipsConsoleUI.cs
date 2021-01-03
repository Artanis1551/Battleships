using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameBrain;
using System.Runtime.Serialization.Json;
using System.Runtime.CompilerServices;
using System.IO;
using DAL;
using Domain;

namespace GameConsoleUI
{
    public enum GAME_TYPE
    {
        HU_VS_HU,
        HU_VS_AI,
        AI_VS_AI
    }

    public static class BattleshipsConsoleUI
    {
        #region Configuration
        public static AppDbContext? db = null!;

        // Keyboard Modifier
        private static readonly ConsoleModifiers CTRL = ConsoleModifiers.Control;
        // Keys used with Console Modifier "Ctrl"
        private static readonly ConsoleKey SAVE = ConsoleKey.S;             // Save game
        private static readonly ConsoleKey UNDO = ConsoleKey.Z;             // Undo move
        private static readonly ConsoleKey EXIT = ConsoleKey.X;             // Exit game
        private static readonly ConsoleKey NEW = ConsoleKey.N;              // New game

        // Navigation of the table
        private static readonly ConsoleKey ENTER = ConsoleKey.Enter;        // Select cell
        private static readonly ConsoleKey LEFT = ConsoleKey.LeftArrow;     // Move left
        private static readonly ConsoleKey RIGHT = ConsoleKey.RightArrow;   // Move right
        private static readonly ConsoleKey UP = ConsoleKey.UpArrow;         // Move up 
        private static readonly ConsoleKey DOWN = ConsoleKey.DownArrow;     // Move down
        private static readonly ConsoleKey ENTER_ALT = ConsoleKey.Spacebar; // Select cell
        private static readonly ConsoleKey LEFT_ALT = ConsoleKey.A;         // Move left
        private static readonly ConsoleKey RIGHT_ALT = ConsoleKey.D;        // Move right
        private static readonly ConsoleKey UP_ALT = ConsoleKey.W;           // Move up 
        private static readonly ConsoleKey DOWN_ALT = ConsoleKey.S;         // Move down

        // Table styling
        private static readonly string COL_SEP = "|";                       // Board Collumn separator;
        private static readonly string ROW_SEP = "---";                     // Board Row separator;
        private static readonly string LINE_INDENT = "   ";                 // Board Line indentation;
        private static readonly string MOCK_CHAR = "X";                     // Mock table filler;
        private static readonly string CROSS_POINT = "+";                   // Cross point character;
        private static string CLEAR_LINE = "";                              // Clear Line Buffer;

        #endregion Configuration

        // Console State
        private static int ConsoleWidth;
        private static int ConsoleHeight;
        private static bool resized = false;
        private static bool restart = true;

        // Runtime UI positions
        private static int firstCellRow;
        private static int firstCellCol;
        private static int lastCellRow;
        private static int lastCellCol;
        private static int BoardRow;
        private static int lastActionRow;
        private static int lastLogRow;
        private static int playerRow;
                
        private static bool hasOutcome;
        private static string? lastAction = null;
        private static CellState[,] battleBoard = new CellState[1, 1];
        private static ShipCell?[,] personalBoard = new ShipCell[1, 1];
        private static Battleships game = new Battleships();

        // Player Controllers
        private static Func<ConsoleKeyInfo> PLAYER1_CONTROLLER = () => { return new ConsoleKeyInfo((char)ENTER, ENTER, false, false, false); };
        private static Func<ConsoleKeyInfo> PLAYER2_CONTROLLER = () => { return new ConsoleKeyInfo((char)ENTER, ENTER, false, false, false); };

        // Player Names
        private static string PLAYER1 = "Player_1";             // First Player Name
        private static string PLAYER2 = "Player_2";             // Second Player Name

        public static void RunConsoleGame(Setting? setting, GameSave? save = null, GAME_TYPE gameType = GAME_TYPE.HU_VS_HU)
        {
            try
            {
                hasOutcome = false;
                lastAction = null;
                if (setting == null)
                    setting = new Setting();
                if (gameType == GAME_TYPE.HU_VS_HU)
                {
                    PLAYER1_CONTROLLER = Selector;
                    PLAYER2_CONTROLLER = Selector;
                }
                else if (gameType == GAME_TYPE.HU_VS_AI)
                {
                    PLAYER1_CONTROLLER = Selector;
                    PLAYER2_CONTROLLER = AISelector;
                }
                if (save == null)
                {
                    restart = true;
                    game = new Battleships(setting);
                }
                else
                {
                    restart = false;
                    game = new Battleships(save);
                }
                PLAYER1 = game.Player_1;
                PLAYER2 = game.Player_2;
                bool loop = true;
                ConsoleKeyInfo pressedKey;
                Init();
                if (save == null)
                {
                    if (setting.PlaceType == PlaceShips.Select)
                        SetShips();
                    else
                        game.PlaceRandom();
                    Console.SetCursorPosition(0, BoardRow);
                    DrawBoard();
                    Console.SetCursorPosition(firstCellCol, firstCellRow);
                    System.Threading.Thread.Sleep(100);
                    if (setting.PlaceType == PlaceShips.Select && gameType == GAME_TYPE.HU_VS_HU)
                        SetShips();
                    else
                        game.PlaceRandom();
                    Console.SetCursorPosition(firstCellCol, firstCellRow);
                }
                Console.SetCursorPosition(0, BoardRow);
                DrawBoard();
                Console.SetCursorPosition(firstCellCol, firstCellRow);
                do
                {
                    if (hasOutcome)
                        pressedKey = Selector();
                    else
                        pressedKey = !game.Turn ? PLAYER1_CONTROLLER() : PLAYER2_CONTROLLER();
                    if (pressedKey.Modifiers == CTRL)
                    {
                        if (pressedKey.Key == EXIT)
                            loop = false;
                        if (pressedKey.Key == NEW)
                        {
                            restart = true;
                            Init();
                            if (setting.PlaceType == PlaceShips.Select)
                                SetShips();
                            else
                                game.PlaceRandom();
                            Console.SetCursorPosition(0, BoardRow);
                            DrawBoard();
                            Console.SetCursorPosition(firstCellCol, firstCellRow);
                            System.Threading.Thread.Sleep(100);
                            if (setting.PlaceType == PlaceShips.Select && gameType == GAME_TYPE.HU_VS_HU)
                                SetShips();
                            else
                                game.PlaceRandom();
                            Console.SetCursorPosition(firstCellCol, firstCellRow);
                            Console.SetCursorPosition(0, BoardRow);
                            DrawBoard();
                            Console.SetCursorPosition(firstCellCol, firstCellRow);

                        }
                        //if (pressedKey.Key == UNDO)
                        //    UpdateGameState(false);
                        if (pressedKey.Key == SAVE && gameType == GAME_TYPE.HU_VS_HU)
                            SaveGame();
                    }
                    else if (pressedKey.Key == ENTER || pressedKey.Key == ENTER_ALT)
                        UpdateGameState(true, (Console.CursorTop - firstCellRow) / 2, (Console.CursorLeft - firstCellCol) / (ROW_SEP.Length + CROSS_POINT.Length));
                } while (loop);

                Console.Clear();
            }
            catch (Exception)
            {
                return;
            }
        }

        #region Methods

        private static bool SetShips()
        {
            bool loop = true;
            ConsoleKeyInfo pressedKey;
            foreach (var ship in game.ShipsReq)
            {
                for (int i = 0; i < ship.BoatCount; i++)
                {
                    PostLastAction($"Place your {ship.BoatName} of {ship.BoatLength} length");
                    loop = true;
                    int sX = -1;
                    int sY = -1;
                    int eX = -1;
                    int eY = -1;
                    do
                    {
                        if (hasOutcome)
                            pressedKey = Selector();
                        else
                            pressedKey = !game.Turn ? PLAYER1_CONTROLLER() : PLAYER2_CONTROLLER();
                        //if (pressedKey.Modifiers == CTRL)
                        //{
                        //    if (pressedKey.Key == EXIT)
                        //        loop = false;
                        //    //if (pressedKey.Key == UNDO)
                        //    //    UpdateGameState(false);
                        //    if (pressedKey.Key == SAVE)
                        //        SaveGame();
                        //}
                        /*else */
                        if (pressedKey.Key == ENTER || pressedKey.Key == ENTER_ALT)
                        {
                            if (sX == -1)
                            {
                                sX = (Console.CursorTop - firstCellRow) / 2;
                                sY = (Console.CursorLeft - firstCellCol) / (ROW_SEP.Length + CROSS_POINT.Length);
                            }
                            else if (eX == -1)
                            {
                                eX = (Console.CursorTop - firstCellRow) / 2;
                                eY = (Console.CursorLeft - firstCellCol) / (ROW_SEP.Length + CROSS_POINT.Length);
                                if ((eX == sX || eY == sY) && Math.Abs(sX - eX) + 1 + Math.Abs(sY - eY) == ship.BoatLength)
                                {
                                    if (game.PlaceShip(sX, sY, eX, eY))
                                        loop = false;
                                    else
                                    {
                                        sX = -1;
                                        sY = -1;
                                        eX = -1;
                                        eY = -1;
                                    }
                                    int row = Console.CursorTop;
                                    int col = Console.CursorLeft;
                                    Console.SetCursorPosition(0, BoardRow);
                                    DrawBoard();
                                    Console.SetCursorPosition(col, row);
                                }
                                else
                                {
                                    sX = -1;
                                    sY = -1;
                                    eX = -1;
                                    eY = -1;
                                }
                            }
                        }
                    } while (loop);
                }
            }
            game.Turn = !game.Turn;
            return true;
        }

        private static void Init()
        {
            if (game == null)
                game = new Battleships();
            if (!resized && restart)
            {
                game.Init();
                System.Threading.Thread.Sleep(100);
                hasOutcome = false;
                lastAction = null;
                restart = false;
            }
            Console.Clear();

            CLEAR_LINE = "".PadRight(Console.BufferWidth);

            #region Header
            Console.WriteLine($"\n Save game: Ctrl+{SAVE};    Exit Game: Ctrl+{EXIT};    New Game: Ctrl+{NEW}\n" +
                        $"\nSelect cell: {ENTER}/{ENTER_ALT};    Move Down: {DOWN}/{DOWN_ALT};    Move Up: {UP}/{UP_ALT};    Move Left: {LEFT}/{LEFT_ALT};    Move Right: {RIGHT}/{RIGHT_ALT};\n\n");
            playerRow = Console.CursorTop;
            WriteLine($" {(!game.Turn ? PLAYER1 : PLAYER2)}'s turn\n", ConsoleColor.Cyan);
            #endregion Header

            #region Board
            BoardRow = Console.CursorTop;
            DrawBoard();
            #endregion Board

            #region Last action
            WriteLine("\n Last action:", ConsoleColor.Yellow);
            lastActionRow = Console.CursorTop;
            if (lastAction != null)
                PostLastAction(lastAction ?? "\n");
            #endregion Last action

            #region Game Log
            WriteLine("\n\n Game Log:", ConsoleColor.Green);
            List<GameMove> log = game.GetLog();
            foreach (var move in log)
                WriteLine($"{(!move.turn ? PLAYER1 : PLAYER2)} {move}");
            lastLogRow = Console.CursorTop;
            #endregion Game Log

            ConsoleWidth = Console.WindowWidth;
            ConsoleHeight = Console.WindowHeight;
            resized = false;
            restart = false;

            Console.SetCursorPosition(firstCellCol, firstCellRow);
        }

        /// <summary>
        /// Makes or erases a game move
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="action">false if you want to erase a move and true if you want to make one</param>
        private static void UpdateGameState(bool action, int x = -1, int y = -1)
        {
            if (game == null)
                return;
            GameMove? lastMove = action ? game.MakeMove(x, y) : game.UndoMove();
            if (lastMove == null)
                return;
            lastAction = $"{(action ? "" : "Undone: ")}{((action ? game.Turn : !game.Turn) ? PLAYER1 : PLAYER2)} {lastMove}";

            int lastRow = Console.CursorTop;
            int lastCol = Console.CursorLeft;

            if (lastMove.res == CellState.SHOT_EMPTY)
            {
                Console.SetCursorPosition(0, BoardRow);
                DrawBoard(true);
                PostLastAction("Press Any Key to start next players turn.");
                Console.ReadKey(true);
            }

            Console.SetCursorPosition(0, playerRow);
            WriteLine(CLEAR_LINE);
            Console.SetCursorPosition(0, playerRow);
            WriteLine($"{(game.Turn ? PLAYER2 : PLAYER1)}'s turn\n", ConsoleColor.Cyan);

            Console.SetCursorPosition(0, BoardRow);
            DrawBoard();

            PostLastAction(lastAction);

            Console.SetCursorPosition(0, lastLogRow);
            if (action)
            {
                WriteLine($"{(game.Turn ? PLAYER1 : PLAYER2)} {lastMove}");
                lastLogRow++;
            }
            else
            {
                Console.CursorTop--;
                WriteLine(CLEAR_LINE ?? "");
                lastLogRow--;
            }

            if (game.Check)
            {
                PostLastAction($"{(game.Turn ? PLAYER2 : PLAYER1)} has Won!!! Congratulations!!!!!");
                hasOutcome = true;
            }
            if (!action)
                hasOutcome = false;

            Console.SetCursorPosition(0, 0);
            Console.SetCursorPosition(lastCol, lastRow);
        }

        private static void DrawBoard(bool mock = false)
        {
            battleBoard = game.GetBattleBoard();
            personalBoard = game.GetPersonalBoard();

            if (personalBoard == null)
                personalBoard = new ShipCell[1, 1];

            int height = battleBoard.GetUpperBound(0) + 1;
            int width = battleBoard.GetUpperBound(1) + 1;

            Console.Write(LINE_INDENT + "  ");
            for (int colIndex = 0; colIndex < width; colIndex++)
                Console.Write($" {(char)('A' + colIndex)}  ");
            Console.Write(LINE_INDENT + "  ");
            for (int colIndex = 0; colIndex < width; colIndex++)
                Console.Write($" {(char)('A' + colIndex)}  ");
            Console.WriteLine();
            Console.Write(LINE_INDENT + " " + CROSS_POINT);
            firstCellRow = Console.CursorTop + 1;
            firstCellCol = Console.CursorLeft + ROW_SEP.Length / 2;
            for (int colIndex = 0; colIndex < width; colIndex++)
                Console.Write(ROW_SEP + CROSS_POINT);
            lastCellCol = Console.CursorLeft - ROW_SEP.Length / 2 - 2;
            Console.Write(LINE_INDENT + " " + CROSS_POINT);
            for (int colIndex = 0; colIndex < width; colIndex++)
                Console.Write(ROW_SEP + CROSS_POINT);
            Console.WriteLine();
            for (int rowIndex = 0; rowIndex < height; rowIndex++)
            {
                for (int colIndex = 0; colIndex < LINE_INDENT.Length - $"{rowIndex + 1}".Length + 1; colIndex++)
                    Console.Write(LINE_INDENT[0]);
                Console.Write($"{rowIndex + 1}{COL_SEP}");
                for (int colIndex = 0; colIndex < width; colIndex++)
                    Console.Write($" {(mock ? MOCK_CHAR : CellString(battleBoard[rowIndex, colIndex]))} {COL_SEP}");
                for (int colIndex = 0; colIndex < LINE_INDENT.Length - $"{rowIndex + 1}".Length + 1; colIndex++)
                    Console.Write(LINE_INDENT[0]);
                Console.Write($"{rowIndex + 1}{COL_SEP}");
                for (int colIndex = 0; colIndex < width; colIndex++)
                    Console.Write($" {(mock ? MOCK_CHAR : CellString(personalBoard[rowIndex, colIndex]?.State ?? new ShipCell().State))} {COL_SEP}");
                Console.WriteLine();
                Console.Write(LINE_INDENT + " " + CROSS_POINT);
                for (int colIndex = 0; colIndex < width; colIndex++)
                    Console.Write(ROW_SEP + CROSS_POINT);
                Console.Write(LINE_INDENT + " " + CROSS_POINT);
                for (int colIndex = 0; colIndex < width; colIndex++)
                    Console.Write(ROW_SEP + CROSS_POINT);
                Console.WriteLine();
            }
            lastCellRow = Console.CursorTop - 2;
        }

        private static void PostLastAction(string action)
        {
            int lastRow = Console.CursorTop;
            int lastCol = Console.CursorLeft;
            Console.SetCursorPosition(0, lastActionRow);
            WriteLine(CLEAR_LINE ?? "");
            Console.SetCursorPosition(0, lastActionRow);
            WriteLine($"{action}", ConsoleColor.Cyan);
            lastAction = action;
            Console.SetCursorPosition(lastCol, lastRow);
        }

        private static void WriteLine(string text = "", ConsoleColor textColor = ConsoleColor.Gray)
        {
            Console.ForegroundColor = textColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static string CellString(CellState cellState)
        {
            return cellState switch
            {
                CellState.EMPTY => " ",
                CellState.SHOT_EMPTY => "0",
                CellState.SHOT_DAMAGED => "*",
                CellState.SHOT_DESTROYED => "X",
                CellState.SHIP => "#",
                _ => throw new Exception($"Invalid Cell State:{cellState}"),
            };
        }

        //private static string CellString(ShipCell? cellState)
        //{
        //    return cellState.State switch
        //    {
        //        null => " ",
        //        _ => "#"
        //    };
        //}

        private static void SaveGame()
        {
            if (BattleshipsConsoleUI.db == null)
                BattleshipsConsoleUI.db = new AppDbContext();
            var save = game.GameSave();
            if (save.GameSaveId == 0)
            {
                BattleshipsConsoleUI.db.GameSaves.Add(save);
                BattleshipsConsoleUI.db.SaveChanges();
                game.GameId = save.GameSaveId;
            }
            else
            {
                var entity = BattleshipsConsoleUI.db.GameSaves.Find(save.GameSaveId) ?? new GameSave();
                entity.MoveLog_ = save.MoveLog_;
                entity.Description = save.Description;
                BattleshipsConsoleUI.db.SaveChanges();
            }
    PostLastAction("Game Saved!");
        }
        #endregion Methods

        #region Selector
        private static ConsoleKeyInfo Selector()
        {
            ConsoleKeyInfo pressedKey;
            while (true)
            {
                pressedKey = Console.ReadKey(true);

                // Check for resized window
                resized = ConsoleWidth != Console.WindowWidth || ConsoleHeight != Console.WindowHeight;
                if (resized)
                    Init();

                if (pressedKey.Modifiers == CTRL)
                {
                    if (hasOutcome)
                    {
                        if (pressedKey.Key == EXIT || pressedKey.Key == NEW || pressedKey.Key == UNDO)
                            return pressedKey;
                    }
                    else
                        return pressedKey;
                }
                if (hasOutcome)
                    continue;
                // Table navigation
                if (pressedKey.Key == ENTER || pressedKey.Key == ENTER_ALT)
                    return pressedKey;
                if (pressedKey.Key == UP || pressedKey.Key == UP_ALT)
                {
                    if (Console.CursorTop <= firstCellRow)
                        Console.CursorTop = lastCellRow;
                    else
                        Console.CursorTop -= 2;
                }
                if (pressedKey.Key == DOWN || pressedKey.Key == DOWN_ALT)
                {
                    if (Console.CursorTop >= lastCellRow)
                        Console.CursorTop = firstCellRow;
                    else
                        Console.CursorTop += 2;
                }
                if (pressedKey.Key == LEFT || pressedKey.Key == LEFT_ALT)
                {
                    if (Console.CursorLeft > firstCellCol)
                        Console.CursorLeft -= ROW_SEP.Length + CROSS_POINT.Length;
                    else
                        Console.CursorLeft += (lastCellCol - Console.CursorLeft);
                }
                if (pressedKey.Key == RIGHT || pressedKey.Key == RIGHT_ALT)
                {
                    if (Console.CursorLeft < lastCellCol)
                        Console.CursorLeft += ROW_SEP.Length + CROSS_POINT.Length;
                    else
                        Console.CursorLeft = firstCellCol;
                }
            }
        }

        private static ConsoleKeyInfo AISelector()
        {
            battleBoard ??= game.GetBattleBoard();
            int x = -1;
            int y = -1;
            int limit = game.BoardSize;
            if (x == -1 || y == -1)
            {
                Random randomGenerator = new Random();
                do
                {
                    x = randomGenerator.Next(0, limit);
                    y = randomGenerator.Next(0, limit);
                } while (battleBoard[x, y] != CellState.EMPTY);
            }
            Thread.Sleep(1500);
            ConsoleKeyInfo pressedKey = new ConsoleKeyInfo((char)ENTER, ENTER, false, false, false);
            Console.SetCursorPosition(firstCellCol + y * (ROW_SEP.Length + CROSS_POINT.Length), firstCellRow + x * 2);
            return pressedKey;
        }
        #endregion Selector
    }
}
