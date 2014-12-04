using System;
namespace Assets.Scripts.Solvers
{
    public class BacktrackingSolver
    {
        protected const int NUM_ROWS = SudokuBoard.NUM_ROWS;
        protected const int NUM_COLS = SudokuBoard.NUM_COLS;
        protected const int NUM_REGIONS = SudokuBoard.NUM_GRIDS;
        protected const int THIRD_OF_GRID = SudokuBoard.THIRD_OF_GRID;

        private static int[,] _rowsOfCells; // index of every cell organized by row
        private static int[,] _colsOfCells; // index of every cell organized by column
        private static int[,] _regionOfCells; // index of every cell organized by region

        private static bool[] _values; // scratch vector

        private static int _actions;

        private static int _maxDepth;

        public static int getActionCount()
        {
            return _actions;
        }

        public static int getMaxDepthLevel()
        {
            return _maxDepth;
        }

        /// <summary>
        /// The main function to solve the sudoku board by backtracking
        /// </summary>
        /// <param name="cells">Sudoku board</param>
        public static void solve(int[] cells)
        {
            _actions = 0;
            _maxDepth = 0;

            // try to provide more to the backtracking algorithm by setting cells that only have one possiblity
            setOnePossibility(cells);

            // pass the sudoku board in and start at row 0, column 0
            backtrack(cells, 0, 0, 0);
        }

        /// <summary>
        /// Check every cell and see if there's only one possible value that can be within a cell. If so, set it.
        /// </summary>
        /// <param name="cells">Sudoku board</param>
        private static void setOnePossibility(int[] cells)
        {
            int count;
            int value;
            int cellId;
            for (int row = 0; row < NUM_ROWS; ++row)
            {
                for (int col = 0; col < NUM_COLS; ++col)
                {
                    cellId = getCellAt(row, col);
                    int cell = cells[cellId];
                    if (cell == SudokuBoard.EMPTY_CELL)
                    {
                        count = 0;
                        value = 0;
                        // traverse all values and update count if value
                        for (int i = 1; i <= 9; ++i)
                        {
                            ++_actions;

                            cells[cellId] = i;
                            if (validCell(cells, cellId))
                            {
                                count++;
                                value = i;
                            }
                        }

                        if (count == 1)
                        { // if count == 1, keep the value since it's our only option!
                            cells[cellId] = value;
                        }
                        else
                        {// there was either no solution or more than 1 solution. RESET
                            cells[cellId] = SudokuBoard.EMPTY_CELL;

                        }
                    }
                    else
                    {
                        ++_actions;
                    }
                }
            }
        }

        /// <summary>
        /// Main solving method. Recursively calls itself to reach a new row/col
        /// </summary>
        /// <param name="cells">Sudoku board</param>
        /// <param name="row">currnet row</param>
        /// <param name="col">currnet column</param>
        /// <returns>Whether the current backtrack is valid or invalid</returns>
        private static bool backtrack(int[] cells, int row, int col, int depth)
        {
            if (depth > _maxDepth)
                _maxDepth = depth;

            if (col >= 9) // reached the last column of the row
                return backtrack(cells, row + 1, 0, depth + 1); // recursively go to the next row

            if (row == 9) // reached the last row
                return true; // we've found a solution! recursively go back out

            int cellId = getCellAt(row, col); // get the index of the 

            if (cells[cellId] == SudokuBoard.EMPTY_CELL)
            { // cell is empty!
                // traverse every value from 1 to 9
                for (int i = 1; i <= 9; ++i)
                {
                    ++_actions;

                    // set value to cell
                    cells[cellId] = i;

                    // check if the value in the cell is valid (only value in the row, in the column, and in the region)
                    if (validCell(cells, cellId))
                    { // valid value!

                        // continue recursively backtracking
                        if (backtrack(cells, row, col + 1, depth + 1)) // continue backtracking to the next column
                            return true; // we've found a solution! backtrack out
                    }
                }

                // we failed to find a solution with this scenario, reset cell value
                cells[cellId] = SudokuBoard.EMPTY_CELL;

                ++_actions;
            }
            else
            {
                ++_actions;

                // the cell is already occupied, continue to the next column
                return backtrack(cells, row, col + 1, depth + 1);
            }

            // we failed to find a solution with this scenario, backtrack out
            return false;
        }

