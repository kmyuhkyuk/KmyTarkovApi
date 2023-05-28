using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EFTConfiguration.UI
{
    public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        [SerializeField] private RectTransform targetRoot;

        private Vector2 _startPosition;

        public void OnDrag(PointerEventData eventData)
        {
            var vector2 = eventData.position - _startPosition;
            targetRoot.position = new Vector2((float)Math.Round(vector2.x), (float)Math.Round(vector2.y));
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = eventData.position - (Vector2)targetRoot.position;
        }
    }
}