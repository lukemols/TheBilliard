using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace TheBilliard
{
    class GameClass : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        
        Background background; //Sfondo
        Billiard billiard; ///istanza del gioco
        Cue cue;//Stecca
        int cueTime;//Tempo di attesa se si cambia l'orientamento di 90°

        Texture2D[] ballsTxt = new Texture2D[16]; //Texture delle palle
        Texture2D cueTxt; //texture della stecca
        SpriteFont font;

        Vector2 boardPos; ///posizione del tavolo
        Rectangle screenBounds;

        bool gameStarted;
        public bool GameStarted { get { return gameStarted; } }
        bool isPaused;
        // Pulsanti e scritte per il menu di pausa
        Texture2D buttonTxt;
        Texture2D arrowTxt;
        Vector2 continueVec;
        Vector2 restartVec;
        Vector2 exitVec;
        Vector2 arrowVec = new Vector2(0, 0);
        int arrowPosition;
        int menuTime;
        int pauseTime;

        public GameClass(Game game)
            : base(game)
        {
            screenBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            background = new Background(game, true);
        }
        /// <summary>
        /// Metodo per caricare tutto il necessario alla visualizzazione
        /// </summary>
        /// <param name="content"></param>
        public void Load(ContentManager content)
        {
            background.Load(content);
            font = content.Load<SpriteFont>("Font");
            buttonTxt = content.Load<Texture2D>("Button");
            arrowTxt = content.Load<Texture2D>("Arrow");
            cueTxt = content.Load<Texture2D>("Cue");
            for (int i = 0; i < ballsTxt.Length; i++)
            {
                string numero = i.ToString();
                ballsTxt[i] = content.Load<Texture2D>(numero);
            }
            //Posiziona i pulsanti del menu di pausa
            float x = screenBounds.Width / 2 - buttonTxt.Width / 2;
            float y = screenBounds.Height / 3 - 50;
            continueVec = new Vector2(x, y);
            restartVec = new Vector2(x, 2 * y);
            exitVec = new Vector2(x, 3 * y);
        }
        /// <summary>
        /// Metodo di aggiornamento del gioco
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //Metti / togli la pausa (se sono passati 10 frame dall'ultima volta che è stato premuto esc
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && pauseTime > 10)
            {
                isPaused = !isPaused;
                pauseTime = 0;
            }
            //aggiorna il tempo di pausa
            if (pauseTime <= 10)
                pauseTime++;
            //Menu di pausa
            if (isPaused)
            {
                if (++menuTime < 7) //controlla ogni 7 frame la scelta dell'utente, altrimenti la freccina del menu va troppo veloce
                    return;
                else
                    menuTime = 0;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))//fai scendere la freccina
                    arrowPosition++;
                else if (Keyboard.GetState().IsKeyDown(Keys.Up))//fai salire la freccina
                    if (--arrowPosition < 0)
                        arrowPosition += 3;
                arrowPosition %= 3;//mantienila tra 0e 2
                arrowVec.X = continueVec.X - 2 * arrowTxt.Width; //scegli la posizione video della freccia
                switch (arrowPosition)
                {
                    case 0:
                        arrowVec.Y = continueVec.Y + buttonTxt.Height / 2 - arrowTxt.Height / 2;
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                            isPaused = false;
                        break;
                    case 1:
                        arrowVec.Y = restartVec.Y + buttonTxt.Height / 2 - arrowTxt.Height / 2;
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                            NewGame();
                        break;
                    case 2:
                        arrowVec.Y = exitVec.Y + buttonTxt.Height / 2 - arrowTxt.Height / 2;
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                            gameStarted = false;
                        break;
                }
            }
            // se nessuno dei due giocatori ancora non ha vinto e non si è in pausa
            else if(!billiard.player1.GameWon && !billiard.player2.GameWon)
            {
                //Se il gioco è fermo e non c'è fallo
                if (billiard.isStopped && !billiard.isFault)
                {
                    //Scegli la posizione della stecca
                    cueTime++;
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                        cue.rotation += 0.01f;
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                        cue.rotation -= 0.01f;
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) && cueTime > 5) //ruota di 90° in senso orario
                    {
                        cue.rotation += 1.57f;
                        cueTime = 0;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Down) && cueTime > 5)//ruota di 90° in senso antiorario
                    {
                        cue.rotation -= 1.57f;
                        cueTime = 0;
                    }
                    //Dai la forza alla stecca
                    if (Keyboard.GetState().IsKeyDown(Keys.F))
                    {
                        if (!cue.isReturning)
                        {
                            cue.power -= 0.5f;
                            if (cue.power < 0)
                                cue.isReturning = true;
                        }
                        else
                        {
                            cue.power += 0.5f;
                            if (cue.power > 30)
                                cue.isReturning = false;
                        }
                        cue.isPointing = true;
                    }
                    //Quando viene rilasciato F colpisci la pallina bianca
                    if (Keyboard.GetState().IsKeyUp(Keys.F) && cue.isPointing == true)
                    {
                        billiard.CollisionWithCue(cue);
                        cue.isPointing = false;
                    }
                }
                //se c'è stato fallo riposiziona la pallina bianca muovendola con le frecce su e giù
                else if (billiard.isStopped && billiard.isFault)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) && billiard.balls[0].position.Y > billiard.boardPos.Y)
                        billiard.balls[0].position.Y -= 2;
                    if (Keyboard.GetState().IsKeyDown(Keys.Down) && billiard.balls[0].position.Y < billiard.boardPos.Y + 330)
                        billiard.balls[0].position.Y += 2;
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        billiard.isFault = false;
                }
                else // if(!billiard.isStopped)
                    billiard.UpdateBilliard(); //altrimenti se non c'è pausa e il gioco sta continuando aggiorna le posizioni 
            }
            else if(billiard.player1.GameWon || billiard.player2.GameWon)
            {
                //quando il gioco è finito aspetta che venga premuto INVIO per tornare al meni
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    gameStarted = false;
            }
        }
        /// <summary>
        /// Metodo per disegnare il gioco
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            //Disegna lo sfondo con il tavolo
            background.DrawBackground(spriteBatch);
            //Le palline in gioco
            float scaleBallDiam = 0.2f;
            for (int i = 0; i < billiard.numBalls; i++)
            {
                if (billiard.balls[i].inGame == true)
                    spriteBatch.Draw(ballsTxt[i], billiard.balls[i].position, null, Color.White, 0.0f, new Vector2(0, 0), scaleBallDiam, SpriteEffects.None, 0f);
            }
            //la stecca se le palline sono ferme
            if (billiard.isStopped)
            {
                Vector2 p = new Vector2(billiard.balls[0].position.X + 10, billiard.balls[0].position.Y + 10);
                int c = (int)cue.power;

                if (c > 30)
                    c = 30;
                int k = 30 - c;
                spriteBatch.Draw(cueTxt, p, null, Color.White, cue.rotation, new Vector2(k - 30, 0), 1.0f, SpriteEffects.None, 0f);
            }
            //E le scritte player 1 e 2 e il tipo di palle da colpire per ogni giocatore
            Vector2 p1pos = new Vector2(10, 20);
            Vector2 p2pos = new Vector2(screenBounds.Width - 100, 20);

            spriteBatch.DrawString(font, "Player 1", p1pos, Color.Yellow);
            spriteBatch.DrawString(font, "Player 2", p2pos, Color.Yellow);
            spriteBatch.DrawString(font, billiard.player1.BallsToPocket, new Vector2(p1pos.X, 40), Color.Yellow);
            spriteBatch.DrawString(font, billiard.player2.BallsToPocket, new Vector2(p2pos.X, 40), Color.Yellow);
            //Il giocatore attuale
            string act = "Giocatore attuale: Player ";
            if (billiard.ActualPlayer == "Player 1")
                act += 1;
            else
                act += 2;
            spriteBatch.DrawString(font, act, new Vector2(screenBounds.Width / 2 - 100, 20), Color.Yellow);
            //e se è stato fatto fallo
            if(billiard.isFault)
                spriteBatch.DrawString(font, "Fallo", new Vector2(screenBounds.Width / 2 - 80, 40), Color.Yellow);
            //Disegna sotto le scritte player 1 e 2 le palle che hanno mandato in buca
            List<int> pocketBalls = billiard.player1.BallPocket;
            int y = 60;
            for (int i = 0; i < pocketBalls.Count; i++ )
            {
                Vector2 position = new Vector2(10 + i * 20, y);
                spriteBatch.Draw(ballsTxt[pocketBalls[i]], position, null, Color.White, 0.0f, new Vector2(0, 0), scaleBallDiam, SpriteEffects.None, 0f);
            }
            pocketBalls = billiard.player2.BallPocket;
            for (int i = 0; i < pocketBalls.Count; i++)
            {
                Vector2 position = new Vector2(screenBounds.Width - 30 - i * 20, y);
                spriteBatch.Draw(ballsTxt[pocketBalls[i]], position, null, Color.White, 0.0f, new Vector2(0, 0), scaleBallDiam, SpriteEffects.None, 0f);
            }
            //se è in pausa anche il menu
            if (isPaused)
            {
                spriteBatch.Draw(buttonTxt, continueVec, Color.White);
                spriteBatch.Draw(buttonTxt, restartVec, Color.White);
                spriteBatch.Draw(buttonTxt, exitVec, Color.White);
                spriteBatch.Draw(arrowTxt, arrowVec, Color.White);

                spriteBatch.DrawString(font, "Continua", new Vector2(continueVec.X + 30, continueVec.Y + 15), Color.Yellow);
                spriteBatch.DrawString(font, "Riavvia", new Vector2(restartVec.X + 30, restartVec.Y + 15), Color.Yellow);
                spriteBatch.DrawString(font, "Esci", new Vector2(exitVec.X + 30, exitVec.Y + 15), Color.Yellow);
            }
            //se il gioco è stato vinto anche i complimenti
            if(billiard.player1.GameWon || billiard.player2.GameWon)
            {
                string text = "Complimenti al player ";
                if (billiard.player1.GameWon)
                    text += "1!";
                else
                    text += "2!";
                text += Environment.NewLine + "Premi INVIO per tornare al menu principale";
                spriteBatch.DrawString(font, text, new Vector2(screenBounds.Width / 2 - 100, screenBounds.Height / 2 - 20), Color.Yellow);
                
            }
            
            //Fine delle cose da disegnare
            spriteBatch.End();
        }
        /// <summary>
        /// Metodo che crea un nuovo gioco
        /// </summary>
        public void NewGame()
        {
            boardPos = new Vector2(screenBounds.Width / 2 - 350, screenBounds.Height / 2 - 175);

            pauseTime = 0;
            cueTime = 0;
            gameStarted = true;
            isPaused = false;
            cue = new Cue();
            billiard = new Billiard(boardPos);
            billiard.InitBilliard();
        }


    }
}
