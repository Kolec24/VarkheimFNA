using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Varkheim
{
    class ContentLoader
    {
        private struct SpriteInfo
        {
            public string Name;
            public Sprite Sprite;
        }

        private struct MapInfo
        {
            public Parser.Map Map;
            public Tuple<int, int> Coordinates;

            public MapInfo(int Horizontal, int Vertical)
            {
                Map = null;
                Coordinates = new Tuple<int, int>(Horizontal, Vertical);
            }
        }

        private static List<SpriteInfo> _Sprites = new List<SpriteInfo>();
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

        private static void _LoadSprites(ContentManager Content, GraphicsDevice GraphicsDevice)
        {
            string PathString = Content.RootDirectory + "/sprites/";
            System.IO.DirectoryInfo Path = new System.IO.DirectoryInfo(PathString);
            Console.WriteLine(PathString);
            System.IO.FileInfo[] Files = Path.GetFiles();
            foreach (System.IO.FileInfo File in Files)
            {
                SpriteInfo NewSprite = new SpriteInfo();
                NewSprite.Name = File.Name;
                NewSprite.Sprite = _AsepriteToSprite(new Parser.Aseprite(Content.RootDirectory + "/sprites/" + File.Name), Content, GraphicsDevice);
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
                NewMap.Map = Parser.Map.Load(Content.RootDirectory + "/levels/" + Horizontal + "x" + Vertical + ".tmx", Content);
                _Maps.Add(NewMap);
            }
        }

        public static void Load(ContentManager Content, GraphicsDevice GraphicsDevice)
        {
            _LoadSprites(Content, GraphicsDevice);
            _LoadMaps(Content);
        }

        public static Sprite FindSprite(string FileName)
        {
            foreach (var SpriteInfo in _Sprites)
            {
                if (SpriteInfo.Name == FileName)
                {
                    return SpriteInfo.Sprite;
                }
            }
            return null;
        }

        public static Parser.Map FindMap(int Horizontal, int Vertical)
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

        private static Sprite _AsepriteToSprite(Parser.Aseprite Aseprite, ContentManager Content, GraphicsDevice GraphicsDevice)
        {
            Sprite Sprite = new Sprite();
            if(Aseprite.Slices.Count() > 0 && Aseprite.Slices[0].Pivot.HasValue)
            {
                Sprite.Origin = new Vector2(Aseprite.Slices[0].Pivot.Value.X, Aseprite.Slices[0].Pivot.Value.Y);
            }

            for (int i = 0; i < Aseprite.Tags.Count(); i++)
            {
                Parser.Aseprite.Tag CurrentTag = Aseprite.Tags[i];
                Sprite.Animation NewAnimation;
                NewAnimation.Name = CurrentTag.Name;
                NewAnimation.Frames = new List<Sprite.Frame>();
                for (int j = CurrentTag.From; j <= CurrentTag.To; j++)
                {
                    Parser.Aseprite.Frame CurrentFrame = Aseprite.Frames[j];
                    Sprite.Frame NewFrame;

                    Texture2D NewTexture = new Texture2D(GraphicsDevice, Aseprite.Width, Aseprite.Height);
                    Color[] NewPixels = new Color[CurrentFrame.Pixels.Count()];
                    for(int p = 0; p < CurrentFrame.Pixels.Count(); p++)
                    {
                        NewPixels[p] = new Color(CurrentFrame.Pixels[p].R, CurrentFrame.Pixels[p].G, CurrentFrame.Pixels[p].B, CurrentFrame.Pixels[p].A);
                    }

                    NewTexture.SetData<Color>(NewPixels);
                    NewFrame.Texture = NewTexture;
                    NewFrame.Duration = (float)Aseprite.Frames[j].Duration / 1000;
                    NewAnimation.Frames.Add(NewFrame);
                }
                Sprite.Animations.Add(NewAnimation);
            }

            return Sprite;
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