        /// <summary>
        /// Initialize static look up tables.
        /// </summary>
        public static void init()
        {
            _values = new bool[NUM_REGIONS];

            int row;
            int col;

            _rowsOfCells = new int[NUM_ROWS, NUM_COLS];
            for (row = 0; row < NUM_ROWS; ++row)
            {
                for (col = 0; col < NUM_COLS; ++col)
                {
                    _rowsOfCells[row, col] = getCellAt(row, col);
                }
            }

            // look up table for every col
            _colsOfCells = new int[NUM_COLS, NUM_ROWS];
            for (col = 0; col < NUM_COLS; ++col)
            {
                for (row = 0; row < NUM_ROWS; ++row)
                {
                    _colsOfCells[col, row] = getCellAt(row, col);
                }
            }

            // look up table for every grid
            _regionOfCells = new int[9, 9];
            int oneRegionCurrentSize;
            for (int grid = 0; grid < NUM_REGIONS; ++grid)
            {
                oneRegionCurrentSize = 0;
                for (row = 0; row < THIRD_OF_GRID; ++row)
                {
                    for (col = 0; col < THIRD_OF_GRID; ++col)
                    {
                        int currRow = Convert.ToInt32(grid / THIRD_OF_GRID) * THIRD_OF_GRID + row;
                        int currCol = (grid % THIRD_OF_GRID) * THIRD_OF_GRID + col;

                        _regionOfCells[grid, oneRegionCurrentSize] = getCellAt(currRow, currCol);
                        ++oneRegionCurrentSize;
                    }
                }
            }
        }

        /// <summary>
        /// Simple way to validate that the board passed in is valid.
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static bool validBoard(int[] cells)
        {
            int len = NUM_ROWS;
            for (int i = 0; i < len; ++i)
            {
                if (!validRow(cells, i))
                    return false;

                if (!validCol(cells, i))
                    return false;

                if (!validRegion(cells, i))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check that the cell is valid in all scenarios (row, column, and region)
        /// </summary>
        /// <param name="cells">Sudoku board</param>
        /// <param name="cellId"></param>
        /// <returns></returns>
        private static bool validCell(int[] cells, int cellId)
        {
            if (!validRow(cells, getRowOfCell(cellId)))
                return false;

            if (!validCol(cells, getColOfCell(cellId)))
                return false;

            if (!validRegion(cells, getRegionOfCell(cellId)))
                return false;

            return true;
        }

        /// <summary>
        /// Validate that the row is valid.
        /// </summary>
        /// <param name="cells">Sudoku board</param>
        /// <param name="row"></param>
        /// <returns>True if valid. False otherwise.</returns>
        private static bool validRow(int[] cells, int row)
        {
            // reset the values array to check against
            resetValues();

            int value;
            int len = NUM_ROWS;
            for (int i = 0; i < len; ++i)
            {
                ++_actions;

                value = cells[_rowsOfCells[row, i]];
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

        /// <summary>
        /// Validate that the column is valid.
        /// </summary>
        /// <param name="cells">Sudoku board</param>
        /// <param name="col"></param>
        /// <returns>True if valid. False otherwise.</returns>
        private static bool validCol(int[] cells, int col)
        {
            // reset the values array to check against
            resetValues();

            int value;
            int len = NUM_COLS;
            for (int i = 0; i < len; ++i)
            {
                ++_actions;

                value = cells[_colsOfCells[col, i]];
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

        /// <summary>
        /// Validate that the region is valid.
        /// </summary>
        /// <param name="cells">Sudoku board</param>
        /// <param name="region"></param>
        /// <returns>True if valid. False otherwise.</returns>
        private static bool validRegion(int[] cells, int region)
        {
            // reset the values array to check against
            resetValues();

            int value;
            int len = NUM_REGIONS;
            for (int i = 0; i < len; ++i)
            {
                ++_actions;

                value = cells[_regionOfCells[region, i]];
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

        // reset possible values scratch vector
        private static void resetValues()
        {
            int len = _values.Length;
            for (int i = 0; i < len; ++i)
            {
                ++_actions;

                _values[i] = true;
            }
        }

        private static int getRowOfCell(int cell)
        {
            return cell / SudokuBoard.NUM_COLS;
        }

        private static int getColOfCell(int cell)
        {
            return cell % SudokuBoard.NUM_COLS;
        }

        private static int getRegionOfCell(int cell)
        {
            return Convert.ToInt32(getRowOfCell(cell) / THIRD_OF_GRID) * THIRD_OF_GRID + Convert.ToInt32(getColOfCell(cell) / THIRD_OF_GRID);
        }

        private static int getCellAt(int row, int col)
        {
            return row * SudokuBoard.NUM_COLS + col;
        }
    }
}
