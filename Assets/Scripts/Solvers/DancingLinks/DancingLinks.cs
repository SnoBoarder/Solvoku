using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Solvers.DancingLinks
{
    public class DancingLinks
    {
                                           // position + row                      + column                         + box
        private const int TOTAL_COLS = 324; // 81 cells + (9 rows * 9 digits rows) + (9 columns * 9 digits columns) + (9 columns * 9 digits box)
        private const int TOTAL_ROWS = 729; // 81 * 9

        private const int START_OF_ROW_COLUMNS = 81 * 1;
        private const int START_OF_COL_COLUMNS = 81 * 2;
        private const int START_OF_BOX_COLUMNS = 81 * 3;

        public DancingLinkHeader _head { get; private set; }
        public List<DancingLinkHeader> _cols { get; private set; }
        public List<DancingLinkNode> _rows { get; private set; }

        Stack<DancingLinkNode> _solutions = new Stack<DancingLinkNode>();




        public DancingLinks(int columnCount = TOTAL_COLS) 
        {
            // initialize links
            _head = new DancingLinkHeader();
            _head.left = _head;
            _head.right = _head;
            
            // create the headers
            _cols = new List<DancingLinkHeader>(columnCount);
            DancingLinkHeader curr = _head;
            DancingLinkHeader next = null;
            for (int i = 0; i < columnCount; ++i)
            {
                // create header and insert them into the linked list from left to right
                next = new DancingLinkHeader();
                _cols.Add(next);
                curr.right = next;
                next.left = curr;
                curr = next;
            }
            // finish wrapping the last node around
            _head.left = next;
            next.right = _head;

            // create matrix of all possible sudoku positions
            _rows = new List<DancingLinkNode>(TOTAL_ROWS);
            for (int i = 0; i < 81; ++i)
            {
                for (int value = 1; value <= 9; ++value)
                {
                    int row = i / 9;
                    int col = i % 9;
                    int box = ((i / 27) * 3) + ((i % 9) / 3);

                    int rowIndex = START_OF_ROW_COLUMNS + (row * 9 + value - 1); // "value - 1" to make it base 0
                    int colIndex = START_OF_COL_COLUMNS + (col * 9 + value - 1);
                    int boxIndex = START_OF_BOX_COLUMNS + (box * 9 + value - 1);
                    addRow(i, rowIndex, colIndex, boxIndex); // adds a row to the matrix and to the _rows array
                }
            }
        }

        /// <summary>
        /// See: http://sudopedia.enjoysudoku.com/Dancing_Links.html
        /// </summary>
        private void addRow(int index, int rowIndex, int colIndex, int boxIndex)
        {
            DancingLinkHeader header;
            DancingLinkNode start;
            DancingLinkNode prev;

            // get column for the appropriate indices

            // set first node of the row to define the matrix's position
            header = _cols[index];
            start = createNode(null, header);
            _rows.Add(start); // only need to store the reference to the first item

            // set rowindex of the row
            header = _cols[rowIndex];
            prev = createNode(start, header);

            // set colIndex of the row
            header = _cols[colIndex];
            prev = createNode(prev, header);

            // set boxIndex of the row
            header = _cols[boxIndex];
            prev = createNode(prev, header);

            // loop the row
            start.left = prev;
            prev.right = start;
        }

        private DancingLinkNode createNode(DancingLinkNode last, DancingLinkHeader head)
        {
            DancingLinkNode newNode = new DancingLinkNode();

            // set left/right connections
            if (last != null)
            {
                newNode.left = last;
                newNode.right = last.right;
                newNode.left.right = newNode;
                newNode.right.left = newNode;
            }
            else
            { // first in the row, refer to itself
                newNode.left = newNode;
                newNode.right = newNode;
            }

            // set up/down connections
            newNode.header = head;
            ++head.count;
            newNode.down = head;
            newNode.up = head.up;
            newNode.up.down = newNode;
            newNode.down.up = newNode;

            return newNode;
        }

        public void loadData(int[] cells)
        {
            int value;
            int len = cells.Length;
            for (int i = 0; i < 81; ++i)
            {
                value = cells[i];
                if (value == SudokuBoard.EMPTY_CELL)
                    continue;

                int row = i / 9;
                int col = i % 9;
                int box = ((i / 27) * 3) + ((i % 9) / 3);

                int rowIndex = START_OF_ROW_COLUMNS + (row * 9 + value - 1); // "value - 1" to make it base 0
                int colIndex = START_OF_COL_COLUMNS + (col * 9 + value - 1);
                int boxIndex = START_OF_BOX_COLUMNS + (box * 9 + value - 1);

                // remove the 4 columns as an option
                cover(_cols[i]);
                cover(_cols[rowIndex]);
                cover(_cols[colIndex]);
                cover(_cols[boxIndex]);

                // TODO: do I actually need to store this solution? we already know it...
                //_solution.Add(w/e);
            }
        }

        /* NOT NEEDED (cover and uncover handle this)
        private void disable(int rowIndex)
        {
            DancingLinkNode start = _rows[rowIndex];
            DancingLinkNode curr = start;

            do
            {
                if (curr.down != curr) // prevent disabling the same node twice
                {
                    curr.down.up = curr.up;
                    curr.up.down = curr.down;
                    curr.up = curr;
                    --curr.header.count;
                }
                curr = curr.right;
            }
            while (curr != start);
        }

        private void enable(int rowIndex)
        {
            DancingLinkNode start = _rows[rowIndex];
            DancingLinkNode curr = start;

            do
            {
                if (curr.down != curr) // prevent disabling the same node twice
                {
                    curr.down = curr.header;
                    curr.up = curr.header.up;
                    curr.down.up = curr;
                    curr.up.down = curr;
                    ++curr.header.count;
                }
                curr = curr.right;
            }
            while (curr != start);
        }
        */
        public void search(int depth)
        {
            if (_head.right == _head)
            { // the wrap around equals itself... we've found the solution
                Console.WriteLine("Found the solution?? WOWOWOW");
                return;
            }

            DancingLinkHeader column = chooseNextColumn();
            cover(column);

            for (DancingLinkNode rowNode = column.down; rowNode != column; rowNode = rowNode.down)
            { // traverse going south as long as we're not the column
                _solutions.Push(rowNode);

                for (DancingLinkNode rightNode = rowNode.right; rightNode != rowNode; rightNode = rightNode.right)
                { // traverse and cover every node going right as long as we're not the current node
                    cover(rightNode.header);
                }

                search(depth + 1);

                // backtrack this step
                _solutions.Pop();
                //column = row.column; // is this necessary?

                for (DancingLinkNode leftNode = rowNode.left; leftNode != rowNode; leftNode = leftNode.left)
                { // traverse and uncover every node going left as long as we're not the current node
                    uncover(leftNode.header);
                }
            }

            uncover(column);
        }

        /// <summary>
        /// </summary>
        /// <param name="column"></param>
        public void cover(DancingLinkHeader column)
        {
            // remove the current column from the other columns
            column.right.left = column.left;
            column.left.right = column.right;

            for (DancingLinkNode rowNode = column.down; rowNode != column; rowNode = rowNode.down)
            { // traverse down the column
                for (DancingLinkNode rightNode = rowNode.right; rightNode != rowNode; rightNode = rightNode.right)
                {
                    // remove the row by traversing the entire row from left to right and remove references accordingly
                    rightNode.up.down = rightNode.down;
                    rightNode.down.up = rightNode.up;
                    
                    // decrement header count
                    --rightNode.header.count;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="column"></param>
        public void uncover(DancingLinkHeader column)
        {
            for (DancingLinkNode rowNode = column.up; rowNode != column; rowNode = rowNode.up)
            {
                for (DancingLinkNode leftNode = rowNode.left; leftNode != rowNode; leftNode = leftNode.left)
                {
                    // attach the reference back to this node by using the data the node already had to reattach
                    leftNode.up.down = leftNode;
                    leftNode.down.up = leftNode;

                    // increment header count
                    ++leftNode.header.count;
                }
            }

            // add the column back into the list
            column.right.left = column;
            column.left.right = column;
        }

        /// <summary>
        /// Choose the column with the least amount of nodes. This decreases the branching algorithm.
        /// </summary>
        /// <returns></returns>
        public DancingLinkHeader chooseNextColumn()
        {
            DancingLinkNode targetCol = null;
            int lowestSize = 9000;

            for (DancingLinkNode node = _head.right; node != _head; node = node.right)
            {
                int count = ((DancingLinkHeader)node).count;
                if (count < lowestSize)
                {
                    lowestSize = count;
                    targetCol = node;
                }
            }

            return (DancingLinkHeader)targetCol;
        }
    }

    public class DancingLinkNode
    {
        // reference to the header node
        public DancingLinkHeader header;

        public DancingLinkNode up;
        public DancingLinkNode down;
        public DancingLinkNode right;
        public DancingLinkNode left;
    }

    public class DancingLinkHeader : DancingLinkNode
    {
        public int count;
    }
}
