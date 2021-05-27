using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCG
{
    class Weapon : CollidableObject
    {
        public static bool active;

        public int activationHeight;
        public bool reachedHeight;

        public Weapon(Texture2D texture) : base(texture) 
        {
            active = false;
        }
    }
}
