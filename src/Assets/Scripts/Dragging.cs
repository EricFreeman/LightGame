using UnityEngine;

namespace Assets.Scripts
{
    public class Dragging : MonoBehaviour
    {
        private GameObject _dragObject;

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (_dragObject == null)
                {
                    var obj = GetDraggableObject();

                    if (obj != null)
                    {
                        _dragObject = obj.gameObject;

                        var joint = gameObject.AddComponent<DistanceJoint2D>();
                        var distance = Vector2.Distance(_dragObject.transform.position, transform.position);
                        joint.connectedBody = _dragObject.rigidbody2D;
                        joint.distance = distance;
                    }
                }
            }
            else if (_dragObject != null)
            {
                _dragObject = null;
                Destroy(gameObject.GetComponent<DistanceJoint2D>());
            }
        }

        private Collider2D GetDraggableObject()
        {
            var left = Physics2D.Raycast(transform.position - new Vector3(.16f, 0, 0), -Vector2.right, .01f);
            var right = Physics2D.Raycast(transform.position + new Vector3(.16f, 0, 0), Vector2.right, .01f);

            var obj = (left.collider != null && left.collider.tag == "Draggable")
                ? left.collider
                : (right.collider != null && right.collider.tag == "Draggable") ? right.collider : null;

            return obj;
        }
    }
}