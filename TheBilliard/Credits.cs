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
    class Credits : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        Rectangle screenBounds;
        Background background; //Sfondo
        string text; //Testo
        Vector2 textVec; //Posizione del testo
        SpriteFont font;//Font
        public Credits(Game game)
            : base(game)
        {
            screenBounds = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
            background = new Background(game);
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            textVec = new Vector2(screenBounds.Width / 2 - 50, screenBounds.Height);

            text = "The Billiard by Luca Mollo" + Environment.NewLine + Environment.NewLine + "Program by Luca Mollo" + Environment.NewLine + Environment.NewLine +
                "Sprites by Luca Mollo" + Environment.NewLine + Environment.NewLine + "Thanks for playing";
        }

        public void Load(ContentManager content)
        {
            font = content.Load<SpriteFont>("Font");
            background.Load(content);
        }

        public override void Update(GameTime gameTime)
        {
            //fai scorrere il testo
            --textVec.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            //Mostra sfondo e testo
            background.DrawBackground(spriteBatch);
            spriteBatch.DrawString(font, text, textVec, Color.White);

            spriteBatch.End();
        }
    }
}
