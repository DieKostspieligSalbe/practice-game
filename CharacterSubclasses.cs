using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticeGame;

namespace PracticeGame
{
    internal class Human : Character
    {
        public override int SwimmingSpeed { get; protected set; } = 3;
        public override int PhysicalDamage { get; protected set; } = 3;

        public Human(string name) : base(name)
        {
        }

    }

    internal class Mermaid : Character
    {
        public override int SwimmingSpeed { get; protected set; } = 6;
        public override int SwimmingFastSpeed { get; protected set; } = 10;
        public override int WalkingSpeed { get; protected set; } = 2;
        public override int RunningSpeed { get; protected set; } = 3;
        public override Race Race { get; protected set; } = Race.Mermaid;
        public override int PhysicalDamage { get; protected set; } = 2;

        public Mermaid(string name) : base(name)
        {
        }
        public override void Move(Direction direction, bool isFast)
        {
            int speedmode = SwimmingSpeed;
            string movingVerb = "swims";
            if (isFast)
            {
                speedmode = SwimmingFastSpeed;
                movingVerb = "quickly swims";
            }
            if ((Coords.Z <= -1 && direction == Direction.ZDown) || (Coords.Z <= -1 && direction == Direction.ZUp))
            {
                IsWet = true;
                switch (direction)
                {
                    case Direction.ZDown:
                        Coords.Z -= speedmode;
                        OnCharAction($"{Name} {movingVerb} {speedmode} meters down", this);
                        break;
                    case Direction.ZUp:
                        Coords.Z += speedmode;
                        if (Coords.Z >= 0)
                        {
                            Coords.Surface = Surface.Ground;
                            Coords.Z = 0;
                            OnCharAction($"{Name} comes out of water", this);
                        }
                        else
                        {
                            OnCharAction($"{Name} {movingVerb} {speedmode} meters up", this);
                        }
                        break;
                }
            }
            else
            {
                base.Move(direction, isFast);
            }
        }
        public override void Info()
        {
            base.Info();
            OnCharAction($"Fast Swimming Speed: {SwimmingFastSpeed}", this);
        }

    }

    internal class Fae : Character.FlyingCharacter
    {
        public override int SwimmingSpeed { get; protected set; } = 2;
        public override Race Race { get; protected set; } = Race.Fae;
        public override int PhysicalDamage { get; protected set; } = 3;
        public override int FlyingSpeed { get; set; } = 3;
        public override int FlyingFastSpeed { get; set; } = 6;

        public Fae(string name) : base(name)
        {
        }

    }
    internal class Valkyrae : Character.FlyingCharacter
    {
        public override int SwimmingSpeed { get; protected set; } = 1;
        public override int RunningSpeed { get; protected set; } = 4;
        public override Race Race { get; protected set; } = Race.Valkyrae;
        public override int PhysicalDamage { get; protected set; } = 4;
        public override int FlyingSpeed { get; set; } = 5;
        public override int FlyingFastSpeed { get; set; } = 10;
        public Valkyrae(string name) : base(name)
        {

        }


    }

}
