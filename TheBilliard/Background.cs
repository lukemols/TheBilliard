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
    /// <summary>
    /// Classe Background che implementa lo sfondo di gioco
    /// </summary>
    class Background : Microsoft.Xna.Framework.DrawableGameComponent
    {
        static Texture2D background; // Sfondo
        static Texture2D board; //Texture del tavolo da biliardo
        Vector2 boardVec; // Vettore che indica dove iniziare a disegnare il tavolo
        Rectangle screenBounds; // Limiti dello schermo
        bool isGame; // Se è lo sfondo del gioco o no

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="game">Istanza del gioco per ottenere le dimensioni dello schermo</param>
        /// <param name="isGame">Bool che indica se sarà lo sfondo di gioco oppure no</param>
        public Background(Game game, bool isGame = false)
            : base(game)
        {
            screenBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            this.isGame = isGame;
        }
        /// <summary>
        /// Metodo per caricare le texture necessarie
        /// </summary>
        /// <param name="content">Content manager per eseguire la load</param>
        public void Load(ContentManager content)
        {
            background = content.Load<Texture2D>("Background");
            board = content.Load<Texture2D>("BoardAll");
            
            // Imposta le coordinate per il tavolo
            boardVec = new Vector2(screenBounds.Width / 2 - board.Width / 2, screenBounds.Height / 2 - board.Height / 2);
        }

        /// <summary>
        /// Metodo che disegna lo sfondo
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch per disegnare su schermo</param>
        public void DrawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            // Se è lo sfondo di gioco, disegna anche il tavolo
            if (isGame)
                spriteBatch.Draw(board, boardVec, Color.White);
        }
    }
}
