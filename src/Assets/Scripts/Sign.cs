using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Sign : MonoBehaviour
    {
        public Text SignTextObject;
        public string SignText;

        private bool _showText;
        private float _fadeSpeed = .05f;

        private void Start()
        {
            SignTextObject.text = SignText;
        }

        private void Update()
        {
            SignTextObject.color = new Color(
                SignTextObject.color.r, 
                SignTextObject.color.g, 
                SignTextObject.color.b,
                Mathf.Clamp(SignTextObject.color.a + (_showText ? _fadeSpeed : -_fadeSpeed), 0, 1));
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            _showText = true;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            _showText = false;
        }
    }
}