using System;
using System.Collections.Generic;
using System.Text;

namespace GameBrain
{
    public class GameMove
    {
        public int X
        {
            get;
            set;
        }
        public int Y
        {
            get;
            set;
        }
        public bool turn;
        public CellState res;

        public GameMove()
        {
        }

        public GameMove(int aX, int aY, bool aTurn, CellState aRes)
        {
            X = aX;
            Y = aY;
            turn = aTurn;
            res = aRes;
        }

        public override string ToString()
        {
            return $"[Row:{X + 1}|Column:{(char)(Y + 'A')}]";
        }

        public GameMove CopyOf()
        {
            return new GameMove(X, Y, turn, res);
        }
    }
}
