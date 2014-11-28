using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GrapplingHook : MonoBehaviour
    {
        public float Length = 3;

        private float _grappleLength;
        private readonly List<Vector2> _points = new List<Vector2>();
        private LineRenderer _line;
        private GameObject _grapple;

        // TODO: Switch to list of points
        // TODO: Put new point when lose sight of first at second to last emelent in point list
        // TODO: If you can see last two, then remove second to last points
        // TODO: THE END

        void Start()
        {
            _line = new GameObject("Line").AddComponent<LineRenderer>();
            _line.SetVertexCount(2);
            _line.SetWidth(.025f, .025f);
            _line.gameObject.SetActive(false);

            _grapple = new GameObject("Grapple");
            _grapple.AddComponent<BoxCollider2D>().size = new Vector2(.1f, .1f);
            _grapple.AddComponent<Rigidbody2D>();
            _grapple.rigidbody2D.isKinematic = true;
        }

        void Update()
        {
            if (_points.Count > 0) UpdateGrapple();
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
                    _line.SetVertexCount(2);
                    _line.SetPosition(0, hit.point);
                    _line.SetPosition(1, transform.position);
                    _line.gameObject.SetActive(true);

                    _points.Add(hit.point);
                    _points.Add(transform.position);

                    _grappleLength = Vector3.Distance(transform.position, hit.point);

//                    var joint = gameObject.AddComponent<DistanceJoint2D>();
//                    joint.connectedBody = _grapple.rigidbody2D;
//                    joint.distance = distance;
//                    joint.maxDistanceOnly = true;

                    _grapple.transform.position = hit.point;
                }
            }
        }

        private void UpdateGrapple()
        {
            if(transform.rigidbody2D.velocity.y <= -1) transform.rigidbody2D.velocity = new Vector2(transform.rigidbody2D.velocity.x, -1);
            var hit = Physics2D.Linecast(transform.position, _grapple.transform.position, ~(1 << 8));

            if(Vector3.Distance(transform.position, _grapple.transform.position) > _grappleLength)
            {
                transform.position -= (transform.position - _grapple.transform.position).normalized * (Vector3.Distance(transform.position, _grapple.transform.position) - _grappleLength);
            }

            if (hit.collider.gameObject != _grapple)
            {
                _points.Insert(_points.Count - 1, hit.point);

                _line.SetVertexCount(_points.Count);
                for(var i = 0; i < _points.Count; i++)
                    _line.SetPosition(i, _points[i]);

                _grapple.transform.position = hit.point;
            }
            else if (Input.GetMouseButtonDown(0) || hit.collider == null)
            {
//                Destroy(gameObject.GetComponent<DistanceJoint2D>());
                _line.gameObject.SetActive(false);
                _points.Clear();
            }
            else
            {
                _line.SetPosition(_points.Count - 1, transform.position);
            }
        }
    }
}