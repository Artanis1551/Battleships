using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameSave
    {
        // PK - Primary Key
        public int GameSaveId
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        } = DateTime.Now.ToString("g");
        [Required]
        [MaxLength(20)]
        public string Player1
        {
            get;
            set;
        } = null!;
        [Required]
        [MaxLength(20)]
        public string Player2
        {
            get;
            set;
        } = null!;
        public int BoardSize_
        {
            get;
            set;
        } = 10;
        public TouchType Touch_
        {
            get;
            set;
        } = 0;
        [MaxLength(2048)]
        public string MoveLog_
        {
            get;
            set;
        } = "";
        [MaxLength(2048)]
        public string ShipList1_
        {
            get;
            set;
        } = "";
        [MaxLength(2048)]
        public string ShipList2_
        {
            get;
            set;
        } = "";
        [MaxLength(2048)]
        public string ShipsReq_
        {
            get;
            set;
        } = "";
    }
}
