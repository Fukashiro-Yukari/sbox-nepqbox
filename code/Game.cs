using Sandbox;
using Gamelib.DayNight;
using System.Linq;

partial class NepQBoxGame : Game
{
	[ConVar.Replicated( "sv_day_night_cycle" )]
	public static bool DayNightCycle { get; set; } = true;

	[ConVar.Replicated( "sv_day_night_cycle_debug" )]
	public static bool DayNightCycleDebug { get; set; } = false;

	private bool LastDayNightCycle = true;
	private Vector3 EnvPosition = new Vector3();
	private Rotation EnvRotation = new Rotation();
	private Color EnvColor = new Color();
	private Color EnvSkyColor = new Color();
	private bool HaveEnv = false;
	private DayNightController dnc = null;

	public NepQBoxGame()
	{
		if ( IsServer )
		{
			// Create the HUD
			_ = new SandboxHud();

			LastDayNightCycle = DayNightCycle;

			if ( All.OfType<EnvironmentLightEntity>().Count() < 1 )
			{
				dnc = new DayNightController();
				dnc.DawnColor = new Color32( 226, 79, 33 ).ToColor();
				dnc.DawnSkyColor = new Color32( 226, 79, 33 ).ToColor();
				dnc.DayColor = new Color( 0.97f, 0.96f, 0.95f );
				dnc.DaySkyColor = new Color( 0.94f, 0.95f, 1.00f );
				dnc.DuskColor = new Color32( 197, 38, 7 ).ToColor();
				dnc.DuskSkyColor = new Color32( 197, 38, 7 ).ToColor();
				dnc.NightColor = new Color32( 23, 60, 150 ).ToColor();
				dnc.NightSkyColor = new Color32( 23, 60, 150 ).ToColor();
				dnc.Enable = DayNightCycle;
				dnc.SetColors();
			}
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new SandboxPlayer( cl );
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

		if ( ConsoleSystem.Caller.GetClientData<bool>( "cl_print_modelname" ) )
			PrintModelPath( To.Single( owner ), modelname );

		new Undo( "Prop" ).SetClient( ConsoleSystem.Caller ).AddEntity( ent ).Finish( $"Prop ({ent.GetModelName()})" );
	}

	[ClientRpc]
	public static void PrintModelPath( string modelname )
	{
		Log.Info( $"Spawn Prop: {modelname}" );
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
			if ( owner.Inventory.Add( ent, true ) )
				return;
		}

		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );

		new Undo( "Entity" ).SetClient( ConsoleSystem.Caller ).AddEntity( ent ).Finish( $"Entity ({ent.ClassInfo.Title})" );

