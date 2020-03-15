using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sudoku.Models;

namespace Sudoku.ServiceLayer
{
    public class SudokuService : ISudokuService
    {
        private List<Cell> tmpSolution = null;

        public Grid SetupGrid(int size, string mode)
        {
            int[] regionSize = CalculateRegionSize(size);

            List<Cell> cells = new List<Cell>();
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Cell cell = new Cell
                    {
                        Coordinates = new Point(x, y),
                        Region = new Point(x / regionSize[0], y / regionSize[1])
                    };
                    cells.Add(cell);
                }
            }

            Grid grid = new Grid
            {
                Size = size,
                RegionWidth = regionSize[0],
                RegionHeight = regionSize[1],
                Cells = cells
            };
            return grid;
        }

        private int[] CalculateRegionSize(int size)
        {
            int width = 0;
            int height = 0;
            double sqrt = Math.Sqrt(size);
            if (sqrt % 1 == 0)
            {
                width = height = (int)sqrt;
            }
            else
            {
                for (int i = 1; i < size; i++)
                {
                    double j = (double)size / i;
                    if (j == Math.Floor(j))
                    {
                        width = i;
                        height = (int)j;
                        if (j <= i) break;
                    }
                }
            }

            return new[] {width, height};
        }

        public Grid GenerateSudoku(Grid grid, string difficulty)
        {
            BacktrackingAlgorithm(grid.Size, grid.Cells, false);

            Random random = new Random();
            foreach (int i in Enumerable.Range(0, grid.Cells.Count - 1).OrderBy(x => random.Next()))
            {
                tmpSolution = null;
                List<Cell> tmp = grid.Cells.ConvertAll(x => new Cell { Coordinates = x.Coordinates, Region = x.Region, Value = x.Value });
                tmp[i].Value = null;
                if (!BacktrackingAlgorithm(grid.Size, tmp, true) && tmpSolution != null)
                {
                    grid.Cells[i].Value = null;
                }
            }

            return grid;
        }

        public Grid UpdateGrid(Grid grid, int?[] sudoku)
        {
            for (int i = 0; i < sudoku.Length; i++)
            {
                grid.Cells[i].Value = sudoku[i];
            }
            return grid;
        }

        public Grid SolveSudoku(Grid grid)
        {
            return BacktrackingAlgorithm(grid.Size, grid.Cells, false) ? grid : null;
        }

        private bool BacktrackingAlgorithm(int gridSize, List<Cell> cells, bool checkUniqueness)
        {
            foreach (Cell cell in cells)
            {
                if (cell.Value == null)
                {
                    Random random = new Random();
                    foreach (int value in Enumerable.Range(1, gridSize).OrderBy(x => random.Next()))
                    {
                        if (IsValuePossible(cells, cell, value))
                        {
                            cell.Value = value;
                            if (BacktrackingAlgorithm(gridSize, cells, checkUniqueness))
                            {
                                return true;
                            }
                            cell.Value = null;
                        }
                    }
                    return false;
                }
            }

            if (checkUniqueness && tmpSolution == null)
            {
                tmpSolution = cells.ConvertAll(x => new Cell { Coordinates = x.Coordinates, Region = x.Region, Value = x.Value });
                return false;
            }

            return true;
        }

        private bool IsValuePossible(List<Cell> cells, Cell cell, int value)
        {
            foreach (Cell cellInHouse in cells.Where(x => x.Coordinates.X == cell.Coordinates.X || x.Coordinates.Y == cell.Coordinates.Y || x.Region == cell.Region))
            {
                if (!cellInHouse.Equals(cell) && cellInHouse.Value == value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}