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
        public List<Character> CharacterList { get; set; }
        
    }


}
