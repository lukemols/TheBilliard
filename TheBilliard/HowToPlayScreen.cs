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
    /// Classe che mostra i tasti di gioco all'utente
    /// </summary>
    class HowToPlayScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        Rectangle screenBounds;
        Background background;
        Texture2D howToTxt;
        Vector2 howToVec;

        public HowToPlayScreen(Game game)
            : base(game)
        {
            screenBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            background = new Background(game);
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
        }

        public void Load(ContentManager content)
        {
            background.Load(content);
            howToTxt = content.Load<Texture2D>("HowToPlay");
            howToVec = new Vector2(screenBounds.Width / 2 - howToTxt.Width / 2, screenBounds.Height / 2 - howToTxt.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            background.DrawBackground(spriteBatch);
            spriteBatch.Draw(howToTxt, howToVec, Color.White);

            spriteBatch.End();
        }
    }
}
