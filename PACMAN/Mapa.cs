using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACMAN
{
    public class Mapa
    {
        private List<PictureBox> walls; // Inicializar la lista de paredes
        public Mapa(Control.ControlCollection controles, List<PictureBox> walls)
        {
            this.walls = walls;

            // Agregar las PictureBox de las paredes a la lista
            foreach (Control control in controles)
            {
                if (control is PictureBox pictureBox && pictureBox.Tag?.ToString() == "wall")
                {
                    walls.Add(pictureBox);
                }
            }
        }

        public bool CollisionWalls(Rectangle rectangulo)
        {
            foreach (PictureBox wall in walls)
            {
                if (rectangulo.IntersectsWith(wall.Bounds))
                {
                    return true; // Hay colisión con pared
                }
            }
            return false; // No hay colisión con pared
        }
    }
}
