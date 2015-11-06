#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace TheBilliard
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Oggetti corrispondenti ciascuno ad una finestra
        MainMenu mainMenu;
        HowToPlayScreen howTo;
        Credits credits;
        GameClass gameClass;
        //tempo di attesa una volta tornati dal gioco (Se si preme un po' di più INVIO si rischia di far ripartire il gioco senza vedere il menu)
        int waitTime = 0;
        //finestra attiva
        string activeWindow;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Metti il gioco in 1024x768
            int winW = graphics.PreferredBackBufferWidth = 1024;
            int winH = graphics.PreferredBackBufferHeight = 768;

            graphics.ApplyChanges();

            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Aggiungi la sprite batch ai servizi cosicché le classi possano usare la stessa istanza
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            //Inizializza gli oggetti
            activeWindow = "Menu";
            mainMenu = new MainMenu(this);
            howTo = new HowToPlayScreen(this);
            credits = new Credits(this);
            gameClass = new GameClass(this);

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
            //Carica tutte le texture necessarie
            mainMenu.Load(Content);
            howTo.Load(Content);
            credits.Load(Content);
            gameClass.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            //Se la finestra attiva è menu e non è ancora passato il tempo d'attesa non fare nulla
            if (activeWindow == "Menu" && --waitTime > 0)
                return;
            switch (activeWindow)
            {
                case "Menu":
                    mainMenu.Update(gameTime);
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        activeWindow = mainMenu.GetNextWindow();
                        //Se la prossima finestra è il gioco, crea una nuova partita
                        if (activeWindow == "Game")
                            gameClass.NewGame();
                    }
                    break;
                case "Game":
                    //finché il gioco non è finito mostra il gioco
                    if (gameClass.GameStarted)
                        gameClass.Update(gameTime);
                    else
                    {
                        //altrimenti il menu e attendi 10 frame
                        activeWindow = "Menu";
                        waitTime = 10;
                    }
                    break;
                case "HowTo":
                    //Se si preme esc nel menu di come giocare torna indietro
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        activeWindow = "Menu";
                    break;
                case "Credits":
                    //aggiorna i crediti (salgono)
                    credits.Update(gameTime);
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        activeWindow = "Menu";
                    break;
                case "Exit":
                    Exit();
                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            //Mostra la finestra attuale
            switch (activeWindow)
            {
                case "Menu":
                    mainMenu.Draw(gameTime);
                    break;
                case "Game":
                    gameClass.Draw(gameTime);
                    break;
                case "HowTo":
                    howTo.Draw(gameTime);
                    break;
                case "Credits":
                    credits.Draw(gameTime);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
