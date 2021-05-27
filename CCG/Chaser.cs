using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CCG
{
    class Chaser : CollidableObject
    {
        bool movingClockwise;
        public static bool jumping;
        public static bool teleporting;
        bool reachedTeleportThreshold;

        int movementSpeed;
        float angleToPlayer;

        int teleportOrigin;
        int teleportDestination;

        Rectangle[] chaserBounds = new Rectangle[4];

        public Chaser(Texture2D texture) : base(texture)
        {
            movingClockwise = true;
            jumping = false;
            teleporting = false;
            reachedTeleportThreshold = false;

            movementSpeed = 8;
            rotationVelocity = 0.1f;
            position = new Vector2(Globals.screenWidth / 2, Globals.screenHeight - texture.Height / 2);

            chaserBounds[0] = new Rectangle(0, 0, Globals.screenWidth, texture.Height);
            chaserBounds[1] = new Rectangle(0, Globals.screenHeight - texture.Height, Globals.screenWidth, texture.Height);
            chaserBounds[2] = new Rectangle(0, 0, texture.Width, Globals.screenHeight);
            chaserBounds[3] = new Rectangle(Globals.screenWidth - texture.Width, 0, texture.Width, Globals.screenHeight);
        }

        public bool Teleporting
        {
            get { return teleporting; }
        }

        public override void Update()
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();
            
            if (/*keyState.IsKeyDown(Keys.R) && prevKeyState.IsKeyUp(Keys.R)*/EventTrigger.value == 1) { movingClockwise = !movingClockwise; }
            
            if (!jumping) { angleToPlayer = UpdateAngleToPalyer(); }

            if (movingClockwise) { Rotate(-rotationVelocity); }
            else { Rotate(rotationVelocity); }

            if (jumping) { Jump(); }
            else if (teleporting) { Teleport(); }
            else { MoveAutomatically(); }


            if (/*Mouse.GetState().LeftButton == ButtonState.Pressed*/EventTrigger.value == 3)
            {
                jumping = true;
            }

            if (/*Mouse.GetState().RightButton == ButtonState.Pressed*/EventTrigger.value == 2 || Weapon.active)
            {
                teleporting = true;
            }

            if (!Globals.screenRect.Intersects(AxisAlignedRectangle) && jumping)
            {
                jumping = false;
                EventTrigger.Reset();
                HoldWithinBounds();
            }
        }

        public override void Reset()
        {
            position = new Vector2(Globals.screenWidth / 2, Globals.screenHeight - texture.Height / 2);
            jumping = false;
            teleporting = false;
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        public void MoveAutomatically()
        {
            if (chaserBounds[0].Contains(AxisAlignedRectangle))
            {
                if (movingClockwise && position.X < Globals.screenEdges[3]) { MoveRight(movementSpeed); }
                else if (!movingClockwise && position.X > Globals.screenEdges[2]) { MoveLeft(movementSpeed); }
            }

            if (chaserBounds[1].Contains(AxisAlignedRectangle))
            {
                if (movingClockwise && position.X > Globals.screenEdges[2]) { MoveLeft(movementSpeed); }
                else if (!movingClockwise && position.X < Globals.screenEdges[3]) { MoveRight(movementSpeed); }
            }

            if (chaserBounds[2].Contains(AxisAlignedRectangle))
            {
                if (movingClockwise) { MoveUp(movementSpeed); }
                else { MoveDown(movementSpeed); }
            }

            if (chaserBounds[3].Contains(AxisAlignedRectangle))
            {
                if (movingClockwise) { MoveDown(movementSpeed); }
                else { MoveUp(movementSpeed); }
            }
        }

        public void Jump()
        {
            MoveRight((float)Math.Cos(angleToPlayer) * 30);
            MoveDown((float)Math.Sin(angleToPlayer) * 30);
        }

        public void Teleport()
        {
            if (!reachedTeleportThreshold) // Move chaser into wall
            {
                if (chaserBounds[0].Intersects(AxisAlignedRectangle))
                {
                    teleportOrigin = 1;
                    MoveUp(movementSpeed / 4);
                }

                if (chaserBounds[1].Intersects(AxisAlignedRectangle))
                {
                    teleportOrigin = 2;
                    MoveDown(movementSpeed / 4);
                }

                if (chaserBounds[2].Intersects(AxisAlignedRectangle))
                {
                    teleportOrigin = 3;
                    MoveLeft(movementSpeed / 4);
                }

                if (chaserBounds[3].Intersects(AxisAlignedRectangle))
                {
                    teleportOrigin = 4;
                    MoveRight(movementSpeed / 4);
                }
            }

            GenerateNewPos();

            if (!Globals.screenRect.Contains(AxisAlignedRectangle) && !Weapon.active) // Move chaser out of wall
            {
                switch (teleportDestination)
                {
                    case 1:
                        MoveDown(movementSpeed / 4);
                        break;
                    case 2:
                        MoveUp(movementSpeed / 4);
                        break;
                    case 3:
                        MoveRight(movementSpeed / 4);
                        break;
                    case 4:
                        MoveLeft(movementSpeed / 4);
                        break;
                }
            }

            if (Globals.screenRect.Contains(AxisAlignedRectangle)) // End the teleportation process
            {
                teleporting = false;
                reachedTeleportThreshold = false;
                teleportDestination = 0;
                EventTrigger.Reset();
            }
        }

        public void HoldWithinBounds()
        {
            if (position.Y < Globals.screenEdges[0])
            {
                position.Y = Globals.screenEdges[0];
                position.X = Math.Clamp(Globals.closestInteger((int)position.X, movementSpeed), Globals.screenEdges[2], Globals.screenEdges[3]);
            }

            if (position.Y > Globals.screenEdges[1])
            {
                position.Y = Globals.screenEdges[1];
                position.X = Math.Clamp(Globals.closestInteger((int)position.X, movementSpeed), Globals.screenEdges[2], Globals.screenEdges[3]);
            }

            if (position.X < Globals.screenEdges[2])
            {
                position.X = Globals.screenEdges[2];
                position.Y = Math.Clamp(Globals.closestInteger((int)position.Y, movementSpeed), Globals.screenEdges[0], Globals.screenEdges[1]);
            }

            if (position.X > Globals.screenEdges[3])
            {
                position.X = Globals.screenEdges[3];
                position.Y = Math.Clamp(Globals.closestInteger((int)position.Y, movementSpeed), Globals.screenEdges[0], Globals.screenEdges[1]);
            }
        }

        public void GenerateNewPos()
        {
            if (!Globals.screenRect.Intersects(AxisAlignedRectangle))
            {
                reachedTeleportThreshold = true;
                teleportDestination = Globals.rand.Next(1, 5);

                if (teleportOrigin == teleportDestination) { GenerateNewPos(); } // Stops the chaser from teleporting to the wall it's already on

                switch (teleportDestination)
                {
                    case 1:
                        position.X = Globals.closestInteger(Globals.rand.Next(Globals.screenEdges[2], Globals.screenEdges[3]), movementSpeed);
                        position.Y = Globals.screenEdges[0] - texture.Height;
                        break;
                    case 2:
                        position.X = Globals.closestInteger(Globals.rand.Next(Globals.screenEdges[2], Globals.screenEdges[3]), movementSpeed);
                        position.Y = Globals.screenEdges[1] + texture.Height;
                        break;
                    case 3:
                        position.Y = Globals.closestInteger(Globals.rand.Next(Globals.screenEdges[0], Globals.screenEdges[1]), movementSpeed);
                        position.X = Globals.screenEdges[2] - texture.Width;
                        break;
                    case 4:
                        position.Y = Globals.closestInteger(Globals.rand.Next(Globals.screenEdges[0], Globals.screenEdges[1]), movementSpeed);
                        position.X = Globals.screenEdges[3] + texture.Width;
                        break;
                }
            }
        }
    }
}
