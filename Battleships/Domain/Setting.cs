using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain
{
    public class Setting
    {
        // PK - Primary Key
        public int SettingId
        {
            get;
            set;
        }

        [MaxLength(20)]
        public string Player_1
        {
            get;
            set;
        } = "Player_1";
        [MaxLength(20)]
        public string Player_2
        {
            get;
            set;
        } = "Player_2";
        public int BoardSize
        {
            get;
            set;
        } = 10;
        public TouchType Touch
        {
            get;
            set;
        } = TouchType.No;
        public LinkedList<Boat> Boats
        {
            get;
            set;
        } = new LinkedList<Boat>();
        public PlaceShips PlaceType
        {
            get;
            set;
        } = PlaceShips.Select;
    }

    public enum TouchType
    {
        No,
        Corners,
        Yes
    }
    public enum PlaceShips
    {
        Select,
        Random
    }
}
