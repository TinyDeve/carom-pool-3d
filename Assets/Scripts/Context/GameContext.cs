using onur.pool.signals;
using onur.pool.commands;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;
using onur.pool.models;

public class GameContext : MVCSContext
{
	public GameContext (MonoBehaviour view) : base (view)
	{
	}
	
	public override IContext Start ()
	{
		base.Start ();
		var startSignal = injectionBinder.GetInstance<StartAppSignal> ();
		startSignal.Dispatch ();
		return this;
	}
	
	protected override void addCoreComponents ()
	{
		base.addCoreComponents ();
		injectionBinder.Unbind<ICommandBinder> ();
		injectionBinder.Bind<ICommandBinder> ().To<SignalCommandBinder> ().ToSingleton ();
	}

	protected override void mapBindings()
	{
		base.mapBindings();

		//signals to command
		commandBinder.Bind<StartAppSignal> ().To<StartAppCommand> ().Once ();
        injectionBinder.Bind<MoveSignal>().ToSingleton();
        injectionBinder.Bind<ShotSignal>().ToSingleton();
        injectionBinder.Bind<ShotPower>().ToSingleton();
        injectionBinder.Bind<GameModeSignal>().ToSingleton();
		injectionBinder.Bind<GameModel> ().ToSingleton ();
        /*
		//services
		injectionBinder.Bind<TaskService> ().ToSingleton ();
		injectionBinder.Bind<IOneSignalService>().To<OneSignalService>().ToSingleton();
		injectionBinder.Bind<ILocalPushService>().To<LocalPushService>().ToSingleton();*/
        /*
		//models
		injectionBinder.Bind<SettingsModel> ().ToSingleton ();*/
    }
}