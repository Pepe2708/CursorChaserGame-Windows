using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCG
{
    class Nailboard : Weapon
    {
        bool isOnLeftSide;
        new bool active;

        public Nailboard(Texture2D texture) : base(texture) 
        {
            active = false;
            rotationVelocity = 0.04f;
            position.Y = Globals.screenHeight * 2;
            origin = new Vector2(0, texture.Height);
        }

        public override void Update()
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();

            if (/*keyState.IsKeyDown(Keys.N) && prevKeyState.IsKeyUp(Keys.N)*/EventTrigger.value == 4) 
            {
                EventTrigger.Reset();

                active = true;
                Weapon.active = true;

                position.X = Globals.RandPosX();
                isOnLeftSide = position.X < Globals.screenWidth / 2;

                if (isOnLeftSide) { Origin = new Vector2(0, texture.Height); }
                else { Origin = new Vector2(texture.Width, texture.Height); }
            }

            if (active)
            {
                if (position.Y > Globals.screenHeight)
                {
                    MoveUp(16);
                }
                else if (position.Y == Globals.screenHeight && MathHelper.ToDegrees(Rotation) < 90 && MathHelper.ToDegrees(Rotation) > -90)
                {
                    if (isOnLeftSide)
                    {
                        Rotate(rotationVelocity);
                    }
                    else
                    {
                        Rotate(-rotationVelocity);
                    }
                }
                else if (MathHelper.ToDegrees(Rotation) > 90 || MathHelper.ToDegrees(Rotation) < -90)
                {
                    Reset();
                }
            }

        }

        public override void Reset()
        {
            Rotation = 0;
            active = false;
            Weapon.active = false;
            position.Y = Globals.screenHeight * 2;
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }
    }
}
