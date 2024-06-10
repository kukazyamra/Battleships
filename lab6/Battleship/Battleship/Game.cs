namespace Battleship
{
    public class Game
    {
        public Player[] Players;

        public bool ValidateName(string name)
        {
            return name != "" && name != Players[0].Name && name != Players[1].Name;
        }

        public Game()
        {
            Players = new Player[2];
            Players[0] = new Player("");
            Players[1] = new Player("");
        }   

        public Player? CheckWin()
        {
            if (!Players[0].HasAliveShips()) return Players[1];
            if (!Players[1].HasAliveShips()) return Players[0];
            return null;
        }

        public void HandleShot((int, int) coordinates, Player player)
        {
            var cell = player.Board.Cells[coordinates.Item1, coordinates.Item2];

            if (cell.CellState == CellStates.Empty)
            {
                cell.CellState = CellStates.Miss;
            }
            else if (cell.CellState == CellStates.Ship)
            {
                if (player.Board.HasAdjacentAliveCells(coordinates.Item1, coordinates.Item2))
                {
                    cell.CellState = CellStates.DamagedShip;
                }
                else
                {
                    var stack = new Stack<(int, int)>();
                    cell.CellState = CellStates.DestroyedShip;
                    var adjacent = player.Board.GetAdjacentCells(coordinates.Item1, coordinates.Item2);
                    for (int i = 0; i < adjacent.Count; i++)
                    {
                        stack.Push(adjacent[i]);
                    }
                    while (stack.Count > 0)
                    {
                        var coords = stack.Pop();
                        if (player.Board.Cells[coords.Item1, coords.Item2].CellState == CellStates.DamagedShip)
                        {
                            player.Board.Cells[coords.Item1, coords.Item2].CellState = CellStates.DestroyedShip;

                            var moreCells = player.Board.GetAdjacentCells(coords.Item1, coords.Item2);
                            for (int j = 0; j < moreCells.Count; j++)
                            {
                                stack.Push(moreCells[j]);
                            }
                        }
                    }
                }

            }
        }
        public void FinishGame()
        {
            Players = new Player[2];
            Players[0] = new Player("");
            Players[1] = new Player("");
        }
    }
}
