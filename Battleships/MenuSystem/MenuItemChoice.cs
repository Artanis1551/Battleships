using System;
using System.Collections.Generic;
using System.Text;

namespace MenuSystem
{
    public class MenuItemChoice<T> : MenuItemDefault where T : struct, IConvertible, IComparable, IFormattable
    {
        public T Value
        {
            get;
            private set;
        }
        private void SetValue(T aValue)
        {
            Value = aValue;
            Update(Value);
        }
        private readonly Action<T> Update;
        public MenuItemChoice(T aValue, Action<T> aUpdate, MenuItemType aType = MenuItemType.Value, string aLabel = DEF_LABEL, Func<MenuItemType>? aMethodToExecute = null)
        {
            if (aLabel.Length < MIN_LEN || aLabel.Length > MAX_LEN_LABEL)
                throw new Exception(message: $"Label \"{aLabel}\" size not in range [{MIN_LEN};{MAX_LEN_LABEL}]");
            Label = aLabel;
            Type = aType;
            Update = aUpdate;
            MethodToExecute = aMethodToExecute ?? DefaultMethod;
            Value = aValue;
        }
        public override MenuItemType DefaultMethod()
        {
            var Values = Enum.GetValues(typeof(T));
            int index = 0;
            string text = Enum.GetName(typeof(T), index);
            Console.CursorLeft = 1 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
            Console.Write("<" + text.PadRight(text.Length + (MAX_LEN_VAL - text.Length) / 2 + (MAX_LEN_VAL - text.Length) % 2).PadLeft(MAX_LEN_VAL) + ">");
            Console.CursorLeft = 1 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
            ConsoleKeyInfo pressedKey;
            while (true)
            {
                pressedKey = Console.ReadKey(true);

                if (pressedKey.Modifiers != 0)
                    continue;
                // Option selection
                if (pressedKey.Key == ConsoleKey.Enter)
                {
                    SetValue((T)Values.GetValue(index));
                    break;
                }
                if (pressedKey.Key == ConsoleKey.Escape)
                    break;
                if (pressedKey.Key == ConsoleKey.LeftArrow)
                {
                    if (index == 0)
                        index = Values.Length - 1;
                    else
                        index--;
                    text = Enum.GetName(typeof(T), index);
                    Console.CursorLeft = 2 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
                    Console.Write("".PadRight(MAX_LEN_VAL));
                    Console.CursorLeft = 2 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
                    Console.Write(text.PadRight(text.Length + (MAX_LEN_VAL - text.Length) / 2 + (MAX_LEN_VAL - text.Length) % 2).PadLeft(MAX_LEN_VAL));
                    Console.CursorLeft = 2 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
                }
                if (pressedKey.Key == ConsoleKey.RightArrow)
                {
                    if (index == Values.Length - 1)
                        index = 0;
                    else
                        index++;
                    text = Enum.GetName(typeof(T), index);
                    Console.CursorLeft = 2 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
                    Console.Write("".PadRight(MAX_LEN_VAL));
                    Console.CursorLeft = 2 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
                    Console.Write(text.PadRight(text.Length + (MAX_LEN_VAL - text.Length) / 2 + (MAX_LEN_VAL - text.Length) % 2).PadLeft(MAX_LEN_VAL));
                    Console.CursorLeft = 2 + MAX_LEN_LABEL + MAX_LEN_VAL + 2 * FIELD_DIST;
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
