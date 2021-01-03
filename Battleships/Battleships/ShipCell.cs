using System;
using System.Collections.Generic;
using System.Text;

namespace GameBrain
{
    public class ShipCell
    {
        public Ship? Ship
        {
            get;
            set;
        } = null;

        public CellState State
        {
            get;
            set;
        } = CellState.EMPTY;

        public ShipCell(Ship aShip)
        {
            Ship = aShip;
            State = CellState.SHIP;
        }

        public ShipCell()
        {
            Ship = null;
            State = CellState.EMPTY;
        }
    }
}
