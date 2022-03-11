using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ECS;

using EntityHandle = System.Object;

namespace Varkheim
{
    static class Factory
    {
        #region Factory

        public static EntityHandle Player(EntityManager Manager, Point NewPosition)
        {
            var Input = new Input();
            var Position = new Position(NewPosition.X, NewPosition.Y);
            var Mover = new Movement();
            Vector2 JumpVelocity = new Vector2(60, 100);
            float JumpMaxTimer = 0.2F;
            var Jumper = new Jump(JumpVelocity, JumpMaxTimer);
            var Collider = new Collision(Mask.Player, new Rectangle(-4, -16, 8, 16));
            {
                Collider.InteractableMasks.Add(Mask.Key);
                Collider.InteractableMasks.Add(Mask.Door);

                Collider.DamagingMasks.Add(Mask.Spike);

                Collider.BlockingMasks.Add(Mask.Solid);
                Collider.BlockingMasks.Add(Mask.Semisolid);
                Collider.BlockingMasks.Add(Mask.Door);
            }
            var Inventory = new Inventory();
            int Health = 1;
            float DeathTimer = 0.5F;
            bool StopGame = true;
            var Damage = new Damageable(Health, DeathTimer, StopGame);
            var Physics = new Physics();
            {
                Physics.MaxGroundSpeed = 70;
                Physics.MaxAirSpeed = 60;
                Physics.MaxFallingSpeed = 250;
                Physics.GroundAccel = 300;
                Physics.AirAccel = 200;
                Physics.Gravity = 500;
                Physics.WallGravity = 250;
                Physics.GroundFriction = 1000;
                Physics.AirFriction = 700;
            }
            var PlayerComp = new Player();
            Point ShootHorizontalOffset = new Point(0, 0);
            Point ShootVerticalOffset = new Point(0, 0);
            float ShootVelocity = 300;
            var Shooter = new Shoot(Projectile.Soul, ShootVelocity, ShootHorizontalOffset, ShootVerticalOffset);
            var Teleport = new Teleport();
            var Animator = new Animation(ContentLoader.FindSprite("player.ase"), "Idle");

            EntityHandle Player = Manager.AddEntity();
            {
                Manager.AddComponent<Input>(Player, Input);
                Manager.AddComponent<Position>(Player, Position);
                Manager.AddComponent<Movement>(Player, Mover);
                Manager.AddComponent<Jump>(Player, Jumper);
                Manager.AddComponent<Collision>(Player, Collider);
                Manager.AddComponent<Inventory>(Player, Inventory);
                Manager.AddComponent<Damageable>(Player, Damage);
                Manager.AddComponent<Physics>(Player, Physics);
                Manager.AddComponent<Player>(Player, PlayerComp);
                Manager.AddComponent<Shoot>(Player, Shooter);
                Manager.AddComponent<Teleport>(Player, Teleport);
                Manager.AddComponent<Animation>(Player, Animator);
            }
            return Player;
        }

        public static EntityHandle Soul(EntityManager Manager, Point NewPosition, Point Facing, Vector2 NewVelocity, EntityHandle Owner)
        {
            var Position = new Position(NewPosition.X, NewPosition.Y);
            {
                Position.Facing = Facing;
            }
            var Mover = new Movement(NewVelocity);
            var Collider = new Collision(Mask.Soul, new Rectangle(-4, -16, 8, 16));
            {
                Collider.InteractableMasks.Add(Mask.Solid); // Toggle between teleporting to walls.

                Collider.DamagingMasks.Add(Mask.Solid); // TODO: onsider if destroying should be handled in Teleport System instead of damage.
                Collider.DamagingMasks.Add(Mask.Spike);
                Collider.DamagingMasks.Add(Mask.Door);
            }
            var Damage = new Damageable(1);
            var SoulComp = new Soul(Owner);
            var Animator = new Animation(ContentLoader.FindSprite("soul.ase"), "Idle");

            EntityHandle Soul = Manager.AddEntity();
            {
                Manager.AddComponent<Position>(Soul, Position);
                Manager.AddComponent<Movement>(Soul, Mover);
                Manager.AddComponent<Animation>(Soul, Animator);
                Manager.AddComponent<Collision>(Soul, Collider);
                Manager.AddComponent<Damageable>(Soul, Damage);
                Manager.AddComponent<Soul>(Soul, SoulComp);
            }
            return Soul;
        }

