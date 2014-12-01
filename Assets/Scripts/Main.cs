using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class Main : MonoBehaviour
    {
        public SudokuBoard _board;
        public GameObject _inputContainer;
        public Button _solveButton;

        // prefabs
        public GameObject _sudokuSlotPrefab;

        // input slots
        private List<SudokuSlot> _inputSlots;

	    // Use this for initialization
	    void Start ()
        {
            createInputSlots();
	    }

        void OnEnable()
        {
            if (_inputSlots == null)
                return;

            _solveButton.onClick += solveBoard;

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

            int len = _inputSlots.Count;
            for (int i = 0; i < len; ++i)
            {
                _inputSlots[i].slotClick -= handleInputClick;
            }
        }

        private void solveBoard()
        {
            _board.solve(SudokuBoard.SolverTypes.EXACT_COVER);
        }

        private void createInputSlots()
        {
            _inputSlots = new List<SudokuSlot>();

            Vector3 pos = Vector3.zero;

            GameObject gObj;
            SudokuSlot slot;
            for (int i = 0; i < SudokuBoard.NUM_COLS; ++i)
            {
                pos.x = i - SudokuBoard.NUM_HALF_COL;
                pos.y = -4;

                gObj = (GameObject)Instantiate(_sudokuSlotPrefab, Vector3.zero, Quaternion.identity);
                gObj.transform.parent = _inputContainer.transform;
                gObj.transform.localPosition = pos; // explicitly set the position of the slot

                slot = gObj.GetComponent<SudokuSlot>();
                slot.setBackgroundColor(i % 2 == 0 ? _board._slotBackgroundColorB : _board._slotBackgroundColorA);
                slot.slotValue = i + 1;
                slot.id = i + 1;

                _inputSlots.Add(slot);
            }

            // force OnEnable to be called so that the events are handled properly at the beginning
            OnEnable();
        }

        private void handleInputClick(int inputId)
        {
            _board.updateSelectedSlot(inputId);
        }
	
	    // Update is called once per frame
	    void Update () {
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
	    }
    }
}
