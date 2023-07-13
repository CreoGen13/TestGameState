using UnityEngine;
using UnityEngine.UI;

namespace Mono
{
    public class Line : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private RectTransform rectTransform;

        public void SetTransform(Vector2 size, Vector3 position, float zRotation)
        {
            rectTransform.sizeDelta = size;
            rectTransform.localPosition = position;
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, zRotation));
        }
    }
}