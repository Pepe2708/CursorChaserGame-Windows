using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCG
{
    class Rocket : Weapon
    {
        new bool active;
        float angleToPlayer;

        public Rocket(Texture2D texture) : base(texture) 
        {
            active = false;
            reachedHeight = false;

            angleToPlayer = 0;

            position.Y = Globals.screenHeight + texture.Height;
        }

        public override void Update()
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();

            if (/*keyState.IsKeyDown(Keys.R) && prevKeyState.IsKeyUp(Keys.R)*/EventTrigger.value == 5)
            {
                EventTrigger.Reset();

                active = true;
                Weapon.active = true;

                position.X = Globals.RandPosX();

                activationHeight = Globals.RandPercentageOfScreen(10, 40);
            }

            if (active)
            {
                if (!reachedHeight)
                {
                    MoveUp(16);
                    reachedHeight = position.Y <= activationHeight;
                }
                else if (reachedHeight)
                {
                    Globals.framesPassed++;
                    
                    if (Globals.framesPassed <= 200)
                    {
                        angleToPlayer = UpdateAngleToPalyer();
                        rotation = angleToPlayer + MathHelper.ToRadians(90);
                    }

                    MoveRight((float)Math.Cos(angleToPlayer) * 20);
                    MoveDown((float)Math.Sin(angleToPlayer) * 20);

                    if (!Globals.screenRect.Intersects(BoundingRectangle))
                    {
                        Reset();
                    }
                }
            }
        }

        public override void Reset()
        {
            Globals.framesPassed = 0;
            reachedHeight = false;
            rotation = 0;

            active = false;
            Weapon.active = false;
            position.Y = Globals.screenHeight + texture.Height;
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }
    }
}
