using UnityEngine;

/// <summary>
/// Utility component to forward mouse or touch input to clicked gameobjects.
/// Calls OnPress, OnClick and OnRelease methods on "first" game object.
/// </summary>
public class InputToEvent : MonoBehaviour
{
    private GameObject lastGo;
    public static Vector3 inputHitPos;
    public bool DetectPointedAtGameObject;
    public static GameObject goPointedAt { get; private set; }

    private Vector2 pressedPosition = Vector2.zero;
    private Vector2 currentPos = Vector2.zero;
    public bool Dragging;

    private Camera m_Camera;

    public Vector2 DragVector
    {
        get { return this.Dragging ? this.currentPos - this.pressedPosition : Vector2.zero; }
    }

    private void Start()
    {
        this.m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.DetectPointedAtGameObject)
        {
            goPointedAt = RaycastObject(ControlFreak2.CF2Input.mousePosition);
        }

        if (ControlFreak2.CF2Input.touchCount > 0)
        {
            ControlFreak2.InputRig.Touch touch = ControlFreak2.CF2Input.GetTouch(0);
            this.currentPos = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Press(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Release(touch.position);
            }

            return;
        }

        this.currentPos = ControlFreak2.CF2Input.mousePosition;
        if (ControlFreak2.CF2Input.GetMouseButtonDown(0))
        {
            Press(ControlFreak2.CF2Input.mousePosition);
        }
        if (ControlFreak2.CF2Input.GetMouseButtonUp(0))
        {
            Release(ControlFreak2.CF2Input.mousePosition);
        }

        if (ControlFreak2.CF2Input.GetMouseButtonDown(1))
        {
            this.pressedPosition = ControlFreak2.CF2Input.mousePosition;
            this.lastGo = RaycastObject(this.pressedPosition);
            if (this.lastGo != null)
            {
                this.lastGo.SendMessage("OnPressRight", SendMessageOptions.DontRequireReceiver);
            }
        }
    }


    private void Press(Vector2 screenPos)
    {
        this.pressedPosition = screenPos;
        this.Dragging = true;

        this.lastGo = RaycastObject(screenPos);
        if (this.lastGo != null)
        {
            this.lastGo.SendMessage("OnPress", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void Release(Vector2 screenPos)
    {
        if (this.lastGo != null)
        {
            GameObject currentGo = RaycastObject(screenPos);
            if (currentGo == this.lastGo)
            {
                this.lastGo.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
            }

            this.lastGo.SendMessage("OnRelease", SendMessageOptions.DontRequireReceiver);
            this.lastGo = null;
        }

        this.pressedPosition = Vector2.zero;
        this.Dragging = false;
    }

    private GameObject RaycastObject(Vector2 screenPos)
    {
        RaycastHit info;
        if (Physics.Raycast(this.m_Camera.ScreenPointToRay(screenPos), out info, 200))
        {
            inputHitPos = info.point;
            return info.collider.gameObject;
        }

        return null;
    }
}