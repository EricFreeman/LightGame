using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class GrapplingHook : MonoBehaviour
    {
        public float Length = 3;

        [HideInInspector]
        public bool IsEnabled
        {
            get { return _points.Count > 0; }
        }

        private readonly List<Vector2> _points = new List<Vector2>();
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
            _grapple.AddComponent<BoxCollider2D>().size = new Vector2(.1f, .1f);
            _grapple.AddComponent<Rigidbody2D>();
            _grapple.rigidbody2D.isKinematic = true;

            _previousGrapple = (GameObject)Instantiate(_grapple);
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

                    _grapple.transform.position = hit.point;

                    var joint = gameObject.AddComponent<DistanceJoint2D>();
                    joint.connectedBody = _grapple.GetComponent<Rigidbody2D>();
                    joint.distance = Vector3.Distance(hit.point, transform.position);
                    joint.maxDistanceOnly = true;
                }
            }
        }

        private void UpdateGrapple()
        {
            var hit = Physics2D.Linecast(transform.position, _grapple.transform.position, ~(1 << 8));
            var hitPrev = Physics2D.Linecast(transform.position, _previousGrapple.transform.position, ~(1 << 8));

            if (hit.collider.gameObject != _grapple)
            {
                // if you lose line of sight on the grappling hook, then add a new point to wrap around

                _points.Insert(_points.Count - 1, hit.point);

                _line.SetVertexCount(_points.Count);
                for(var i = 0; i < _points.Count; i++)
                    _line.SetPosition(i, _points[i]);

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
                _points.Clear();
            }
            else
            {
                // always update the last points in the line to track player

                _line.SetPosition(_points.Count - 1, transform.position);
                rigidbody2D.AddForce(Vector3.right * Input.GetAxisRaw("Horizontal") * 25);

                // if you can see previous point then unroll back to that point
                if (_points.Count > 2 && hitPrev.collider != null && hitPrev.transform == _previousGrapple.transform)
                {
                    _points.RemoveAt(_points.Count - 2);

                    _line.SetVertexCount(_points.Count);
                    for (var i = 0; i < _points.Count; i++)
                        _line.SetPosition(i, _points[i]);
                    _line.SetPosition(_points.Count - 1, transform.position);

                    GetComponent<DistanceJoint2D>().distance += 
                        Vector3.Distance(_grapple.transform.position, _previousGrapple.transform.position);
                    _grapple.transform.position = _previousGrapple.transform.position;

                    if(_points.Count > 2)
                        _previousGrapple.transform.position = _points.ElementAt(_points.Count - 2);
                    else
                        _previousGrapple.transform.position = new Vector3(0,0,-1);
                }
            }
        }
    }
}