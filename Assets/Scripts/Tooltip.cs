using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Tooltip : MonoBehaviour
{
    private static Tooltip _current = null;

    /// <summary>
    /// Current selected tooltip
    /// </summary>
    public static Tooltip current { get { return _current; } }
    
    public bool displayOverride = true;

    /// <summary>
    /// Current master canvas
    /// </summary>
    public static Canvas canvas;

    void Awake ()
    {
        _current = this;
    }

	// Use this for initialization
	void Start ()
    {
        Canvas[] c = GetComponentsInParent<Canvas>();
        canvas = c[c.Length - 1];
    }

    public void ForceShow()
    {
        displayOverride = true;
    }

    public void ForceHide()
    {
        displayOverride = false;
    }

    public void SetText(string text)
    {
        GetComponentInChildren<Text>(true).text = text;
    }
	
	// Update is called once per frame
	void Update ()
    {
        RectTransform rectTransform = (RectTransform)transform;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pe = new PointerEventData(EventSystem.current);
            pe.position = Input.mousePosition;

            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pe, hits);

            bool displayTooltip = false;
            
            foreach (RaycastResult h in hits)
            {
                GameObject g = h.gameObject;
                TooltipReceiver tt = g.GetComponent<TooltipReceiver>();

                if (tt)
                {
                    displayTooltip = true;
                    tt.CallOnTooltip();
                }
            }

            transform.FindChild("Panel").gameObject.SetActive(displayTooltip & displayOverride);
            
            CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
            RectTransform canvasTransform = canvas.GetComponent<RectTransform>();

            Vector2 mousePosition = Input.mousePosition;

            float ratio = Screen.width / canvasScaler.referenceResolution.x;

            Vector2 clampedMousePosition = new Vector2(
                Mathf.Clamp(mousePosition.x, 0, canvas.pixelRect.width  - rectTransform.sizeDelta.x * ratio),
                Mathf.Clamp(mousePosition.y, 0, canvas.pixelRect.height - rectTransform.sizeDelta.y * ratio)
                );

            rectTransform.position = clampedMousePosition;
        }
	}
}
