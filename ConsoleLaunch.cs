using PracticeGame;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using static PracticeGame.Character;

namespace Gam
{
    internal class ConsoleLaunch
    {
        //static void FLyAction(IFly flyer, string direction)
        //{
        //    flyer.FlyMove();
        //}
        static void Instruct()
        {
            Console.WriteLine("Welcum!\nType create to create a new character or type load to load save file");
            Console.WriteLine("\nMovement: move your char with W, S, A, D. Toggle sprint mode with r.");
            Console.WriteLine("Flight: press fly to Fly, land to Land. Fly up with up, fly down with down.");
            Console.WriteLine("Swim: press down to enter the water, press up to come out of it");
            Console.WriteLine("Info: type who to see what characters are nearby. Type where or info %name% to see their coords or info.");
            Console.WriteLine("Use save and load if needed, press E to exist");
            Console.WriteLine("Type name of a nearby character to see possible actions");

            
        }
        static void CharacterEventHandler(string message, Character character)
        {
            Console.WriteLine(message);
        }

        static void TwoCharacterEventHandler(string message, Character firstChar, Character secondChar)
        {
            Console.WriteLine(message);
        }
        static Character CreateCharacter(string name, Race race)
        {
            Character? ch = null;
            switch (race)
            {
                case Race.Mermaid:
                    ch = new Mermaid(name);
                    return ch;
                case Race.Human:
                    ch = new Human(name);
                    return ch;
                case Race.Fae:
                    ch = new Fae(name);
                    return ch;
                case Race.Valkyrae:
                    ch = new Valkyrae(name);
                    return ch;
                default:
                    Console.WriteLine("Your race input is invalid, please try again");
                    CreateCharacter(name, race);
                    return ch;
            }
        }
        static Character CreateCharacter(string name, string race)
        {
            Character? ch = null;
            switch (race)
            {
                case "Mermaid":
                    ch = new Mermaid(name);
                    return ch;
                case "Human":
                    ch = new Human(name);
                    return ch;
                case "Fae":
                    ch = new Fae(name);
                    return ch;
                case "Valkyrae":
                    ch = new Valkyrae(name);
                    return ch;
                default:
                    Console.WriteLine("Your race input is invalid, please try again");
                    CreateCharacter(name, race);
                    return ch;
            }
        }
        static string CharacterCreation(out Character ch)
        {
            Console.WriteLine("What's your char name?");
            string? name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be blank");
                CharacterCreation(out ch);
            }
            Console.WriteLine("What's your char race? \n1. Human   2. Mermaid   3. Fae   4.Valkyrae");
            ConsoleKeyInfo choice = Console.ReadKey();
            switch (choice.KeyChar)
            {
                case '2':
                    Console.Write("\nYou've created a Mermaid");
                    ch = CreationEventHelper(name!, Race.Mermaid);
                    return name!;
                case '1':
                    Console.Write("\nYou've created a Human");
                    ch = CreationEventHelper(name!, Race.Human);
                    return name!;
                case '3':
                    Console.Write("\nYou've created a Fae");
                    ch = CreationEventHelper(name!, Race.Fae);
                    return name!;
                case '4':
                    Console.Write("\nYou've created a Valkyrae");
                    ch = CreationEventHelper(name!, Race.Valkyrae);
                    return name!;
                default:
                    Console.WriteLine("Your input is invalid, please try again");
                    CharacterCreation(out ch);
                    return name!;
            }

