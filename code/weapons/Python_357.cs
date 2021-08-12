using Sandbox;


[Library( "weapon_357", Title = ".357 Revolver", Spawnable = true )]
[Hammer.EditorModel("models/weapons/w_357.vmdl")]
partial class Python357 : Weapon
{ 
	public override string ViewModelPath => "models/weapons/v_357.vmdl";

	public override int ClipSize => 6;
	public override float PrimaryRate => 1.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 3.6f;
    public override string ReloadSound => "weapon_357.reload";
    public override bool RealReload => false;
    public override CType Crosshair => CType.Common;

	public TimeSince TimeSinceDischarge { get; set; }

	public override int Bucket => 1;

	public override void Spawn()
	{
		base.Spawn();

		SetModel("models/weapons/w_357.vmdl");
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{
		if ( !BaseAttackPrimary() ) return;

		PlaySound("weapon_357.fire");

		//
		// Shoot the bullets
		//
		ShootBullet( 0f, 3f, 40f, 3.0f );
	}

	private void Discharge()
	{
		if ( TimeSinceDischarge < 0.5f )
			return;

		TimeSinceDischarge = 0;

		var muzzle = GetAttachment( "muzzle" ) ?? default;
		var pos = muzzle.Position;
		var rot = muzzle.Rotation;

		TakeAmmo( 1 );
		ShootEffects();
		PlaySound("weapon_357.fire");
		ShootBullet( pos, rot.Forward, 0f, 3f, 40f, 3.0f);

		ApplyAbsoluteImpulse( rot.Backward * 200.0f );
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		if ( eventData.Speed > 500.0f )
		{
			if (AmmoClip > 0)
				Discharge();
		}
	}
}
