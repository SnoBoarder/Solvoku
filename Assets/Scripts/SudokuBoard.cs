using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts.Solvers;
using Assets.Scripts.Solvers.DancingLinks;

namespace Assets.Scripts
{
    public class SudokuBoard : MonoBehaviour
    {
        public enum SolverTypes { BRUTE_FORCE, BACKTRACKING, SCHOTASTIC, EXACT_COVER };

        public const int EMPTY_CELL = 0;

        public const int NUM_ROWS = 9;
        public const int NUM_COLS = 9;
        public const int NUM_GRIDS = 9;

        public const int THIRD_OF_GRID = 3;

        public const int NUM_HALF_ROW = 4; // 9 divided by 2 round down
        public const int NUM_HALF_COL = 4; // 9 divided by 2 round down

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
        private SudokuSlot[,] _gridsOfSlots;

        private SudokuSlot _selectedSlot = null;

        private int[] _cells;

        // Use this for initialization
        void Start()
        {
            _cells = new int[SudokuBoard.NUM_ROWS * SudokuBoard.NUM_COLS];

            BaseSolver.init();

            // instantiate the board
            createSlots();

            // use test board
            testBoard();
        }

        private void testBoard()
        {
            //string sudokuStr = "6,7,2,1,4,5,3,9,8,1,4,5,9,8,3,6,7,2,3,8,9,7,6,2,4,5,1,2,6,3,5,7,4,8,1,9,9,5,8,6,2,1,7,4,3,7,1,4,3,9,8,5,2,6,5,9,7,2,3,6,1,8,4,4,2,6,8,1,7,9,3,5,8,3,1,4,5,9,2,6,7";


            string sudokuStr = "0,0,0,1,0,5,0,0,0,1,4,0,0,0,0,6,7,0,0,8,0,0,0,2,4,0,0,0,6,3,0,7,0,0,1,0,9,0,0,0,0,0,0,0,3,0,1,0,0,9,0,5,2,0,0,0,7,2,0,0,0,8,0,0,2,6,0,0,0,0,3,5,0,0,0,4,0,9,0,0,0";
            //6,7,2,1,4,5,3,9,8,
            //1,4,5,9,8,3,6,7,2,
            //3,8,9,7,6,2,4,5,1,
            //2,6,3,5,7,4,8,1,9,
            //9,5,8,6,2,1,7,4,3,
            //7,1,4,3,9,8,5,2,6,
            //5,9,7,2,3,6,1,8,4,
            //4,2,6,8,1,7,9,3,5,
            //8,3,1,4,5,9,2,6,7

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

        public void updateSelectedSlot(int newValue)
        {
            _selectedSlot.setTextColor(_textSetupColor);
            _selectedSlot.slotValue = newValue;
        }

        public void clear()
        {
            int len = _slots.Count;
            for (int i = 0; i < len; ++i)
            {
                _slots[i].slotValue = EMPTY_CELL;
            }
        }

        public void solve(SolverTypes type)
        {
            int len = _slots.Count;
            string str = "";
            for (int i = 0; i < len; ++i)
            { // poulate the cells array wit the slots values
                _cells[i] = _slots[i].slotValue;

                if (Main.DEBUG_ENABLED)
                {
                    str += _cells[i];
                    if (i + 1 < len)
                        str += ",";
                }
            }

            if (Main.DEBUG_ENABLED)
                Debug.Log(str);

            // TODO: DECIDE HERE WHICH SOLVER TO USE
            switch (type)
            {
                case SolverTypes.BRUTE_FORCE:
                    break;
                case SolverTypes.BACKTRACKING:
                    //BacktrackingSolver.solve(_cells);
                    BacktrackingSolver2.solve(_cells);
                    break;
                case SolverTypes.EXACT_COVER:
                    SudokuDancingLinks dl = new SudokuDancingLinks();
                    dl.loadAndSearch(_cells);
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
        }

        private void handleSlotClick(int slotId)
        {
            if (_selectedSlot != null)
                _selectedSlot.unhighlight();

            _selectedSlot = _slots[slotId];
            _selectedSlot.highlight();
        }

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
            _gridsOfSlots = new SudokuSlot[9, 9];
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

                        _gridsOfSlots[grid, oneGridCurrentSize] = getSlotAt(currRow, currCol);
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
                    _gridsOfSlots[i, j].setBackgroundColor(currColor);
                }
            }
        }

        public SudokuSlot getSlotAt(int row, int col)
        {
            return _slots[row * NUM_COLS + col];
        }

        public int getRowOfSlot(int slot)
        {
            return slot / NUM_COLS;
        }

        public int getColOfSlot(int slot)
        {
            return slot % NUM_COLS;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
