using UnityEngine;

namespace God
{

public class Draggable : MonoBehaviour
{
    public System.Action OnStartDrag;
    public System.Action OnDragging;
    public System.Action OnDragEnd;

    public bool IsDragged = false;
}

} // namespace God

