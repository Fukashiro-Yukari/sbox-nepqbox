using Sandbox;

[Library( "weapon_hl2_pistol", Title = "HL2 Pistol", Spawnable = true )]
[Hammer.EditorModel( "weapons/hl2_pistol/w_hl2_pistol.vmdl" )]
partial class HL2Pistol : Weapon
{ 
	public override string ViewModelPath => "weapons/hl2_pistol/v_hl2_pistol.vmdl";
	public override string WorldModelPath => "weapons/hl2_pistol/w_hl2_pistol.vmdl";

	public override int ClipSize => 18;
	public override int Bucket => 1;
	public override float PrimaryRate => 20.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 1.43f;
	public override bool Automatic => false;
	public override CType Crosshair => CType.Pistol;
	public override string Icon => "ui/weapons/weapon_hl2_pistol.png";
	public override string ShootSound => "hl2_pistol.fire";
	public override string ReloadSound => "hl2_pistol.reload";
	public override bool CanDischarge => true;
	public override float Spread => 0.05f;
	public override float Force => 1.5f;
	public override float Damage => 9.0f;
	public override float BulletSize => 3.0f;
	public override ScreenShake ScreenShake => new ScreenShake { };

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 1 );
		anim.SetParam( "aimat_weight", 1.0f );
		anim.SetParam( "holdtype_handedness", 1 );
	}
}
