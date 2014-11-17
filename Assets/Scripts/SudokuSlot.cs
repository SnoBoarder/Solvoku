using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class SudokuSlot : MonoBehaviour
    {
        public delegate void SlotClickEvent(int id);
        public event SlotClickEvent slotClick;

        // editor reference
        public SpriteRenderer _background;
        public SpriteRenderer _highlight;
        public TextMesh _text;

        protected int _id;

        protected int _value = 0;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseDown()
        {
            if (slotClick != null)
                slotClick(_id);
        }

        public void highlight()
        {
            _highlight.gameObject.SetActive(true);
        }

        public void unhighlight()
        {
            _highlight.gameObject.SetActive(false);
        }

        public void setBackgroundColor(Color c)
        {
            _background.color = c;
        }

        public int slotValue
        {
            get { return _value; }

            set
            {
                _value = value;
                _text.text = _value.ToString();
            }
        }

        public int id
        {
            set { _id = value; }
        }
    }
}
