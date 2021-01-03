using System;
using System.Collections.Generic;
using System.Text;

namespace GameBrain
{
    public enum ShipStatus
    {
        INTACT,
        DAMAGED,
        DESTROYED
    }

    public class Ship
    {
        public ShipStatus status;
        private int health;
        public int SX
        {
            get;
            set;
        }
        public int SY
        {
            get;
            set;
        }
        public int EX
        {
            get;
            set;
        }
        public int EY
        {
            get;
            set;
        }

        public Ship()
        {
        }

        public Ship(int asX, int asY, int aeX, int aeY)
        {
            SX = asX;
            SY = asY;
            EX = aeX;
            EY = aeY;
            health = Math.Abs(SX == EX ? EY - SY + 1 : EX - SX + 1);
        }

        public void Shot()
        {
            health--;
            if (health <= 0)
                status = ShipStatus.DESTROYED;
            else
                status = ShipStatus.DAMAGED;
        }
    }
}
