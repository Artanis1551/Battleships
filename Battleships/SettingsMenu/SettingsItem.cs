using System;
using System.Collections.Generic;
using System.Text;

namespace SettingsMenu
{
    public enum SettingsType
    {
        Number,
        Choice,
        Text
    }
    public class SettingsItem<T> : ISettingsItem
    {
        public string? Name
        {
            get;
            set;
        } = null!;
        public T Value
        {
            get;
            set;
        } = default!;

        private SettingsItem()
        {
        }

        public SettingsItem(string aName, T aValue)
        {
            Name = aName;
            Value = aValue;
        }

        public override string ToString()
        {
            return Name ?? "";
        }
        public string ValueToString()
        {
            return Value?.ToString() ?? "";
        }
    }
}
