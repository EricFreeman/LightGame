using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class GrapplingHook : MonoBehaviour
    {
        public float Length = 4;

        [HideInInspector]
        public bool IsEnabled
        {
            get { return _points.Any(); }
        }

        private readonly List<GameObject> _points = new List<GameObject>();
        private LineRenderer _line;
        private GameObject _grapple;
        private GameObject _previousGrapple;

        void Start()
        {
            _line = new GameObject("Line").AddComponent<LineRenderer>();
            _line.SetVertexCount(2);
            _line.SetWidth(.025f, .025f);
            _line.gameObject.SetActive(false);
            _line.SetColors(Color.black, Color.black);
            _line.renderer.material.color = Color.black;

            _grapple = new GameObject("Grapple");
            _grapple.AddComponent<CircleCollider2D>().radius = .1f;
            _grapple.AddComponent<Rigidbody2D>();
            _grapple.rigidbody2D.isKinematic = true;

            _previousGrapple = (GameObject)Instantiate(_grapple);
        }

        void Update()
        {
            if (IsEnabled) UpdateGrapple();
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

                    _points.Add(CreateGrapplePoint(hit));

                    _grapple.transform.position = hit.point;

                    var joint = gameObject.AddComponent<DistanceJoint2D>();
                    joint.connectedBody = _grapple.GetComponent<Rigidbody2D>();
                    joint.distance = Vector3.Distance(hit.point, transform.position);
                    joint.maxDistanceOnly = true;
                }
            }
        }

        private GameObject CreateGrapplePoint(RaycastHit2D hit)
        {
            var p = new GameObject("GrapplePoint");
            p.transform.SetParent(hit.collider.transform);
            p.transform.position = hit.point;
            return p;
        }

        private void UpdateGrapple()
        {
            var hit = Physics2D.Linecast(transform.position, _grapple.transform.position, ~(1 << 8));
            var hitPrev = Physics2D.Linecast(transform.position, _previousGrapple.transform.position, ~(1 << 8));

            if (hit.collider.gameObject != _grapple && hit.collider.gameObject != _previousGrapple)
            {
                // if you lose line of sight on the grappling hook, then add a new point to wrap around

                _points.Add(CreateGrapplePoint(hit));

                UpdateLineDrawing();

                _previousGrapple.transform.position = _grapple.transform.position;
                _grapple.transform.position = hit.point;

                GetComponent<DistanceJoint2D>().distance -=
                        Vector3.Distance(_grapple.transform.position, _previousGrapple.transform.position);
            }
            else if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                // if you retract the grappling hook

                Destroy(gameObject.GetComponent<DistanceJoint2D>());
                _line.gameObject.SetActive(false);
                _points.ForEach(Destroy);
                _points.Clear();
                _grapple.transform.position = new Vector3(0, 0, -1);
                _previousGrapple.transform.position = new Vector3(0, 0, -1);

                if(Input.GetKeyDown(KeyCode.Space))
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 3);
            }
            else if (Vector3.Distance(_grapple.transform.position, _previousGrapple.transform.position) <= .05f)
            {
                RemoveLastCollider();
                // figure out why they are being put so close together
                // and maybe make it so last point ISN'T player in _points
            }
            else
            {
                // always update the last points in the line to track player

                _line.SetPosition(_points.Count, transform.position);
                rigidbody2D.AddForce(Vector3.right * Input.GetAxisRaw("Horizontal") * 25);
                GetComponent<DistanceJoint2D>().distance -= Input.GetAxisRaw("Vertical") * Time.deltaTime;

                // if you can see previous point then unroll back to that point
                if (hitPrev.collider != null && hitPrev.transform == _previousGrapple.transform)
                    RemoveLastCollider();
            }
        }

        private void RemoveLastCollider()
        {
            if (_points.Count > 1)
            {
                Destroy(_points[_points.Count - 1]);
                _points.RemoveAt(_points.Count - 1);

                UpdateLineDrawing();

                GetComponent<DistanceJoint2D>().distance +=
                    Vector3.Distance(_grapple.transform.position, _previousGrapple.transform.position);
                _grapple.transform.position = _previousGrapple.transform.position;
            }

            if (_points.Count > 1)
                _previousGrapple.transform.position = _points.ElementAt(_points.Count - 2).transform.position;
            else
                _previousGrapple.transform.position = new Vector3(0, 0, -1);
        }

        private void UpdateLineDrawing()
        {
            _line.SetVertexCount(_points.Count + 1);
            for (var i = 0; i < _points.Count; i++)
                _line.SetPosition(i, _points[i].transform.position);
            _line.SetPosition(_points.Count, transform.position);
        }
    }
}