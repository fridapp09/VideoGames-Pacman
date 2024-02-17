using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PACMAN
{
    public class Ghost
    {
        private List<PictureBox> ghosts;

        private int speed;
        private bool ghostEatable; // Agrega una variable para controlar si los fantasmas son vulnerables o no

        // Diccionario para almacenar la dirección actual de cada fantasma
        private Dictionary<PictureBox, Point> ghostDirections;

        public Ghost(List<PictureBox> ghosts, int speed)
        {
            this.ghosts = ghosts;
            this.speed = speed;
            this.ghostEatable = false; // Inicialmente, los fantasmas no son vulnerables
            this.ghostDirections = new Dictionary<PictureBox, Point>();

            // Inicializar la dirección de cada fantasma como moviéndose hacia la derecha o hacia abajo
            foreach (PictureBox ghost in ghosts)
            {
                if (ghost.Name == "redGhost" || ghost.Name == "blueGhost")
                {
                    ghostDirections.Add(ghost, new Point(speed, 0));
                }
                else
                {
                    ghostDirections.Add(ghost, new Point(0, speed));
                }
            }
        }

        // Método para cambiar la imagen del fantasma a deadGhost
        public void SetGhostImageToDead()
        {
            foreach (PictureBox ghost in ghosts)
            {
                // Verificar si el fantasma ya está muerto para evitar cambios innecesarios
                if (ghost.Image != Resource1.deadGhost)
                {
                    ghost.Image = Resource1.deadGhost;
                }
            }
        }

        // Método para actualizar el estado de vulnerabilidad de los fantasmas
        public void UpdateGhostVulnerability(bool vulnerable)
        {
            ghostEatable = vulnerable;
        }

        public void MoveGhosts(List<PictureBox> walls, PictureBox pacman, Form1 form)
        {
            foreach (PictureBox ghost in ghosts)
            {
                Point direction = ghostDirections[ghost]; // Obtener la dirección actual

                // Calcular la nueva posición del fantasma
                int newX = ghost.Location.X + direction.X;
                int newY = ghost.Location.Y + direction.Y;

                // Verificar si el movimiento resulta en una colisión con las paredes
                if (IsWallAhead(ghost, walls, newX, newY))
                {
                    // Si hay colisión, cambiar la dirección en el eje correspondiente
                    if (direction.X != 0)
                    {
                        direction.X = -direction.X; // Cambiar la dirección horizontal
                    }
                    else
                    {
                        direction.Y = -direction.Y; // Cambiar la dirección vertical
                    }
                }

                // Verificar colisión con Pacman
                if (pacman.Bounds.IntersectsWith(ghost.Bounds))
                {
                    // Verificar si el fantasma es comestible (tiene la imagen de deadGhost)
                    if (ghostEatable)
                    {
                        // "Comer" al fantasma
                        form.Controls.Remove(ghost);
                        ghosts.Remove(ghost);
                        return; // Salir del método para detener el movimiento de los fantasmas
                    }
                    else if (!ghostEatable)
                    {
                        // Mostrar el label "Perdiste" solo si el fantasma no es vulnerable
                        form.ShowLossMessage();
                        return; // Salir del método para detener el movimiento de los fantasmas
                    }
                }

                // Actualizar la dirección del fantasma
                ghostDirections[ghost] = direction;

                // Mover el fantasma a la nueva posición
                ghost.Left = newX;
                ghost.Top = newY;
            }
        }

        private bool IsWallAhead(PictureBox ghost, List<PictureBox> walls, int newX, int newY)
        {
            foreach (PictureBox wall in walls)
            {
                if (new Rectangle(newX, newY, ghost.Width, ghost.Height).IntersectsWith(wall.Bounds))
                {
                    return true; // Hay una pared en el camino
                }
            }
            return false; // No hay pared en el camino
        }
    }
}