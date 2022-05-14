using Sandbox;

[Library( "weapon_357", Title = ".357 Revolver", Spawnable = true )]
[Hammer.EditorModel("models/weapons/w_357.vmdl")]
partial class Python357 : Weapon
{ 
	public override string ViewModelPath => "weapons/hl2_357/v_hl2_357.vmdl";
	public override string WorldModelPath => "weapons/hl2_357/w_hl2_357.vmdl";

	public override int ClipSize => 6;
	public override int Bucket => 1;
	public override float PrimaryRate => 1.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 3.67f;
    public override bool RealReload => false;
	public override bool Automatic => false;
	public override CType Crosshair => CType.Common;
	public override string Icon => "ui/weapons/weapon_357.png";
	public override string ShootSound => "hl2_357.fire";
	public override bool CanDischarge => true;
	public override float Spread => 0f;
	public override float Force => 3f;
	public override float Damage => 40f;
	public override float BulletSize => 3.0f;
	public override ScreenShake ScreenShake => new ScreenShake { };

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 1 );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
		anim.SetAnimParameter( "holdtype_handedness", 0 );
	}
}
