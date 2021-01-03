using System;
using System.Collections.Generic;
using System.Text;

namespace SettingsMenu
{
    public interface ISettingsItem
    {
        public string? Name
        {
            get;
            set;
        }
        public string ValueToString();
    }
}
