using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        public static bool DEBUG_ENABLED = false;

        public SudokuBoard _board;
        public GameObject _inputContainer;
        public Button _solveButton;
        public Button _clearAllButton;
        public Button _clearAnswersButton;
        public SolverSelectionButton _backtrackingButton;
        public SolverSelectionButton _exactCoverButton;
        public TextMesh _errorHandling;

        // prefabs
        public GameObject _sudokuSlotPrefab;
        public GameObject _sudokuSlotDeletPrefab;

        // input slots
        private List<SudokuSlot> _inputSlots;

        private SolverSelectionButton _selectedSolver;

	    // Use this for initialization
	    void Start ()
        {
            createInputSlots();

            // set solver types
            _backtrackingButton.solverType = SudokuBoard.SolverTypes.BACKTRACKING;
            _exactCoverButton.solverType = SudokuBoard.SolverTypes.EXACT_COVER;

            // defaulting exact cover
            _backtrackingButton.selected = false;
            _exactCoverButton.selected = true;
            _selectedSolver = _exactCoverButton;
	    }

        void OnEnable()
        {
            if (_inputSlots == null)
                return;

            _solveButton.onClick += solveBoard;
            _clearAllButton.onClick += clearBoard;
            _clearAnswersButton.onClick += clearAnswers;
            _board.onMessage += handleMessage;

            _backtrackingButton.onClick += selectBacktrack;
            _exactCoverButton.onClick += selectExactCover;

            int len = _inputSlots.Count;
            for (int i = 0; i < len; ++i)
            {
                _inputSlots[i].slotClick += handleInputClick;
            }
        }

        void OnDisable()
        {
            if (_inputSlots == null)
                return;

            _solveButton.onClick -= solveBoard;
            _clearAllButton.onClick -= clearBoard;
            _clearAnswersButton.onClick -= clearAnswers;
            _board.onMessage -= handleMessage;

            int len = _inputSlots.Count;
            for (int i = 0; i < len; ++i)
            {
                _inputSlots[i].slotClick -= handleInputClick;
            }
        }

        /// <summary>
        /// Set solver to the backtracking algorithm
        /// </summary>
        private void selectBacktrack()
        {
            if (_selectedSolver == _backtrackingButton)
                return; // already been set

            _backtrackingButton.selected = true;
            _exactCoverButton.selected = false;
            _selectedSolver = _backtrackingButton;
        }

        /// <summary>
        /// Set solver to the exact cover algorithm
        /// </summary>
        private void selectExactCover()
        {
            if (_selectedSolver == _exactCoverButton)
                return; // already been set

            _backtrackingButton.selected = false;
            _exactCoverButton.selected = true;
            _selectedSolver = _exactCoverButton;
        }

        private void solveBoard()
        {
            _board.solve(_selectedSolver.solverType);
        }

        private void clearBoard()
        {
            _board.clear();
        }

        private void clearAnswers()
        {
            _board.clearAnswers();
        }

        private void handleMessage(string error, float time)
        {
            CancelInvoke("clearMessage");

            _errorHandling.text = error;

            if (time > 0)
                Invoke("clearMessage", time);
        }

        private void clearMessage()
        {
            _errorHandling.text = "";
        }

        private void createInputSlots()
        {
            _inputSlots = new List<SudokuSlot>();

            Vector3 pos = Vector3.zero;
            pos.y = -3.75f;
            int i;
            for (i = 0; i < 4; ++i)
            {
                pos.x = i - SudokuBoard.NUM_HALF_COL / 2 + 2;

                addSlot(_sudokuSlotPrefab, i, pos, i % 2 == 0 ? _board._slotBackgroundColorB : _board._slotBackgroundColorA);
            }

            // add delete key
            pos.x = i - SudokuBoard.NUM_HALF_COL / 2 + 2;

            addSlot(_sudokuSlotDeletPrefab, -1, pos, Color.white);

            pos.y = -4.75f;
            for (; i < 9; ++i)
            {
                pos.x = i - 4 - SudokuBoard.NUM_HALF_COL / 2 + 2;

                addSlot(_sudokuSlotPrefab, i, pos, i % 2 == 0 ? _board._slotBackgroundColorA : _board._slotBackgroundColorB);
            }

            // force OnEnable to be called so that the events are handled properly at the beginning
            OnEnable();
        }

        private void addSlot(GameObject prefab, int index, Vector3 pos, Color c)
        {
            GameObject gObj = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
            gObj.transform.parent = _inputContainer.transform;
            gObj.transform.localPosition = pos; // explicitly set the position of the slot

            SudokuSlot slot = gObj.GetComponent<SudokuSlot>();
            slot.setBackgroundColor(c);
            slot.slotValue = index + 1;
            slot.id = index + 1;

            _inputSlots.Add(slot);
        }

        private void handleInputClick(int inputId)
        {
            _board.updateSelectedSlot(inputId);
        }

	    // Update is called once per frame
	    void Update () {
            if (Input.GetKeyDown(KeyCode.Escape)) // see if the android BACK button was pressed.
                Application.Quit();

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                handleInputClick(1);

            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                handleInputClick(2);

            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                handleInputClick(3);

            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                handleInputClick(4);

            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                handleInputClick(5);

            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                handleInputClick(6);

            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                handleInputClick(7);

            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                handleInputClick(8);

            if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                handleInputClick(9);

            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
                handleInputClick(0);
	    }
    }
}
