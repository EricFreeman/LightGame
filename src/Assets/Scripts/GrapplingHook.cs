using UnityEngine;

namespace Assets.Scripts
{
    public class GrapplingHook : MonoBehaviour
    {
        public float Length = 3;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = Input.mousePosition;
                mousePosition.z = -Camera.main.transform.position.z;
                var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                var hit = Physics2D.Linecast(transform.position, worldPosition, ~(1 << 8));
                if (hit.collider != null)
                {
                    var line = new GameObject().AddComponent<LineRenderer>();
                    line.SetVertexCount(2);
                    line.SetPosition(0, transform.position);
                    line.SetPosition(1, hit.point);
                    line.SetWidth(0.025F, 0.025F);

                    Debug.Log(transform.position);
                    Debug.Log(hit.point);

                    Debug.Log("Should be working!");
                }
            }
        }
    }
}