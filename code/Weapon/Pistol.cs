using Sandbox;

[Spawnable]
[Library( "weapon_pistol", Title = "Pistol" )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
partial class Pistol : Weapon
{ 
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
	public override string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";

	public override int ClipSize => 18;
	public override int Bucket => 1;
	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 3.0f;
	public override bool Automatic => false;
	public override CType Crosshair => CType.Pistol;
	public override string Icon => "ui/weapons/weapon_pistol.png";
	public override string ShootSound => "rust_pistol.shoot";
	public override bool CanDischarge => true;
	public override float Spread => 0.05f;
	public override float Force => 1.5f;
	public override float Damage => 9.0f;
	public override float BulletSize => 3.0f;
	public override ScreenShake ScreenShake => new ScreenShake { };

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 1 );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
		anim.SetAnimParameter( "holdtype_handedness", 0 );
	}

	public override void NPCAnimator( NPC npc )
	{
		npc.SetAnimParameter( "holdtype", 1 );
		npc.SetAnimParameter( "aim_body_weight", 1.0f );
		npc.SetAnimParameter( "holdtype_handedness", 0 );
	}
}