        // TODO: BIG! Add destroy component, so removing entities always goes through destroy system (renamed death system).
        public static EntityHandle Key(EntityManager Manager, Point InPosition)
        {
            var Position = new Position(InPosition.X, InPosition.Y);
            var Collider = new Collision(Mask.Key, new Rectangle(-4, -8, 8, 8));
            var Collectible = new Collectible(Item.Key);
            var Animator = new Animation(ContentLoader.FindSprite("key.ase"), "Idle");

            EntityHandle Key = Manager.AddEntity();
            Manager.AddComponent<Position>(Key, Position);
            Manager.AddComponent<Collision>(Key, Collider);
            Manager.AddComponent<Collectible>(Key, Collectible);
            Manager.AddComponent<Animation>(Key, Animator);
            return Key;
        }

        public static EntityHandle Door(EntityManager Manager, Point InPosition)
        {
            var Position = new Position(InPosition.X, InPosition.Y);
            var Collider = new Collision(Mask.Door, new Rectangle(-12, -24, 24, 24));
            var Open = new Openable(Item.Key);
            var Animator = new Animation(ContentLoader.FindSprite("door.ase"), "Idle");

            EntityHandle Door = Manager.AddEntity();
            Manager.AddComponent<Position>(Door, Position);
            Manager.AddComponent<Collision>(Door, Collider);
            Manager.AddComponent<Openable>(Door, Open);
            Manager.AddComponent<Animation>(Door, Animator);
            return Door;
        }

        public static EntityHandle Tilemap(EntityManager Manager, Parser.Map NewMap, Point InPosition)
        {
            var Position = new Position(InPosition.X, InPosition.Y);
            var Tilemap = new Tilemap(NewMap);
            var Colliders = _MapCollisions(NewMap);
            EntityHandle Terrain = Manager.AddEntity();
            Manager.AddComponent<Position>(Terrain, Position);
            foreach(var Collider in Colliders)
            {
                Manager.AddComponent<Collision>(Terrain, Collider);
            }
            Manager.AddComponent<Tilemap>(Terrain, Tilemap);
            return Terrain;
        }

        #endregion

        #region HelperMethods

        private static List<Collision> _MapCollisions(Parser.Map Map)
        {
            List<Collision> Colliders = new List<Collision>();
            foreach (KeyValuePair<string, Parser.Layer> Layer in Map.Layers)
            {
                int LayerMask;
                if(Layer.Key == "Semisolids")
                {
                    //continue;
                }
                switch(Layer.Key)
                {
                    case "Solids":
                        LayerMask = Mask.Solid;
                        break;
                    case "Spikes":
                        LayerMask = Mask.Spike;
                        break;
                    case "Semisolids":
                        LayerMask = Mask.Semisolid;
                        break;
                    default:
                        LayerMask = Mask.NONE;
                        break;
                }

                List<bool> Cells = _Cells(Map, Layer.Value);
                Colliders.Add(new Collision(LayerMask, Map.Width, Map.Height, 8, Cells));
            }
            return Colliders;
        }

        private static List<bool> _Cells(Parser.Map Map, Parser.Layer Layer)
        {
            int Width = Map.Width;
            int Height = Map.Height;
            bool[] Cells = new bool[Width * Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Layer.Tiles[x + y * Width] > 0)
                        Cells[x + y * Width] = true;
                    else
                        Cells[x + y * Width] = false;
                }
            }
            return new List<bool>(Cells);
        }

        #endregion
    }
}
