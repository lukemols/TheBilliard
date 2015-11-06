using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace TheBilliard
{
    class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Background background;

        Texture2D buttonTxt; //Texture del pulsante
        Texture2D arrow; //Texture della freccina
        Texture2D title; //Texture del titolo
        SpriteFont font; //Font della scrittura

        Rectangle screenBounds; //Limiti dello schermo
        //Vettori che indicano la posizione dei pulsanti
        Vector2 gameBtVec; //"Gioca"
        Vector2 creditsBtVec; //"Crediti"
        Vector2 howToBtVec; //"Come giocare"
        Vector2 exitBtVec; //"Esci"
        Vector2 arrowVec; // del puntatore
        Vector2 titleVec; //dell'immagine del titolo
        Vector2 gameStringVec; // e delle stringhe Gioca
        Vector2 creditsStringVec; //Crediti
        Vector2 howToStringVec; // Come giocare
        Vector2 exitStringVec; //ed esci

        int position;
        int time;
        protected SpriteBatch spriteBatch;

        public MainMenu(Game game)
            : base(game)
        {
            screenBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            position = 0;
            time = 0;
            background = new Background(game);
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
        }

        public void Load(ContentManager content)
        {
            //carica texture e font
            buttonTxt = content.Load<Texture2D>("Button");
            title = content.Load<Texture2D>("Title");
            arrow = content.Load<Texture2D>("Arrow");
            font = content.Load<SpriteFont>("Font");
            background.Load(content);
            //posiziona scritte e pulsanti
            SetPositions();
        }

        void SetPositions()
        {
            int startY = 200;
            float y = (screenBounds.Height - 200) / 4;
            float x = (screenBounds.Width / 2) - (buttonTxt.Width / 2);

            gameBtVec = new Vector2(x, startY);
            gameStringVec = new Vector2(x + 25, gameBtVec.Y + buttonTxt.Height / 2 - 10);

            howToBtVec = new Vector2(x, startY + y);
            howToStringVec = new Vector2(x + 15, howToBtVec.Y + buttonTxt.Height / 2 - 10);

            creditsBtVec = new Vector2(x, startY + 2 * y);
            creditsStringVec = new Vector2(x + 20, creditsBtVec.Y + buttonTxt.Height / 2 - 10);

            exitBtVec = new Vector2(x, startY + 3 * y);
            exitStringVec = new Vector2(x + 25, exitBtVec.Y + buttonTxt.Height / 2 - 10);

            arrowVec = new Vector2(x - arrow.Width - 10, gameBtVec.Y + buttonTxt.Height / 2 - arrow.Height / 2);
            titleVec = new Vector2(x, -75);
        }
        /// <summary>
        /// Metodo che aggiorna le scelte dell'utente
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //Fai scendere il titolo dall'alto
            if (titleVec.Y < 50)
                titleVec.Y += 1;
            //Aspetta 7 frame prima di cambiare la posizione della freccina
            if (++time < 7)
                return;

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                position++;
                time = 0;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (--position < 0)
                    position += 4;
                time = 0;
            }
            position %= 4;
            switch (position)
            {
                case 0:
                    arrowVec.Y = gameBtVec.Y + buttonTxt.Height / 2 - arrow.Height / 2;
                    break;
                case 1:
                    arrowVec.Y = howToBtVec.Y + buttonTxt.Height / 2 - arrow.Height / 2;
                    break;
                case 2:
                    arrowVec.Y = creditsBtVec.Y + buttonTxt.Height / 2 - arrow.Height / 2;
                    break;
                case 3:
                    arrowVec.Y = exitBtVec.Y + buttonTxt.Height / 2 - arrow.Height / 2;
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            //mostra tutto
            background.DrawBackground(spriteBatch);

            spriteBatch.Draw(title, titleVec, Color.White);
            spriteBatch.Draw(buttonTxt, gameBtVec, Color.White);
            spriteBatch.Draw(buttonTxt, howToBtVec, Color.White);
            spriteBatch.Draw(buttonTxt, creditsBtVec, Color.White);
            spriteBatch.Draw(buttonTxt, exitBtVec, Color.White);
            spriteBatch.Draw(arrow, arrowVec, Color.White);
            spriteBatch.DrawString(font, "Gioca!", gameStringVec, Color.Yellow);
            spriteBatch.DrawString(font, "Come Giocare", howToStringVec, Color.Yellow);
            spriteBatch.DrawString(font, "Crediti", creditsStringVec, Color.Yellow);
            spriteBatch.DrawString(font, "Esci", exitStringVec, Color.Yellow);

            spriteBatch.End();
        }

        public string GetNextWindow()
        {
            string s = "Menu";
            switch (position)
            {
                case 0:
                    s = "Game";
                    break;
                case 1:
                    s = "HowTo";
                    break;
                case 2:
                    s = "Credits";
                    break;
                case 3:
                    s = "Exit";
                    break;
            }
            return s;
        }
    }
}
