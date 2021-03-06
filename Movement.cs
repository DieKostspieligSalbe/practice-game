using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PracticeGame.Character;

namespace PracticeGame
{
    public enum Surface
    {
        Ground,
        Water,
        Air,
        Ice,
        Fire
    }

    public enum BorderCell
    {
        LeftBC,
        RightBC,
        TopBC,
        BottomBC,
        NotBC
    }

    public enum Direction
    {
        YForward,
        YBackward,
        XRight,
        XLeft,
        ZUp,
        ZDown
    }

    public enum MovementType
    {
        GroundMovement,
        Flight
    }

    internal class Coords
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;
        public Surface Surface { get; set; } = Surface.Ground;

        public Coords()
        {

        }
        public Coords(int x, int y, int z, Surface surface)
        {
            X = x;
            Y = y;
            Z = z;
            Surface = surface;
        }

    }

    internal class Map
    {
        public List<Character>? CharacterList { get; set; } = new();
        public List<Cell>? CellList { get; set; } = new();
        private readonly int _size;
        private readonly int _middleCellIndex;
        public Map()
        {
            _size = 15;
            _middleCellIndex = _size * _size / 2;
            MakeMap();
        }

        public Map(int size)
        {
            if (size < 3)
            {
                _size = 15;
            }
            else if (size % 2 == 0)
            {
                _size = ++size;
            }
            else
            {
                _size = size;
            }
            _middleCellIndex = _size * _size / 2;
            MakeMap();
        }

        private void MakeMap()
        {
            for (int i = 0; i < _size * _size; i++)
            {
                CellList?.Add(new Cell());
                CellList![i].indexInCurrentList = i;
            }
            GiveCellCoords();
            GenerateCellSurface();
        }
        
        public void DrawMapConsole()
        {
            char surfaceSymbol;
            Console.WriteLine("\nMap:");
            for (int i = 0; i < CellList?.Count; i++)
            {
                if (CellList[i].CellCharacterList?.Count > 0)
                {
                    DrawCellWithCharacter(CellList[i]);
                }
                else
                {
                    DrawCell(CellList[i]);
                }
                if ((i + 1) % _size == 0)
                {
                    Console.WriteLine();
                }
            }

            void DrawCell(Cell cell)
            {
                surfaceSymbol = cell.CellCoords.Surface == Surface.Water ? '~' : '_';
                Console.Write($"|{surfaceSymbol}{surfaceSymbol}{surfaceSymbol}|");
            }

            void DrawCellWithCharacter(Cell cell)
            {
                char nameFirstLetter = cell.CellCharacterList![0].Name[0];
                surfaceSymbol = cell.CellCoords.Surface == Surface.Water ? '~' : '_';
                Console.Write($"|{surfaceSymbol}{nameFirstLetter}{surfaceSymbol}|");
            }
        }
        private void GiveCellCoords()
        {
            
            int xMin; int yMax;
            yMax = _size / 2;
            xMin = ~yMax + 1;
            
            FindBorderCells();
            AssignCoordinates();
            FindNeighborCells();

            void FindBorderCells()
            {
                for (int i = 0; i < CellList!.Count; i++)
                {
                    if (i < _size)
                    {
                        CellList[i].BorderCellState = BorderCell.TopBC;
                    }
                    if (i > CellList.Count - _size)
                    {
                        CellList[i].BorderCellState = BorderCell.BottomBC;
                    }
                    if (i % _size == 0)
                    {
                        CellList[i].BorderCellState = BorderCell.LeftBC;
                    }
                    if ((i + 1) % _size == 0)
                    {
                        CellList[i].BorderCellState = BorderCell.RightBC;
                    }
                }
            }
            void AssignCoordinates()
            {
                for (int i = 0, x = xMin, y = yMax; i < CellList!.Count; i++)
                {
                    if (i != 0 && CellList[i].BorderCellState == BorderCell.LeftBC) //resets coordinates when a new row is approached
                    {
                        x = xMin;
                        y--;
                        CellList[i].CellCoords.X = x++;
                        CellList[i].CellCoords.Y = y;
                    }
                    else //fills the rest of the cells in the row
                    {
                        CellList[i].CellCoords.X = x;
                        CellList[i].CellCoords.Y = y;
                        x++;
                    }
                }
            }
            void FindNeighborCells()
            {
                for (int i = 0; i < CellList!.Count; i++)
                {
                    if (CellList[i].BorderCellState == BorderCell.RightBC)
                    {
                        if (i > CellList.Count - _size)
                        {
                            continue; //to avoid right bottom cell
                        }
                        CellList[i].NeighborCells.Add(CellList[i + _size]);
                        continue;
                    }
                    if (CellList[i].BorderCellState == BorderCell.BottomBC || i == CellList.Count - _size)
                    {
                        CellList[i].NeighborCells.Add(CellList[i + 1]);
                        continue;
                    }
                    CellList[i].NeighborCells.Add(CellList[i + 1]);
                    CellList[i].NeighborCells.Add(CellList[i + _size]);
                }
            }
        }
        private void GenerateCellSurface() //make it better someday
        {
            Random rand = new();
            for (int i = 0; i < CellList!.Count; i++)
            {
                if (CellList[i].NeighborCells.Count > 0 && rand.NextDouble() >= 0.8)
                {
                    CellList[i].NeighborCells[0].CellCoords.Surface = Surface.Water;
                    if (CellList[i].NeighborCells.Count > 1 && rand.NextDouble() >= 0.8)
                    {
                        CellList[i].NeighborCells[0].CellCoords.Surface = Surface.Water;
                    }
                }
            }
            CellList![_middleCellIndex].CellCoords.Surface = Surface.Ground;
        }
        public void AddCharacter(Character ch)
        {
            CharacterList?.Add(ch);
            CellList![_middleCellIndex].CellCharacterList?.Add(ch);
            ch.Coords.Surface = CellList![_middleCellIndex].CellCoords.Surface;
            if (ch.Coords.Surface == Surface.Water)
            {
                ch.IsWet = true;
            }
            ch.Cell = CellList![_middleCellIndex];
        }
        public void AddCharacterToOtherCell(Character ch)
        {
            CharacterList?.Add(ch);
            int charCellIndex = CellList!.Where(x => x.CellCoords.X == ch.Coords.X).Where(x => x.CellCoords.Y == ch.Coords.Y).Single().indexInCurrentList;
            Cell charCell = CellList![charCellIndex];
            ch.Coords = new(charCell.CellCoords.X, charCell.CellCoords.Y, charCell.CellCoords.Z, charCell.CellCoords.Surface);
            if (ch.Coords.Surface == Surface.Water)
            {
                ch.IsWet = true;
            }
            ch.Cell = CellList![charCellIndex];
            CellList![charCellIndex].CellCharacterList?.Add(ch);
        }
        public void CharacterMovementUpdate(Character ch, Direction direction, bool isFast, MovementType movementType)
        {
            Coords initicalCoords = new(ch.Coords.X, ch.Coords.Y, ch.Coords.Z, ch.Coords.Surface); //maybe replace to Clone 
            int currentCellIndex = CellList!.Where(x => x.CellCoords.X == ch.Coords.X).Where(x => x.CellCoords.Y == ch.Coords.Y).Single().indexInCurrentList;

            bool moveSuccess;
            string message = string.Empty;
            if (movementType == MovementType.GroundMovement)
            {
                moveSuccess = ch.Move(direction, isFast, out message);
            }
            else
            {
                moveSuccess = ((FlyingCharacter)ch).FlyMove(direction, isFast, out message);
            }
            if (moveSuccess)
            {
                int? newCellIndex = CellList!.Where(x => x.CellCoords.X == ch.Coords.X).Where(x => x.CellCoords.Y == ch.Coords.Y).SingleOrDefault()?.indexInCurrentList;

                if (newCellIndex is null)
                {
                    ch.OnCharAction($"The cell you want to move to is outside the bounds of the map. {ch.Name} remains in the same position", ch);
                    ch.MoveCharacterBack(initicalCoords);
                }
                else
                {
                    CellList![currentCellIndex].CellCharacterList!.RemoveAll(x => x == ch);
                    CellList![(int)newCellIndex].CellCharacterList?.Add(ch);
                    if (ch.Coords.Surface != Surface.Air)
                    {
                        ch.Coords.Surface = CellList![(int)newCellIndex].CellCoords.Surface;
                    }
                    if (ch.Coords.Surface == Surface.Water)
                    {
                        ch.IsWet = true;
                    }
                    ch.Cell = CellList![(int)newCellIndex];
                    ch.OnCharAction(message, ch);
                }                                       
            }
        }

    
    }
    //add map size to save file and changing char coor if smaller
    //save a map to the file
    //add surface check to char when changes a cell or smth
    //change messages when switching ground to water and vv

    internal class Cell
    {
        public List<Character>? CellCharacterList { get; set; } = new();
        public Coords CellCoords { get; set; } = new();
        public BorderCell BorderCellState { get; set; } = BorderCell.NotBC;
        public List<Cell> NeighborCells { get; set; } = new();
        public int indexInCurrentList;

        public Cell()
        {
            CellCoords = new Coords();
        }



    }

}
