using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeGame
{
    internal interface IFly
    {
        int FlyingSpeed { get; set; }
        int FlyingFastSpeed { get; set; }
        void Fly();
        void FlyMove(Direction direction, bool isFast);
        void Land();

    }

    public enum Race
    {
        Human,
        Mermaid,
        Fae,
        Valkyrae
    }

    internal abstract class Character
    {

        public virtual event Action<string, Character>? MessageCharAction;
        public virtual event Action<string, Character, Character>? MessageTwoCharAction;
        public virtual event Action<string, Character, Character[]>? MessageManyCharAction;
        public int Health { get; set; } = 100;
        public Coords Coords { get; protected set; } = new Coords();
        public string Name { get; protected set; }
        public virtual int SwimmingSpeed { get; protected set; } = 1;
        public virtual int SwimmingFastSpeed { get; protected set; }
        public virtual int WalkingSpeed { get; protected set; } = 3;
        public virtual int RunningSpeed { get; protected set; } = 5;
        public virtual Race Race { get; protected set; } = Race.Human;
        public virtual int PhysicalDamage { get; protected set; }
        public bool IsAlive { get; set; } = true;
        public bool IsWet { get; set; } = false;

        public virtual void OnCharAction(string message, Character character)
        {        
            MessageCharAction?.Invoke(message, this);
        }
        public Character(string name)
        {
            Name = name;
        }
        public void Where()
        {
            MessageCharAction?.Invoke($"Location: {Coords.Surface}. \nCoordinate X - {Coords.X}, Coordinate Y - {Coords.Y}, Coordinate Z - {Coords.Z}", this);
        }
        public virtual void Info()
        {
            MessageCharAction?.Invoke($"Name: {Name}, Race: {Race}, Health: {Health} \nWalking speed: {WalkingSpeed}, Running Speed: {RunningSpeed} \nSwimming speed: {SwimmingSpeed}", this);
        }
        public virtual void MoveCharacterBack(Coords coords)
        {
            this.Coords = coords;
        }
        public virtual bool Move(Direction direction, bool isFast)
        {
            string movingVerb = "";
            bool success = true;
            int distanceModifier = 0;
            string directionLocal = "";
            switch (Coords.Surface)
            {
                case Surface.Ground:
                    if (!isFast)
                    {
                        movingVerb = "walks";
                        distanceModifier = WalkingSpeed;
                    }
                    else
                    {
                        movingVerb = "runs";
                        distanceModifier = RunningSpeed;
                    }
                    break;
                case Surface.Water:
                    if (!isFast)
                    {
                        movingVerb = "swims";
                        distanceModifier = SwimmingSpeed;
                    }
                    else
                    {
                        movingVerb = "quickly swims";
                        distanceModifier = SwimmingFastSpeed;
                    }
                    break;
            }
            switch (direction)
            {
                case Direction.YForward:
                    Coords.Y += distanceModifier;
                    directionLocal = "forward";
                    break;
                case Direction.YBackward:
                    Coords.Y -= distanceModifier;
                    directionLocal = "backward";
                    break;
                case Direction.XLeft:
                    Coords.X -= distanceModifier;
                    directionLocal = "left";
                    break;
                case Direction.XRight:
                    Coords.X += distanceModifier;
                    directionLocal = "right";
                    break;
                case Direction.ZUp:
                    if (Coords.Z == -1 && Coords.Surface == Surface.Water)
                    {
                        Coords.Z++;
                        Coords.Surface = Surface.Ground;
                        MessageCharAction?.Invoke($"{Name} comes out of water", this);
                        return true;
                    }
                    else
                    {
                        MessageCharAction?.Invoke($"You cannot move up from there", this);
                        return false;
                    }
                case Direction.ZDown:
                    if (Coords.Z == 0 && Coords.Surface == Surface.Ground)
                    {
                        Coords.Z--;
                        Coords.Surface = Surface.Water;
                        IsWet = true;
                        MessageCharAction?.Invoke($"{Name} enters the water", this);
                        return true;
                    }
                    else
                    {
                        MessageCharAction?.Invoke("You cannot move down from there", this);
                        return false;
                    }

            }
            if (success)
            {
                MessageCharAction?.Invoke($"{Name} {movingVerb} {distanceModifier} meters {directionLocal}", this);
                return true;
            }
            return false;
        }

        public bool IsClose(Character char2, int distance = 2)
        {
            return ((Math.Abs(this.Coords.X - char2.Coords.X) < distance) && (Math.Abs(this.Coords.Y - char2.Coords.Y) < distance) && (Math.Abs(this.Coords.Z - char2.Coords.Z) < distance));
        }
        public virtual void BasicAttacks(Character attackedChar, int distance = 2)
        {
            if (this.IsClose(attackedChar, distance))
            {
                attackedChar.TakesDamage(this.PhysicalDamage, this);
                MessageTwoCharAction?.Invoke($"\n{Name} deals {PhysicalDamage} damage to {attackedChar.Name}", this, attackedChar);
                return;
            }
            MessageTwoCharAction?.Invoke($"\n{Name} is too far from {attackedChar.Name}", this, attackedChar);
        }

        public virtual void TakesDamage(int damageDealt, Character attacker)
        {
            this.Health -= damageDealt;
            if (this.Health <= 0)
            {
                Health = 0;
                IsAlive = false;
                MessageTwoCharAction?.Invoke($"\n{Name} takes fatal damage from {attacker.Name}", this, attacker);
                return;
            }
            MessageTwoCharAction?.Invoke($"\n{Name} takes {damageDealt} damage from {attacker.Name}", this, attacker);
        }


    internal abstract class FlyingCharacter : Character, IFly
    {
        public FlyingCharacter(string name) : base(name)
        {

        }
        public virtual int FlyingSpeed { get; set; }
        public virtual int FlyingFastSpeed { get; set; }

        public void Fly()
        {
            if (IsWet)
            {
               MessageCharAction?.Invoke($"{Name} cannot fly while wet", this);
            } 
            else
            {
               Coords.Surface = Surface.Air;
               Coords.Z = 1;
               MessageCharAction?.Invoke($"{Name} starts flying", this);
            }           
        }

        public void FlyMove(Direction direction, bool isFast)
        {
            string directionLocal = string.Empty;
            int speedMode = FlyingSpeed;
            if (isFast)
            {
                speedMode = FlyingFastSpeed;
            }
            switch (direction)
            {
                case Direction.YForward:
                    Coords.Y += speedMode;
                    directionLocal = "forward";
                    break;
                case Direction.YBackward:
                    Coords.Y -= speedMode;
                    directionLocal = "backward";
                    break;
                case Direction.XLeft:
                    Coords.X -= speedMode;
                    directionLocal = "left";
                    break;
                case Direction.XRight:
                    Coords.X += speedMode;
                    directionLocal = "right";
                    break;
                case Direction.ZUp:
                    Coords.Z += speedMode;
                    directionLocal = "up";
                    break;
                case Direction.ZDown:
                    Coords.Z -= speedMode;
                    directionLocal = "down";
                    break;
            }
            if (Coords.Z <= 0)
            {
                Land();             
            }
            else
            {
                MessageCharAction?.Invoke($"{Name} flies {speedMode} meters {directionLocal}", this);
            }
        }

        public void Land()
        {
            Coords.Surface = Surface.Ground;
            MessageCharAction?.Invoke($"{Name} lands", this);
            if (Coords.Z > 2)
            {
                Health -= Coords.Z * 10;
                if (Health <= 0)
                {
                    IsAlive = false;
                    Health = 0;
                    MessageCharAction?.Invoke($"{Name} takes fatal fall damage falling from {Coords.Z} meters", this);
                }
                else
                {
                    MessageCharAction?.Invoke($"{Name} takes {Coords.Z * 10} fall damage and is left with {Health} health", this);
                }
            }
            Coords.Z = 0;
        }
        public override void Info()
        {
            base.Info();
            MessageCharAction?.Invoke($"Flying speed: {FlyingSpeed}, Fast flying speed: {FlyingFastSpeed}", this);
        }
    }
    }
}