		//Log.Info( $"ent: {ent}" );
	}

	[ServerCmd( "give_weapon" )]
	public static void GiveWeapon( string entName )
	{
		var target = ConsoleSystem.Caller.Pawn;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		var wep = Library.Create<Weapon>( entName );

		if ( wep.IsValid() )
			inventory.Add( wep );
	}

	[ServerCmd( "give_all_weapons" )]
	public static void GiveAllWeapon()
	{
		var target = ConsoleSystem.Caller.Pawn;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		foreach ( var wep in Library.GetAllAttributes<Weapon>() )
		{
			if ( wep.Title.StartsWith( "Weapon" ) )
				continue;

			var w = Library.Create<Weapon>( wep.Name );

			if ( w.IsValid() )
				inventory.Add( w );
		}
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

	[ClientCmd( "debug_write" )]
	public static void Write()
	{
		ConsoleSystem.Run( "quit" );
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
				Log.Info( $"Color: R:{env.Color.r * 255},G:{env.Color.g * 255},B:{env.Color.b * 255},A:{env.Color.a * 255}" );
				Log.Info( $"SkyColor: R:{env.SkyColor.r * 255},G:{env.SkyColor.g * 255},B:{env.SkyColor.b * 255},A:{env.SkyColor.a * 255}" );
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
	public override void OnKilled( Entity pawn )
	{
		Host.AssertServer();

		var client = pawn.Client;
		if ( client != null )
		{
			OnKilled( client, pawn );
		}
		else
		{
			OnEntKilled( pawn );
		}
	}

	/// <summary>
	/// An entity, which is a pawn, and has a client, has been killed.
	/// </summary>
	public override void OnKilled( Client client, Entity pawn )
	{
		Host.AssertServer();

		var isHeadShot = false;

		Log.Info( $"{client.Name} was killed" );

		if ( pawn is SandboxPlayer ply )
			isHeadShot = ply.IsHeadShot;

		if ( pawn is NPC npc )
			isHeadShot = npc.IsHeadShot;

		if ( pawn.LastAttacker != null )
		{
			var attackerClient = pawn.LastAttacker.Client;

			if ( attackerClient != null )
			{
				if ( pawn.LastAttackerWeapon != null )
					OnKilledMessage( attackerClient.SteamId, attackerClient.Name, client.SteamId, client.Name, pawn.LastAttackerWeapon.ClassInfo?.Name, isHeadShot );
				else
					OnKilledMessage( attackerClient.SteamId, attackerClient.Name, client.SteamId, client.Name, pawn.LastAttacker.ClassInfo?.Name, isHeadShot );
			}
			else
			{
				OnKilledMessage( (ulong)pawn.LastAttacker.NetworkIdent, pawn.LastAttacker.ToString(), client.SteamId, client.Name, "killed", isHeadShot );
			}
		}
		else
		{
			OnKilledMessage( 0, "", client.SteamId, client.Name, "died", isHeadShot );
		}
	}

	public void OnEntKilled( Entity ent )
	{
		Host.AssertServer();

		var isHeadShot = false;

		if ( ent is NPC npc )
			isHeadShot = npc.IsHeadShot;

		if ( ent.LastAttacker != null )
		{
			var attackerClient = ent.LastAttacker.Client;

			if ( attackerClient != null )
			{
				if ( ent.LastAttackerWeapon != null )
					OnKilledMessage( attackerClient.SteamId, attackerClient.Name, ent.ClassInfo.Title, ent.LastAttackerWeapon?.ClassInfo?.Name, isHeadShot );
				else
					OnKilledMessage( attackerClient.SteamId, attackerClient.Name, ent.ClassInfo.Title, ent.LastAttacker.ClassInfo?.Name, isHeadShot );
			}
			else
			{
				OnKilledMessage( (ulong)ent.LastAttacker.NetworkIdent, ent.LastAttacker.ToString(), ent.ClassInfo.Title, "killed", isHeadShot );
			}
		}
		else
		{
			OnKilledMessage( 0, "", ent.ClassInfo.Title, "died", isHeadShot );
		}
	}

	/// <summary>
	/// Called clientside from OnKilled on the server to add kill messages to the killfeed. 
	/// </summary>
	[ClientRpc]
	public virtual void OnKilledMessage( ulong leftid, string left, ulong rightid, string right, string method, bool isHeadShot )
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry( leftid, left, rightid, right, method, isHeadShot );
	}

	[ClientRpc]
	public virtual void OnKilledMessage( ulong leftid, string left, string right, string method, bool isHeadShot )
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry( leftid, left, right, method, isHeadShot );
	}

	[ClientRpc]
	public virtual void OnKilledMessage( string left, ulong rightid, string right, string method, bool isHeadShot )
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry( left, rightid, right, method, isHeadShot );
	}

	[ClientRpc]
	public virtual void OnKilledMessage( string left, string right, string method, bool isHeadShot )
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry( left, right, method, isHeadShot );
	}

	public static void AddHint( string text )
	{
		if ( Host.IsClient ) return;

		AddHintMessage( text );
	}

	[ClientRpc]
	public static void AddHintMessage( string text )
	{
		Hint.Current?.AddHint( text );
	}
}
