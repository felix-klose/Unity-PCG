using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace PCG.Types.Planar
{
    public class CellGrid
    {
        private int m_width;
        private int m_height;

        private Dictionary<Vector2Int, Cell> m_cells;
        private List<int> m_areas;
        private Dictionary<int, int> m_areaSizes;

        public int Width { get => m_width; }
        public int Height { get => m_height; }
        public Dictionary<Vector2Int, Cell> Cells { get => m_cells; }
        public List<int> Areas { get => m_areas; }
        public Dictionary<int, int> AreaSizes { get => m_areaSizes; }

        public CellGrid(int width, int height)
        {
            m_width = width;
            m_height = height;
            m_cells = new Dictionary<Vector2Int, Cell>();
            m_areas = new List<int>();
            m_areaSizes = new Dictionary<int, int>();

            Init();
        }

        public void DetermineAreas()
        {
            m_areas.Clear();
            PreMapWallAreas();
            while(FindFirstUnmarkedFloor() != new Vector2Int(-1, -1))
            {
                Vector2Int areaStart = FindFirstUnmarkedFloor();
                FloodFillArea(areaStart);
            }
        }

        public void FillArea(int area, ECellType cellType, int newArea)
        {
            IEnumerable<Cell> areaCells = m_cells.Values.Where(cell => { return cell.AreaID == area; });
            foreach (Cell cell in areaCells)
            {
                cell.CellType = cellType;
                cell.AreaID = newArea;
            }
        }

        public void GetAreaSizes()
        {
            m_areaSizes.Clear();
            foreach (int area in m_areas)
            {
                m_areaSizes[area] = m_cells.Values.Count(cell => { return cell.AreaID == area; });
            }
        }
        
        private void Init()
        {
            m_cells.Clear();
            m_areas.Clear();
            m_areaSizes.Clear();

            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    Vector2Int targetPosition = new Vector2Int(x, y);
                    m_cells[targetPosition] = new Cell();
                }
            }
        }

        private void PreMapWallAreas()
        {
            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    Vector2Int targetPosition = new Vector2Int(x, y);
                    if (m_cells[targetPosition].CellType == ECellType.WALL)
                        m_cells[targetPosition].AreaID = 0;
                }
            }
        }

        private Vector2Int FindFirstUnmarkedFloor()
        {
            for(int x = 0; x < m_width; x++)
            {
                for(int y = 0; y < m_height; y++)
                {
                    Vector2Int targetPosition = new Vector2Int(x, y);
                    if (m_cells[targetPosition].CellType == ECellType.FLOOR && m_cells[targetPosition].AreaID == -1)
                    {
                        return targetPosition;
                    }
                }
            }
            return new Vector2Int(-1, -1);
        }

        private void FloodFillArea(Vector2Int areaStart)
        {
            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            stack.Push(areaStart);

            int curArea = 1;
            if (m_areas.Count > 0)
                curArea = m_areas.Max() + 1;
            m_areas.Add(curArea);

            while (stack.Count > 0)
            {
                Vector2Int currentPosition = stack.Pop();
                if(m_cells[currentPosition].AreaID == -1)
                {
                    m_cells[currentPosition].AreaID = curArea;

                    if(currentPosition.x > 1)
                        stack.Push(currentPosition + Vector2Int.left);
                    if (currentPosition.x < m_width + 1)
                        stack.Push(currentPosition + Vector2Int.right);
                    if (currentPosition.y > 1)
                        stack.Push(currentPosition + Vector2Int.down);
                    if (currentPosition.y < m_height + 1)
                        stack.Push(currentPosition + Vector2Int.up);
                }
            }
        }
    }
}
