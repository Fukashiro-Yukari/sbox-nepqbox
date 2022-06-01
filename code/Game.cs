using Sandbox;
using Gamelib.DayNight;
using System.Linq;
using System.Threading.Tasks;

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

	[ConCmd.Server( "spawn" )]
	public static async Task Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller.Pawn as Player;

		if ( owner == null )
			return;

		var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		var modelRotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );

		//
		// Does this look like a package?
		//
		if ( modelname.Count( x => x == '.' ) == 1 && !modelname.EndsWith( ".vmdl", System.StringComparison.OrdinalIgnoreCase ) && !modelname.EndsWith( ".vmdl_c", System.StringComparison.OrdinalIgnoreCase ) )
		{
			modelname = await SpawnPackageModel( modelname, tr.EndPosition, modelRotation, owner );
			if ( modelname == null )
				return;
		}

		var model = Model.Load( modelname );
		if ( model == null || model.IsError )
			return;

		var ent = new Prop
		{
			Position = tr.EndPosition + Vector3.Down * model.PhysicsBounds.Mins.z,
			Rotation = modelRotation,
			Model = model
		};

		// Let's make sure physics are ready to go instead of waiting
		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		// If there's no physics model, create a simple OBB
		if ( !ent.PhysicsBody.IsValid() )
		{
			ent.SetupPhysicsFromOBB( PhysicsMotionType.Dynamic, ent.CollisionBounds.Mins, ent.CollisionBounds.Maxs );
		}

		if ( ConsoleSystem.Caller.GetClientData<bool>( "cl_print_modelname" ) )
			PrintModelPath( To.Single( owner ), modelname );

		new Undo( "Prop" ).SetClient( ConsoleSystem.Caller ).AddEntity( ent ).Finish( $"Prop ({ent.GetModelName()})" );
	}

	static async Task<string> SpawnPackageModel( string packageName, Vector3 pos, Rotation rotation, Entity source )
	{
		var package = await Package.Fetch( packageName, false );
		if ( package == null || package.PackageType != Package.Type.Model || package.Revision == null )
		{
			// spawn error particles
			return null;
		}

		if ( !source.IsValid ) return null; // source entity died or disconnected or something

		var model = package.GetMeta( "PrimaryAsset", "models/dev/error.vmdl" );
		var mins = package.GetMeta( "RenderMins", Vector3.Zero );
		var maxs = package.GetMeta( "RenderMaxs", Vector3.Zero );

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();

		return model;
	}

	[ClientRpc]
	public static void PrintModelPath( string modelname )
	{
		Log.Info( $"Spawn Prop: {modelname}" );
	}

	[ConCmd.Server( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		var owner = ConsoleSystem.Caller.Pawn as Player;

		if ( owner == null )
			return;

		var attribute = Library.GetAttribute( entName );

		if ( attribute == null || !attribute.Spawnable )
			return;

		var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 200 )
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

		ent.Position = tr.EndPosition;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) );

		var di = DisplayInfo.For( ent );

		new Undo( "Entity" ).SetClient( ConsoleSystem.Caller ).AddEntity( ent ).Finish( $"Entity ({di.Name})" );

		//Log.Info( $"ent: {ent}" );
	}

	[ConCmd.Server( "give_weapon" )]
	public static void GiveWeapon( string entName )
	{
		var target = ConsoleSystem.Caller.Pawn as Player;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		var wep = Library.Create<Weapon>( entName );

		if ( wep.IsValid() )
			inventory.Add( wep );
	}

	[ConCmd.Server( "give_all_weapons" )]
	public static void GiveAllWeapon()
	{
		var target = ConsoleSystem.Caller.Pawn as Player;
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

	[ConCmd.Admin( "respawn_entities" )]
	public static void RespawnEntities()
	{
		Map.Reset( DefaultCleanupFilter );
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
					KillFeed.OnKilledMessage( attackerClient.PlayerId, attackerClient.Name, client.PlayerId, client.Name, pawn.LastAttackerWeapon?.ClassName, isHeadShot );
				else
					KillFeed.OnKilledMessage( attackerClient.PlayerId, attackerClient.Name, client.PlayerId, client.Name, pawn.LastAttacker?.ClassName, isHeadShot );
			}
			else
			{
				KillFeed.OnKilledMessage( pawn.LastAttacker.NetworkIdent, pawn.LastAttacker.ToString(), client.PlayerId, client.Name, "killed", isHeadShot );
			}
		}
		else
		{
			KillFeed.OnKilledMessage( 0, "", client.PlayerId, client.Name, "died", isHeadShot );
		}
	}

	public void OnEntKilled( Entity ent )
	{
		Host.AssertServer();

		var isHeadShot = false;
		var di = DisplayInfo.For( ent );

		if ( ent is NPC npc )
			isHeadShot = npc.IsHeadShot;

		if ( ent.LastAttacker != null )
		{
			var attackerClient = ent.LastAttacker.Client;

			if ( attackerClient != null )
			{
				if ( ent.LastAttackerWeapon != null )
					KillFeed.OnKilledMessage( attackerClient.PlayerId, attackerClient.Name, di.Name, ent.LastAttackerWeapon?.ClassName, isHeadShot );
				else
					KillFeed.OnKilledMessage( attackerClient.PlayerId, attackerClient.Name, di.Name, ent.LastAttacker.ClassName, isHeadShot );
			}
			else
			{
				KillFeed.OnKilledMessage( ent.LastAttacker.NetworkIdent, ent.LastAttacker.ToString(), di.Name, "killed", isHeadShot );
			}
		}
		else
		{
			KillFeed.OnKilledMessage( 0, "", di.Name, "died", isHeadShot );
		}
	}
}
