using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Player
    {
        public string Name;
        public Board Board;

        public Player(string name)
        {
            this.Name = name;
            this.Board = new Board();
        }

        public bool HasAliveShips()
        {
            var alive = false;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Board.Cells[i, j].CellState == CellStates.Ship)
                    {
                        alive = true;
                    }
                }
            }
            return alive;
        }
    }
}
