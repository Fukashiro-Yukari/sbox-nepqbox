using Sandbox;

[Library( "weapon_mac10", Title = "MAC-10", Spawnable = true )]
[Hammer.EditorModel( "weapons/swb/css/mac10/css_w_smg_mac10.vmdl" )]
partial class MAC10 : Weapon
{
	public override string ViewModelPath => "weapons/swb/css/mac10/css_v_smg_mac10.vmdl";
	public override string WorldModelPath => "weapons/swb/css/mac10/css_w_smg_mac10.vmdl";

	public override int ClipSize => 32;
	public override int Bucket => 0;
	public override float PrimaryRate => 11f;
	public override float ReloadTime => 3.14f;
	public override CType Crosshair => CType.SMG;
	public override string Icon => "ui/weapons/weapon_mac10.png";
	public override string ShootSound => "css_mac10.fire";
	public override float Spread => 0.2f;
	public override float Force => 3f;
	public override float Damage => 12f;
	public override float BulletSize => 2f;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Speed = 4.0f,
		Size = 0.5f,
		Rotation = 0.5f
	};

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 1 ); // TODO this is shit
		anim.SetParam( "holdtype_handedness", 0 );
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
