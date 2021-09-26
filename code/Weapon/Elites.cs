using Sandbox;

[Library( "weapon_elite", Title = "Dual Elites", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_elite/w_css_pist_elite_dropped.vmdl" )]
partial class Elites : WeaponDouble
{
	public override string ViewModelPath => "weapons/css_elite/v_css_pist_elite.vmdl";
	public override string WorldModelPath => "weapons/css_elite/w_css_pist_elite.vmdl";
	public override string DroppedModelPath => "weapons/css_elite/w_css_pist_elite_dropped.vmdl";

	public override int ClipSize => 30;
	public override int Bucket => 1;
	public override int AmmoMultiplier => 5;
	public override bool Automatic => false;
	public override float PrimaryRate => 10f;
	public override float ReloadTime => 3.76f;
	public override CType Crosshair => CType.Pistol;
	public override string Icon => "ui/weapons/weapon_elite.png";
	public override string ShootSound => "css_elite.fire";
	public override float Spread => 0.3f;
	public override float Force => 5f;
	public override float Damage => 35f;
	public override float BulletSize => 3f;
	public override float FOV => 75;
	public override ScreenShake ScreenShake => new ScreenShake
	{
		Length = 0.5f,
		Speed = 4.0f,
		Size = 1.0f,
		Rotation = 0.5f
	};

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 4 ); // TODO this is shit
		anim.SetParam( "holdtype_pose", 1f );
		anim.SetParam( "holdtype_pose_hand", 0f );
		anim.SetParam( "holdtype_handedness", 0 );
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
