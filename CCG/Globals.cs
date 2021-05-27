using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCG
{
    static class Globals
    {
        public static Random rand = new Random();

        public static int framesPassed = 0;
        
        public static int screenWidth = 720;
        public static int screenHeight = 720;

        public static int textureLength = 32;

        public static Rectangle screenRect = new Rectangle(0, 0, 720, 720);

        public static int[] screenEdges = new int[4]; // 1 = Top, 2 = Bottom, 3 = Left, 4 = Right
    
        
        public static void SetScreenEdges()
        {
            screenEdges[0] = textureLength / 2;
            screenEdges[1] = screenHeight - textureLength / 2;
            screenEdges[2] = textureLength / 2;
            screenEdges[3] = screenWidth - textureLength / 2;
        }
        
        public static int closestInteger(int a, int b) // Find closest number divisible by the movement speed.
        {
            int c1 = a - (a % b);
            int c2 = (a + b) - (a % b);
            if (a - c1 > c2 - a)
            {
                return c2;
            }
            else
            {
                return c1;
            }
        }

        public static int RandPosX()
        {
            return rand.Next(screenEdges[2], screenEdges[3]);
        }

        public static int RandPercentageOfScreen(int min, int max)
        {
            return rand.Next(min, max) * screenHeight / 100;
        }

        public static Texture2D createCircleTexture(int radius, GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new Texture2D(graphicsDevice, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
    }
}
