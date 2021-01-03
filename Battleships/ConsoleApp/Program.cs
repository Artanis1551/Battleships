using System;
using MenuSystem;
using GameConsoleUI;
using GameBrain;
using System.ComponentModel.Design;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Domain;
using System.Collections.Generic;
using DAL;

namespace ConsoleApp
{
    class Program
    {
        public enum GameSaveDecision
        {
            Play,
            Delete
        }

        private static Setting setting = new Setting();
        static void Main ()
        {
            try
            {
                BattleshipsConsoleUI.db = new AppDbContext();
                if (BattleshipsConsoleUI.db.Settings.Find(1) == null)
                {
                    BattleshipsConsoleUI.db.Settings.Add(setting);
                    BattleshipsConsoleUI.db.SaveChanges();
                }
                else foreach (var s in BattleshipsConsoleUI.db.Settings.Include(g => g.Boats))
                {
                        setting = s;
                        break;
                }
                Menu menu;
                Menu settingsMenu;
                Menu loadGameMenu;

                loadGameMenu = new Menu(() =>
                {
                    Menu tMenu = new Menu(Menu.LEVEL_TYPES.TRUNK, "Load Game");
                    foreach (var save in BattleshipsConsoleUI.db.GameSaves)
                    {
                        tMenu.AddMenuItem(new MenuItemChoice<GameSaveDecision>(GameSaveDecision.Play, delegate (GameSaveDecision t)
                        {
                            if (t == GameSaveDecision.Play)
                                BattleshipsConsoleUI.RunConsoleGame(setting, save);
                            if (t == GameSaveDecision.Delete)
                            {
                                BattleshipsConsoleUI.db.GameSaves.Remove(save);
                                BattleshipsConsoleUI.db.SaveChanges();
                            }
                        },
                        MenuItemType.Reload, $"[{save.Player1}]-vs-[{save.Player2}]--{save.Description}"));
                    }
                    return tMenu;
                });

                settingsMenu = new Menu(() =>
                {
                    Menu tMenu = new Menu(Menu.LEVEL_TYPES.TRUNK, "Settings");

                    tMenu.AddMenuItem(new MenuItemString(setting.Player_1, delegate (string t)
                    {
                        setting.Player_1 = t;
                        BattleshipsConsoleUI.db.SaveChanges();
                    }, MenuItemType.Value, "Player 1 Name"));
                    tMenu.AddMenuItem(new MenuItemString(setting.Player_2, delegate (string t)
                    {
                        setting.Player_2 = t;
                        BattleshipsConsoleUI.db.SaveChanges();
                    }, MenuItemType.Value, "Player 2 Name"));
                    tMenu.AddMenuItem(new MenuItemChoice<TouchType>(setting.Touch, delegate (TouchType t)
                    {
                        setting.Touch = t;
                        BattleshipsConsoleUI.db.SaveChanges();
                    }, MenuItemType.Choice, "Boats Touching"));
                    tMenu.AddMenuItem(new MenuItemChoice<PlaceShips>(setting.PlaceType, delegate (PlaceShips t)
                    {
                        setting.PlaceType = t;
                        BattleshipsConsoleUI.db.SaveChanges();
                    }, MenuItemType.Choice, "Placing Ships"));
                    tMenu.AddMenuItem(new MenuItemInt(setting.BoardSize, delegate (int t)
                    {
                        setting.BoardSize = t;
                        BattleshipsConsoleUI.db.SaveChanges();
                    }, MenuItemType.Value, "Board Size"));
                    tMenu.AddMenuItem(new MenuItemDefault(MenuItemType.Reload, "Add Boat", delegate ()
                    {
                        setting.Boats.AddLast(new Boat("N/A"));
                        BattleshipsConsoleUI.db.SaveChanges();
                        return MenuItemType.Reload;
                    }));
                    int index = 1;
                    foreach (var boat in setting.Boats)
                    {
                        tMenu.AddMenuItem(new MenuItemString(boat.BoatName, delegate (string t)
                        {
                            boat.BoatName = t;
                            BattleshipsConsoleUI.db.SaveChanges();
                        }, MenuItemType.Value, $"Boat {index} Name"));
                        tMenu.AddMenuItem(new MenuItemInt(boat.BoatCount, delegate (int t)
                        {
                            boat.BoatCount = t;
                            BattleshipsConsoleUI.db.SaveChanges();
                        }, MenuItemType.Value, $"Boat {index} Count"));
                        tMenu.AddMenuItem(new MenuItemInt(boat.BoatLength, delegate (int t)
                        {
                            boat.BoatLength = t;
                            BattleshipsConsoleUI.db.SaveChanges();
                        }, MenuItemType.Value, $"Boat {index} Length"));
                        tMenu.AddMenuItem(new MenuItemDefault(MenuItemType.Reload, $"Remove Boat {index}", () =>
                        {
                            setting.Boats.Remove(boat);
                            BattleshipsConsoleUI.db.Boats.Remove(boat);
                            BattleshipsConsoleUI.db.SaveChanges();
                            return MenuItemType.Reload;
                        }));
                        index++;
                    }

                    return tMenu;
                });

                menu = new Menu(() =>
                {
                    Menu tMenu = new Menu(Menu.LEVEL_TYPES.ROOT, "Battleships Petrea");

                    tMenu.AddMenuItem(new MenuItemDefault(MenuItemType.Execute, "New Game: Human vs Human", () =>
                    {
                        BattleshipsConsoleUI.RunConsoleGame(setting, null, GAME_TYPE.HU_VS_HU);

                        return MenuItemType.Execute;
                    }));
                    tMenu.AddMenuItem(new MenuItemDefault(MenuItemType.Execute, "New Game: Puny Human vs MIGHTY AI", () =>
                    {
                        BattleshipsConsoleUI.RunConsoleGame(setting, null, GAME_TYPE.HU_VS_AI);

                        return MenuItemType.Execute;
                    }));
                    tMenu.AddMenuItem(new MenuItemDefault(MenuItemType.Execute, "Load Game", loadGameMenu.RunMenu));
                    tMenu.AddMenuItem(new MenuItemDefault(MenuItemType.Execute, "Settings", settingsMenu.RunMenu));

                    return tMenu;
                });

                menu.RunMenu();
                Console.Clear();
                Console.WriteLine("Closing down...", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {e.Message}");
                Console.ResetColor();
            }
        }
    }
}
