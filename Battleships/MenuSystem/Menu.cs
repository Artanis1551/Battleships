using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;

namespace MenuSystem
{
    /// <summary>
    /// Menu class with methods to display and operate the menu
    /// </summary>
    public class Menu
    {
        #region Fields and Properties
        private Func<Menu>? buildModel;

        /// <summary>
        /// ROOT -> Main Menu
        /// TRUNK -> First Level of Submenu
        /// BRANCH -> Second and further Levels of Submenu
        /// </summary>
        public enum LEVEL_TYPES
        {
            ROOT,
            TRUNK,
            BRANCH
        }
        /// <summary>
        /// Specifies Menu type from enum type Menu.LEVEL_TYPES
        /// </summary>
        public LEVEL_TYPES LEVEL
        {
            get;
            private set;
        }

        private const int SND_DUR = 200;                            // Selection sound duration
        private const int SND_FREQ = 800;                          // Non-default valid Menu Item selected sound frequency 

        private const string DEF_NAME = "Menu Name not assigned";   // Default Menu name

        public int highRow;
        public int lowRow;

        /// <summary>
        /// Display name of the Menu
        /// </summary>
        public string menuName;

        /// <summary>
        /// List of Menu Items in Menu
        /// </summary>
        private LinkedList<IMenuItem> MenuItems
        {
            get;
            set;
        } = new LinkedList<IMenuItem>();
        #endregion Fields and Properties

        #region Constructor
        /// <summary>
        /// Constructor of Menu object
        /// </summary>
        /// <param name="aLEVEL">
        /// Specifies Menu type from enum type Menu.LEVEL_TYPES
        /// </param>
        /// <param name="aMenuName">
        /// Display name of the Menu
        /// </param>
        public Menu(LEVEL_TYPES aLEVEL, string aMenuName = DEF_NAME)
        {
            if (aLEVEL > LEVEL_TYPES.BRANCH)
                throw new Exception($"Unknown menu depth {aLEVEL}");

            MenuItems.AddFirst(new MenuItemDefault(MenuItemType.Exit, "Exit"));
            if (aLEVEL > LEVEL_TYPES.ROOT)
                MenuItems.AddFirst(new MenuItemDefault(MenuItemType.Main, "Main Menu"));
            if (aLEVEL > LEVEL_TYPES.TRUNK)
                MenuItems.AddFirst(new MenuItemDefault(MenuItemType.Return, "Return"));

            LEVEL = aLEVEL;
            menuName = aMenuName;
        }

        public Menu(Func <Menu> aBuildModel) 
        {
            buildModel = aBuildModel;
            menuName = DEF_NAME;
        }

        private Menu() 
        {
            buildModel = null;
            menuName = DEF_NAME;
        }
        #endregion Constructor

        #region Methods
        private void Build()
        {
            Menu model;
            if (buildModel != null)
                model = buildModel();
            else
                return;
            menuName = model.menuName;
            LEVEL = model.LEVEL;
            MenuItems.Clear();
            MenuItems = new LinkedList<IMenuItem>(model.MenuItems);
        }
        public void AddMenuItem(IMenuItem aMenuItem)
        {
            IMenuItem similarMenuItem = MenuItems.FirstOrDefault(t => t.Label.ToUpper().Trim() == aMenuItem.Label.ToUpper().Trim());
            //if (similarMenuItem != null)
            //    throw new Exception(message: $"Label duplicate \"{aMenuItem.Label}\"");
            var firstDefault = MenuItems.Last;
            for (int it = 0; it < (int)LEVEL; it++)
                firstDefault = firstDefault.Previous;
            MenuItems.AddBefore(firstDefault, aMenuItem);
        }

        private static void WriteLine(string text = "", ConsoleColor textColor = ConsoleColor.Gray)
        {
            Console.ForegroundColor = textColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        #endregion Methods

        #region Running Loop
        /// <summary>
        /// Main loop of Menu
        /// </summary>
        /// <returns>
        /// Returns a nullable string with the last option chosen
        /// </returns>
        public virtual MenuItemType RunMenu()
        {
            Build();
            MenuItemType result;
            IMenuItem userChoice;
            do
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Menu.WriteLine();
                Menu.WriteLine($"==========> {menuName} <==========", ConsoleColor.DarkYellow);

                highRow = Console.CursorTop;
                lowRow = highRow + MenuItems.Count - 1;
                foreach (var menuItem in MenuItems)
                    Menu.WriteLine(menuItem.ToString(), ConsoleColor.Cyan);

                Console.SetCursorPosition(0, highRow);

                userChoice = Selector();

                result = userChoice.MethodToExecute();
                // Return to previous menu
                if (result == MenuItemType.Return && LEVEL > LEVEL_TYPES.TRUNK)
                {
                    result = MenuItemType.Execute;
                    break;
                }
                if (result == MenuItemType.Reload)
                    Build();
            } while (result != MenuItemType.Exit && !(LEVEL > LEVEL_TYPES.ROOT && result == MenuItemType.Main));
            Console.Clear();

            return result;
        }
        #endregion Running Loop

        #region Option Selector
        private IMenuItem Selector()
        {
            IMenuItem userChoice;
            ConsoleKeyInfo pressedKey;
            while (true)
            {
                Console.CursorLeft = 0;
                pressedKey = Console.ReadKey(true);

                // Option selection
                if (pressedKey.Key == ConsoleKey.Enter || pressedKey.Key == ConsoleKey.Spacebar)
                {
                    userChoice = MenuItems.ElementAt(Console.CursorTop - highRow);
                    Console.Beep(SND_FREQ, SND_DUR);
                    break;
                }

                if (pressedKey.Key == ConsoleKey.UpArrow || pressedKey.Key == ConsoleKey.W)
                {
                    if (Console.CursorTop == highRow)
                        Console.CursorTop = lowRow;
                    else
                        Console.CursorTop--;
                }
                else if (pressedKey.Key == ConsoleKey.DownArrow || pressedKey.Key == ConsoleKey.S)
                {
                    if (Console.CursorTop == lowRow)
                        Console.CursorTop = highRow;
                    else
                        Console.CursorTop++;
                }
            }
            return userChoice;
        }
        #endregion Option Selector
    }
}
