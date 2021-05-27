using System;
using System.Collections.Generic;
using System.Text;

namespace CCG
{
    static class EventTrigger
    {
        public static int value = 0;

        // VALUE INDEX
        // 0 = Nothing
        // 1 = Chaser direction change
        // 2 = Chaser teleport
        // 3 = Chaser jump
        // 4 = Nailboard
        // 5 = Rocket
        // 6 = Bomb

        public static void Update()
        {
            if (!Weapon.active && !Chaser.teleporting && !Chaser.jumping)
            {
                Globals.framesPassed++;
                if (Globals.framesPassed > 60)
                {
                    GenerateNewValue();
                }
            }
        }

        public static void GenerateNewValue()
        {
            value = Globals.rand.Next(0, 7);
        }

        public static void Reset()
        {
            value = 0;
            Globals.framesPassed = 0;
        }
    }
}
