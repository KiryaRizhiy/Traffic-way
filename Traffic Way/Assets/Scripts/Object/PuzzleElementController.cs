using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElementController : MonoBehaviour
{   
    private Transform _waypointsParent
    {
        get
        {
            return transform.GetChild(0);
        }
    }    
    private SpriteRenderer _elementSpriteRenderer
    {
        get
        {
            return GetComponent<SpriteRenderer>();
        }
    }
    private PolygonCollider2D _elementCollider
    {
        get
        {
            return GetComponent<PolygonCollider2D>();
        }
    }
    public List<Transform> _waypointsList
    {
        get
        {
            List<Transform> _result = new List<Transform>();
            for (int i = 0; i < _waypointsParent.childCount; i++)
            {
                _result.Add(_waypointsParent.GetChild(i));
            }
            return _result;
        }
    }
    public Axis movingAxis;
    public Vector2 movingAxisVector
    {
        get
        {
            if (movingAxis == Axis.LeftRight)
                return Vector2.right;
            else
                return Vector2.up;
        }
    }
    private Vector2 _controlsPosition;
    public bool _isUnderControl { get; private set; }
    private Camera _camera
    {
        get
        {
            return transform.parent.parent.GetChild(2).gameObject.GetComponent<Camera>();
        }
    }
    private Vector2 _initialControlsPosition;
    private Vector2 _initialControlsWorldPosition
    {
        get
        {
            return _camera.ScreenToWorldPoint(
                new Vector3(
                _initialControlsPosition.x,
                _initialControlsPosition.y,
                _camera.nearClipPlane)
                );
        }
    }
    private Vector2 _controlsWorldPosition
    {
        get
        {
            return _camera.ScreenToWorldPoint(
                new Vector3(
                _controlsPosition.x,
                _controlsPosition.y,
                _camera.nearClipPlane)
                );
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        CheckIn();
        Untapped();
    }
    private void CheckIn()
    {
        Vector2 _disp = PuzzleController.CheckInPuzzleElement(this);
        transform.position += new Vector3(_disp.x, _disp.y, 0f);
    }
    private void Untapped()
    {
        _elementSpriteRenderer.color = Color.white;
        _isUnderControl = false;
    }
    private void Tapped()
    {
        if ((_isUnderControl || !PuzzleController.isAnyElementUnderControl) && movingAxis != Axis.Static)
        {
            _elementSpriteRenderer.color = Color.gray;
            _initialControlsPosition = _controlsPosition;
            _isUnderControl = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            if (Input.GetMouseButton(0))
                _controlsPosition = Input.mousePosition;
            if (Input.touchCount > 0)
                _controlsPosition = Input.touches[0].position;
            if (!_isUnderControl && _elementCollider.OverlapPoint(_controlsWorldPosition))
            {
                Tapped();
            }
            if (_isUnderControl)
            {
                Vector2 _disp = PuzzleController.ProcessMoveRequest(this, _controlsWorldPosition - _initialControlsWorldPosition);
                Logger.AddContent(UILogDataType.Controls, "Move request with controls displacement " +
                    (_controlsWorldPosition - _initialControlsWorldPosition) + " was processed with result " + _disp);
                if (_disp.magnitude > 0)
                {
                    transform.position += new Vector3(_disp.x, _disp.y, 0f);
                    Tapped();
                    CheckIn();
                }
            }

        }
        else
            if (_isUnderControl) 
            Untapped();
    }
}
public enum Axis {UpDown, LeftRight, Static}