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
                Collider.DamagingMasks.Add(Mask.Spike);

                Collider.BlockingMasks.Add(Mask.Solid);
                Collider.BlockingMasks.Add(Mask.Spike);
            }
            int Health = 1;
            float DeathTimer = 0.5F;
            bool StopGame = true;
            var Damage = new Damageable(Health, DeathTimer, StopGame);
            var Physics = new Physics();
            {
                Physics.MaxGroundSpeed = 70;
                Physics.MaxAirSpeed = 60;
                Physics.MaxFallingSpeed = 300;
                Physics.GroundAccel = 300;
                Physics.AirAccel = 200;
                Physics.Gravity = 700;
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
                Manager.AddComponent<Damageable>(Player, Damage);
                Manager.AddComponent<Physics>(Player, Physics);
                Manager.AddComponent<Player>(Player, PlayerComp);
                Manager.AddComponent<Shoot>(Player, Shooter);
                Manager.AddComponent<Teleport>(Player, Teleport);
                Manager.AddComponent<Animation>(Player, Animator);
            }
            return Player;
        }

        public static EntityHandle Soul(EntityManager Manager, Point NewPosition, int Facing, Vector2 NewVelocity, EntityHandle Owner)
        {
            var Position = new Position(NewPosition.X, NewPosition.Y);
            {
                Position.Facing.X = Facing;
            }
            var Mover = new Movement(NewVelocity);
            var Collider = new Collision(Mask.Soul, new Rectangle(-4, -16, 8, 16));
            {
                Collider.InteractableMasks.Add(Mask.Solid); // Toggle between teleporting to walls.

                Collider.DamagingMasks.Add(Mask.Solid);
                Collider.DamagingMasks.Add(Mask.Spike);

                Collider.BlockingMasks.Add(Mask.Solid);
                Collider.BlockingMasks.Add(Mask.Spike);
            }
            var Damage = new Damageable(1);
            var Soul = new Soul(Owner);
            var Animator = new Animation(ContentLoader.FindSprite("spirit.ase"), "Idle");

            EntityHandle Spirit = Manager.AddEntity();
            {
                Manager.AddComponent<Position>(Spirit, Position);
                Manager.AddComponent<Movement>(Spirit, Mover);
                Manager.AddComponent<Animation>(Spirit, Animator);
                Manager.AddComponent<Collision>(Spirit, Collider);
                Manager.AddComponent<Damageable>(Spirit, Damage);
                Manager.AddComponent<Soul>(Spirit, Soul);
            }
            return Spirit;
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
                switch(Layer.Key)
                {
                    case "Solids":
                        LayerMask = Mask.Solid;
                        break;
                    case "Spikes":
                        LayerMask = Mask.Spike;
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
