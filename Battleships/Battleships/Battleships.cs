using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain;

namespace GameBrain
{
    public enum CellState
    {
        EMPTY,
        SHOT_EMPTY,
        SHOT_DAMAGED,
        SHOT_DESTROYED,
        SHIP
    }

    public class Battleships
    {
        public int BoardSize
        {
            get;
            private set;
        } = 10;
        public int GameId
        {
            get;
            set;
        } = 0;
        public TouchType Touch
        {
            get;
            private set;
        }
        public bool Turn
        {
            get;
            set;
        }
        public bool Check
        {
            get;
            private set;
        }
        public int MoveCount
        {
            get
            {
                return MoveLog.Count;
            }
        }
        private int shipsDestroyed1 = 0;
        private int shipsDestroyed2 = 0;

        private readonly List<GameMove> MoveLog = new List<GameMove>();
        private readonly List<Ship> ShipList1 = new List<Ship>();
        private readonly List<Ship> ShipList2 = new List<Ship>();
        public readonly LinkedList<Boat> ShipsReq = new LinkedList<Boat>();

        public readonly string Player_1;
        public readonly string Player_2;

        private readonly ShipCell[,] personalBoard1;
        private readonly CellState[,] battleBoard1;
        private readonly ShipCell[,] personalBoard2;
        private readonly CellState[,] battleBoard2;

        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = false,
            AllowTrailingCommas = true,
        };

        public Battleships(Setting setting)
        {
            Player_1 = setting.Player_1;
            Player_2 = setting.Player_2;
            BoardSize = setting.BoardSize;
            Touch = setting.Touch;
            ShipsReq = setting.Boats;
            personalBoard1 = new ShipCell[BoardSize, BoardSize];
            battleBoard1 = new CellState[BoardSize, BoardSize];
            personalBoard2 = new ShipCell[BoardSize, BoardSize];
            battleBoard2 = new CellState[BoardSize, BoardSize];
            Init();
        }

        public Battleships(GameSave save)
        {
            Player_1 = save.Player1;
            Player_2 = save.Player2;
            Touch = save.Touch_;
            BoardSize = save.BoardSize_;
            GameId = save.GameSaveId;
            personalBoard1 = new ShipCell[BoardSize, BoardSize];
            battleBoard1 = new CellState[BoardSize, BoardSize];
            personalBoard2 = new ShipCell[BoardSize, BoardSize];
            battleBoard2 = new CellState[BoardSize, BoardSize];
            for (int col = 0; col < BoardSize; col++)
                for (int row = 0; row < BoardSize; row++)
                {
                    personalBoard1[row, col] = new ShipCell();
                    personalBoard2[row, col] = new ShipCell();
                }
            var tShipList1 = JsonSerializer.Deserialize<List<Ship>>(save.ShipList1_, jsonOptions)?? new List<Ship>();
            try
            {
                foreach (var ship in tShipList1)
                    PlaceShip(ship.SX, ship.SY, ship.EX, ship.EY);
            }
            catch (Exception e) 
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
            Turn = !Turn;
            var tShipList2 = JsonSerializer.Deserialize<List<Ship>>(save.ShipList2_, jsonOptions) ?? new List<Ship>();
            foreach (var ship in tShipList2)
                PlaceShip(ship.SX, ship.SY, ship.EX, ship.EY);
            Turn = !Turn;
            List<GameMove> tLog = JsonSerializer.Deserialize<List<GameMove>>(save.MoveLog_, jsonOptions) ?? new List<GameMove>();
            foreach (var move in tLog)
                MakeMove(move.X, move.Y);
            ShipsReq = JsonSerializer.Deserialize<LinkedList<Boat>>(save.ShipsReq_, jsonOptions) ?? new LinkedList<Boat>();
        }

        public Battleships()
        {
            Player_1 = "Player_1";
            Player_2 = "Player_2";
            BoardSize = 1;
            Touch = 0;
            personalBoard1 = new ShipCell[BoardSize, BoardSize];
            battleBoard1 = new CellState[BoardSize, BoardSize];
            personalBoard2 = new ShipCell[BoardSize, BoardSize];
            battleBoard2 = new CellState[BoardSize, BoardSize];
        }

