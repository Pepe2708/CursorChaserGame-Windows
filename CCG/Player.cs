using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCG
{
    class Player : CollidableObject
    {
        public static bool isAlive;
        
        public Player(Texture2D texture) : base(texture) 
        {
            isAlive = true;
        }

        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }

        public override void Update()
        {
            position = new Vector2(Mouse.GetState().X - texture.Width / 2, Mouse.GetState().Y - texture.Height / 2);
            position = Vector2.Clamp(position, boundsMin, boundsMax);
        }
    }
}
