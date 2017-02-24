
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace IWKS_3400_Lab4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont Font1;
        Vector2 FontPos;
        Boolean done = false;  // if true, we will display the victory/consolation message
        public string victory;  // used to hold the congratulations/better luck next time message
        //  Paddles 
        Paddle computer_paddle;
        Paddle player_paddle;
        // Ball
        Ball ball;
        // Sound Effects
        SoundEffect ballhit;
        SoundEffect killshothit;
        SoundEffect paddlemiss;

        Block blockUp;

        Block blockRight;

        Block blockLeft;

        Block blockDown;

        Border_Line Border;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //  changing the back buffer size changes the window size (when in windowed mode)
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 500;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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
            //Load the SoundEffect resources

            // Load the SoundEffect resource
            ballhit = Content.Load<SoundEffect>("ballhit");
            killshothit = Content.Load<SoundEffect>("killshot");
            paddlemiss = Content.Load<SoundEffect>("miss");
            Font1 = Content.Load<SpriteFont>("Courier New");

            //Drawing Right Paddle
            computer_paddle = new Paddle(Content.Load<Texture2D>("right_paddle"), new Vector2(444f, 268f), new Vector2(24f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            //Border Line
            Border = new Border_Line(Content.Load<Texture2D>("Center"), new Vector2(467f, -87f), new Vector2(32f, 32f), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            //Drawing Left Paddle
            player_paddle = new Paddle(Content.Load<Texture2D>("left_paddle"), new Vector2(505f, 268f), new Vector2(24f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            //Ball
            ball = new Ball(Content.Load<Texture2D>("small_ball"), new Vector2(600f, 100f), new Vector2(32f, 32f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            //  set the speed the objects will move
            // the ball always starts in the middle and moves toward the player
            ball.Reset();
            computer_paddle.velocity = new Vector2(0, 3);

            //Block Class
            blockUp = new Block(Content.Load<Texture2D>("Up Arrow125"), new Vector2(503f, 0f), new Vector2(24f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);


        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //  Free the previously alocated resources
            computer_paddle.texture.Dispose();
            player_paddle.texture.Dispose();
            ball.texture.Dispose();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // this check allows to freeze the game to display the victory/consolation message
            if (done == false)
            {
                // Move the ball and computer paddle
                // All the Move calls do is adjust velocity; position will be changed after
                // velocity adjustments.
                ball.Move(player_paddle, computer_paddle, Border);
                computer_paddle.Move(ball);

                //Sound for when collision ball and paddle happens
                if (ball.collision_occured && ball.playit)
                {
                    ballhit.Play();
                    ball.playit = false;
                }

                //ball velocity and position. Needs work...
                ball.position += ball.velocity;
                 computer_paddle.position += computer_paddle.velocity;
                KeyboardState keyboardState = Keyboard.GetState();
                KeyboardState keyboardState2 = Keyboard.GetState();


                if (blockUp.isDown == true )
                {            
                   if(keyboardState2.IsKeyDown(Keys.Up))
                    {                     
                        blockUp.position += new Vector2(0,-100);
                        blockUp.isDown = false;
                    }
                }

                if (ball.velocity.X < 0)
                {
                    blockUp.position = new Vector2(503,0);
                }

                if (keyboardState.IsKeyDown(Keys.W))
                    if (player_paddle.position.Y > 0)  // don't run off the edge
                        player_paddle.position += new Vector2(0, -8);
                if (keyboardState.IsKeyDown(Keys.S))
                    if (player_paddle.position.Y < (graphics.PreferredBackBufferHeight - player_paddle.size.Y)) // don't run off the edge
                        player_paddle.position += new Vector2(0, 8);
          
            }
            if (ball.scorePlayer > ball.scoreFinal)
            {
                victory = "Congratulations!  You Win!  Your Score: " + ball.scorePlayer + "     Computer Score: " + ball.scoreComputer;
                done = true;
            }
            else if (ball.scoreComputer > ball.scoreFinal)
            {
                victory = "Better luck next time!  Your Score: " + ball.scorePlayer + "     Computer Score: " + ball.scoreComputer;
                done = true;
            }
            if (done == false)
            {
                base.Update(gameTime);
            }




        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MediumVioletRed);
            // Draw the sprites
            spriteBatch.Begin();
            Border.Draw(spriteBatch);
            // Draw running score string
            spriteBatch.DrawString(Font1, "Computer: " + ball.scoreComputer, new Vector2(5, 10), Color.Yellow);
            spriteBatch.DrawString(Font1, "Player: " + ball.scorePlayer,
                new Vector2(graphics.GraphicsDevice.Viewport.Width - Font1.MeasureString("Player: " + ball.scorePlayer).X - 5, 10), Color.Yellow);
            if (done) //draw victory/consolation message
            {
                FontPos = new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 300,
                    (graphics.GraphicsDevice.Viewport.Height / 2) - 50);
                spriteBatch.DrawString(Font1, victory, FontPos, Color.Yellow);
            }
            //Draw the other sprites
            computer_paddle.Draw(spriteBatch);
            player_paddle.Draw(spriteBatch);
            ball.Draw(spriteBatch);
            blockUp.Draw(spriteBatch, ball);
            spriteBatch.End();
            if (done == false)
            {
                base.Draw(gameTime);
            }

        }
    }
}