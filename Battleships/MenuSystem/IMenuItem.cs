using System;
using System.Collections.Generic;
using System.Text;

namespace MenuSystem
{
    public enum MenuItemType
    {
        Exit,
        Main,
        Return,
        Value,
        Choice,
        Execute,
        Reload
    }
    public interface IMenuItem
    {
        public string Label
        {
            get;
            set;
        }
        public MenuItemType Type
        {
            get;
        }
        public Func<MenuItemType> MethodToExecute
        {
            get;
        }
    }
}
