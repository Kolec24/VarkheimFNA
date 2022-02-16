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
            var Jumper = new Jump(120, 0.2F);
            var Collider = new Collision(Mask.Player, new Rectangle(-4, -16, 8, 16));
            {
                Collider.BlockingMasks.Add(Mask.Solid);
            }
            var Physics = new Physics();
            {
                Physics.MaxGroundSpeed = 70;
                Physics.MaxAirSpeed = 60;
                Physics.MaxFallingSpeed = 300;
                Physics.GroundAccel = 300;
                Physics.AirAccel = 200;
                Physics.Gravity = 700;
                Physics.WallGravity = 400;
                Physics.GroundFriction = 1000;
                Physics.AirFriction = 700;
            }
            var PlayerComp = new Player();
            var Shooter = new Shoot(Projectile.Spirit, new Point(0, -8), 200);
            var Teleport = new Teleport();
            var Animator = new Animation(ContentLoader.FindSprite("player.ase"), "Idle");

            EntityHandle Player = Manager.AddEntity();
            {
                Manager.AddComponent<Input>(Player, Input);
                Manager.AddComponent<Position>(Player, Position);
                Manager.AddComponent<Movement>(Player, Mover);
                Manager.AddComponent<Jump>(Player, Jumper);
                Manager.AddComponent<Collision>(Player, Collider);
                Manager.AddComponent<Physics>(Player, Physics);
                Manager.AddComponent<Player>(Player, PlayerComp);
                Manager.AddComponent<Shoot>(Player, Shooter);
                Manager.AddComponent<Teleport>(Player, Teleport);
                Manager.AddComponent<Animation>(Player, Animator);
            }
            return Player;
        }

        public static EntityHandle Spirit(EntityManager Manager, Point NewPosition, int Facing, Vector2 NewVelocity, Point Offset, EntityHandle Owner)
        {
            var Position = new Position(NewPosition.X, NewPosition.Y);
            {
                Position.Facing = Facing;
            }
            var Mover = new Movement(NewVelocity);
            var Collider = new Collision(Mask.Spirit, new Rectangle(-4, -8, 8, 16));
            {
                Collider.InteractableMasks.Add(Mask.Solid);
                Collider.BlockingMasks.Add(Mask.Solid);
            }
            var SpiritComp = new Spirit(Owner, Offset);
            var Animator = new Animation(ContentLoader.FindSprite("spirit.ase"), "Idle");

            EntityHandle Spirit = Manager.AddEntity();
            {
                Manager.AddComponent<Position>(Spirit, Position);
                Manager.AddComponent<Movement>(Spirit, Mover);
                Manager.AddComponent<Animation>(Spirit, Animator);
                Manager.AddComponent<Collision>(Spirit, Collider);
                Manager.AddComponent<Spirit>(Spirit, SpiritComp);
            }
            return Spirit;
        }

        public static EntityHandle Tilemap(EntityManager Manager, Parser.Map NewMap, Point Room)
        {
            var Position = new Position(Room.X * 320, Room.Y * 240);
            var Tilemap = new Tilemap(NewMap);
            var SolidCells = _SolidCells(NewMap);
            var Collider = new Collision(Mask.Solid, NewMap.Width, NewMap.Height, 8, SolidCells);
            EntityHandle Terrain = Manager.AddEntity();
            Manager.AddComponent<Position>(Terrain, Position);
            Manager.AddComponent<Collision>(Terrain, Collider);
            Manager.AddComponent<Tilemap>(Terrain, Tilemap);
            return Terrain;
        }

        #endregion

        #region HelperMethods

        private static List<bool> _SolidCells(Parser.Map Map)
        {
            int Width = Map.Width;
            int Height = Map.Height;
            bool[] Cells = new bool[Width * Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Map.Layers["Solid"].Tiles[x + y * Width] > 0)
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
