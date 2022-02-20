using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Collision : Component<Collision>
    {
        public Collision(int Mask, Rectangle Rect)
        {
            _Shape = ShapeType.Rect;
            _Rectangle = Rect;
            _Mask = Mask;
        }

        public Collision(int Mask, int Columns, int Rows, int TileSize, List<bool> Cells)
        {
            GridType NewGrid = new GridType();
            NewGrid.Columns = Columns;
            NewGrid.Rows = Rows;
            NewGrid.TileSize = TileSize;
            NewGrid.Cells = Cells;

            _Shape = ShapeType.Grid;
            _Grid = NewGrid;
            _Mask = Mask;
        }

        public enum ShapeType
        {
            Rect,
            Grid
        }

        public ShapeType Shape()
        {
            return _Shape;
        }

        public Rectangle Rectangle()
        {
            return _Rectangle;
        }

        public GridType Grid()
        {
            return _Grid;
        }

        public int Mask()
        {
            return _Mask;
        }

        public struct GridType
        {
            public int Columns;
            public int Rows;
            public int TileSize;
            public List<bool> Cells;
        }

        public List<Collision> Collisions = new List<Collision>();

        private int _Mask;
        private ShapeType _Shape;
        private Rectangle _Rectangle;
        private GridType _Grid;

        public List<int> InteractableMasks = new List<int>();
        public List<int> DamagingMasks = new List<int>();
        public List<int> BlockingMasks = new List<int>();
    }
}