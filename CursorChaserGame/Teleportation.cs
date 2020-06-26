using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CursorChaserGame
{
    class Teleportation
    {
        static int chaserTeleportationOrigin;
        static int chaserTeleportationDestination;
        public static bool chaserOutOfScreen = false;


        public static void MoveChaserIntoWall(Random randNum, CollidableObject chaser, int screenWidth, int screenHeight, Rectangle[] chaserRails, int defaultTextureRadius)
        {
            if (chaserRails[0].Contains(chaser.ObjectRectangle) || chaser.Position.Y < defaultTextureRadius + 1 && chaserOutOfScreen == false)
            {
                chaserTeleportationOrigin = 0;
                chaser.MoveUp(1);
            }

            if ((int)chaser.Position.Y == -defaultTextureRadius - 1)
            {
                chaserOutOfScreen = true;
                GenerateNewChaserPosition(randNum, chaser, screenWidth, screenHeight, defaultTextureRadius);
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (chaserRails[1].Contains(chaser.ObjectRectangle) || chaser.Position.X < defaultTextureRadius + 1 && chaserOutOfScreen == false)
            {
                chaserTeleportationOrigin = 1;
                chaser.MoveLeft(1);
            }

            if ((int)chaser.Position.X == -defaultTextureRadius - 1)
            {
                chaserOutOfScreen = true;
                GenerateNewChaserPosition(randNum, chaser, screenWidth, screenHeight, defaultTextureRadius);
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (chaserRails[2].Contains(chaser.ObjectRectangle) || chaser.Position.Y > screenHeight - defaultTextureRadius - 1 && chaserOutOfScreen == false)
            {
                chaserTeleportationOrigin = 2;
                chaser.MoveDown(1);
            }

            if ((int)chaser.Position.Y == screenHeight + defaultTextureRadius + 1)
            {
                chaserOutOfScreen = true;
                GenerateNewChaserPosition(randNum, chaser, screenWidth, screenHeight, defaultTextureRadius);
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (chaserRails[3].Contains(chaser.ObjectRectangle) || chaser.Position.X > screenWidth - defaultTextureRadius - 1 && chaserOutOfScreen == false)
            {
                chaserTeleportationOrigin = 3;
                chaser.MoveRight(1);
            }

            if ((int)chaser.Position.X == screenWidth + defaultTextureRadius + 1)
            {
                chaserOutOfScreen = true;
                GenerateNewChaserPosition(randNum, chaser, screenWidth, screenHeight, defaultTextureRadius);
            }
        }



        public static void GenerateNewChaserPosition(Random randNum, CollidableObject chaser, int screenWidth, int screenHeight, int defaultTextureRadius)
        {
            chaserTeleportationDestination = randNum.Next(0, 4);

            if (chaserTeleportationOrigin == chaserTeleportationDestination) 
            {
                chaserTeleportationDestination = randNum.Next(0, 4);
            }

            // The above code is used to make sure that the chaser doesn't teleport near its current location

            if (chaserTeleportationDestination == 0 && chaserTeleportationOrigin != chaserTeleportationDestination)
            {
                chaser.Position = new Vector2(RandomPosX(randNum, defaultTextureRadius, screenWidth), -defaultTextureRadius - 1);
            }
            else if (chaserTeleportationDestination == 1 && chaserTeleportationOrigin != chaserTeleportationDestination)
            {
                chaser.Position = new Vector2(-defaultTextureRadius - 1, RandomPosY(randNum, defaultTextureRadius, screenHeight));
            }
            else if (chaserTeleportationDestination == 2 && chaserTeleportationOrigin != chaserTeleportationDestination)
            {
                chaser.Position = new Vector2(RandomPosX(randNum, defaultTextureRadius, screenWidth), screenHeight + defaultTextureRadius + 1);
            }
            else if (chaserTeleportationDestination == 3 && chaserTeleportationOrigin != chaserTeleportationDestination)
            {
                chaser.Position = new Vector2(screenWidth + defaultTextureRadius + 1, RandomPosY(randNum, defaultTextureRadius, screenHeight));
            }
        }

        public static void MoveChaserOutOfWall(CollidableObject chaser, int screenWidth, int screenHeight, Rectangle[] chaserRails, int defaultTextureRadius, bool weaponActive)
        {
            if (chaserTeleportationOrigin != chaserTeleportationDestination && weaponActive == false)
            {
                if (chaser.Position.Y < defaultTextureRadius + 1 && chaserOutOfScreen == true)
                {
                    chaser.MoveDown(1);
                }

                if (chaser.Position.X < defaultTextureRadius + 1 && chaserOutOfScreen == true)
                {
                    chaser.MoveRight(1);
                }

                if (chaser.Position.Y > screenHeight - defaultTextureRadius - 1 && chaserOutOfScreen == true)
                {
                    chaser.MoveUp(1);
                }

                if (chaser.Position.X > screenWidth - defaultTextureRadius - 1 && chaserOutOfScreen == true)
                {
                    chaser.MoveLeft(1);
                }
            }
        }

        public static int RandomPosX(Random randNum, int defaultTextureRadius, int screenWidth)
        {
            return randNum.Next(defaultTextureRadius + 1, screenWidth - defaultTextureRadius);
        }

        public static int RandomPosY(Random randNum, int defaultTextureRadius, int screenHeight)
        {
            return randNum.Next(defaultTextureRadius + 1, screenHeight - defaultTextureRadius);
        }
    }
}
