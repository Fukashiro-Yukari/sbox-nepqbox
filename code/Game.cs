using Sandbox;
using Sandbox.DayNight;
using System.Linq;

[Library( "nepqbox", Title = "NepQ Box" )]
partial class SandboxGame : Game
{
	[ConVar.Replicated("sv_day_night_cycle")]
	public static bool DayNightCycle { get;set; } = true;

	[ConVar.Replicated( "sv_day_night_cycle_debug" )]
	public static bool DayNightCycleDebug { get; set; } = false;

	private bool LastDayNightCycle = true;
	private Vector3 EnvPosition = new Vector3();
	private Rotation EnvRotation = new Rotation();
	private Color EnvColor = new Color();
	private Color EnvSkyColor = new Color();
	private bool HaveEnv = false;
	private DayNightController dnc = null;

	public SandboxGame()
	{
		if ( IsServer )
		{
			// Create the HUD
			_ = new SandboxHud();

			LastDayNightCycle = DayNightCycle;

			if (All.OfType<EnvironmentLightEntity>().Count() < 1)
            {
				dnc = new DayNightController();
				dnc.DawnColor = new Color(1.00f, 0.42f, 0f);
				dnc.DawnSkyColor = new Color(0.73f, 0.42f, 0f);
				dnc.DayColor = new Color(0.97f, 0.96f, 0.95f);
				dnc.DaySkyColor = new Color(0.94f, 0.95f, 1.00f);
				dnc.DuskColor = new Color(1.00f, 0.35f, 0f);
				dnc.DuskSkyColor = new Color(1.00f, 0.35f, 0f);
				dnc.NightColor = new Color(0f, 0.58f, 1.00f);
				dnc.NightSkyColor = new Color(0f, 0.58f, 1.00f);
				dnc.Enable = DayNightCycle;
				dnc.SetColors();
			}
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new SandboxPlayer();
		player.Respawn();

		cl.Pawn = player;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[ConVar.ClientData]
	public static bool cl_print_modelname { get; set; } = false;

	[ServerCmd( "spawn" )]
	public static void Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		var ent = new Prop();
		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
		ent.SetModel( modelname );
		ent.Position = tr.EndPos - Vector3.Up * ent.CollisionBounds.Mins.z;

		if (ConsoleSystem.Caller.GetClientData<bool>("cl_print_modelname"))
			PrintModelPath(To.Single(owner), modelname);

		new Undo( $"Prop ({ent.GetModelName()})" ).SetClient( ConsoleSystem.Caller ).AddEntity( ent ).Finish();
	}

	[ClientRpc]
	public static void PrintModelPath(string modelname)
    {
		Log.Info($"Spawn Prop: {modelname}");
    }

	[ServerCmd( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;

		var attribute = Library.GetAttribute( entName );

		if ( attribute == null || !attribute.Spawnable )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = Library.Create<Entity>( entName );
		if ( ent is BaseCarriable && owner.Inventory != null )
		{
			if ( owner.Inventory.Add( ent, true ))
				return;
		}

		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );

		new Undo( ent.ClassInfo.Title ).SetClient( ConsoleSystem.Caller ).AddEntity( ent ).Finish();

		//Log.Info( $"ent: {ent}" );
	}

	[ServerCmd("undo")]
	public static void UndoCmd()
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;

		var text = Undo.DoUndo( ConsoleSystem.Caller );

		if ( text != null )
		{
			var game = Current as SandboxGame;

			game.AddUndoText( To.Single( owner ), text );
		}
	}

	[ClientRpc]
	public void AddUndoText(string text)
	{
		UndoUI.Current.AddEntry( text );
	}

	[ClientRpc]
	public void AddUndoText( Entity ent )
	{
		UndoUI.Current.AddEntry( ent.ClassInfo.Title );
	}

	public override void DoPlayerNoclip( Client player )
	{
		if ( player.Pawn is Player basePlayer )
		{
			if ( basePlayer.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				basePlayer.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				basePlayer.DevController = new NoclipController();
			}
		}
	}

	private EnvironmentLightEntity env;
	private bool envnotfind;
	//private SkyboxObject sky;
	//private bool skynotfind;

