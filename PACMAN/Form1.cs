using Microsoft.VisualBasic.Logging;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PACMAN
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        int speedPacman = 5;
        int speedGhosts = 5;

        private Point pacmanDirection = new Point(0, 0); // Campo para almacenar la dirección de movimiento de Pacman

        private System.Windows.Forms.Timer starTimer; // Agrega starTimer como un campo de clase
        
        private bool ghostEatable = false; // Definir ghostEatable como un campo de la clase
        private bool gameOver = false;

        List<Point> initialCoinPositions = new List<Point>(); // Lista para almacenar las posiciones iniciales de las monedas
        List<PictureBox> walls = new List<PictureBox>(); // Lista que contiene todos los PictureBox de las paredes
        List<PictureBox> coins = new List<PictureBox>(); // Lista para almacenar las PictureBox de monedas
        List<PictureBox> ghosts = new List<PictureBox>(); // Lista que contiene todos los PictureBox de los fantasmas

        Star star; // Declarar una instancia de Star
        Ghost ghost; // Declarar una instancia de Ghost
        Mapa mapa; // Declarar una instancia de Mapa
        Monedas monedas; // Declarar una instancia de Monedas

        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(500, 500);
            PCT_CANVAS.Image = bmp;
            this.KeyPreview = true;

            foreach (Control control in Controls)
            {
                if (control is PictureBox pictureBox && pictureBox.Tag?.ToString() == "wall")
                {
                    walls.Add(pictureBox);
                }
                else if (control is PictureBox ghostPictureBox && ghostPictureBox.Tag?.ToString() == "ghost")
                {
                    ghosts.Add(ghostPictureBox);
                }
            }

            // Crear una instancia de Monedas y pasarle los controles del formulario
            monedas = new Monedas(this.Controls, coins);
            monedas.InitializeCoins(this.Controls); // Inicializar las monedas antes del bucle foreach

            foreach (PictureBox coin in coins)
            {
                initialCoinPositions.Add(coin.Location);
            }

            // Inicializar la clase Ghost
            ghost = new Ghost(ghosts, speedGhosts);

            // Luego crear una instancia de Star y pasarle la instancia de Ghost
            star = new Star(this.Controls, ghosts, ghostEatable, this, ghost);

            // Inicializar el temporizador de la estrella
            starTimer = new System.Windows.Forms.Timer();
            starTimer.Interval = 4000; // Duración del efecto en milisegundos (4 segundos)
            starTimer.Tick += StarTimer_Tick;

            // Establece la imagen del Pacman para que mire hacia la derecha
            pacman.Image = Resource1.pacmanright;

            // Crear una instancia de Monedas y pasarle los controles del formulario
            monedas = new Monedas(this.Controls, coins);
            monedas.InitializeCoins(this.Controls);

            // Inicializar la clase Paredes
            mapa = new Mapa(this.Controls, walls);

            // Iniciar el temporizador
            timer1.Start();

            timer1.Enabled = true;
            timer1.Tick += Timer1_Tick;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Cuando el jugador presiona una tecla de dirección, cambiar la dirección de Pacman
            switch (keyData)
            {
                case Keys.Left:
                    pacmanDirection = new Point(-speedPacman, 0);
                    pacman.Image = Resource1.pacmanleft; // Cambiar la imagen del pacman hacia la izquierda
                    break;
                case Keys.Right:
                    pacmanDirection = new Point(speedPacman, 0);
                    pacman.Image = Resource1.pacmanright; // Cambiar la imagen del pacman hacia la derecha
                    break;
                case Keys.Up:
                    pacmanDirection = new Point(0, -speedPacman);
                    pacman.Image = Resource1.pacmanup; // Cambiar la imagen del pacman hacia arriba
                    break;
                case Keys.Down:
                    pacmanDirection = new Point(0, speedPacman);
                    pacman.Image = Resource1.pacmandown; // Cambiar la imagen del pacman hacia abajo
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MovePacman(int deltaX, int deltaY)
        {
            int newX = pacman.Location.X + deltaX;
            int newY = pacman.Location.Y + deltaY;

            // Verificar si Pacman atraviesa los lados verticales abiertos del mapa
            if (newX < 0)
            {
                newX = this.ClientSize.Width - pacman.Width; // Coloca a Pacman en el lado derecho
            }
            else if (newX > this.ClientSize.Width - pacman.Width)
            {
                newX = 0; // Coloca a Pacman en el lado izquierdo
            }

            // Verificar si el movimiento está permitido en el eje Y
            bool canMoveY = true;
            foreach (PictureBox wall in walls)
            {
                if (new Rectangle(newX, newY, pacman.Width, pacman.Height).IntersectsWith(wall.Bounds))
                {
                    canMoveY = false;
                    break;
                }
            }

            // Realizar el movimiento si es posible en el eje Y
            if (canMoveY)
            {
                pacman.Location = new Point(newX, newY);
            }

            // Verificar colisiones con las estrellas
            star.CollisionStar(pacman);

            // Verificar colisiones con las monedas
            monedas.CollisionCoins(pacman, label2);

            // Checar si todas las monedas y estrellas han sido recolectadas
            if (monedas.coins.Count == 0 && star.stars.Count == 0)
            {
                // Mostrar etiqueta de victoria
                label3.Visible = true;
                // Detener el juego
                timer1.Stop();
                gameOver = true;
                StopAllMovements(); // Detener los movimientos al perder el juego
            }
        }

        private void StartPacmanAutoMove()
        {
            // Iniciar el movimiento automático de Pacman hacia la derecha
            pacmanDirection = new Point(speedPacman, 0);
            timer1.Start(); // Iniciar el temporizador para controlar el movimiento de Pacman
        }

        private void StopPacmanAutoMove()
        {
            // Detener el movimiento automático de Pacman
            pacmanDirection = new Point(0, 0);
            timer1.Stop(); // Detener el temporizador que controla el movimiento de Pacman
        }

        private void StarTimer_Tick(object sender, EventArgs e)
        {
            // Restaura la imagen original de los fantasmas cuando el temporizador finaliza
            star.RestoreGhostsImage();
            star.ResetEffect(); // Restablece el estado del efecto
            starTimer.Stop(); // Detiene el temporizador
            ghost.UpdateGhostVulnerability(false);
        }

        public void ShowLossMessage()
        {
            label4.Visible = true;
            gameOver = true;
            StopAllMovements(); // Detener los movimientos al perder el juego
        }

        public void Timer1_Tick(object sender, EventArgs e)
        {
            // Llamar al método para mover los fantasmas en cada intervalo de temporizador
            ghost.MoveGhosts(walls, pacman, this);
            MovePacman(pacmanDirection.X, pacmanDirection.Y); // Mover Pacman en la dirección predeterminada
            // Verificar colisiones con los fantasmas muertos
            DeadGhost.CollisionDeadGhost(pacman, ghosts, ghostEatable, this);

        }
        public void StopPacmanMovement()
        {
            // Detener el temporizador para evitar que Pacman se mueva
            timer1.Enabled = false;
        }

        private void StopAllMovements()
        {
            timer1.Stop(); // Detiene el temporizador que controla los movimientos de los fantasmas y Pacman
        }
    }
}