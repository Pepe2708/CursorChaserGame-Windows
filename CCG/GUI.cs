using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCG
{
    static class GUI
    {
        public static Texture2D startScreen;

        public static bool gameIsActive = false;
        public static int score = 0;
        public static int highScore = 0;

        public static ButtonState buttonStateL;
        public static ButtonState prevButtonStateL;
        public static ButtonState buttonStateR;
        public static ButtonState prevButtonStateR;

        public static void UpdateMouseInput()
        {
            prevButtonStateL = buttonStateL;
            buttonStateL = Mouse.GetState().LeftButton;

            prevButtonStateR = buttonStateR;
            buttonStateR = Mouse.GetState().RightButton;
        }

        public static char Input()
        {
            if (prevButtonStateL == ButtonState.Released && buttonStateL == ButtonState.Pressed)
            {
                return 'L';
            }
            else if (prevButtonStateR == ButtonState.Released && buttonStateR == ButtonState.Pressed)
            {
                return 'R';
            }
            else
            {
                return 'N';
            }
        }

        public static void UpdateScore()
        {
            score++;
            highScore = Math.Max(score, highScore);
        }

        public static void ResetScore()
        {
            score = 0;
        }

        public static void Draw(SpriteFont font, SpriteBatch spriteBatch)
        {
            if (!gameIsActive && Player.isAlive && score == 0) { DrawStartScreen(font, spriteBatch); }
            else if (!gameIsActive && !Player.isAlive) { DrawEndScreen(font, spriteBatch); }

            DrawScore(font, spriteBatch);
        }

        public static void DrawStartScreen(SpriteFont font, SpriteBatch spriteBatch)
        {
            //Vector2 position = new Vector2(Globals.screenWidth / 2, Globals.screenHeight - Globals.screenHeight / 6);

            spriteBatch.Draw(startScreen, new Vector2(0, 0), Color.White);
            DrawInputPrompt(font, spriteBatch, "[Left click] Play", 5);
            DrawInputPrompt(font, spriteBatch, NetworkClient.downloadStatus, 7);

            if (Input() == 'L')
            {
                gameIsActive = true;
                NetworkClient.ResetStatus();
            }

            if (Input() == 'R')
            {
                NetworkClient.DownloadHighscore();
            }
        }

        public static void DrawEndScreen(SpriteFont font, SpriteBatch spriteBatch)
        {
            string text = "You died!";
            
            Vector2 textCenter = font.MeasureString(text) / 2;
            Vector2 position = new Vector2(Globals.screenWidth / 2, Globals.screenHeight / 2);

            spriteBatch.DrawString(font, text, position, Color.White, 0, textCenter, 0.5f, SpriteEffects.None, 0.5f);

            DrawInputPrompt(font, spriteBatch, "[Left click] Return to menu", 5);
            DrawInputPrompt(font, spriteBatch, NetworkClient.uploadStatus, 7);

            if (Input() == 'L')
            {
                Player.isAlive = true;
                ResetScore();
                NetworkClient.ResetStatus();
            }

            if (Input() == 'R')
            {
                NetworkClient.UploadHighscore();
            }
        }

        public static void DrawInputPrompt(SpriteFont font, SpriteBatch spriteBatch, string text, int posY)
        {
            Vector2 position = new Vector2(50, Globals.screenHeight - Globals.screenHeight / posY);

            spriteBatch.DrawString(font, text, position, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0.5f);
        }

        public static void DrawScore(SpriteFont font, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, $"Score: {score}", new Vector2(0, 0), Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(font, $"Highscore: {highScore}", new Vector2(0, 20), Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0.5f);
        }
    }
}
