using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

public class PatternPoint : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler {
	public int pointId;
	public Vector2 pointPosition;
	public bool isSelected;

	public void OnBeginDrag(PointerEventData eventData)
	{
		//TODO A mess, do something about this, call only once
		PatternLockManager.Instance.StartDrag(this.pointId);
		PatternLockManager.Instance.DrawAtPoint(transform.position);
		PatternLockManager.Instance.MarkPoint(this.pointId);
		isSelected = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		PatternLockManager.Instance.DrawLine(new Vector3(eventData.position.x, eventData.position.y, 0));
	}
	
	public void OnEndDrag(PointerEventData eventData)
	{
		PatternLockManager.Instance.ClearPoints();
		PatternLockManager.Instance.ClearLines();
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{

	}
		
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.pointerDrag != null && isSelected == false) {
			//TODO A terrible fucking mess, do something about this
			Vector3 endPoint = new Vector3(transform.position.x, transform.position.y, 0);
			PatternLockManager pattern = PatternLockManager.Instance;
			pattern.EnterPointDrag(this.pointId);
			pattern.DrawLine(endPoint);
			pattern.SetStartPoint(endPoint);
			pattern.DrawNextLine();
			pattern.MarkPoint(this.pointId);
			//TODO Especially this shit here
			pattern.IsUnsignedPointExistBetweenLine(pattern.GetPointById(pattern.patternNumbers[pattern.patternNumbers.Count - 2]).GetComponent<PatternPoint>().pointPosition, this.pointPosition);
		}
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{

	}

	public void OnDrop(PointerEventData eventData)
	{

	}
}
