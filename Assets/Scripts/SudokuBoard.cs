using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Solvers;
using Assets.Scripts.Solvers.DancingLinks;

namespace Assets.Scripts
{
    public class SudokuBoard : MonoBehaviour
    {
        public const string ERROR_MIN_NOT_REACHED = "Board setup is too small. Try again.";
        public const string ERROR_INVALID_BOARD = "Board setup has errors! Try again.";
        public const string ERROR_NO_SELECTION = "Please select a slot before inputting.";

        public const int MINIMUM_CELL_COUNT = 20;

        public const int EMPTY_CELL = 0;

        public const int NUM_ROWS = 9;
        public const int NUM_COLS = 9;
        public const int NUM_GRIDS = 9;

        public const int THIRD_OF_GRID = 3;

        public const int NUM_HALF_ROW = 4; // 9 divided by 2 round down
        public const int NUM_HALF_COL = 4; // 9 divided by 2 round down
        
        public enum SolverTypes { BACKTRACKING, EXACT_COVER };

        public delegate void OnMessage(string error, float time = 4.0f);
        public event OnMessage onMessage;

        // prefabs
        public GameObject _sudokuSlotPrefab;

        // background color references
        public Color _slotBackgroundColorA;
        public Color _slotBackgroundColorB;

        // text color references
        public Color _textSetupColor;
        public Color _textAnswerColor;

        private List<SudokuSlot> _slots;

        // helper arrays
        private SudokuSlot[,] _rowsOfSlots;
        private SudokuSlot[,] _colsOfSlots;
        private SudokuSlot[,] _regionsOfSlots;

        private SudokuSlot _selectedSlot = null;

        private int[] _cells;

        // Use this for initialization
        void Start()
        {
            SudokuSlot.textSetupColor = _textSetupColor;
            SudokuSlot.textAnswerColor = _textAnswerColor;

            _cells = new int[SudokuBoard.NUM_ROWS * SudokuBoard.NUM_COLS];

            BacktrackingSolver.init();

            // instantiate the board
            createSlots();

            // use test board
            //testBoard();
        }

        /// <summary>
        /// For unit testing purposes
        /// </summary>
        private void testBoard()
        {
            // easy setup
            string sudokuStr = "0,0,0,1,0,5,0,0,0,1,4,0,0,0,0,6,7,0,0,8,0,0,0,2,4,0,0,0,6,3,0,7,0,0,1,0,9,0,0,0,0,0,0,0,3,0,1,0,0,9,0,5,2,0,0,0,7,2,0,0,0,8,0,0,2,6,0,0,0,0,3,5,0,0,0,4,0,9,0,0,0";

            // hard setup
            //string sudokuStr = "3,0,9,0,0,0,4,0,0,2,0,0,7,0,9,0,0,0,0,8,7,0,0,0,0,0,0,7,5,0,0,6,0,2,3,0,6,0,0,9,0,4,0,0,8,0,2,8,0,5,0,0,4,1,0,0,0,0,0,0,5,9,0,0,0,0,1,0,6,0,0,7,0,0,6,0,0,0,1,0,4";

            // evil setup
            //string sudokuStr = "0,0,0,0,9,6,0,3,0,0,0,0,1,4,0,0,0,5,0,1,4,7,0,0,0,0,0,0,0,1,0,0,0,0,8,0,0,3,0,8,0,7,0,9,0,0,5,0,0,0,0,6,0,0,0,0,0,0,0,1,9,5,0,7,0,0,0,2,4,0,0,0,0,9,0,3,5,0,0,0,0";

            string[] sudokuArr = sudokuStr.Split(',');
            int len = sudokuArr.Length;
            for (int i = 0; i < len; ++i)
            {
                _slots[i].setTextColor(_textSetupColor);
                _slots[i].slotValue = Convert.ToInt32(sudokuArr[i]);
            }
        }

        void OnEnable()
        {
            if (_slots == null)
                return;

            int len = _slots.Count;
            for (int i = 0; i < len; ++i)
            {
                _slots[i].slotClick += handleSlotClick;
            }
        }

        void OnDisable()
        {
            if (_slots == null)
                return;

            int len = _slots.Count;
            for (int i = 0; i < len; ++i)
            {
                _slots[i].slotClick -= handleSlotClick;
            }
        }

        /// <summary>
        /// Update the selected slot with the value inputted.
        /// </summary>
        /// <param name="newValue"></param>
        public void updateSelectedSlot(int newValue)
        {
            if (_selectedSlot == null)
            {
                if (onMessage != null)
                    onMessage(ERROR_NO_SELECTION);
                return;
            }

            _selectedSlot.setTextColor(_textSetupColor);
            _selectedSlot.slotValue = newValue;

            if (newValue == EMPTY_CELL)
                clearSelectedSlot();
        }

        /// <summary>
        /// Clear the board of all values
        /// </summary>
        public void clear()
        {
            clearSelectedSlot();

            int len = _slots.Count;
            for (int i = 0; i < len; ++i)
            {
                _slots[i].slotValue = EMPTY_CELL;
            }
        }

        public void clearAnswers()
        {
            clearSelectedSlot();

            SudokuSlot slot;
            int len = _slots.Count;
            for (int i = 0; i < len; ++i)
            {
                slot = _slots[i];

                if (slot.isAnswer)
                    slot.slotValue = EMPTY_CELL;
            }
        }

