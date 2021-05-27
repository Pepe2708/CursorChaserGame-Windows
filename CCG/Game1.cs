using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace CCG
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont font;

        List<CollidableObject> weapons = new List<CollidableObject>();

        Player player;
        Chaser chaser;
        Nailboard nailboard;
        Bomb bomb;
        Rocket rocket;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            _graphics.PreferredBackBufferWidth = Globals.screenWidth;
            _graphics.PreferredBackBufferHeight = Globals.screenHeight;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use Content to load your game content here

            font = Content.Load<SpriteFont>("Score");

            GUI.startScreen = Content.Load<Texture2D>("start");

            player = new Player(Content.Load<Texture2D>("player"));
            chaser = new Chaser(Content.Load<Texture2D>("chaser"));
            nailboard = new Nailboard(Content.Load<Texture2D>("nailboard"));
            rocket = new Rocket(Content.Load<Texture2D>("rocket"));
            bomb = new Bomb(Globals.createCircleTexture(32, GraphicsDevice), GraphicsDevice);

            weapons.Add(nailboard);
            weapons.Add(rocket);
            weapons.Add(bomb);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            Globals.SetScreenEdges();

            player.Update();
            GUI.UpdateMouseInput();

            if (GUI.Input() == 'R' && player.IsAlive && GUI.score > 0)
            {
                PauseGame();
            }

            if (player.IsAlive && GUI.gameIsActive)
            {
                chaser.Update();
                nailboard.Update();
                bomb.Update();
                rocket.Update();

                CheckForCollision();
                EventTrigger.Update();
                GUI.UpdateScore();
            }

            if (!GUI.gameIsActive && !Player.isAlive)
            {
                RestartGame();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            GUI.Draw(font, _spriteBatch);

            player.Draw(_spriteBatch);

            if (GUI.gameIsActive)
            {
                chaser.Draw(_spriteBatch);
                nailboard.Draw(_spriteBatch);
                bomb.Draw(_spriteBatch);
                rocket.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CheckForCollision()
        {
            foreach (var weapon in weapons)
            {
                if (player.IsColliding(weapon))
                {
                    EndGame();
                }
                if (player.IsColliding(chaser) && !chaser.Teleporting)
                {
                    EndGame();
                }
            }
        }

        public void EndGame()
        {
            player.IsAlive = false;
            GUI.gameIsActive = false;
        }

        public void RestartGame()
        {
            GUI.gameIsActive = false;
            foreach (var weapon in weapons)
            {
                weapon.Reset();
            }
            chaser.Reset();
            EventTrigger.Reset();
        }

        public void PauseGame()
        {
            GUI.gameIsActive = !GUI.gameIsActive;
        }
    }
}
