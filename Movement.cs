using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    internal class Coords
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;
        public Surface Surface { get; set; } = Surface.Ground;

    }

    internal class Map
    {
        public List<Character>? CharacterList { get; set; } = new();
        public List<Cell>? CellList { get; set; } = new();
        private int _size;
        public Map()
        {
            _size = 5;
            for (int i = 0; i < _size * _size; i++)
            {
                CellList?.Add(new Cell());
            }
            GiveCellCoords();
        }
        
        public void DrawMapConsole()
        {
            Console.WriteLine("Map:");
            for (int i = 0; i < CellList?.Count; i++)
            {
                if (CellList[i].CellCharacter != null )
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
                Console.Write($"|___|");
            }

            void DrawCellWithCharacter(Cell cell)
            {
                char nameFirstLetter = cell.CellCharacter!.Name[0];
                Console.Write($"|_{nameFirstLetter}_|");
            }
        }
        private void GiveCellCoords()
        {
            int middleCellIndex = CellList.Count / 2;
            Cell middleCell = CellList[middleCellIndex];
            

            int xMin; int yMax;
            yMax = _size / 2;
            xMin = ~yMax + 1;
            

            middleCell.CellCoords.X = 0; middleCell.CellCoords.Y = 0;
            FindBorderCells();

            for (int i = 0, x = xMin, y = yMax; i < CellList.Count; i++)
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

            void FindBorderCells()
            {
                for (int i = 0; i < CellList.Count; i++)
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


        }
    }

    internal class Cell
    {
        public Character? CellCharacter { get; set; } = null;
        public Coords CellCoords { get; set; } = new();
        public Surface CellSurface { get; set; } = Surface.Ground;
        public BorderCell BorderCellState { get; set; } = BorderCell.NotBC;

        public Cell()
        {
            CellCoords = new Coords();
        }

    }

}
