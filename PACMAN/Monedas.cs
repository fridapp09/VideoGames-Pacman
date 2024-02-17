using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACMAN
{
    public class Monedas
    {
        public List<PictureBox> coins; // Inicializar la lista de monedas
        public Monedas(Control.ControlCollection controles, List<PictureBox> coins)
        {
            this.coins = coins;
            InitializeCoins(controles);
        }

        public void InitializeCoins(Control.ControlCollection controles)
        {
            // Limpiar la lista de monedas si ya contiene elementos
            coins.Clear();

            // Agregar las PictureBox de las monedas a la lista
            foreach (Control control in controles)
            {
                if (control is PictureBox pictureBox && pictureBox.Tag?.ToString() == "coin")
                {
                    coins.Add(pictureBox);
                }
            }
        }
        public void CollisionCoins(PictureBox pacman, Label scoreLabel)
        {
            foreach (PictureBox coin in coins.ToArray()) // Usamos ToArray para evitar problemas de modificación de la lista durante la iteración
            {
                if (pacman.Bounds.IntersectsWith(coin.Bounds))
                {
                    coin.Visible = false; // "Come" la moneda
                    coins.Remove(coin); // Elimina la moneda de la lista

                    // Incrementa el contador de puntos
                    int currentScore = int.Parse(scoreLabel.Text.Split(':')[1].Trim()); // Extract numeric part and parse
                    scoreLabel.Text = $"Score: {currentScore + 5}"; // Update label text with incremented score
                }
            }
        }
    }
}