        public void Init()
        {
            GameId = 0;
            Turn = false;
            Check = false;
            shipsDestroyed1 = 0;
            shipsDestroyed2 = 0;
            MoveLog.Clear();
            ShipList1.Clear();
            ShipList2.Clear();
            for (int rowIndex = 0; rowIndex < BoardSize; rowIndex++)
                for (int colIndex = 0; colIndex < BoardSize; colIndex++)
                {
                    personalBoard1[rowIndex, colIndex] = new ShipCell();
                    personalBoard2[rowIndex, colIndex] = new ShipCell();
                    battleBoard1[rowIndex, colIndex] = CellState.EMPTY;
                    battleBoard2[rowIndex, colIndex] = CellState.EMPTY;
                }
        }

        public CellState[,] GetBattleBoard()
        {
            var board = (Turn ? battleBoard2 : battleBoard1);
            var res = new CellState[BoardSize, BoardSize];
            Array.Copy(board, res, board.Length);
            return res;
        }

        public ShipCell[,] GetPersonalBoard()
        {
            var board = (Turn ? personalBoard2 : personalBoard1);
            var res = new ShipCell[BoardSize, BoardSize];
            Array.Copy(board, res, board.Length);
            return res;
        }

        public List<GameMove> GetLog()
        {
            var res = new List<GameMove>(MoveLog);
            return res;
        }

        public bool PlaceShip(int sX, int sY, int eX, int eY)
        {
            if (sX > eX)
            {
                int t = eX;
                eX = sX;
                sX = t;
                t = eY;
                eY = sY;
                sY = t;
            }
            if (sX == eX && sY > eY)
            {
                int t = eY;
                eY = sY;
                sY = t;
            }
            if (CheckFree(sX, sY, eX, eY) == false)
                return false;
            if (!Turn)
                ShipList1.Add(new Ship(sX, sY, eX, eY));
            else
                ShipList2.Add(new Ship(sX, sY, eX, eY));
            for (int indexRow = sX; indexRow <= eX; indexRow++)
                for (int indexCol = sY; indexCol <= eY; indexCol++)
                    if (!Turn)
                        personalBoard1[indexRow, indexCol] = new ShipCell(ShipList1.Last());
                    else
                        personalBoard2[indexRow, indexCol] = new ShipCell(ShipList2.Last());
            return true;
        }

        public void PlaceRandom()
        {
            DateTime init = DateTime.Now;
            foreach (var boatType in ShipsReq)
                for (int indexCount = 0; indexCount < boatType.BoatCount; indexCount++)
                {
                    var RandomGenerator = new Random(DateTime.Now.Millisecond);
                    int sX, sY;
                    while (true)
                    {
                        sX = RandomGenerator.Next(0, BoardSize - 1);
                        sY = RandomGenerator.Next(0, BoardSize - 1);
                        if (CheckFree(sX, sY, sX, sY + boatType.BoatLength - 1))
                        {
                            PlaceShip(sX, sY, sX, sY + boatType.BoatLength - 1);
                            break;
                        }
                        else if (CheckFree(sX, sY, sX + boatType.BoatLength - 1, sY))
                        {
                            PlaceShip(sX, sY, sX + boatType.BoatLength - 1, sY);
                            break;
                        }
                        if (DateTime.Now.Subtract(init).TotalMilliseconds > 2000)
                            throw new Exception("Blocked in cycle");
                    }
                }
            Turn = !Turn;
        }

        private bool CheckFree(int sX, int sY, int eX, int eY)
        {
            if (eX >= BoardSize || eY >= BoardSize)
                return false;
            for (int row = sX; row <= eX; row++)
                for (int col = sY; col <= eY; col++)
                    if (!NoNeighbors(row, col))
                        return false;
            return true;
        }

