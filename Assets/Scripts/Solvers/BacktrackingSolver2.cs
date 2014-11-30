using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Solvers
{
    public class BacktrackingSolver2 : BaseSolver
    {

        public static void solve(int[] cells)
        {
            backtrack(cells, 0, 0);
        }

        public static bool backtrack(int[] cells, int row, int col)
        {
            if (col >= 9)
                return backtrack(cells, row + 1, 0);

            if (row == 9)
                return true;

            int slotId = getSlotAt(row, col);

            if (cells[slotId] == SudokuBoard.EMPTY_CELL)
            {
                for (int i = 1; i <= 9; ++i)
                {
                    cells[slotId] = i;
                    if (validSlot(cells, slotId))
                    {
                        if (backtrack(cells, row, col+1))
                            return true;
                    }
                }

                cells[slotId] = SudokuBoard.EMPTY_CELL;
            }
            else
            {
                return backtrack(cells, row, col + 1);
            }

            return false;
        }
    }
}
