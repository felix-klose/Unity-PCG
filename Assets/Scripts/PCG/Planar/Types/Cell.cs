namespace PCG.Types.Planar
{
    public enum ECellType
    {
        UNDEFINED = -1,
        WALL = 0,
        FLOOR = 1
    }

    public class Cell
    {
        public ECellType CellType = ECellType.UNDEFINED;
        public int AreaID = -1;
    }
}