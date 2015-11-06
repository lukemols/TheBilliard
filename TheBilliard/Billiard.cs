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
    /// Classe che contiene il gioco vero e proprio
    /// </summary>
    class Billiard
    {
        public Vector2 boardPos; ///posizione del tavolo
        public Vector2 boardDim; ///dimensioni del tavolo
        public Ball[] balls;///array di palle
        public int numBalls; ///numero di palle
        double ballDiam; ///grandezza delle palle
        public bool initialized;///bool che dice se il biliardo è stato inizializzato
        public bool isStopped;///bool che dice se le palline sono tutte ferme
        public int firstBall; /// intero che indica la prima pallina colpita
        public bool isFault; ///Bool che indica se il giocatore ha fatto fallo
        public bool gamePocketed; /// Bool che indica se è stata messa in buca una pallina nel turno attuale

        public Player player1;//Giocatore 1
        public Player player2;// Giocatore 2
        bool nextPlayerSelected; // Indica se è già stato scelto il prossimo giocato a tirare
        string actualPlayer; // Prossimo giocatore
        public string ActualPlayer { get { return actualPlayer; } }
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="bp">Posizione del tavolo</param>
        public Billiard(Vector2 bp)
        {
            boardDim = new Vector2(700, 350);
            boardPos = bp;
            numBalls = 16;
            balls = new Ball[numBalls];
            ballDiam = 20;
            initialized = false;
        }

        /// <summary>
        /// Metodo che inizializza il biliardo
        /// </summary>
        public void InitBilliard()
        {
            int xPos = (int)boardPos.X + 100;
            int yPos = (int)boardPos.Y + 125;
            Vector2 zero = new Vector2(0, 0);
            firstBall = 0;
            gamePocketed = false;

            player1 = new Player();
            player2 = new Player();
            actualPlayer = "Player 1";
            nextPlayerSelected = true;
            ///Posiziona le palline a triangolo
            int i;
            int j = 1;
            for (i = 0; i < 5; i++, j++)
            {
                balls[j] = new Ball(new Vector2(xPos, yPos + i * (int)ballDiam), zero, ballDiam);
            }
            xPos += (int)ballDiam;
            yPos += (int)ballDiam / 2;

            for (i = 0; i < 4; i++, j++)
            {
                balls[j] = new Ball(new Vector2(xPos, yPos + i * (int)ballDiam), zero, ballDiam);
            }
            xPos += (int)ballDiam;
            yPos += (int)ballDiam / 2;

            for (i = 0; i < 3; i++, j++)
            {
                balls[j] = new Ball(new Vector2(xPos, yPos + i * (int)ballDiam), zero, ballDiam);
            }
            xPos += (int)ballDiam;
            yPos += (int)ballDiam / 2;

            for (i = 0; i < 2; i++, j++)
            {
                balls[j] = new Ball(new Vector2(xPos, yPos + i * (int)ballDiam), zero, ballDiam);
            }
            xPos += (int)ballDiam;
            yPos += (int)ballDiam / 2;

            for (i = 0; i < 1; i++, j++)
            {
                balls[j] = new Ball(new Vector2(xPos, yPos + i * (int)ballDiam), zero, ballDiam);
            }

            balls[0] = new Ball(new Vector2((int)boardPos.X + 600, (int)boardPos.Y + 165), new Vector2(0, 0), ballDiam);

            initialized = true;
            isStopped = true;
        }

        /// <summary>
        /// Metodo che viene chiamato ad ogni refresh e che si occupa di chiamare tutti i metodi necessari al gioco
        /// </summary>
        public void UpdateBilliard()
        {
            int i;
            double T = 1.0;

            Vector2 PT;

            //Se dovesse avvenire una collisione tra i due frame si perderebbe. Per questo è stata inserito il seguente codice
            if ((T = NextCollisionTime()) > 1.0)
                T = 1.0;

            ///aggiorna la posizione per tutte le palle
            for (i = 0; i < balls.Length; i++)
            {
                PT = balls[i].position; //posizione di partenza
                PT += Vector2.Multiply(balls[i].speed, (float)T); //Spazio = velocità x tempo
                balls[i].position = PT;
            }

            CollisionWithHole();//Controlla se vi sono palle che entrano in buca
            CollisionWithBall();//Controlla se vi sono palle che si colpiscono tra loro
            CollisionWithFrame();//Controlla se vi sono palle che colpiscono il bordo
            UpdateVelocity(); //Aggiorna la velocità

            //Se sono tutte ferme
            if (isStopped)
            {
                CheckMissingBalls();//controlla se un giocatore deve mettere in buca solo la 8
                //ResetWhiteBall();//Dopo un fallo riposiziona la bianca
                SelectNextPlayer();//Seleziona il prossimo giocatore
            }
        }
        /// <summary>
        /// Metodo che controlla le eventuali palle in buca
        /// </summary>
        public void CollisionWithHole()
        {
            for (int i = 0; i < balls.Length; i++)
            {//Sei casi per la buca
                if (balls[i].inGame == true && (balls[i].position.Y < boardPos.Y + (ballDiam / 2) || balls[i].position.Y > boardPos.Y + 350 - (ballDiam)))
                {
                    if (balls[i].position.X < boardPos.X + (ballDiam / 2))
                        balls[i].inGame = false;
                    if (balls[i].position.X > boardPos.X + 700 - (ballDiam))
                        balls[i].inGame = false;
                    if (balls[i].position.X == boardPos.X + 350 - (ballDiam / 2))
                        balls[i].inGame = false;
                    if (!balls[i].inGame) //se è appena stata mandata in buca
                    {
                        if(i != 0)
                            gamePocketed = true; //Il giocatore ha mandato una palla in buca (diversa dalla bianca)
                        if (actualPlayer == "Player 1")
                        {
                            if (i == 8 && !player1.Only8ToPocket) // se il giocatore ha mandato la 8 in buca senza aver mandato le altre prima
                                player2.GameWon = true; //vince l'avversario,
                            else if (i == 8 && player1.Only8ToPocket)//altrimenti ha vinto lui
                                player1.GameWon = true;
                            if (player1.BallsToPocket == "") //se non aveva ancora mandato nulla in buca
                            {
                                player1.SetBallsToPocket(i); //assegna il tipo di palle uguale a quella appena mandata in buca a lui
                                player2.SetBallsToPocket((i + 8) % 16); //e l'altro all'avversario
                            }
                            if(!isFault) // Se non è ancora stato fatto fallo controlla
                                isFault = !player1.BallCompatible(i);//che non abbia mandato in buca quella dell'avversario
                            //(Quanto fatto sopra è necessario poiché se il giocatore mandasse in buca una sua palla ma aveva già fatto fallo
                            //in precedenza il fallo sarebbe stato "annullato"
                            if(i != 0)
                                player1.AddBallToPocket(i);//aggiungi la palla alla lista di quelle mandate in buca
                        }
                        else if (actualPlayer == "Player 2") // vedi sopra
                        {
                            if (i == 8 && !player2.Only8ToPocket)
                                player1.GameWon = true;
                            else if (i == 8 && player2.Only8ToPocket)
                                player2.GameWon = true;
                            if (player2.BallsToPocket == "")
                            {
                                player2.SetBallsToPocket(i);
                                player1.SetBallsToPocket((i + 8) % 16);
                            }
                            if (!isFault) 
                                isFault = !player2.BallCompatible(i);
                            if (i != 0)
                                player2.AddBallToPocket(i);
                        }
                    }
                }
            }
            if (!balls[0].inGame) // Se la bianca non è più in gioco -> fallo!
                isFault = true;
        }

        /// <summary>
        /// Metodo che imprime la velocità iniziale alla palla bianca
        /// </summary>
        public void CollisionWithCue(Cue cue)
        {
            isFault = false;
            gamePocketed = false;
            nextPlayerSelected = false;
            isStopped = false;
            //mantieni la rotazione tra 0 e 2pi
            while (cue.rotation >= 6.28f)
            {
                cue.rotation -= 6.28f;
            }
            while (cue.rotation < 0.0f)
            {
                cue.rotation += 6.28f;
            }

            double alpha;

            if (cue.rotation <= Math.PI)
                alpha = -1 * (Math.PI - cue.rotation);
            else
                alpha = cue.rotation - Math.PI;
            //calcolo della velocità impressa
            balls[0].speed.X = (float)(cue.power / 2 * Math.Cos(alpha));
            balls[0].speed.Y = (float)(cue.power / 2 * Math.Sin(alpha));
            firstBall = 0; // prima palla colpita = 0 -> se non ne colpisce altre sarà fallo (o anche se colpisce quella sbagliata)
        }

        /// <summary>
        /// Metodo che calcola il tempo della prossima collisione (serve per evitare che un attimo prima le palle si
        /// stiano avvicinando e un attimo dopo si stiano allontanando senza essersi scontrate)
        /// </summary>
        /// <returns>Ritorna un valore minore di 1 se ci sarà una collisione prima del prossimo frame</returns>
        public double NextCollisionTime()
        {
            double Time = 1.0;
            double TimeCollisionIJ = 1.0;
            int i, j;


            for (i = 0; i < balls.Length; i++)
            {
                for (j = i + 1; j < balls.Length; j++)
                {
                    if (balls[i].inGame == false || balls[j].inGame == false)
                        continue;
                    if (balls[i].MovingToBall(balls[j]))
                        TimeCollisionIJ = balls[i].TimeCollision(balls[j]);
                    if (TimeCollisionIJ > 0)
                        Time = Math.Min(Time, TimeCollisionIJ);
                }
            }

            return Time;
        }

        /// <summary>
        /// Metodo che effettua il controllo per la collisione con il bordo
        /// </summary>
        public void CollisionWithFrame()
        {
            //se colpisce il bordo fai tornare indietro la palla moltiplicando per -1
            for (int i = 0; i < balls.Length; i++)
            {
                if ((balls[i].position.X - boardPos.X < 0 && balls[i].speed.X < 0) || (balls[i].position.X > boardPos.X + 700 - ballDiam && balls[i].speed.X > 0))
                {
                    balls[i].speed.X *= -1;
                }

                if ((balls[i].position.Y - boardPos.Y < 0 && balls[i].speed.Y < 0) || (balls[i].position.Y > boardPos.Y + 350 - ballDiam && balls[i].speed.Y > 0))
                {
                    balls[i].speed.Y *= -1;
                }
            }
        }

        /// <summary>
        /// Metodo che contiene il controllo della collisione tra le palle
        /// </summary>
        public void CollisionWithBall()
        {
            int i, j;
            for (i = 0; i < balls.Length; i++)
            {
                for (j = i + 1; j < balls.Length; j++)
                {
                    if (balls[i].inGame == false || balls[j].inGame == false)
                        continue;
                    if (balls[i].Distance(balls[j]) && balls[i].MovingToBall(balls[j]))
                    {
                        balls[i].Collide(balls[j]);
                        if (i == 0 && firstBall == 0) //La prima palla colpita è la j-esima
                        {
                            firstBall = j;
                            //Se non sono "compatibili" è fallo
                            if(actualPlayer == "Player 1")
                                isFault = !player1.BallCompatible(j);
                            else //if (actualPlayer == "Player 2")
                                isFault = !player2.BallCompatible(j);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metodo che aggiorna la velocità delle palle
        /// </summary>
        public void UpdateVelocity()
        {
            isStopped = true;
            int i;
            for (i = 0; i < balls.Length; i++)
            {
                //0.992 è il coefficiente di attrito con il campo
                balls[i].speed = Vector2.Multiply(balls[i].speed, 0.992f);

                //se la velocità è in modulo minore di 0.05 ferma la pallina oppure se è andata in buca
                //(Quest'ultimo perché se la palla finita in buca ci finisce con una certa velocità le ci vorrà un po' per fermarsi)
                if (Math.Sqrt(balls[i].speed.X * balls[i].speed.X + balls[i].speed.Y * balls[i].speed.Y) < 0.05 || !balls[i].inGame)
                {
                    balls[i].speed.X = 0;
                    balls[i].speed.Y = 0;
                }
                else
                    isStopped = false;
                
            }
        }
        /// <summary>
        /// Controlla se mancano delle palle prima di poter imbucare la 8
        /// </summary>
        public void CheckMissingBalls()
        {
            bool missingFirst8 = true;
            bool missingLast8 = true;
            for(int i = 1; i < 16; i++)
            {
                if(!balls[i].inGame && i < 8)
                {
                    missingFirst8 = false;
                    i = 8;
                }
                else if(!balls[i].inGame && i > 8)
                {
                    missingLast8 = false;
                    break;
                }
            }
            if (missingFirst8)
            {
                if (player1.BallsToPocket == "Solids")
                    player1.Only8ToPocket = true;
                else if (player2.BallsToPocket == "Solids")
                    player2.Only8ToPocket = true;
            }

            if (missingLast8)
            {
                if (player1.BallsToPocket == "Stripes")
                    player1.Only8ToPocket = true;
                else if (player2.BallsToPocket == "Stripes")
                    player2.Only8ToPocket = true;
            }
        }

        public void SelectNextPlayer()
        {
            if (nextPlayerSelected)
                return;
            //se non è stata mandata in buca nessuna palla, oppure è stato fatto fallo cambia giocatore
            if(isFault || firstBall == 0 || !gamePocketed)
            {
                if (actualPlayer == "Player 1")
                    actualPlayer = "Player 2";
                else //if (actualPlayer == "Player 2")
                    actualPlayer = "Player 1";
                if (firstBall == 0)
                    isFault = true;
                if (isFault)
                    ResetWhiteBall();
            }
            nextPlayerSelected = true;
        }
        /// <summary>
        /// Riposiziona la palla bianca
        /// </summary>
        public void ResetWhiteBall()
        {
            //questo serve per settare la posizione di partenza una volta al turno
            if (nextPlayerSelected || !isFault)
                return;
            balls[0].inGame = true;
            balls[0].position.X = (int)boardPos.X + 600;
            balls[0].position.Y = (int)boardPos.Y + 165;
        }
    }
}
