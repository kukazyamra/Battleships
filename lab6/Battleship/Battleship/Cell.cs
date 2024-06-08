namespace Battleship
{
    public enum CellStates
    {
        Empty,
        Ship,
        DamagedShip,
        Miss,
        DestroyedShip
    }

    public class Cell
    {
        public CellStates CellState;
        public Cell()
        {
            CellState = CellStates.Empty;
        }
    }
}