	[Event.Tick.Server]
	private void Tick()
	{
		if ( env == null )
		{
			env = All.OfType<EnvironmentLightEntity>().FirstOrDefault();

			if ( env != null )
			{
				Log.Info( "Env Info: " );
				Log.Info( "Color: " + env.Color );
				Log.Info( "SkyColor: " + env.SkyColor );
				Log.Info( "SkyIntensity: " + env.SkyIntensity );
				Log.Info( "Brightness: " + env.Brightness );
				Log.Info( "Position: " + env.Position );
				Log.Info( "Rotation: " + env.Rotation );

				HaveEnv = true;
				EnvPosition = env.Position;
				EnvRotation = env.Rotation;
				EnvColor = env.Color;
				EnvSkyColor = env.SkyColor;
			}
			else if ( !envnotfind )
			{
				Log.Info( "Env Not Find !!!" );

				envnotfind = true;
			}
		}

		// Can't get the 'env_sky' entity
		//if ( sky == null )
		//{
		//	sky = All.OfType<SkyboxObject>().FirstOrDefault();

		//	if ( sky != null )
		//	{
		//		Log.Info( "Sky Info: " );
		//	}
		//	else if ( !skynotfind )
		//	{
		//		Log.Info( "Sky Not Find !!!" );
		//		//Log.Info( "Sky Info: " + Library.GetAttribute( "env_sky" ) + " | " + (Library.GetAttribute( "env_sky" ) == null) );

		//		skynotfind = true;
		//	}
		//}

		if ( DayNightCycle != LastDayNightCycle )
		{
			LastDayNightCycle = DayNightCycle;

			if ( HaveEnv && dnc != null )
			{
				dnc.Enable = DayNightCycle;

				if ( !DayNightCycle )
				{
					env.Position = EnvPosition;
					env.Rotation = EnvRotation;
					env.Color = EnvColor;
					env.SkyColor = EnvSkyColor;
				}
			}
		}
	}

	/// <summary>
	/// An entity has been killed. This is usually a pawn but anything can call it.
	/// </summary>
	public override void OnKilled(Entity pawn)
	{
		Host.AssertServer();

		var client = pawn.GetClientOwner();
		if (client != null)
		{
			OnKilled(client, pawn);
		}
		else
        {
			OnEntKilled(pawn);
		}
	}

	/// <summary>
	/// An entity, which is a pawn, and has a client, has been killed.
	/// </summary>
	public override void OnKilled(Client client, Entity pawn)
	{
		Host.AssertServer();

		Log.Info($"{client.Name} was killed");

		if (pawn.LastAttacker != null)
		{
			var attackerClient = pawn.LastAttacker.GetClientOwner();

			if (attackerClient != null)
			{
				if (pawn.LastAttackerWeapon != null)
					OnKilledMessage(attackerClient.SteamId, attackerClient.Name, client.SteamId, client.Name, pawn.LastAttackerWeapon.ClassInfo?.Name);
                else
                    OnKilledMessage(attackerClient.SteamId, attackerClient.Name, client.SteamId, client.Name, pawn.LastAttacker.ClassInfo?.Name);
			}
			else
			{
				OnKilledMessage((ulong)pawn.LastAttacker.NetworkIdent, pawn.LastAttacker.ToString(), client.SteamId, client.Name, "killed");
			}
		}
		else
		{
			OnKilledMessage(0, "", client.SteamId, client.Name, "died");
		}
	}

	public void OnEntKilled(Entity ent)
	{
		Host.AssertServer();

		if (ent.LastAttacker != null)
		{
			var attackerClient = ent.LastAttacker.GetClientOwner();

			if (attackerClient != null)
			{
				if (ent.LastAttackerWeapon != null)
					OnKilledMessage(attackerClient.SteamId, attackerClient.Name, ent.ClassInfo.Title, ent.LastAttackerWeapon?.ClassInfo?.Name);
				else
					OnKilledMessage(attackerClient.SteamId, attackerClient.Name, ent.ClassInfo.Title, ent.LastAttacker.ClassInfo?.Name);
			}
			else
			{
				OnKilledMessage((ulong)ent.LastAttacker.NetworkIdent, ent.LastAttacker.ToString(), ent.ClassInfo.Title, "killed");
			}
		}
		else
		{
			OnKilledMessage(0, "", ent.ClassInfo.Title, "died");
		}
	}

	/// <summary>
	/// Called clientside from OnKilled on the server to add kill messages to the killfeed. 
	/// </summary>
	[ClientRpc]
	public override void OnKilledMessage(ulong leftid, string left, ulong rightid, string right, string method)
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry(leftid, left, rightid, right, method);
	}

	[ClientRpc]
	public virtual void OnKilledMessage(ulong leftid, string left, string right, string method)
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry(leftid, left, right, method);
	}

	[ClientRpc]
	public virtual void OnKilledMessage(string left, ulong rightid, string right, string method)
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry(left, rightid, right, method);
	}

	[ClientRpc]
	public virtual void OnKilledMessage(string left, string right, string method)
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry(left, right, method);
	}
}
