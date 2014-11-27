using UnityEngine;

namespace Assets.Scripts
{
    public class GrapplingHook : MonoBehaviour
    {
        public float Length = 3;

        private bool _isGrappled;
        private LineRenderer _line;
        private GameObject _grapple;

        void Start()
        {
            _line = new GameObject("Line").AddComponent<LineRenderer>();
            _line.SetVertexCount(2);
            _line.SetWidth(.025f, .025f);
            _line.gameObject.SetActive(false);

            _grapple = new GameObject("Grapple");
            _grapple.AddComponent<Rigidbody2D>();
            _grapple.rigidbody2D.isKinematic = true;
        }

        void Update()
        {
            if (_isGrappled) UpdateGrapple();
            else CheckForGrapple();
        }

        private void CheckForGrapple()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = Input.mousePosition;
                mousePosition.z = -Camera.main.transform.position.z;
                var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                var grapplePoint = transform.position + (worldPosition - transform.position) * Length;

                var hit = Physics2D.Linecast(transform.position, grapplePoint, ~(1 << 8));
                var distance = Vector3.Distance(transform.position, hit.point);
                if (hit.collider != null && distance <= Length)
                {
                    _line.SetPosition(0, transform.position);
                    _line.SetPosition(1, hit.point);
                    _line.gameObject.SetActive(true);

                    var joint = gameObject.AddComponent<DistanceJoint2D>();
                    joint.connectedBody = _grapple.rigidbody2D;
                    joint.distance = distance;

                    _grapple.transform.position = hit.point;

                    _isGrappled = true;
                }
            }
        }

        private void UpdateGrapple()
        {
            var hit = Physics2D.Linecast(transform.position, _grapple.transform.position, ~(1 << 8));
            _line.SetPosition(0, transform.position);
            _line.SetPosition(1, hit.point);

            if (Input.GetMouseButtonDown(0))
            {
                Destroy(gameObject.GetComponent<DistanceJoint2D>());
                _line.gameObject.SetActive(false);

                _isGrappled = false;
            }
        }
    }
}