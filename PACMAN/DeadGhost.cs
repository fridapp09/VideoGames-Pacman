using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PACMAN
{
    public class DeadGhost
    {
        public static void CollisionDeadGhost(PictureBox pacman, List<PictureBox> ghosts, bool ghostEatable, Form1 form)
        {
            foreach (PictureBox ghost in ghosts.ToArray())
            {
                if (pacman.Bounds.IntersectsWith(ghost.Bounds))
                {
                    if (ghostEatable && ghost.Image == Resource1.deadGhost)
                    {
                        // Solo marca el fantasma muerto como eliminado, no lo elimina directamente
                        ghost.Tag = "removed";
                        return;
                    }
                }
            }
        }
    }
}