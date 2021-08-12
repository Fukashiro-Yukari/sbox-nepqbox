using Sandbox;

[Library( "weapon_crossbow", Title = "Crossbow", Spawnable = true )]
[Hammer.EditorModel( "weapons/rust_crossbow/rust_crossbow.vmdl" )]
partial class Crossbow : Weapon
{ 
	public override string ViewModelPath => "weapons/rust_crossbow/v_rust_crossbow.vmdl";

	public override int ClipSize => 1;
	public override float PrimaryRate => 1;
	public override int Bucket => 3;
    public override string ReloadSound => "rush_crossbow.reload";

    [Net]
	public bool Zoomed { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_crossbow/rust_crossbow.vmdl" );
	}

	public override void AttackPrimary()
	{
		if ( !BaseAttackPrimary() ) return;

		PlaySound("rush_crossbow.shoot");

		if ( IsServer )
		using ( Prediction.Off() )
		{
			var bolt = new CrossbowBolt();
			bolt.Position = Owner.EyePos;
			bolt.Rotation = Owner.EyeRot;
			bolt.Owner = Owner;
			bolt.Velocity = Owner.EyeRot.Forward * 100;
		}
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Zoomed = Input.Down( InputButton.Attack2 );

        //if (Owner.IsLocalPawn)
			//ViewModelEntity?.SetAnimBool("loaded", true);
		//Log.Info(ViewModelEntity?.GetAnimBool("loaded"));
		//	ViewModelEntity?.SetAnimBool("loaded", false);
	}

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		if ( Zoomed )
		{
			camSetup.FieldOfView = 20;
		}
	}

	public override void BuildInput( InputBuilder owner ) 
	{
		if ( Zoomed )
		{
			owner.ViewAngles = Angles.Lerp( owner.OriginalViewAngles, owner.ViewAngles, 0.2f );
		}
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin( 0.5f, 4.0f, 1.0f, 0.5f );
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 3 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}

	//[Event.Tick]
	//void Tick()
	//{
		//Log.Info((IsServer ? "Server " : "Client ") + (AmmoClip >= 1));

		//ViewModelEntity?.SetAnimBool( "loaded", false );
	//}
}
