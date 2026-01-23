using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public interface IUnRegister
{
	void UnRegister();
}

public interface IUnRegisterList
{
	List<IUnRegister> UnregisterList { get; }
}

public static class IUnRegisterListExtension
{
	public static void AddToUnregisterList(this IUnRegister self, IUnRegisterList unRegisterList) =>
		unRegisterList.UnregisterList.Add(self);
	public static void UnRegisterAll(this IUnRegisterList self)
	{
		foreach (var unRegister in self.UnregisterList) 
		{
			unRegister.UnRegister();
		}
		self.UnregisterList.Clear();
	}
}

public struct CustomUnRegister : IUnRegister
{
	private Action mOnUnRegister { get; set; }
	public CustomUnRegister(Action onUnRegister) => mOnUnRegister = onUnRegister;


    public void UnRegister()
    {
		mOnUnRegister.Invoke();
		mOnUnRegister = null;
    }
}


public abstract class UnRegisterTrigger : MonoBehaviour
{

	private readonly HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();
	public IUnRegister AddUnRegister(IUnRegister unRegister)
	{
		mUnRegisters.Add(unRegister);
		return unRegister;
	}

	public void RemoveUnRegister(IUnRegister unRegister) => mUnRegisters.Remove(unRegister);

	public void UnRegister()
	{
		foreach (var unRegister in mUnRegisters) 
		{
			unRegister.UnRegister();
		}
		mUnRegisters.Clear();
	}

}

public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
{
	private void OnDestroy()
	{
		UnRegister();
	}
}

public class UnRegisterOnDisableTrigger : UnRegisterTrigger
{
	private void OnDisable()
	{
		UnRegister();
	}
}

public class UnRegisterCurrentSceneUnloadedTrigger : UnRegisterTrigger
{
	private static UnRegisterCurrentSceneUnloadedTrigger mDefault;
	public static UnRegisterCurrentSceneUnloadedTrigger Get
	{
		get
		{
			if (!mDefault)
			{
				mDefault = new GameObject("UnRegisterCurrentUnloadedTrigger")
					.AddComponent<UnRegisterCurrentSceneUnloadedTrigger>();
			}
			return mDefault;
		}
	}

	private void Awake()
	{
		DontDestroyOnLoad(this);
		hideFlags = HideFlags.HideInHierarchy;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
	}

	private void OnDestroy() => SceneManager.sceneUnloaded -= OnSceneUnloaded;
	void OnSceneUnloaded(Scene scene) => UnRegister();

}

public static class UnRegisterExtension
{
	static T GetOrAddComponent<T>(GameObject gameobject) where T : Component
	{
		var trigger = gameobject.GetComponent<T>();
		if (!trigger)
		{
			trigger = gameobject.AddComponent<T>();
		}
		return trigger;
	}

	public static IUnRegister UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister,
		GameObject gameobject) =>
		GetOrAddComponent<UnRegisterOnDestroyTrigger>(gameobject)
		.AddUnRegister(unRegister);

	public static IUnRegister UnRegisterWhenGameObjectDestroyed<T>(this IUnRegister self, T component)
		where T : Component =>
		self.UnRegisterWhenGameObjectDestroyed(component.gameObject);

	public static IUnRegister UnRegisterWhenDisabled<T>(this IUnRegister self, T component)
		where T : Component =>
		self.UnRegisterWhenDisabled(component.gameObject);

	public static IUnRegister UnRegisterWhenDisabled(this IUnRegister unRegister,
			GameObject gameObject) =>
			GetOrAddComponent<UnRegisterOnDisableTrigger>(gameObject)
				.AddUnRegister(unRegister);

	public static IUnRegister UnRegisterWhenCurrentSceneUnloaded(this IUnRegister self) =>
		   UnRegisterCurrentSceneUnloadedTrigger.Get.AddUnRegister(self);

}



public class TypeEventSystem
{
	private readonly EasyEvents mEvents = new EasyEvents();
	public static readonly TypeEventSystem Global = new TypeEventSystem();
	public void Send<T>() where T : new() => mEvents.GetEvent<EasyEvent<T>>()?.Trigger(new T());
	public void Send<T>(T e) => mEvents.GetEvent<EasyEvent<T>>()?.Trigger(e);
	public IUnRegister Register<T>(Action<T> onEvent) => mEvents.GetOrAddEvent<EasyEvent<T>>().Register(onEvent);

	public void UnRegister<T>(Action<T> onEvent)
	{
		var e = mEvents.GetEvent<EasyEvent<T>>();
		e?.UnRegister(onEvent);
	}
	
}