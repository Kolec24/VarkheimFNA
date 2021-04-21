using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Varkheim
{
    class ContentLoader
    {
        private struct TextureInfo
        {
            public string Name;
            public Texture2D Texture;
        }

        private static List<TextureInfo> _Sprites = new List<TextureInfo>();
        private static List<TextureInfo> _Tilesets = new List<TextureInfo>();
        private static List<TextureInfo> _Subtextures = new List<TextureInfo>();

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

        private static void _LoadTilesets(ContentManager Content)
        {
            string PathString = Content.RootDirectory + "/tilesets/";
            System.IO.DirectoryInfo Path = new System.IO.DirectoryInfo(PathString);
            Console.WriteLine(PathString);
            System.IO.FileInfo[] Files = Path.GetFiles();
            foreach (System.IO.FileInfo File in Files)
            {
                TextureInfo NewTiletes = new TextureInfo();
                NewTiletes.Name = File.Name;
                NewTiletes.Texture = Content.Load<Texture2D>("tilesets/" + File.Name);
                _PremultiplyTexture(NewTiletes.Texture);
                _Tilesets.Add(NewTiletes);
            }
        }
        
        public static void Load(ContentManager Content)
        {
            _LoadSprites(Content);
            _LoadTilesets(Content);
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

        public static Texture2D FindTileset(string FileName)
        {
            foreach (var TilesetInfo in _Tilesets)
            {
                if (TilesetInfo.Name == FileName)
                {
                    return TilesetInfo.Texture;
                }
            }
            return null;
        }
    }
}