        public bool NoNeighbors(int x, int y)
        {
            ShipCell[,] personalBoard = Turn ? personalBoard2 : personalBoard1;
            if (personalBoard[x, y].Ship != null)
                return false;
            if (Touch == TouchType.Yes)
                return true;
            int[] pos = new int[] { 0, 1, -1 };
            for (int j = 0; j < pos.Length; j++)
                if (y + pos[j] >= 0 && y + pos[j] < BoardSize)
                    if (personalBoard[x, y + pos[j]].Ship != null)
                        return false;
            for (int i = 0; i < pos.Length; i++)
                if (x + pos[i] >= 0 && x + pos[i] < BoardSize)
                    if (personalBoard[x + pos[i], y].Ship != null)
                        return false;
            if (Touch == TouchType.Corners)
                return true;
            for (int i = 0; i < pos.Length; i++)
                if (x + pos[i] >= 0 && x + pos[i] < BoardSize)
                    for (int j = 0; j < pos.Length; j++)
                        if (y + pos[j] >= 0 && y + pos[j] < BoardSize)
                            if (personalBoard[x + pos[i], y + pos[j]].Ship != null)
                                return false;
            return true;
        }

        public GameMove? MakeMove(int x, int y)
        {
            var target = Turn ? battleBoard2 : battleBoard1;
            var player = Turn ? personalBoard1 : personalBoard2;
            int destroyedCount = Turn ? shipsDestroyed2 : shipsDestroyed1;

            if (target[x, y] != CellState.EMPTY)
                return null;
            ShipCell ship = player[x, y];
            if (ship.Ship == null)
            {
                target[x, y] = CellState.SHOT_EMPTY;
                player[x, y].State = CellState.SHOT_EMPTY;
                Turn = !Turn;
            }
            else
            {
                target[x, y] = CellState.SHOT_DAMAGED;
                player[x, y].State = CellState.SHOT_DAMAGED;
                ship.Ship.Shot();
                if (ship.Ship.status == ShipStatus.DESTROYED)
                {
                    destroyedCount = (int)destroyedCount + 1;
                    for (int row = ship.Ship.SX; row <= ship.Ship.EX; row++)
                        for (int col = ship.Ship.SY; col <= ship.Ship.EY; col++)
                        {
                            target[row, col] = CellState.SHOT_DESTROYED;
                            player[row, col].State = CellState.SHOT_DESTROYED;
                        }
                }
            }
            if (Turn)
                shipsDestroyed2 = destroyedCount;
            else
                shipsDestroyed1 = destroyedCount;
            MoveLog.Add(new GameMove(x, y, (target[x, y] == CellState.SHOT_EMPTY ? !Turn : Turn), target[x, y]));
            CheckWin();
            return MoveLog[MoveCount - 1].CopyOf();
        }

        public GameMove? UndoMove()
        {
            if (MoveCount == 0)
                return null;
            if (Turn != MoveLog[MoveCount - 1].turn)
                return null;
            GameMove lastMove = MoveLog[MoveCount - 1];
            MoveLog.RemoveAt(MoveCount - 1);
            if (!Turn)
                battleBoard1[lastMove.X, lastMove.Y] = CellState.EMPTY;
            else
                battleBoard2[lastMove.X, lastMove.Y] = CellState.EMPTY;
            Check = false;
            return lastMove.CopyOf();
        }

        private void CheckWin()
        {
            if (shipsDestroyed1 == ShipList2.Count || shipsDestroyed2 == ShipList1.Count)
                Check = !Check;
        }

        public GameSave GameSave()
        {
            var save = new GameSave()
            {
                Player1 = Player_1,
                Player2 = Player_2,
                BoardSize_ = BoardSize,
                Touch_ = Touch,
                MoveLog_ = JsonSerializer.Serialize(MoveLog, jsonOptions),
                ShipList1_ = JsonSerializer.Serialize(ShipList1, jsonOptions),
                ShipList2_ = JsonSerializer.Serialize(ShipList2, jsonOptions),
                ShipsReq_ = JsonSerializer.Serialize(ShipsReq, jsonOptions),
            };
            if (GameId != 0)
                save.GameSaveId = GameId;
            return save;
        }
    }
}
