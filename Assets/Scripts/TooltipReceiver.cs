using UnityEngine;
using System.Collections;

public class TooltipReceiver : MonoBehaviour {

    public UnityEngine.Events.UnityEvent onTooltip;
    
    /// <summary>
    /// Calls OnTooltip for all attached components, and invokes any onTooltip callbacks
    /// </summary>
    public void CallOnTooltip()
    {
        gameObject.SendMessage("OnTooltip", null, SendMessageOptions.DontRequireReceiver);
        onTooltip.Invoke();
    }
}
