using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class SolverSelectionButton : Button
    {
        public SudokuBoard.SolverTypes solverType;

        public SpriteRenderer _background;
        public Color _unselectedColor;
        public Color _selectedColor;

        private bool _selected = false;

        void Start()
        {
            _background.color = _selected ? _selectedColor : _unselectedColor;
        }

        public bool selected
        {
            get { return _selected; }
            set {
                _selected = value;
                _background.color = _selected ? _selectedColor : _unselectedColor;
            }
        }
    }
}
