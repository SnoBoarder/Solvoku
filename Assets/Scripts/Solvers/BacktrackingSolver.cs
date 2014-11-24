using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Solvers
{
    public class BacktrackingSolver : BaseSolver
    {

        public static void solve(int[] cells)
        {
            backtrack(cells);
        }

        public static bool backtrack(int[] cells)
        {
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
                            newCells[slotId] = i;
                            if (validSlot(newCells, slotId))
                            {
                                if (backtrack(newCells))
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

            return validBoard(cells);
        }
    }
}
