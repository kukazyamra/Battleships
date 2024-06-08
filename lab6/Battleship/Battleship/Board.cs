namespace Battleship
{
    public class Board
    {
        public Cell[,] Cells;
        public Board()
        {
            Cells = new Cell[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Cells[i, j] = new Cell();
                }
            }
        }

        public bool ValidatePlacements()
        {
            if (!CheckDiagonalNeighbours()) return false;
            if (!CheckShipSizes()) return false;

            return true;
        }

        private int GetShipSize(Cell[,] board, bool[,] visited, int x, int y)
        {
            int size = 0;
            int nx = x, ny = y;
            while (ny < 10 && board[nx, ny].CellState == CellStates.Ship)
            {
                visited[nx, ny] = true;
                size++;
                ny++;
            }
            if (size > 1)
            {
                return size;
            }
            size = 0;
            ny = y;
            while (nx < 10 && board[nx, ny].CellState == CellStates.Ship)
            {
                visited[nx, ny] = true;
                size++;
                nx++;
            }
            return size;
        }

        public List<(int, int)> GetDiagonalNeighbours(int x, int y)
        {
            var res = new List<(int, int)>();
            var possibleNeighbours = new (int, int)[] { (x - 1, y - 1), (x + 1, y - 1), (x + 1, y + 1), (x - 1, y + 1) };
            foreach (var neighbour in possibleNeighbours)
            {
                if (neighbour.Item1 > -1 && neighbour.Item1 < 10 && neighbour.Item2 > -1 && neighbour.Item2 < 10)
                {
                    res.Add(neighbour);
                }
            }
            return res;
        }
        public List<int> FindShips()
        {
            var foundShips = new List<int>();
            var visited = new bool[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Cells[i, j].CellState == CellStates.Ship && !visited[i, j])
                    {
                        int shipSize = GetShipSize(Cells, visited, i, j);
                        foundShips.Add(shipSize);
                    }
                }
            }
            return foundShips;
        }

        public bool CheckShipSizes()
        {
            var shipSizes = new Dictionary<int, int>
            {
                { 1, 4 },
                { 2, 3 },
                { 3, 2 },
                { 4, 1 }
            };
            var foundShips = FindShips();
            var foundShipCounts = new Dictionary<int, int>();
            foreach (var size in foundShips)
            {
                if (!foundShipCounts.ContainsKey(size))
                {
                    foundShipCounts[size] = 0;
                }
                foundShipCounts[size]++;
            }
            foreach (var kvp in shipSizes)
            {
                if (!foundShipCounts.ContainsKey(kvp.Key) || foundShipCounts[kvp.Key] != kvp.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckDiagonalNeighbours()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Cells[i, j].CellState == CellStates.Ship)
                    {
                        var diagonalNeighbours = GetDiagonalNeighbours(i, j);
                        foreach (var n in diagonalNeighbours)
                        {
                            if (Cells[n.Item1, n.Item2].CellState == CellStates.Ship)
                            {
                                return false;
                            }
                        }
                    }

                }
            }
            return true;
        }

        public bool HasAdjacentAliveCells(int x, int y)
        {
            var visited = new List<(int, int)>();
            var stack = new Stack<(int, int)>();
            visited.Add((x, y));
            var adjacentCells = GetAdjacentCells(x, y);
            foreach (var cell in adjacentCells)
            {
                stack.Push(cell);
            }
            while (stack.Count > 0)
            {
                var cell = stack.Pop();
                if (Cells[cell.Item1, cell.Item2].CellState == CellStates.Ship)
                {
                    return true;
                }
                if (Cells[cell.Item1, cell.Item2].CellState == CellStates.DamagedShip)
                {
                    var neighbours = GetAdjacentCells(cell.Item1, cell.Item2);
                    foreach (var neighbour in neighbours)
                    {
                        if (!visited.Contains(neighbour))
                        {
                            stack.Push(neighbour);
                            visited.Add(neighbour);
                        }
                    }
                }
            }
            return false;
        }

        public List<(int, int)> GetAdjacentCells(int x, int y)
        {
            List<(int, int)> res = new List<(int, int)>();
            if (x > 0) res.Add((x - 1, y));
            if (x < 9) res.Add((x + 1, y));
            if (y > 0) res.Add((x, y - 1));
            if (y < 9) res.Add((x, y + 1));
            return res;
        }
    }
}
