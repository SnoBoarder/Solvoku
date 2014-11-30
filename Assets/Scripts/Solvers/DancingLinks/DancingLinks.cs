using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Solvers.DancingLinks
{
    public class DancingLinks
    {
        public DancingLinkHeader _head { get; private set; }
        public List<DancingLinkHeader> _cols { get; private set; }

        Stack<DancingLinkNode> _solutions = new Stack<DancingLinkNode>();

        public DancingLinks(int columnCount = 10)
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

            // finish wrapping the column headers
            DancingLinkHeader firstHeader = _cols[0];
            _head.right = firstHeader;
            firstHeader.left = _head;

            DancingLinkHeader lastHeader = _cols[_cols.Count - 1];
            _head.left = lastHeader;
            lastHeader.right = _head;
        }

        /// <summary>
        /// See: http://sudopedia.enjoysudoku.com/Dancing_Links.html
        /// </summary>
        private void setupColumns()
        {
            // setup columns for initial setting

            // setup headers


        }

        /// <summary>
        /// See: http://sudopedia.enjoysudoku.com/Dancing_Links.html
        /// </summary>
        private void addRow(int index, int row, int col, int block)
        {

        }

        public void buildFullMatrix()
        {
            setupColumns();

            for (int i = 0; i < 81; ++i)
            {
                for (int value = 1; value <= 9; ++value)
                {
                    int row = i / 9;
                    int col = i % 9;
                    int block = ((i / 27) * 3) + ((i % 9) / 3);
                    addRow(i, row, col, block);
                }
            }
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


            }
        }

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
