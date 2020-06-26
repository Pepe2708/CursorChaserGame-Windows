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
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;

        private static CollidableObject player;
        private static CollidableObject chaser;
        private static CollidableObject nailboard;
        private static CollidableObject rocket;
        private static CollidableObject bomb;

        private Rectangle[] chaserRails = new Rectangle[4];
        private Texture2D[] explosionTextures = new Texture2D[70];

        private bool playerDied = false;

        private bool chaserMovingClockwise = true;
        private bool chaserJumping = false;
        private bool chaserTeleporting = false;

        private bool weaponActive = false;
        private bool nailboardActive = false;

        private bool rocketActive = false;
        private bool rocketReachedHeight = false;

        private bool bombActive = false;
        private bool bombReachedHeight = false;
        private bool firstExplosion = true;

        public static int defaultTextureSize = 16;
        public static int defaultTextureRadius => defaultTextureSize / 2;

        private int screenWidth = 400;
        private int screenHeight = 240;

        private int frameCounter;

        private int scoreCounter;
        private int highScore;

        private int rocketTimeLimit = 240;

        private int bombExplosionSize;
        private int currentExplosionState;
        private int bombTimeLimit = 400;
        private float bombAttackHeight;
        private float bombOpacity = 1f;

        Random randNum = new Random();

        KeyboardState keyState, prevKeyState;

        Vector2 outOfBounds;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            outOfBounds = new Vector2(screenWidth + defaultTextureRadius, screenHeight + defaultTextureRadius);

            player = new CollidableObject(Content.Load<Texture2D>("player"), new Vector2(0, 0), 0);
            chaser = new CollidableObject(Content.Load<Texture2D>("chaser"), new Vector2(screenWidth / 2, screenHeight - defaultTextureRadius - 1), 1);
            nailboard = new CollidableObject(Content.Load<Texture2D>("nailboard"), outOfBounds, 0.04f);
            rocket = new CollidableObject(Content.Load<Texture2D>("rocket"), outOfBounds, 0);
            bomb = new CollidableObject(Content.Load<Texture2D>("bomb"), outOfBounds, 0);

            chaserRails[0] = new Rectangle(0, 0, screenWidth, defaultTextureSize + 2);
            chaserRails[1] = new Rectangle(0, 0, defaultTextureSize + 2, screenHeight);
            chaserRails[2] = new Rectangle(0, screenHeight - defaultTextureSize - 2, screenWidth, defaultTextureSize + 2);
            chaserRails[3] = new Rectangle(screenWidth - defaultTextureSize - 2, 0, defaultTextureSize + 2, screenHeight);

            Probability.RerollActionFrequency(randNum);
            Probability.GenerateNewRandomNumber(randNum);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("File");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            player.Position = Vector2.Clamp(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), new Vector2(defaultTextureRadius, defaultTextureRadius), new Vector2(screenWidth - defaultTextureRadius, screenHeight - defaultTextureRadius));

            if (playerDied == false)
            {
                frameCounter++;
                scoreCounter++;

                highScore = Math.Max(highScore, scoreCounter);

                prevKeyState = keyState;
                keyState = Keyboard.GetState();

                if (Probability.currentRandNum != 0)
                {
                    Probability.RerollActionFrequency(randNum);
                    Probability.GenerateNewRandomNumber(randNum);
                }

                if (frameCounter % Probability.actionFrequency == 0 && Probability.currentRandNum < 16)
                {
                    chaserMovingClockwise = !chaserMovingClockwise; // Change chaser direction
                }

                // Automatic chaser controls

                chaser.UpdateObjectRectangle();

                if (chaserRails[0].Contains(chaser.ObjectRectangle) && chaserJumping == false && chaserTeleporting == false)
                {
                    if (chaserMovingClockwise == true) { chaser.MoveRight(4); }
                    else if (chaserMovingClockwise == false) { chaser.MoveLeft(4); }

                    HoldChaserWithinBounds();
                }


                if (chaserRails[1].Contains(chaser.ObjectRectangle) && chaserJumping == false && chaserTeleporting == false)
                {
                    if (chaserMovingClockwise == true) { chaser.MoveUp(4); }
                    else if (chaserMovingClockwise == false) { chaser.MoveDown(4); }

                    HoldChaserWithinBounds();
                }


                if (chaserRails[2].Contains(chaser.ObjectRectangle) && chaserJumping == false && chaserTeleporting == false)
                {
                    if (chaserMovingClockwise == true) { chaser.MoveLeft(4); }
                    else if (chaserMovingClockwise == false) { chaser.MoveRight(4); }

                    HoldChaserWithinBounds();
                }


                if (chaserRails[3].Contains(chaser.ObjectRectangle) && chaserJumping == false && chaserTeleporting == false)
                {
                    if (chaserMovingClockwise == true) { chaser.MoveDown(4); }
                    else if (chaserMovingClockwise == false) { chaser.MoveUp(4); }

                    HoldChaserWithinBounds();
                }

                // Chaser rotation

                if (chaserMovingClockwise == true)
                {
                    chaser.Rotate(-chaser.RotationVelocity);
                }
                else if (chaserMovingClockwise == false)
                {
                    chaser.Rotate(chaser.RotationVelocity);
                }

                // Chaser teleporting

                if (frameCounter % Probability.actionFrequency == 0 && Probability.currentRandNum > 15 && Probability.currentRandNum < 30)
                {
                    chaserTeleporting = !chaserTeleporting;
                }

                if (chaserTeleporting == true)
                {
                    Probability.currentRandNum = 0;
                    Teleportation.MoveChaserIntoWall(randNum, chaser, screenWidth, screenHeight, chaserRails, defaultTextureRadius);
                    Teleportation.MoveChaserOutOfWall(chaser, screenWidth, screenHeight, chaserRails, defaultTextureRadius, weaponActive);

                    for (int i = 0; i < chaserRails.Length; i++)
                    {
                        if (chaserRails[i].Contains(chaser.ObjectRectangle))
                        {
                            chaserTeleporting = false;
                            Teleportation.chaserOutOfScreen = false;
                            Probability.GenerateNewRandomNumber(randNum);
                        }
                    }
                }

                // Chaser jumping

                if (frameCounter % Probability.actionFrequency == 0 && Probability.currentRandNum > 29 && Probability.currentRandNum < 42)
                {
                    Probability.currentRandNum = 0;
                    chaserJumping = true;
                }

                if (chaserJumping == false)
                {
                    chaser.CalculateAngleToPlayer(player.Position.X, player.Position.Y);
                }
                else if (chaserJumping == true)
                {
                    chaser.MoveRight((float)Math.Cos(chaser.AngleToPlayer) * 15);
                    chaser.MoveDown((float)Math.Sin(chaser.AngleToPlayer) * 15);

                    HoldChaserWithinBounds();
                }

                // Nailboard activation

                if (frameCounter % Probability.actionFrequency == 0 && Probability.currentRandNum > 41 && Probability.currentRandNum < 45)
                {
                    Probability.currentRandNum = 0;

                    weaponActive = true;
                    nailboardActive = true;
                    chaserTeleporting = true;

                    nailboard.Position = new Vector2(randNum.Next(0, screenWidth - nailboard.Texture.Width), 480);

                    if (nailboard.Position.X < screenWidth / 2) // Change pivot of rotation to bottom right or bottom left
                    {
                        nailboard.ChangeOrigin(new Vector2(0, nailboard.Texture.Height));
                    }
                    else if (nailboard.Position.X > screenWidth / 2)
                    {
                        nailboard.ChangeOrigin(new Vector2(nailboard.Texture.Width, nailboard.Texture.Height));
                    }
                }

                if (nailboardActive == true && Teleportation.chaserOutOfScreen == true)
                {
                    if (nailboard.Position.Y > 240)
                    {
                        nailboard.MoveUp(8);
                    }

                    if (nailboard.Position.Y == 240 && MathHelper.ToDegrees(nailboard.Rotation) < 90 && nailboard.Position.X < screenWidth / 2)
                    {
                        nailboard.Rotate(nailboard.RotationVelocity);
                    }
                    else if (nailboard.Position.Y == 240 && MathHelper.ToDegrees(nailboard.Rotation) > -90 && nailboard.Position.X > screenWidth / 2)
                    {
                        nailboard.Rotate(-nailboard.RotationVelocity);
                    }

                    if (MathHelper.ToDegrees(nailboard.Rotation) > 90 || MathHelper.ToDegrees(nailboard.Rotation) < -90)
                    {
                        nailboard.Rotation = 0;
                        weaponActive = false;
                        nailboardActive = false;
                        nailboard.Position = outOfBounds;
                        nailboard.ChangeOrigin(new Vector2(nailboard.Texture.Width / 2, nailboard.Texture.Height / 2));
                        Probability.GenerateNewRandomNumber(randNum);
                    }
                }

                // Rocket activation

                if (frameCounter % Probability.actionFrequency == 0 && Probability.currentRandNum > 44 && Probability.currentRandNum < 48)
                {
                    Probability.currentRandNum = 0;

                    weaponActive = true;
                    rocketActive = true;
                    chaserTeleporting = true;

                    rocket.Position = new Vector2(randNum.Next(defaultTextureRadius, screenWidth - nailboard.Texture.Width), screenHeight + defaultTextureSize);
                }

                if (rocketActive == true)
                {
                    if (rocket.Position.Y > screenHeight * 0.25f && rocketReachedHeight == false)
                    {
                        rocket.MoveUp(3);
                    }

                    if (rocket.Position.Y == screenHeight * 0.25f + 1)
                    {
                        rocketReachedHeight = true;
                        frameCounter = 0;
                    }

                    if (rocketReachedHeight == true && frameCounter < rocketTimeLimit)
                    {
                        if (frameCounter % 2 == 0)
                        {
                            rocket.CalculateAngleToPlayer(player.Position.X, player.Position.Y);
                        }
                        rocket.Rotation = rocket.AngleToPlayer + MathHelper.ToRadians(90);
                    }

                    if (rocketReachedHeight == true)
                    {
                        rocket.MoveRight((float)Math.Cos(rocket.AngleToPlayer) * 10);
                        rocket.MoveDown((float)Math.Sin(rocket.AngleToPlayer) * 10);
                    }

                    // Check if rocket is out of bounds

                    if (!GraphicsDevice.Viewport.Bounds.Intersects(rocket.ObjectRectangle) && frameCounter > 150 && rocketReachedHeight == true)
                    {
                        weaponActive = false;
                        rocketActive = false;
                        rocketReachedHeight = false;
                        rocket.Position = outOfBounds;
                        rocket.Rotation = 0;
                        frameCounter = 0;
                        Probability.GenerateNewRandomNumber(randNum);
                    }
                }

                // Bomb activation

                if (frameCounter % Probability.actionFrequency == 0 && Probability.currentRandNum > 47 && Probability.currentRandNum < 51)
                {
                    Probability.currentRandNum = 0;

                    weaponActive = true;
                    bombActive = true;
                    chaserTeleporting = true;

                    bomb.Position = new Vector2(randNum.Next(defaultTextureRadius, screenWidth - nailboard.Texture.Width), screenHeight + defaultTextureSize);
                    bombAttackHeight = (float)randNum.Next(2, 9) / 10; // Random number between 0.2 and 0.8 with one decimal
                }

                if (bombActive == true)
                {
                    if (bomb.Position.Y > screenHeight * bombAttackHeight && bombReachedHeight == false)
                    {
                        bomb.MoveUp(3);
                    }

                    float v1 = bomb.Position.Y;
                    float v2 = screenHeight * bombAttackHeight + 1;

                    if (v1 == v2) // Putting both values directly into the condition doesn't work. I would love to know why...
                    {
                        bombExplosionSize = defaultTextureSize;
                        frameCounter = 0;
                    }

                    if (bomb.Position.Y < screenHeight * bombAttackHeight)
                    {
                        bombReachedHeight = true;
                    }
                }

                if (bombReachedHeight == true && frameCounter < 30) // Bomb contracting just before the explosion
                {
                    if (bombExplosionSize > 7)
                    {
                        bombExplosionSize--;
                    }
                    ChangeBombCircle();
                }

                if (bombReachedHeight == true && bombExplosionSize < bombTimeLimit && frameCounter > 30) // Bomb explosion
                {
                    bombExplosionSize += 10;
                    ChangeBombCircle();
                }

                if (bombExplosionSize > bombTimeLimit && bombOpacity > 0)
                {
                    bombOpacity -= 0.02f;
                }

                if (bombOpacity < 0)
                {
                    bombExplosionSize = 16;
                    ChangeBombCircle();

                    bomb.Position = outOfBounds;
                    weaponActive = false;
                    bombActive = false;
                    bombReachedHeight = false;
                    firstExplosion = false;
                    frameCounter = 0;

                    bombOpacity = 1;

                    Probability.GenerateNewRandomNumber(randNum);
                }

                // Check for collisions with enemies

                if (player.IsColliding(chaser) || player.IsColliding(nailboard) || player.IsColliding(rocket) || player.IsColliding(bomb))
                {
                    KillPlayer();
                }
            }


            if (Mouse.GetState().LeftButton == ButtonState.Pressed && playerDied == true)
            {
                playerDied = false;
                scoreCounter = 0;
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            if (playerDied == true)
            {
                spriteBatch.DrawString(font, $"Score: {scoreCounter}", new Vector2(screenWidth / 2, screenHeight / 2 - 80), Color.White, 0, font.MeasureString($"Score: {scoreCounter}") / 2, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(font, $"Highscore: {highScore}", new Vector2(screenWidth / 2, screenHeight / 2 - 40), Color.White, 0, font.MeasureString($"Highscore: {highScore}") / 2, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(font, "You died", new Vector2(screenWidth / 2, screenHeight / 2 + 40), Color.White, 0, font.MeasureString("You died") / 2, 1, SpriteEffects.None, 1);
                spriteBatch.DrawString(font, "Click to restart", new Vector2(screenWidth / 2, screenHeight / 2 + 80), Color.White, 0, font.MeasureString("Click to restart") / 2, 1, SpriteEffects.None, 1);
            }

            player.Draw(spriteBatch, SpriteEffects.None, 1);

            if (playerDied == false)
            {
                chaser.Draw(spriteBatch, SpriteEffects.None, 1);

                if (nailboard.Position.X < screenWidth / 2)
                {
                    nailboard.Draw(spriteBatch, SpriteEffects.None, 1);
                }
                else if (nailboard.Position.X > screenWidth / 2)
                {
                    nailboard.Draw(spriteBatch, SpriteEffects.FlipHorizontally, 1);
                }

                rocket.Draw(spriteBatch, SpriteEffects.None, 1);
                bomb.Draw(spriteBatch, SpriteEffects.None, bombOpacity);

                spriteBatch.DrawString(font, $"Score: {scoreCounter}", new Vector2(0, 0), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HoldChaserWithinBounds()
        {
            if (chaserTeleporting == false && chaserJumping == false)
            {
                if (chaser.Position.X > 391)
                {
                    chaser.Position = new Vector2(screenWidth - defaultTextureRadius - 1, chaser.Position.Y);
                    chaser.UpdateObjectRectangle();
                }
                if (chaser.Position.X < defaultTextureRadius + 1)
                {
                    chaser.Position = new Vector2(defaultTextureRadius + 1, chaser.Position.Y);
                    chaser.UpdateObjectRectangle();
                }
                if (chaser.Position.Y > 231)
                {
                    chaser.Position = new Vector2(chaser.Position.X, screenHeight - defaultTextureRadius - 1);
                    chaser.UpdateObjectRectangle();
                }
                if (chaser.Position.Y < defaultTextureRadius + 1)
                {
                    chaser.Position = new Vector2(chaser.Position.X, defaultTextureRadius + 1);
                    chaser.UpdateObjectRectangle();
                }
            }

            if (chaserJumping == true)
            {
                if (chaser.Position.X > 391)
                {
                    chaser.Position = new Vector2(screenWidth - defaultTextureRadius - 1, chaser.Position.Y);
                    chaserJumping = false;
                    Probability.GenerateNewRandomNumber(randNum);
                }
                if (chaser.Position.X < defaultTextureRadius + 1)
                {
                    chaser.Position = new Vector2(defaultTextureRadius + 1, chaser.Position.Y);
                    chaserJumping = false;
                    Probability.GenerateNewRandomNumber(randNum);
                }
                if (chaser.Position.Y > 231)
                {
                    chaser.Position = new Vector2(chaser.Position.X, screenHeight - defaultTextureRadius - 1);
                    chaserJumping = false;
                    Probability.GenerateNewRandomNumber(randNum);
                }
                if (chaser.Position.Y < defaultTextureRadius + 1)
                {
                    chaser.Position = new Vector2(chaser.Position.X, defaultTextureRadius + 1);
                    chaserJumping = false;
                    Probability.GenerateNewRandomNumber(randNum);
                }
            }
        }

        public void KillPlayer()
        {
            frameCounter = 0;

            chaserMovingClockwise = true;
            chaserJumping = false;
            chaserTeleporting = false;
            Teleportation.chaserOutOfScreen = false;

            weaponActive = false;
            nailboardActive = false;

            rocketActive = false;
            rocketReachedHeight = false;

            bombActive = false;
            bombReachedHeight = false;

            chaser.Position = new Vector2(screenWidth / 2, screenHeight - defaultTextureRadius - 1);
            nailboard.Position = outOfBounds;
            rocket.Position = outOfBounds;
            bomb.Position = outOfBounds;

            nailboard.ChangeOrigin(new Vector2(nailboard.Texture.Width / 2, nailboard.Texture.Height / 2));

            nailboard.Rotation = 0;
            rocket.Rotation = 0;

            bombExplosionSize = 16;
            ChangeBombCircle();

            bombOpacity = 1;

            Probability.RerollActionFrequency(randNum);
            Probability.GenerateNewRandomNumber(randNum);

            playerDied = true;
        }

        public void ChangeBombCircle()
        {
            // Check if the current explosion is the first one, so that the explosion textures only get generated once and can be reused later
            if (firstExplosion == true)
            {
                explosionTextures[currentExplosionState] = createCircleText(bombExplosionSize);
            }
            bomb.LoadTexture(explosionTextures[currentExplosionState]);

            currentExplosionState++;
            if (currentExplosionState == 70) { currentExplosionState = 0; }
        }

        public Texture2D createCircleText(int radius)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        private bool ToggleKeyState(Keys currentKey)
        {
            return keyState.IsKeyDown(currentKey) && prevKeyState.IsKeyUp(currentKey);
        }
    }
}
