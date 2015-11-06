using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheBilliard
{
    /// <summary>
    /// Classe che contiene tutte le variabili del giocatore
    /// </summary>
    class Player
    {
        //stringa che salva le palle da mandare in buca
        string ballsToPocket;
        public string BallsToPocket { get { return ballsToPocket; } }
        List<int> ballsPocket; // numeri delle palle imbucate
        public List<int> BallPocket { get { return ballsPocket; } }
        public bool GameWon; //true se il gioco è stato vinto 
        public bool Only8ToPocket; //true se manca solo la 8

        public Player()
        {
            GameWon = false;
            Only8ToPocket = false;
            ballsToPocket = "";
            ballsPocket = new List<int>();
        }
        /// <summary>
        /// Metodo che aggiunge una palla alla lista delle imbucate
        /// </summary>
        /// <param name="ball">Numero della palla</param>
        public void AddBallToPocket(int ball)
        {
            ballsPocket.Add(ball);
        }
        /// <summary>
        /// Metodo che setta il tipo di palle da imbucare dello stesso tipo di numberBall
        /// </summary>
        /// <param name="numberBall">Numero di palla</param>
        public void SetBallsToPocket(int numberBall)
        {
            if(ballsToPocket == "")
            {
                if (numberBall < 8 && numberBall > 0)
                    ballsToPocket = "Solids";
                else if (numberBall > 8 && numberBall < 16)
                    ballsToPocket = "Stripes";
            }
        }
        /// <summary>
        /// Metodo che ritorna se il giocatore ha imbucato la palla giusta
        /// </summary>
        /// <param name="numberBall">Numero di palla</param>
        /// <returns></returns>
        public bool BallCompatible(int numberBall)
        {
            if (ballsToPocket == "" && numberBall != 0)
                return true;
            else if (ballsToPocket == "Solids" && numberBall < 8 && numberBall > 0)
                return true;
            else if (ballsToPocket == "Stripes" && numberBall > 8 && numberBall < 16)
                return true;
            else
                return false;
        }
    }
}
