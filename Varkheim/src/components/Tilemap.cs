using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Tilemap : Component<Tilemap>
    {
        public Tilemap(int Columns, int Rows, int TileWidth, int TileHeight, List<bool> Cells)
        {
            _Columns = Columns;
            _Rows = Rows;
            _TileWidth = TileWidth;
            _TileHeight = TileHeight;
            _Cells = Cells;
        }

        public int Columns()
        {
            return _Columns;
        }

        public int Rows()
        {
            return _Rows;
        }

        public int TileWidth()
        {
            return _TileWidth;
        }

        public int TileHeight()
        {
            return _TileHeight;
        }

        public List<bool> Cells()
        {
            return _Cells;
        }

        private int _Columns;
        private int _Rows;
        private int _TileWidth;
        private int _TileHeight;
        private List<bool> _Cells;
    }
}
