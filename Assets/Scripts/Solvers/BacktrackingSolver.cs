using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Assets.Scripts.Solvers
{
    public class BacktrackingSolver : BaseSolver
    {

        private static int _depthLevel = 0;

        private static int _actions;

        private static int[] _cells;

        public static void solve(int[] cells)
        {
            _actions = 0;

            setOnePossibility(cells);

            backtrack(cells, 0);

            //Debug.Log("actions: " + _actions);
        }

        public static void setOnePossibility(int[] cells)
        {
            int count;
            int value;
            int slotId;
            for (int row = 0; row < NUM_ROWS; ++row)
            {
                for (int col = 0; col < NUM_COLS; ++col)
                {
                    slotId = getSlotAt(row, col);
                    int cell = cells[slotId];
                    if (cell == SudokuBoard.EMPTY_CELL)
                    {
                        count = 0;
                        value = 0;
                        // traverse all values and update count if value
                        for (int i = 1; i <= 9; ++i)
                        {
                            ++_actions;

                            cells[slotId] = i;
                            if (validSlot(cells, slotId))
                            {
                                count++;
                                value = i;
                            }
                        }

                        if (count == 1) // if count == 1, keep the value since it's our only option!
                        {
                            cells[slotId] = value;
                        }
                        else // there was either no solution or more than 1 solution. RESET
                        {
                            cells[slotId] = SudokuBoard.EMPTY_CELL;

                        }
                    }
                }
            }
        }

        public static bool backtrack(int[] cells, int depthLevel)
        {
            if (depthLevel > _depthLevel)
                _depthLevel = depthLevel;

            int slotId;
            for (int row = 0; row < SudokuBoard.NUM_ROWS; ++row)
            {
                for (int col = 0; col < SudokuBoard.NUM_COLS; ++col)
                {
                    slotId = getSlotAt(row, col);
                    int cell = cells[slotId];
                    if (cell == SudokuBoard.EMPTY_CELL)
                    { // cell is empty! traverse all possible values and attempt to assign it to the cell
                        int[] newCells = new int[cells.Length];
                        Array.Copy(cells, newCells, cells.Length);
                        for (int i = 1; i <= 9; ++i)
                        {
                            ++_actions;

                            newCells[slotId] = i;
                            if (validSlot(newCells, slotId))
                            {
                                if (backtrack(newCells, depthLevel + 1))
                                {
                                    Array.Copy(newCells, cells, newCells.Length);
                                    return true;
                                }
                            }

                            // invalid value! try the next value
                        }

                        // impossible to place any value in this scenario, kick out
                        return false;
                    }
                }
            }

            return true;//validBoard(cells);
        }
    }
}
