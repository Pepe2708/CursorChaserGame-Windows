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
    class Probability
    {
        // Every chaser direction change, jump towards the player, teleport or attack is called an action.
        // How often these actions happen is determined is controlled by the "actionFrequency" variable.
        // The value of that variable is the number of frames it is waiting before starting a new action.
        // There is always a 50% probability that the chaser does nothing, if an action is triggered.

        // Every action has its own probability which looks as follows:
        // 30% Direction change
        // 28% Teleport
        // 24% Jump
        // 18% Weapon

        public static int currentRandNum = 0;
        public static int actionFrequency = 0;

        public static void RerollActionFrequency(Random randNum)
        {
            actionFrequency = randNum.Next(20, 61);
        }

        public static int ActionFrequency()
        {
            return actionFrequency;
        }

        public static void GenerateNewRandomNumber(Random randNum)
        {
            currentRandNum = randNum.Next(1, 101);
        }

        public static int RandomNumber()
        {
            return currentRandNum;
        }
    }
}
