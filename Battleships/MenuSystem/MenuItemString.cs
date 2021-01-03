using System;
using System.Collections.Generic;
using System.Text;

namespace MenuSystem
{
    public class MenuItemString : MenuItemDefault
    {
        public string Value
        {
            get;
            private set;
        } = "N/A";
        private readonly Action<string> Update;
        private void SetValue(string aValue)
        {
            if (aValue.Length < MAX_LEN_VAL)
                Value = aValue;
            Update(Value);
        }
        public MenuItemString(String aValue, Action<string> aUpdate, MenuItemType aType = MenuItemType.Value, string aLabel = DEF_LABEL, Func<MenuItemType>? aMethodToExecute = null)
        {
            if (aLabel.Length < MIN_LEN || aLabel.Length > MAX_LEN_LABEL)
                throw new Exception(message: $"Label \"{aLabel}\" size not in range [{MIN_LEN};{MAX_LEN_LABEL}]");
            Label = aLabel;
            Type = aType;
            Update = aUpdate;
            MethodToExecute = aMethodToExecute ?? DefaultMethod;
            SetValue(aValue);
        }
        public override MenuItemType DefaultMethod()
        {
            SortedSet<ConsoleKey> doNotWrite = new SortedSet<ConsoleKey>{ConsoleKey.Enter, ConsoleKey.Tab, ConsoleKey.UpArrow, ConsoleKey.DownArrow,
                                                                         ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Backspace, ConsoleKey.Escape,
                                                                         ConsoleKey.F1, ConsoleKey.F2, ConsoleKey.F3, ConsoleKey.F4, ConsoleKey.F5,
                                                                         ConsoleKey.F6, ConsoleKey.F7, ConsoleKey.F8, ConsoleKey.F9, ConsoleKey.F10,
                                                                         ConsoleKey.F11, ConsoleKey.F12, ConsoleKey.Insert, ConsoleKey.PrintScreen, ConsoleKey.Delete,
                                                                         ConsoleKey.LeftWindows, ConsoleKey.RightWindows};
            Console.CursorLeft = 1 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
            ConsoleKeyInfo pressedKey;
            StringBuilder valueBuilder = new StringBuilder("");
            while (true)
            {
                pressedKey = Console.ReadKey(true);

                if (pressedKey.Modifiers != 0)
                    continue;

                // Option selection
                if (pressedKey.Key == ConsoleKey.Enter)
                {
                    if (valueBuilder.Length != 0)
                        SetValue(valueBuilder.ToString());
                    break;
                }
                if (pressedKey.Key == ConsoleKey.Escape)
                    break;
                if (pressedKey.Key == ConsoleKey.Backspace && valueBuilder.Length > 0)
                {
                    Console.CursorLeft--;
                    Console.Write(" ");
                    Console.CursorLeft--;
                    valueBuilder.Remove(valueBuilder.Length - 1, 1);
                }
                if (!doNotWrite.Contains(pressedKey.Key) && valueBuilder.Length < MAX_LEN_VAL)
                {
                    Console.Write(pressedKey.KeyChar);
                    valueBuilder.Append(pressedKey.KeyChar);
                }
            }
            Console.CursorLeft = 1 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
            Console.Write("".PadRight(MAX_LEN_VAL));
            return Type;
        }

        public override string ToString()
        {
            return " " + Label.PadRight(MAX_LEN_LABEL + FIELD_DIST) + Value.ToString().PadRight(MAX_LEN_VAL + FIELD_DIST);
        }
    }
}
