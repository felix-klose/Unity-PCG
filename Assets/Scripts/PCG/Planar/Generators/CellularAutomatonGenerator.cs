using PCG.Types.Planar;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace PCG.Generators.Planar
{
    public struct SCellularAutomatonRule
    {
        public int Iterations;
        public int MinWalls;
        public int MaxWalls;

        public SCellularAutomatonRule(int iterations, int minWalls = -1, int maxWalls = -1)
        {
            Iterations = iterations;
            MinWalls = minWalls;
            MaxWalls = maxWalls;
        }
    }
    public class CellularAutomatonGenerator
    {
        private int m_width;
        private int m_height;

        private CellGrid m_grid;
        private CellGrid m_tmpGrid;

        public SCellularAutomatonRule[] Rules = { new SCellularAutomatonRule(4, 2, 5), new SCellularAutomatonRule(3, 0, 5) };
        public int InitialWallRatio = 60;

        public CellGrid Cells { get => m_grid; }
        public int Width { get => m_width; }
        public int Height { get => m_height; }

        public CellularAutomatonGenerator(int width, int height)
        {
            m_width = width;
            m_height = height;

            m_grid = new CellGrid(m_width, m_height);
            m_tmpGrid = new CellGrid(m_width, m_height);
        }

        public void Generate(bool fillHoles = false)
        {
            Init();
            foreach (SCellularAutomatonRule rule in Rules)
            {
                for (int i = 0; i < rule.Iterations; i++)
                {
                    Step(rule);
                }
            }
            if (fillHoles)
            {
                m_grid.DetermineAreas();
                m_grid.GetAreaSizes();
                FillHoles();
            }
        }

        private void FillHoles()
        {
            int maxSize = m_grid.AreaSizes.Max(areaSize => { return areaSize.Value; });
            IEnumerable<KeyValuePair<int, int>> smallAreas = m_grid.AreaSizes.Where(areaSize => { return areaSize.Value < maxSize; });

            foreach(KeyValuePair<int, int> areaSize in smallAreas)
            {
                m_grid.FillArea(areaSize.Key, ECellType.WALL, -1);
            }
        }

        private void Init()
        {
            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    Vector2Int curPos = new Vector2Int(x, y);
                    if (x <= 1 || x >= m_width - 2 || y <= 1 || y >= m_height - 2)
                    {
                        m_grid.Cells[curPos].CellType = ECellType.WALL;
                    }
                    else
                    {
                        if (Random.Range(0, 101) < InitialWallRatio)
                            m_grid.Cells[curPos].CellType = ECellType.WALL;
                        else
                            m_grid.Cells[curPos].CellType = ECellType.FLOOR;
                    }
                }
            }
        }

        private void Step(SCellularAutomatonRule rule)
        {
            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    Vector2Int curPos = new Vector2Int(x, y);
                    if (x <= 1 || x >= m_width - 2 || y <= 1 || y >= m_height - 2)
                    {
                        m_tmpGrid.Cells[curPos] = new Cell();
                        m_tmpGrid.Cells[curPos].CellType = ECellType.WALL;
                    } else if(WallNeighbors(curPos, 1) >= rule.MaxWalls || WallNeighbors(curPos, 2) <= rule.MinWalls)
                    {
                        m_tmpGrid.Cells[curPos] = new Cell();
                        m_tmpGrid.Cells[curPos].CellType = ECellType.WALL;
                    } else
                    {
                        m_tmpGrid.Cells[curPos] = new Cell();
                        m_tmpGrid.Cells[curPos].CellType = ECellType.FLOOR;
                    }
                }
            }

            m_grid.Cells.Clear();
            foreach(Vector2Int pos in m_tmpGrid.Cells.Keys)
            {
                m_grid.Cells[pos] = m_tmpGrid.Cells[pos];
            }
            m_tmpGrid.Cells.Clear();
        }

        private int WallNeighbors(Vector2Int position, int steps)
        {
            int wallNeighbors = 0;
            for (int x = position.x - steps; x <= position.x + steps; x++)
            {
                for (int y = position.y - steps; y <= position.y + steps; y++)
                {
                    if (x == position.x && y == position.y)
                        continue;
                    if (m_grid.Cells[new Vector2Int(x, y)].CellType == ECellType.WALL)
                        wallNeighbors++;
                }
            }
            return wallNeighbors;
        }
    }
}