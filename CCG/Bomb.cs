using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCG
{
    class Bomb : Weapon
    {
        int radius, minRadius, maxRadius, explosionSpeed, implosionSpeed;
        float opacity;

        new bool active;

        Texture2D[] circleTextures;
        GraphicsDevice graphicsDevice;
        
        public Bomb(Texture2D texture, GraphicsDevice graphicsDevice) : base(texture) 
        {
            this.graphicsDevice = graphicsDevice;

            active = false;
            reachedHeight = false;
            position.Y = Globals.screenHeight + texture.Height;

            radius = 32;

            implosionSpeed = 2;
            explosionSpeed = 32;
            minRadius = 4;
            maxRadius = 800;

            circleTextures = new Texture2D[maxRadius + 5];
            GenereateTextures(minRadius, maxRadius);

            Globals.framesPassed = 0;
            opacity = 1;
        }

        public override void Update()
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();

            if (/*keyState.IsKeyDown(Keys.B) && prevKeyState.IsKeyUp(Keys.B)*/EventTrigger.value == 6)
            {
                EventTrigger.Reset();

                active = true;
                Weapon.active = true;

                position.X = Globals.RandPosX();

                activationHeight = Globals.RandPercentageOfScreen(25, 75);
            }

            if (active)
            {
                reachedHeight = position.Y <= activationHeight;

                if (!reachedHeight)
                {
                    MoveUp(16);
                }
                else if (radius > minRadius && Globals.framesPassed < 30)
                {
                    radius -= implosionSpeed;
                    LoadTexture(Globals.createCircleTexture(radius, graphicsDevice));
                }
                else if (radius == minRadius && Globals.framesPassed < 30)
                {
                    Globals.framesPassed++;
                }
                else if (radius < maxRadius)
                {
                    radius += explosionSpeed;
                    LoadTexture(circleTextures[radius]);
                }
                else if (radius >= maxRadius && opacity > 0)
                {
                    opacity -= 1f / 60f;
                    if (opacity < 0) { opacity = 0; }
                }
                else if (opacity == 0)
                {
                    Reset();
                }
            }
        }

        public override void Reset()
        {
            radius = 32;
            Globals.framesPassed = 0;
            opacity = 1;

            LoadTexture(circleTextures[radius]);

            active = false;
            Weapon.active = false;
            position.Y = Globals.screenHeight + texture.Height;
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(texture, position, null, Color.White * opacity, rotation, origin, 1, SpriteEffects.None, 0);
        }

        public void GenereateTextures(int minRadius, int maxRadius)
        {
            for (int i = minRadius; i <= maxRadius + 4; i++)
            {
                circleTextures[i] = Globals.createCircleTexture(i, graphicsDevice);
            }
        }
    }
}
