using UnityEngine;

namespace Utils
{
    public static class Utility
    {
        private static Transform worldTextParent;
        static Utility()
        {
            worldTextParent = new GameObject("TextParent").transform;
        }

        public static TextMesh CreateWorldText(
            string text,
            int fontSize = 25,
            TextAnchor anchor = TextAnchor.MiddleCenter,
            TextAlignment alignment = TextAlignment.Center,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3 localScale = default,
            Color color = default
            )
        {
            GameObject gameObject = new GameObject("WordText", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = localScale;
            transform.SetParent(worldTextParent);
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.anchor = anchor;
            textMesh.color = color;
            textMesh.alignment = alignment;
            return textMesh;
        }

        public static Vector3 GetMouseWorldPosition3D(LayerMask groundLayerMask)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, groundLayerMask)) 
            {
                return hitInfo.point;
            }

            return Vector3.zero;
        }
    }
}