        /// <summary>
        /// Solve the board based on solver type.
        /// </summary>
        /// <param name="type">Type of solver to use.</param>
        public void solve(SolverTypes type)
        {
            clearSelectedSlot();

            int markedSlots = 0;

            int len = _slots.Count;
            string str = "";
            for (int i = 0; i < len; ++i)
            { // poulate the cells array wit the slots values
                _cells[i] = _slots[i].slotValue;

                if (_cells[i] != EMPTY_CELL)
                    markedSlots++;

                if (Main.DEBUG_ENABLED)
                {
                    str += _cells[i];
                    if (i + 1 < len)
                        str += ",";
                }
            }

            if (markedSlots < MINIMUM_CELL_COUNT)
            {
                if (onMessage != null)
                    onMessage(ERROR_MIN_NOT_REACHED);
                return;
            }

            if (!BacktrackingSolver.validBoard(_cells))
            {
                if (onMessage != null)
                    onMessage(ERROR_INVALID_BOARD);
                return;
            }

            if (Main.DEBUG_ENABLED)
                Debug.Log(str);

            long startTime = 0;
            long functionTime = 0;
            int totalActions = 0;
            int maxDepth = 0;

            switch (type)
            {
                case SolverTypes.BACKTRACKING:
                    startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    BacktrackingSolver.solve(_cells);
                    functionTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - startTime;

                    totalActions = BacktrackingSolver.getActionCount();
                    maxDepth = BacktrackingSolver.getMaxDepthLevel();
                    break;
                case SolverTypes.EXACT_COVER:
                    SudokuDancingLinks dl = new SudokuDancingLinks();
                    startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    dl.loadAndSearch(_cells);
                    functionTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - startTime;

                    totalActions = dl.getActionCount();
                    maxDepth = dl.getMaxDepthLevel();
                    break;
            }

            SudokuSlot slot;
            len = _slots.Count;
            for (int i = 0; i < len; ++i)
            {
                slot = _slots[i];
                if (slot.slotValue == EMPTY_CELL)
                {
                    slot.setTextColor(_textAnswerColor);
                    slot.slotValue = _cells[i];
                }
            }
            
            // compile result string
            string result = "Total Time: " + functionTime + "ms";
            result += "\nTotal Actions: " + string.Format("{0:n0}", totalActions);
            result += "\nMax Depth: " + maxDepth;

            string singleLineResult = "Total Time: " + functionTime + "ms. Total Actions: " + string.Format("{0:n0}", totalActions) + ". Max Depth: " + maxDepth;

            Debug.Log(singleLineResult);

#if UNITY_ANDROID
            new MobileNativeMessage(type + " SOLVER RESULTS:", result);
#else
            if (onMessage != null)
                onMessage(singleLineResult, 0);
#endif
        }

        private void clearSelectedSlot()
        {
            if (_selectedSlot != null)
            {
                _selectedSlot.unhighlight();
                _selectedSlot = null;
            }
        }

        /// <summary>
        /// Handle the currently selected slot.
        /// </summary>
        /// <param name="slotId"></param>
        private void handleSlotClick(int slotId)
        {
            if (_selectedSlot != null)
                _selectedSlot.unhighlight();

            _selectedSlot = _slots[slotId];
            _selectedSlot.highlight();
        }

        /// <summary>
        /// Dynamically create the visual slots for the sudoku board
        /// </summary>
        private void createSlots()
        {
            int row;
            int col;

            _slots = new List<SudokuSlot>();

            Vector3 pos = Vector3.zero;
            int slotId = 0;

            GameObject gObj;
            SudokuSlot slot;
            for (row = 0; row < NUM_ROWS; ++row)
            {
                for (col = 0; col < NUM_COLS; ++col)
                {
                    pos.x = col - NUM_HALF_COL;
                    pos.y = -row + NUM_HALF_ROW;

                    gObj = (GameObject)Instantiate(_sudokuSlotPrefab, Vector3.zero, Quaternion.identity);
                    gObj.transform.parent = this.gameObject.transform;
                    gObj.transform.localPosition = pos; // explicitly set the position of the slot

                    slot = gObj.GetComponent<SudokuSlot>();
                    slot.id = slotId;

                    _slots.Add(slot);

                    ++slotId;
                }
            }

            // force OnEnable to be called so that the events are handled properly at the beginning
            OnEnable();

            // setup helper arrays for checking purposes
            // look up table for every row
            _rowsOfSlots = new SudokuSlot[NUM_ROWS, NUM_COLS];
            for (row = 0; row < NUM_ROWS; ++row)
            {
                for (col = 0; col < NUM_COLS; ++col)
                {
                    _rowsOfSlots[row, col] = getSlotAt(row, col);
                }
            }

            // look up table for every col
            _colsOfSlots = new SudokuSlot[NUM_COLS, NUM_ROWS];
            for (col = 0; col < NUM_COLS; ++col)
            {
                for (row = 0; row < NUM_ROWS; ++row)
                {
                    _colsOfSlots[col, row] = getSlotAt(row, col);
                }
            }

            // look up table for every grid
            _regionsOfSlots = new SudokuSlot[9, 9];
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

                        _regionsOfSlots[grid, oneGridCurrentSize] = getSlotAt(currRow, currCol);
                        ++oneGridCurrentSize;
                    }
                }
            }

            // setup colors
            Color currColor;
            for (int i = 0; i < NUM_GRIDS; ++i)
            {
                currColor = i % 2 == 0 ? _slotBackgroundColorB : _slotBackgroundColorA;

                for (int j = 0; j < NUM_GRIDS; ++j)
                {
                    _regionsOfSlots[i, j].setBackgroundColor(currColor);
                }
            }
        }

        /// <summary>
        /// Get slot based on row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public SudokuSlot getSlotAt(int row, int col)
        {
            return _slots[row * NUM_COLS + col];
        }

        /// <summary>
        /// Get the row of a slot
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public int getRowOfSlot(int slot)
        {
            return slot / NUM_COLS;
        }

        /// <summary>
        /// Get the column of a slot
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public int getColOfSlot(int slot)
        {
            return slot % NUM_COLS;
        }
    }
}
