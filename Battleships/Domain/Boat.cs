using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Boat
    {
        public int BoatId
        {
            get;
            set;
        }
        public string BoatName
        {
            get;
            set;
        } = "N/A";
        public int BoatLength
        {
            get;
            set;
        } = 0;
        public int BoatCount
        {
            get;
            set;
        } = 0;
        public Boat(string aName)
        {
            BoatName = aName;
        }
        public Boat()
        {
        }
    }
}
