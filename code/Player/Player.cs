using Sandbox;

partial class SandboxPlayer : Player
{
	private TimeSince timeSinceDropped;
	private TimeSince timeSinceJumpReleased;
	private TimeSince timeSinceFall;

	private DamageInfo lastDamage;

	[ConVar.Replicated( "sv_fall_damage" )]
	public static bool HavaFallDamage { get; set; } = false;

	/// <summary>
	/// The clothing container is what dresses the citizen
	/// </summary>
	public Clothing.Container Clothing = new();

	public bool IsHeadShot { get; private set; }
	public bool SupressPickupNotices { get; private set; }

	/// <summary>
	/// Default init
	/// </summary>
	public SandboxPlayer()
	{
		Inventory = new Inventory( this );
	}

	/// <summary>
	/// Initialize using this client
	/// </summary>
	public SandboxPlayer( Client cl ) : this()
	{
		// Load clothing from client data
		Clothing.LoadFromClient( cl );
	}

	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();

		if ( DevController is NoclipController )
		{
			DevController = null;
		}

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Clothing.DressEntity( this );

		SupressPickupNotices = true;

		Inventory.Add( new PhysGun(), true );
		Inventory.Add( new GravGun() );
		Inventory.Add( new Tool() );
		Inventory.Add( new Flying() );
		Inventory.Add( new Crowbar() );
		Inventory.Add( new Knife() );
		Inventory.Add( new Pistol() );
		Inventory.Add( new HL2Pistol() );
		Inventory.Add( new Python357() );
		Inventory.Add( new Flashlight() );
		Inventory.Add( new Fists() );

		SupressPickupNotices = false;

		CameraMode = new FirstPersonCamera();

		base.Respawn();
	}

	public override void OnKilled()
	{
		IsHeadShot = GetHitboxGroup( lastDamage.HitboxIndex ) == 1;

		base.OnKilled();

		if ( lastDamage.Flags.HasFlag( DamageFlags.Vehicle ) )
		{
			Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", lastDamage.Position );
			Particles.Create( "particles/impact.flesh-big.vpcf", lastDamage.Position );
			PlaySound( "kersplat" );
		}

		BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );

		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		CameraMode = new SpectateRagdollCamera();

		foreach ( var child in Children )
		{
			child.EnableDrawing = false;
		}

		Inventory.DropActive();
		Inventory.DeleteContents();
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
		{
			info.Damage *= 2.0f;
		}

		lastDamage = info;

		base.TakeDamage( info );

		//Log.Info( info.Attacker is SandboxPlayer attacker && attacker != this );

		if ( (info.Attacker != null && (info.Attacker is SandboxPlayer || info.Attacker.Owner is SandboxPlayer)) )
		{
			SandboxPlayer attacker = info.Attacker as SandboxPlayer;

			if ( attacker == null )
				attacker = info.Attacker.Owner as SandboxPlayer;

			// Note - sending this only to the attacker!
			if ( attacker != this )
				attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, Health.LerpInverse( 100, 0 ), Health <= 0 );
		}

		TookDamage( To.Single( this ), (info.Weapon != null && info.Weapon.IsValid()) ? info.Weapon.Position : (info.Attacker != null && info.Attacker.IsValid()) ? info.Attacker.Position : Position );
	}

	[ClientRpc]
	public void DidDamage( Vector3 pos, float amount, float healthinv, bool isdeath )
	{
		Sound.FromScreen( "dm.ui_attacker" )
			.SetPitch( 1 + healthinv * 1 );

		HitIndicator.Current?.OnHit( pos, amount, isdeath );
	}

	[ClientRpc]
	public void TookDamage( Vector3 pos )
	{
		//DebugOverlay.Sphere( pos, 5.0f, Color.Red, false, 50.0f );

		DamageIndicator.Current?.OnHit( pos );
	}

	public override PawnController GetActiveController()
	{
		if ( DevController != null ) return DevController;

		return base.GetActiveController();
	}

	public bool IsOnGround()
	{
		var tr = Trace.Ray( Position, Position + Vector3.Down * 5 )
				.Radius( 1 )
				.Ignore( this )
				.Run();

		return tr.Hit;
	}

	string oldavatar = ConsoleSystem.GetValue( "avatar" );

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( LifeState != LifeState.Alive )
			return;

		var newavatar = ConsoleSystem.GetValue( "avatar" );

		if ( oldavatar != newavatar )
		{
			oldavatar = newavatar;

			Clothing.LoadFromClient( Client );
			Clothing.DressEntity( this );
		}

		var controller = GetActiveController();
		if ( controller != null )
			EnableSolidCollisions = !controller.HasTag( "noclip" );

		TickPlayerUse();
		SimulateActiveChild( cl, ActiveChild );

		if ( Input.Pressed( InputButton.View ) )
		{
			if ( CameraMode is ThirdPersonCamera )
			{
				CameraMode = new FirstPersonCamera();
			}
			else
			{
				CameraMode = new ThirdPersonCamera();
			}
		}

		if ( Input.Pressed( InputButton.Drop ) )
		{
			var dropped = Inventory.DropActive();
			if ( dropped != null )
			{
				dropped.PhysicsGroup.ApplyImpulse( Velocity + EyeRotation.Forward * 500.0f + Vector3.Up * 100.0f, true );
				dropped.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * 100.0f, true );

				timeSinceDropped = 0;
			}
		}

		if ( Input.Released( InputButton.Jump ) )
		{
			if ( timeSinceJumpReleased < 0.3f )
			{
				Game.Current?.DoPlayerNoclip( cl );
			}

			timeSinceJumpReleased = 0;
		}

		// Why?
		//if ( Input.Left != 0 || Input.Forward != 0 )
		//{
		//	timeSinceJumpReleased = 1;
		//}

		var DownVel = Velocity * Rotation.Down;
		var falldamage = DownVel.z / 50;

		if ( timeSinceFall > 0.02f && DownVel.z > 750 && IsOnGround() && !controller.HasTag( "noclip" ) && HavaFallDamage )
		{
			timeSinceFall = 0;

			var dmg = new DamageInfo()
			{
				Position = Position,
				Damage = falldamage
			};

			PlaySound( "dm.ui_attacker" );
			TakeDamage( dmg );
		}

		CanUseEntityGlow();
	}

	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 1 ) return;

		base.StartTouch( other );
	}

	[ConCmd.Server( "inventory_current" )]
	public static void SetInventoryCurrent( string entName )
	{
		var target = ConsoleSystem.Caller.Pawn as Player;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		for ( int i = 0; i < inventory.Count(); ++i )
		{
			var slot = inventory.GetSlot( i );
			if ( !slot.IsValid() )
				continue;

			if ( !slot.ClassInfo.IsNamed( entName ) )
				continue;

			inventory.SetActiveSlot( i, false );

			break;
		}
	}
}
