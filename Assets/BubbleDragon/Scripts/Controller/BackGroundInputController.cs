using QMVC;
using UnityEngine;
using UnityEngine.EventSystems;


public class BackGroundInputController : MonoController
{
	[SerializeField] BackGroundInputView _backGroundInputView;


	public override void Init()
	{
		_backGroundInputView.RegisterPointerDownEvent(OnPointerDown);
		_backGroundInputView.RegisterPointerUpEvent(OnPointerUp);
		_backGroundInputView.RegisterBeginDragEvent(OnBeginDrag);
		_backGroundInputView.RegisterDragEvent(OnDrag);
		_backGroundInputView.RegisterEndDragEvent(OnEndDrag);
		this.RegisterEvent<BackGroundInputAllowEvent>(OnInputAllow);
	}

	private void OnInputAllow(BackGroundInputAllowEvent evt)
	{
		_backGroundInputView.SetColliderAllow(evt.isAllow);
	}

	private void OnPointerDown(PointerEventData eventData)
	{
		this.SendCommand(new BackGroundPointerDownCommand(eventData));
	}

	private void OnPointerUp(PointerEventData eventData)
	{
		this.SendCommand(new BackGroundPointerUpCommand(eventData));
	}

	private void OnBeginDrag(PointerEventData eventData)
	{
		this.SendCommand(new BackGroundBeginDragCommand(eventData));
	}

	private void OnDrag(PointerEventData eventData)
	{
		this.SendCommand(new BackGroundDragCommand(eventData));
	}

	private void OnEndDrag(PointerEventData eventData)
	{
		this.SendCommand(new BackGroundEndDragCommand(eventData));
	}

}


