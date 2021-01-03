using System;
using System.Collections.Generic;

namespace SettingsMenu
{
    public class SettingsMenu
    {
        private const int LINE_START = 0;
        private const int VALUE_POS = 30;
        public readonly List<ISettingsItem> Items = new List<ISettingsItem>();

        public SettingsMenu()
        {
            Items.Add(new SettingsItem<string>("Player 1", "Player 1"));
            Items.Add(new SettingsItem<string>("Player 1", "Player 2"));
            Items.Add(new SettingsItem<int>("Board Size", 10));
            Items.Add(new SettingsItem<int>("Cruiser Size", 5));
            Items.Add(new SettingsItem<int>("Cruiser number", 1));
        }

        public void RunMenu()
        {
            foreach (var item in Items)
            {
                Console.Write(item.ToString());
                Console.CursorLeft = VALUE_POS;
                Console.WriteLine(item.ValueToString());
            }
        }
    }
}
