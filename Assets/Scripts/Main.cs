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
            _solveButton.onClick += solveBoard;

            if (_inputSlots == null)
                return;

            int len = _inputSlots.Count;
            for (int i = 0; i < len; ++i)
            {
                _inputSlots[i].slotClick += handleInputClick;
            }
        }

        void OnDisable()
        {
            _solveButton.onClick -= solveBoard;

            if (_inputSlots == null)
                return;

            int len = _inputSlots.Count;
            for (int i = 0; i < len; ++i)
            {
                _inputSlots[i].slotClick -= handleInputClick;
            }
        }

        private void solveBoard()
        {
            _board.solve(SudokuBoard.SolverTypes.BRUTE_FORCE);
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
	
	    }
    }
}
