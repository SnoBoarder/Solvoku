using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Solvers
{
    public class BaseSolver
    {
        protected const int NUM_ROWS = SudokuBoard.NUM_ROWS;
        protected const int NUM_COLS = SudokuBoard.NUM_COLS;
        protected const int NUM_GRIDS = SudokuBoard.NUM_GRIDS;
        protected const int THIRD_OF_GRID = SudokuBoard.THIRD_OF_GRID;

        private static int[,] _rowsOfSlots;
        private static int[,] _colsOfSlots;
        private static int[,] _3x3OfSlots;

        private static bool[] _values;

        public static void init()
        {
            _values = new bool[NUM_GRIDS];

            int row;
            int col;

            _rowsOfSlots = new int[NUM_ROWS, NUM_COLS];
            for (row = 0; row < NUM_ROWS; ++row)
            {
                for (col = 0; col < NUM_COLS; ++col)
                {
                    _rowsOfSlots[row, col] = getSlotAt(row, col);
                }
            }

            // look up table for every col
            _colsOfSlots = new int[NUM_COLS, NUM_ROWS];
            for (col = 0; col < NUM_COLS; ++col)
            {
                for (row = 0; row < NUM_ROWS; ++row)
                {
                    _colsOfSlots[col, row] = getSlotAt(row, col);
                }
            }

            // look up table for every grid
            _3x3OfSlots = new int[9, 9];
            int oneGridCurrentSize;
            for (int grid = 0; grid < NUM_GRIDS; ++grid)
            {
                oneGridCurrentSize = 0;
                for (row = 0; row < THIRD_OF_GRID; ++row)
                {
                    for (col = 0; col < THIRD_OF_GRID; ++col)
                    {
                        int currRow = Convert.ToInt32(grid / THIRD_OF_GRID) * THIRD_OF_GRID + row;
                        int currCol = (grid % THIRD_OF_GRID) * THIRD_OF_GRID + col;

                        _3x3OfSlots[grid, oneGridCurrentSize] = getSlotAt(currRow, currCol);
                        ++oneGridCurrentSize;
                    }
                }
            }
        }

        protected static void resetValues()
        {
            int len = _values.Length;
            for (int i = 0; i < len; ++i)
            {
                _values[i] = true;
            }
        }

        public static bool validBoard(int[] cells)
        {
            int len = NUM_ROWS;
            for (int i = 0; i < len; ++i)
            {
                if (!validRow(cells, i))
                    return false;

                if (!validCol(cells, i))
                    return false;

                if (!validGrid(cells, i))
                    return false;
            }

            return true;
        }

        public static bool validSlot(int[] cells, int slotId)
        {
            if (!validRow(cells, getRowOfSlot(slotId)))
                return false;

            if (!validCol(cells, getRowOfSlot(slotId)))
                return false;

            if (!validGrid(cells, getGridOfSlot(slotId)))
                return false;

            return true;
        }

        public static bool validRow(int[] cells, int row)
        {
            // reset the values array to check against
            resetValues();

            int value;
            int len = NUM_ROWS;
            for (int i = 0; i < len; ++i)
            {
                value = cells[_rowsOfSlots[row, i]];
                if (value == SudokuBoard.EMPTY_CELL)
                    continue;

                if (_values[value - 1])
                { // valid move, clear the value for next time
                    _values[value - 1] = false;
                    continue;
                }

                // invalid move! GET OUT
                return false;
            }

            return true;
        }

        public static bool validCol(int[] cells, int col)
        {
            // reset the values array to check against
            resetValues();

            int value;
            int len = NUM_COLS;
            for (int i = 0; i < len; ++i)
            {
                value = cells[_colsOfSlots[col, i]];
                if (value == SudokuBoard.EMPTY_CELL)
                    continue;

                if (_values[value - 1])
                { // valid move, clear the value for next time
                    _values[value - 1] = false;
                    continue;
                }

                // invalid move! GET OUT
                return false;
            }

            return true;
        }

        public static bool validGrid(int[] cells, int grid)
        {
            // reset the values array to check against
            resetValues();

            int value;
            int len = NUM_GRIDS;
            for (int i = 0; i < len; ++i)
            {
                value = cells[_3x3OfSlots[grid, i]];
                if (value == SudokuBoard.EMPTY_CELL)
                    continue;

                if (_values[value - 1])
                { // valid move, clear the value for next time
                    _values[value - 1] = false;
                    continue;
                }

                // invalid move! GET OUT
                return false;
            }

            return true;
        }

        public static int getRowOfSlot(int slot)
        {
            return slot / SudokuBoard.NUM_COLS;
        }

        public static int getColOfSlot(int slot)
        {
            return slot % SudokuBoard.NUM_COLS;
        }

        public static int getGridOfSlot(int slot)
        {
            return Convert.ToInt32(getRowOfSlot(slot) / THIRD_OF_GRID) * THIRD_OF_GRID + Convert.ToInt32(getColOfSlot(slot) / THIRD_OF_GRID);
        }

        public static int getSlotAt(int row, int col)
        {
            return row * SudokuBoard.NUM_COLS + col;
        }
    }

}
