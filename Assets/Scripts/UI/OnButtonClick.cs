using UnityEngine;


namespace PING_PONG
{
    public class OnButtonClick : MonoBehaviour
    {

        [SerializeField] private int buttonID;
        [SerializeField] private Color buttonColor;


        public void OnClick()
        {
            EventManager.RaiseColorButtonClick(buttonID);
        }

        public Color GetColor()
        {
            return buttonColor;
        }

    }
}