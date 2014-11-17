using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Button : MonoBehaviour
    {
        public delegate void OnClick();
        public event OnClick onClick;

        // Use this for initialization
        void Start()
        {

        }

        void OnMouseDown()
        {
            if (onClick != null)
                onClick();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
