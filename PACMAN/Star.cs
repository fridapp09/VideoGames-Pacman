using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Forms;

namespace PACMAN
{
    public class Star
    {
        private Ghost ghost; // Agrega un campo para almacenar la instancia de Ghost

        private Dictionary<PictureBox, Image> originalGhostImages = new Dictionary<PictureBox, Image>();

        public List<PictureBox> stars;
        private List<PictureBox> ghosts;

        //private List<Image> originalGhostImages;
        private System.Windows.Forms.Timer starTimer;

        private bool ghostEatable;

        private bool isEffectActive = false;

        private const int starEffectDuration = 3000;

        public Star(Control.ControlCollection controles, List<PictureBox> ghosts, bool ghostEatable, Form form, Ghost ghost)
        {
            stars = new List<PictureBox>();
            this.ghosts = ghosts;
            this.ghost = ghost;
            this.ghostEatable = ghostEatable;

            InitializeStars(controles);
            InitializeTimer();
        }

        public void InitializeStars(Control.ControlCollection controles)
        {
            stars.Clear();

            foreach (Control control in controles)
            {
                if (control is PictureBox pictureBox && pictureBox.Tag?.ToString() == "star")
                {
                    stars.Add(pictureBox);
                }
            }
        }

        private void InitializeTimer()
        {
            starTimer = new System.Windows.Forms.Timer();
            starTimer.Interval = starEffectDuration;
            starTimer.Tick += StarTimer_Tick;
        }

        private void StarTimer_Tick(object sender, EventArgs e)
        {
            RestoreGhostsImage();
            ResetEffect(); // Restablece el estado del efecto
            starTimer.Stop(); // Detiene el temporizador

            // Actualiza la vulnerabilidad de los fantasmas después de que el efecto de la estrella haya terminado
            ghost.UpdateGhostVulnerability(false);

        }

        private void StartEffectTimer()
        {
            starTimer.Start();
            isEffectActive = true; // Actualizar el estado del efecto a true
        }

        public void CollisionStar(PictureBox pacman)
        {
            foreach (PictureBox star in stars.ToArray())
            {
                if (pacman.Bounds.IntersectsWith(star.Bounds) && !isEffectActive)
                {
                    star.Visible = false;
                    isEffectActive = true; // Marcar el efecto como activado

                    // Actualizar el estado de vulnerabilidad de los fantasmas a través del objeto Ghost
                    ghost.UpdateGhostVulnerability(true); // Establece a true porque Pacman ha consumido una estrella

                    MakeGhostsEatable(); // Cambia las imágenes de los fantasmas
                    StartEffectTimer(); // Inicia el temporizador para restaurar las imágenes de los fantasmas

                    // Eliminar la estrella de la lista para evitar que se active nuevamente
                    stars.Remove(star);
                    break; // Salir del bucle después de eliminar la estrella
                }
            }
        }

        public void ResetEffect()
        {
            isEffectActive = false;
        }

        public void RestoreGhostsImage()
        {
            foreach (var kvp in originalGhostImages)
            {
                kvp.Key.Image = kvp.Value;
            }
        }
        private void MakeGhostsEatable()
        {
            foreach (PictureBox ghost in ghosts)
            {
                originalGhostImages[ghost] = ghost.Image;
                ghost.Image = Resource1.deadGhost;
            }
        }
    }
}
