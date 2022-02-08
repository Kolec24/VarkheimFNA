using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Squared.Tiled;

namespace Varkheim
{
    class ContentLoader
    {
        private struct TextureInfo
        {
            public string Name;
            public Texture2D Texture;
        }

        private struct MapInfo
        {
            public Map Map;
            public Tuple<int, int> Coordinates;

            public MapInfo(int Horizontal, int Vertical)
            {
                Map = null;
                Coordinates = new Tuple<int, int>(Horizontal, Vertical);
            }
        }

        private static List<TextureInfo> _Sprites = new List<TextureInfo>();
        private static List<MapInfo> _Maps = new List<MapInfo>();

        // Maybe useless but left for future
        private static void _PremultiplyTexture(Texture2D texture)
        {
            Color[] buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            }
            texture.SetData(buffer);
        }

        private static void _LoadSprites(ContentManager Content)
        {
            string PathString = Content.RootDirectory + "/sprites/";
            System.IO.DirectoryInfo Path = new System.IO.DirectoryInfo(PathString);
            Console.WriteLine(PathString);
            System.IO.FileInfo[] Files = Path.GetFiles();
            foreach (System.IO.FileInfo File in Files)
            {
                TextureInfo NewSprite = new TextureInfo();
                NewSprite.Name = File.Name;
                NewSprite.Texture = Content.Load<Texture2D>("sprites/" + File.Name);
                _PremultiplyTexture(NewSprite.Texture);
                _Sprites.Add(NewSprite);
            }
        }

        private static void _LoadMaps(ContentManager Content)
        {
            string PathString = Content.RootDirectory + "/levels/";
            System.IO.DirectoryInfo Path = new System.IO.DirectoryInfo(PathString);
            Console.WriteLine(PathString);
            System.IO.FileInfo[] Files = Path.GetFiles();
            foreach (System.IO.FileInfo File in Files)
            {
                int Horizontal = _Horizontal(File.Name);
                int Vertical = _Vertical(File.Name);
                MapInfo NewMap = new MapInfo(Horizontal, Vertical);
                NewMap.Map = Map.Load(Content.RootDirectory + "/levels/" + Horizontal + "x" + Vertical + ".tmx", Content);
                _Maps.Add(NewMap);
            }
        }

        public static void Load(ContentManager Content)
        {
            _LoadSprites(Content);
            _LoadMaps(Content);
        }

        public static Texture2D FindSprite(string FileName)
        {
            foreach(var SpriteInfo in _Sprites)
            {
                if (SpriteInfo.Name == FileName)
                {
                    return SpriteInfo.Texture;
                }
            }
            return null;
        }

        public static Map FindMap(int Horizontal, int Vertical)
        {
            foreach (var MapInfo in _Maps)
            {
                if (MapInfo.Coordinates.Item1 == Horizontal && MapInfo.Coordinates.Item2 == Vertical)
                {
                    return MapInfo.Map;
                }
            }
            return null;
        }

        private static int _Horizontal(string MapName)
        {
            string StringNumber = String.Empty;
            for(int i = 0; i< MapName.Length; i++)
            {
                if(MapName[i] == 'x')
                {
                    break;
                }

                StringNumber += MapName[i];
            }
            return int.Parse(StringNumber);
        }

        private static int _Vertical(string MapName)
        {
            string StringNumber = String.Empty;
            bool Before = true;
            for (int i = 0; i < MapName.Length; i++)
            {
                if (Before)
                {
                    if(MapName[i] == 'x')
                    {
                        Before = false;
                    }
                    continue;
                }

                if(MapName[i] == '.')
                {
                    break;
                }

                Before = false;
                StringNumber += MapName[i];
            }
            return int.Parse(StringNumber);
        }
    }
}
