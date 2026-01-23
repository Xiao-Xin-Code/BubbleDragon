using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UnityEngine.Rendering;

namespace QMVC
{
	public interface IArchitecture
	{
		void RegisterSystem<T>(T system) where T : ISystem;
		void RegisterModel<T>(T model) where T : IModel;
		void RegisterUtility<T>(T utility) where T : IUtility;

		T GetSystem<T>() where T : class, ISystem;
		T GetModel<T>() where T : class, IModel;
		T GetUtility<T>() where T : class, IUtility;

		void SendCommand<T>(T command) where T : ICommand;
		TResult SendCommand<TResult>(ICommand<TResult> command);

		TResult SendQuery<TResult>(IQuery<TResult> query);

		void SendEvent<T>() where T : new();
		void SendEvent<T>(T e);

		IUnRegister RegisterEvent<T>(Action<T> onEvent);
		void UnRegisterEvent<T>(Action<T> onEvent);
		void Deinit();

	}

	public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
	{
		private bool mInited = false;
		public static Action<T> OnRegisterPatch = architecture => { };
		protected static T mArchitecture;

		public static IArchitecture Interface
		{
			get
			{
				if (mArchitecture == null) MakeSureArchitecture();
				return mArchitecture;
			}
		}

		static void MakeSureArchitecture()
		{
			if (mArchitecture == null)
			{
				mArchitecture = new T();
				mArchitecture.Init();
				OnRegisterPatch?.Invoke(mArchitecture);

				foreach (var model in mArchitecture.mContainer.GetInstancesByType<IModel>().Where(m => !m.Initialized))
				{
					model.Init();
					model.Initialized = true;
				}


				foreach (var system in mArchitecture.mContainer.GetInstancesByType<ISystem>().Where(s => !s.Initialized))
				{
					system.Init();
					system.Initialized = true;
				}
			}
		}

		protected abstract void Init();


		public void Deinit()
		{
			OnDeinit();
			foreach (var model in mArchitecture.mContainer.GetInstancesByType<IModel>().Where(m => !m.Initialized)) model.Deinit();
			foreach (var system in mArchitecture.mContainer.GetInstancesByType<ISystem>().Where(s => !s.Initialized)) system.Deinit();
			mContainer.Clear();
			mArchitecture = null;
		}

		protected virtual void OnDeinit()
		{
		}


		/// <summary>
		/// 用于记录各个类型的实例
		/// </summary>
		private IOCContainer mContainer = new IOCContainer();

		public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
		{
			system.SetArchitecture(this);
			mContainer.Register<TSystem>(system);
			if (mInited)
			{
				system.Init();
				system.Initialized = true;
			}
		}

		public void RegisterModel<TModel>(TModel model) where TModel : IModel
		{
			model.SetArchitecture(this);
			mContainer.Register<TModel>(model);
			if (mInited)
			{
				model.Init();
				model.Initialized = true;
			}
		}

		public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility => mContainer.Register<TUtility>(utility);

		public TSystem GetSystem<TSystem>() where TSystem : class, ISystem => mContainer.Get<TSystem>();

		public TModel GetModel<TModel>() where TModel : class, IModel => mContainer.Get<TModel>();

		public TUtility GetUtility<TUtility>() where TUtility : class, IUtility => mContainer.Get<TUtility>();

		public TResult SendCommand<TResult>(ICommand<TResult> command) => ExecuteCommand(command);
		public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand => ExecuteCommand(command);

		protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command)
		{
			command.SetArchitecture(this);
			return command.Execute();
		}

		protected virtual void ExecuteCommand(ICommand command)
		{
			command.SetArchitecture(this);
			command.Execute();
		}

		public TResult SendQuery<TResult>(IQuery<TResult> query) => DoQuery<TResult>(query);

		protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)
		{
			query.SetArchitecture(this);
			return query.Do();
		}

		private TypeEventSystem mTypeEventSystem = new TypeEventSystem();

		public void SendEvent<TEvent>() where TEvent : new() => mTypeEventSystem.Send<TEvent>();
		public void SendEvent<TEvent>(TEvent e) => mTypeEventSystem.Send<TEvent>(e);
		public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent) => mTypeEventSystem.Register<TEvent>(onEvent);
		public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent) => mTypeEventSystem.UnRegister<TEvent>(onEvent);

	}

	#region Entity

	/// <summary>
	/// 实体数据
	/// </summary>
	public interface IEntity : IBelongToArchitecture
	{

	}


	#endregion

	#region View

	public interface IView : IBelongToArchitecture
	{

	}

	#endregion

	#region Controller

	public interface IController : IBelongToArchitecture,
		ICanGetSystem, ICanGetModel, ICanGetUtility,
		ICanSendCommand, ICanSendQuery,
		ICanRegisterEvent
	{

	}

	#endregion


	#region Model

	public interface IModel : IBelongToArchitecture, ICanSetArchitecture,
		ICanGetUtility,
		ICanSendEvent,
		ICanInit
	{

	}

    public abstract class AbstractModel : IModel
    {
		private IArchitecture mArchitecture;

        public bool Initialized { get; set; }

		public IArchitecture GetArchitecture() => mArchitecture;
		public void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
		void ICanInit.Init() => OnInit();
		public void Deinit() => OnDeinit();

		protected abstract void OnInit();
		protected virtual void OnDeinit() { }
	}

    #endregion

    #region System

    public interface ISystem : IBelongToArchitecture, ICanSetArchitecture,
		ICanGetModel, ICanGetUtility, ICanGetSystem,
		ICanRegisterEvent,
		ICanSendEvent,
		ICanInit
	{

	}

    public abstract class AbstractSystem : ISystem
    {
		private IArchitecture mArchitecture;
        public bool Initialized { get; set; }

       
		public IArchitecture GetArchitecture() => mArchitecture;
		public void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
		void ICanInit.Init() => OnInit();
		public void Deinit() => OnDeinit();

		protected abstract void OnInit();

		protected virtual void OnDeinit() { }



	}


    #endregion

    #region Utility

    public interface IUtility
	{

	}

	#endregion

	#region Command

	public interface ICommand : IBelongToArchitecture, ICanSetArchitecture,
		ICanGetSystem, ICanGetModel, ICanGetUtility,
		ICanSendEvent, ICanSendCommand, ICanSendQuery
	{
		void Execute();
	}

	public interface ICommand<TResult> : IBelongToArchitecture, ICanSetArchitecture,
		ICanGetSystem, ICanGetModel, ICanGetUtility,
		ICanSendEvent, ICanSendCommand, ICanSendQuery
	{
		TResult Execute();
	}

	public abstract class AbstractCommand : ICommand
	{
		private IArchitecture mArchitecture;

		IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;

		void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;

		void ICommand.Execute() => OnExecute();

		protected abstract void OnExecute();
	}

	public abstract class AbstractCommand<TResult> : ICommand<TResult>
	{
		private IArchitecture mArchitecture;

		IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;

		void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;

		TResult ICommand<TResult>.Execute() => OnExecute();

		protected abstract TResult OnExecute();
	}



	#endregion

	#region Query

	public interface IQuery<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem, ICanSendQuery
	{
		TResult Do();
	}

	public abstract class AbstractQuery<T> : IQuery<T>
	{
		public T Do() => OnDo();

		protected abstract T OnDo();


		private IArchitecture mArchitecture;

		public IArchitecture GetArchitecture() => mArchitecture;

		public void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
	}

	#endregion

	#region Rule

	public interface IBelongToArchitecture
	{
		IArchitecture GetArchitecture();
	}


	public interface ICanSetArchitecture
	{
		void SetArchitecture(IArchitecture architecture);
	}


	public interface ICanGetModel : IBelongToArchitecture
	{

	}
	public static class CanGetModelExtension
	{
		public static T GetModel<T>(this ICanGetModel self) where T : class, IModel
			=> self.GetArchitecture().GetModel<T>();
	}



	public interface ICanGetSystem : IBelongToArchitecture
	{

	}

	public static class CanGetSystemExtension
	{
		public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
			self.GetArchitecture().GetSystem<T>();
	}


	public interface ICanGetUtility : IBelongToArchitecture
	{

	}
	public static class CanGetUtilityExtension
	{
		public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility =>
			self.GetArchitecture().GetUtility<T>();
	}



	public interface ICanRegisterEvent : IBelongToArchitecture
	{

	}

	public static class CanRegisterEventExtension
	{
		public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) =>
			self.GetArchitecture().RegisterEvent<T>(onEvent);

		public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) =>
			self.GetArchitecture().UnRegisterEvent<T>(onEvent);
	}

	public interface ICanSendCommand : IBelongToArchitecture
	{
	}

	public static class CanSendCommandExtension
	{
		public static void SendCommand<T>(this ICanSendCommand self) where T : ICommand, new() =>
			self.GetArchitecture().SendCommand<T>(new T());

		public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand =>
			self.GetArchitecture().SendCommand<T>(command);

		public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command) =>
			self.GetArchitecture().SendCommand(command);
	}



	public interface ICanSendEvent : IBelongToArchitecture
	{

	}

	public static class CanSendEventExtension
	{
		public static void SendEvent<T>(this ICanSendEvent self) where T : new() =>
			self.GetArchitecture().SendEvent<T>();

		public static void SendEvent<T>(this ICanSendEvent self, T e) => self.GetArchitecture().SendEvent<T>(e);
	}

	public interface ICanSendQuery : IBelongToArchitecture
	{
	}

	public static class CanSendQueryExtension
	{
		public static TResult SendQuery<TResult>(this ICanSendQuery self, IQuery<TResult> query) =>
			self.GetArchitecture().SendQuery(query);
	}


	public interface ICanInit
	{
		bool Initialized { get; set; }
		void Init();
		void Deinit();
	}


	#endregion
}

