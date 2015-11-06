#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace TheBilliard
{
    /// <summary>
    /// Classe che implementa la stecca
    /// </summary>
    class Cue
    {
        public float rotation; //rotazione
        public float power; //potenza impressa
        public bool isReturning;//se si sta allontanando dalla palla
        public bool isPointing;//se sta puntando la palla

        public Cue()
        {
            rotation = 0;
            power = 0f;
            isReturning = false;
            isPointing = false;
        }
    }
}