            Character CreationEventHelper(string name, Race race)
            {
                Character ch = CreateCharacter(name, race);
                ch.MessageCharAction += CharacterEventHandler;
                ch.MessageTwoCharAction += TwoCharacterEventHandler;
                return ch;
            }
        }
        static void SaveToFile(Dictionary<string, Character> charTable) //name, race, health, x, y, z, alive, wet
        {
            File.WriteAllText("save.txt", "");
            string[] data = new string[charTable.Count];
            int i = 0;
            foreach (var entry in charTable)
            {
                data[i] = entry.Key + "-" + entry.Value.Race + "-" + entry.Value.Health + "-" + entry.Value.Coords.X + "-" + entry.Value.Coords.Y + "-" + entry.Value.Coords.Z + "-" + entry.Value.IsAlive + "-" + entry.Value.IsWet + ".";
                i++;
            }
            try
            {
                File.AppendAllLines("save.txt", data);
                Console.WriteLine("The data was written");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static Dictionary<string, Character> LoadFromFile(out string lastEntryName)
        {
            var charTable = new Dictionary<string, Character>();
            string text = string.Empty;
            lastEntryName = string.Empty;
            try
            {
                text = File.ReadAllText("save.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong when reading the save file");
                Console.WriteLine(ex);
            }
            text = Regex.Replace(text, @"\s+", ""); //replaces whitespaces with nutting

            string[] data = text.Split(".");
            try
            {
                foreach (string str in data)
                {
                    string[] charFields = str.Split("-");
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        charTable.Add(charFields[0], CreateCharacter(charFields[0], charFields[1]));
                        charTable[charFields[0]].Health = int.Parse(charFields[2]);
                        charTable[charFields[0]].Coords.X = int.Parse(charFields[3]);
                        charTable[charFields[0]].Coords.Y = int.Parse(charFields[4]);
                        charTable[charFields[0]].Coords.Z = int.Parse(charFields[5]);
                        charTable[charFields[0]].IsAlive = bool.Parse(charFields[6]);
                        charTable[charFields[0]].IsWet = bool.Parse(charFields[7]);
                        lastEntryName = charFields[0]; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong when trying to fill the characters data");
                Console.WriteLine(ex);
            }
            return charTable;
        }

        static void WhoIsAround(Dictionary<string, Character> dict)
        {
            if (dict.Count == 1)
            {
                Console.WriteLine("No one besides you is on the map");
                return;
            }
            Console.WriteLine("There are:");
            foreach (Character character in dict.Values)
            {
                Console.WriteLine($"{character.Name}, {character.Race}, has {character.Health} health");
            }
        }
        static void CheckForOtherOptions(string indicator, Character currentChar, Dictionary<string, Character> dict)
        {
            indicator = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(indicator);
            Character? character = CheckForNames(indicator);
            if (character != null)
            {
                ProcessCharacterChosen(currentChar, character);
            }

            Character? CheckForNames(string name)
            {
                if (dict.ContainsKey(name))
                {
                    return dict.GetValueOrDefault(name);
                }
                return null;
            }

        }
        static void ProcessCharacterChosen(Character currentChar, Character chosenCharacter)
        {
            Console.WriteLine($"You, {currentChar.Name}, chose {chosenCharacter.Name} for some action");
            Console.WriteLine($"1. Attack {chosenCharacter.Name}      2. Exit");
            ConsoleKeyInfo actionChoise = Console.ReadKey();
            switch (actionChoise.KeyChar)
            {
                case '1':
                    currentChar.BasicAttacks(chosenCharacter);
                    break;                 
                case '2':
                    break;
                default:
                    Console.WriteLine("\nInvalid answer input");
                    ProcessCharacterChosen(currentChar, chosenCharacter);
                    break;
            }
        }


        static void Main(string[] args)
        {
            Map map = new();
            var charTable = new Dictionary<string, Character>();
            bool sprintMode = false;
            string? lookCharByName;
            string? name;
            bool nameExists;
            Character? checkChar;
            Character? usedChar = null;
            

            //добавить метод, который при загрузке автоматически определит isAlive и isWet
            Instruct();
            string? indicator = string.Empty;
            while (indicator != "e")
            {
                map.DrawMapConsole();
                indicator = Console.ReadLine();
                switch (indicator)
                {
                    case "switch":
                        Console.WriteLine("Name of the char you're looking for is:");
                        lookCharByName = Console.ReadLine();
                        nameExists = charTable.TryGetValue(lookCharByName, out checkChar);
                        if (nameExists)
                        {
                            usedChar = checkChar;
                            Console.WriteLine($"You chose {usedChar.Name}");
                        }
                        else
                        {
                            Console.WriteLine("Character with that name doesn't exist");
                        }
                        break;
                    case "create":
                        name = CharacterCreation(out Character ch1);
                        charTable.Add(name, ch1);
                        nameExists = charTable.TryGetValue(name, out checkChar);
                        if (nameExists)
                        {
                            usedChar = checkChar;
                        }
                        break;
                    case "save":
                        SaveToFile(charTable);
                        break;
                    case "load":
                        charTable = LoadFromFile(out string lastEntryName);
                        nameExists = charTable.TryGetValue(lastEntryName, out checkChar);
                        if (nameExists)
                        {
                            usedChar = checkChar;
                            Console.WriteLine($"Your current character is {lastEntryName}");
                            WhoIsAround(charTable);
                        }
                        break;
                    case "who":
                        WhoIsAround(charTable);
                        break;
                    case "r":
                        sprintMode = !sprintMode;
                        string result = sprintMode ? "Sprint activated" : "Sprint deactivated";
                        Console.WriteLine(result);
                        break;
                    case "w":
                        if (usedChar is FlyingCharacter fly && usedChar.Coords.Surface == Surface.Air) //pattern matching
                        {
                            fly.FlyMove(Direction.YForward, sprintMode);
                            break;
                        } else
                        {
                            usedChar.Move(Direction.YForward, sprintMode);
                            break;
                        }
                    case "s":
                        if (usedChar is FlyingCharacter && usedChar.Coords.Surface == Surface.Air) //manual casting
                        {
                            ((FlyingCharacter)usedChar).FlyMove(Direction.YBackward, sprintMode);
                            break;
                        }
                        else
                        {
                            usedChar.Move(Direction.YBackward, sprintMode);
                            break;
                        }
                    case "a":
                        if (usedChar is FlyingCharacter && usedChar.Coords.Surface == Surface.Air)
                        {
                            ((FlyingCharacter)usedChar).FlyMove(Direction.XLeft, sprintMode);
                            break;
                        }
                        else
                        {
                            usedChar.Move(Direction.XLeft, sprintMode);
                            break;
                        }
                    case "d":
                        if (usedChar is FlyingCharacter && usedChar.Coords.Surface == Surface.Air)
                        {
                            ((FlyingCharacter)usedChar).FlyMove(Direction.XRight, sprintMode);
                            break;
                        }
                        else
                        {
                            usedChar.Move(Direction.XRight, sprintMode);
                            break;
                        }
                    case "up":
                        if (usedChar is FlyingCharacter && usedChar.Coords.Surface == Surface.Air)
                        {
                            ((FlyingCharacter)usedChar).FlyMove(Direction.ZUp, sprintMode);
                            break;
                        }
                        else if (usedChar is FlyingCharacter && usedChar.Coords.Surface == Surface.Ground)
                        {
                            ((FlyingCharacter)usedChar).Fly();
                            break;
                        }
                        else
                        {
                            usedChar.Move(Direction.ZUp, sprintMode);
                            break;
                        }
                    case "down":
                        if (usedChar is FlyingCharacter && usedChar.Coords.Surface == Surface.Air)
                        {
                            ((FlyingCharacter)usedChar).FlyMove(Direction.ZDown, sprintMode);
                            break;
                        }
                        else
                        {
                            usedChar.Move(Direction.ZDown, sprintMode);
                            break;
                        }
                    case "fly":
                        if (usedChar is FlyingCharacter)
                        {
                            ((FlyingCharacter)usedChar).Fly();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Your character can't fly");
                            break;
                        }
                    case "land":
                        if (usedChar is FlyingCharacter && usedChar.Coords.Surface == Surface.Air)
                        {
                            ((FlyingCharacter)usedChar).Land();
                            if (!usedChar.IsAlive)
                            {
                                Console.WriteLine("Your character died. Please, switch to another one");
                                goto case "switch";
                            }
                            break;
                        }
                        else
                        {
                            Console.WriteLine("You're already grounded, lad");
                            break;
                        }
                    case "where":
                        Console.WriteLine("Who are you looking for?");
                        lookCharByName = Console.ReadLine();
                        nameExists = charTable.ContainsKey(lookCharByName);
                        if (nameExists)
                        {
                            charTable[lookCharByName].Where();
                        }
                        else
                        {
                            Console.WriteLine("That name doesn't exist");
                        }
                        break;                   
                    case "info":
                        Console.WriteLine("Who are you looking for?");
                        lookCharByName = Console.ReadLine();
                        nameExists = charTable.ContainsKey(lookCharByName);
                        if (nameExists)
                        {
                            charTable[lookCharByName].Info();
                        }
                        else
                        {
                            Console.WriteLine("That name doesn't exist");
                        }
                        break;
                    default:
                        CheckForOtherOptions(indicator, usedChar, charTable);
                        break;
                }
            }
        }
    }
}
