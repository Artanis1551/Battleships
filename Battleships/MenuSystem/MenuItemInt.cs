using System;
using System.Collections.Generic;
using System.Text;

namespace MenuSystem
{

    public class MenuItemInt : MenuItemDefault
    {
        private int Min
        {
            get;
            set;
        } = 0;
        private int Max
        {
            get;
            set;
        } = 19;
        public int Value
        {
            get;
            private set;
        } = default;
        private readonly Action<int> Update;
        private void SetValue(int aValue)
        {
            if (aValue < Min || aValue > Max)
                Value = Value;
            else
                Value = aValue;
            if (Value.ToString().Length > MAX_LEN_VAL)
                Value = Convert.ToInt32(Value.ToString().Substring(0, Math.Min(Value.ToString().Length, MAX_LEN_VAL)));
            Update(Value);
        }
        private void SetValue(string sValue)
        {
            int aValue;
            try
            {
                aValue = Convert.ToInt32(sValue);
            }
            catch (Exception)
            {
                aValue = 0;
            }
            SetValue(aValue);
        }
        public void SetRange(int aMin, int aMax)
        {
            Min = aMin;
            Max = aMax;
        }
        public MenuItemInt(int aValue, Action<int> aUpdate, MenuItemType aType = MenuItemType.Value, string aLabel = DEF_LABEL, Func<MenuItemType>? aMethodToExecute = null)
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
            SortedSet<ConsoleKey> numWriteKeys = new SortedSet<ConsoleKey>{ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4,
                                                                            ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8,
                                                                            ConsoleKey.D9, ConsoleKey.D0, ConsoleKey.Subtract};
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
                    SetValue(valueBuilder.ToString());
                    break;
                }
                if (pressedKey.Key == ConsoleKey.Escape)
                    break;
                if (numWriteKeys.Contains(pressedKey.Key) && valueBuilder.Length < MAX_LEN_VAL)
                {
                    Console.Write(pressedKey.KeyChar);
                    valueBuilder.Append(pressedKey.KeyChar);
                }
                if (pressedKey.Key == ConsoleKey.Backspace && valueBuilder.Length > 0)
                {
                    Console.CursorLeft--;
                    Console.Write(" ");
                    Console.CursorLeft--;
                    valueBuilder.Remove(valueBuilder.Length - 1, 1);
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
