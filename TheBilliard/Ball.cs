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
    class Ball
    {
        public Vector2 position;/// Posizione della palla
        public Vector2 speed;/// Velocità della palla
        public double diam; /// Diametro della palla
        public bool inGame; /// Indica se la palla è in gioco o è finita in buca
         
        ///Costruttore di default
        public Ball()
        {
            position = new Vector2(0, 0);
            speed = new Vector2(0, 0);
            diam = 1.0f;
            inGame = true;
        }
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="p">Posizione iniziale</param>
        /// <param name="s">Velocità iniziale</param>
        /// <param name="d">Diametro</param>
        public Ball(Vector2 p, Vector2 s, double d)
        {
            position = p;
            speed = s;
            diam = d;
            inGame = true;
        }
        /// <summary>
        /// Metodo che aggiorna la posizione della sfera
        /// </summary>
        public void UpdatePosition()
        {
            position.X += speed.X;
            position.Y += speed.Y;
        }

        /// <summary>
        /// Metodo che ritorna il tempo della prossima collisione tra le due palline
        /// </summary>
        /// <param name="otherBall">Altra pallina con cui effettuare il confronto</param>
        /// <returns>Ritorna in quanto avverrà la prossima collisione</returns>
        public double TimeCollision(Ball otherBall)
        {
            Vector2 dp = position - otherBall.position;
            Vector2 dv = speed - otherBall.speed;
            double dp2 = dp.X * dp.X + dp.Y * dp.Y;
            double dv2 = dv.X * dv.X + dv.Y * dv.Y;

            double A, B, C, Disc;
            A = dv2;
            B = 2.0 * (dp.X * dv.X + dp.Y * dv.Y);
            C = dp2 - Math.Pow(diam, 2.0);

            Disc = Math.Pow(B, 2.0) - 4.0 * A * C;

            if (Disc <= 0)
                return -1.0;

            double T = (-2.0 * (dp.X * dv.X + dp.Y * dv.Y) - Math.Sqrt(Disc)) / (2 * dv2);
            return T;
        }

        /// <summary>
        /// Metodo che calcola la distanza tra la pallina attuale e un'altra
        /// </summary>
        /// <param name="otherBall">Altra sfera con cui effettuare il confronto</param>
        /// <returns></returns>
        public bool Distance(Ball otherBall)
        {
            //per trovare la distanza tra le palle si usa il teorema di pitagora
            //ovvero radice di dx^2 + dy^2
            double dx = position.X - otherBall.position.X;
            double dy = position.Y - otherBall.position.Y;
            dx *= dx;//eleva al quadrato
            dy *= dy;
            double distance = dx + dy;
            distance = Math.Sqrt(distance); //effettua la radice quadrata
            //se la distanza è minore del raggio + 1 della pallina c'è stata una collisione
            if (distance <= diam + 1)
                return true;//collisione
            else
                return false;//no collisione
        }

        /// <summary>
        /// Metodo che controlla se le due palline si stanno muovendo l'una contro l'altra
        /// </summary>
        /// <param name="otherBall">Altra sfera con cui effettuare il confronto</param>
        /// <returns></returns>
        public bool MovingToBall(Ball otherBall)
        {
            Vector2 dp, dv; //vettori differenza di posizione e velocità;
            dp = position - otherBall.position;
            dv = speed - otherBall.speed;

            double X = dp.X * dv.X + dp.Y * dv.Y;
            if (X < 0)
                return true;//si stanno avvicinando
            else
                return false;//si stanno allontanando
        }

        /// <summary>
        /// Metodo che fa scontrare due palline
        /// </summary>
        /// <param name="otherBall">Altra pallina con cui si scontra</param>
        public void Collide(Ball otherBall)
        {
            //calcolo della "normale": differenza tra le posizioni / norma della diff tra p
            Vector2 dp = position - otherBall.position;
            double normaDP = Math.Sqrt(dp.X * dp.X + dp.Y * dp.Y);

            Vector2 normale = Vector2.Divide(dp, (float)normaDP);

            Vector2 tangente = new Vector2(normale.Y * -1, normale.X);

            //si calcolano velocità normali e tangenziali delle due palline
            double vNorm, vTang, oNorm, oTang;

            ///vNorm è il prodotto scalare tra V e la normale, quindi (vector1.X * vector2.X) + (vector1.Y * vector2.Y) 
            vNorm = speed.X * normale.X + speed.Y * normale.Y;
            vTang = speed.X * tangente.X + speed.Y * tangente.Y;///vTang prodotto scalare tra V e la tangente
            oNorm = otherBall.speed.X * normale.X + otherBall.speed.Y * normale.Y; ///come sopra per l'altra palla
            oTang = otherBall.speed.X * tangente.X + otherBall.speed.Y * tangente.Y;

            double nvNorm = oNorm;
            double noNorm = vNorm;
            double nvTang = vTang;
            double noTang = oTang;

            speed = Vector2.Multiply(normale, (float)nvNorm) + Vector2.Multiply(tangente, (float)nvTang);
            otherBall.speed = Vector2.Multiply(normale, (float)noNorm) + Vector2.Multiply(tangente, (float)noTang);

        }
    }
}
