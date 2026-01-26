using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class BackGroundInputView : BaseView,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] Collider2D viewCollider;


    event Action<PointerEventData> onPointerDown;
    event Action<PointerEventData> onPointerUp;
    event Action<PointerEventData> onBeginDragEvent;
    event Action<PointerEventData> onDragEvent;
    event Action<PointerEventData> onEndDragEvent;

	#region Down/Up

	public void OnPointerDown(PointerEventData eventData)
	{
        onPointerDown?.Invoke(eventData);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
        onPointerUp?.Invoke(eventData);
	}

	#endregion

	#region Drag

	public void OnBeginDrag(PointerEventData eventData)
	{
		onBeginDragEvent?.Invoke(eventData);
        
	}

	public void OnDrag(PointerEventData eventData)
	{
		onDragEvent?.Invoke(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		onEndDragEvent?.Invoke(eventData);
	}

   

    #endregion

    #region Register

    public void RegisterPointerDownEvent(Action<PointerEventData> action)
    {
        onPointerDown += action;
    }

    public void RegisterPointerUpEvent(Action<PointerEventData> action)
    {
        onPointerUp += action;
    }

    public void RegisterBeginDragEvent(Action<PointerEventData> action)
    {
        onBeginDragEvent += action;
    }

    public void RegisterDragEvent(Action<PointerEventData> action)
    {
        onDragEvent += action;
    }

    public void RegisterEndDragEvent(Action<PointerEventData> action)
    {
        onEndDragEvent += action;
    }

    #endregion

    #region UnRegister

    public void UnRegisterPointerDownEvent(Action<PointerEventData> action)
    {
        onPointerDown -= action;
    }

    public void UnRegisterPointerUpEvent(Action<PointerEventData> action)
    {
        onPointerUp -= action;
    }

    public void UnRegisterBeginDragEvent(Action<PointerEventData> action)
    {
        onBeginDragEvent -= action;
    }

    public void UnRegisterDragEvent(Action<PointerEventData> action)
    {
        onDragEvent -= action;
    }

    public void UnRegisterEndDragEvent(Action<PointerEventData> action)
    {
        onEndDragEvent -= action;
    }

    #endregion


    public void SetColliderAllow(bool isAllow)
    {
        viewCollider.enabled = isAllow;
    }
}
