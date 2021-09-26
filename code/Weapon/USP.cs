using Sandbox;

[Library( "weapon_usp", Title = "USP", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_usp/w_css_pist_usp.vmdl" )]
partial class USP : Weapon
{
	public override string ViewModelPath => "weapons/css_usp/v_css_pist_usp.vmdl";
	public override string WorldModelPath => "weapons/css_usp/w_css_pist_usp.vmdl";
	public override string SilencerWorldModelPath => "weapons/css_usp/w_css_pist_usp_silencer.vmdl";

	public override int ClipSize => 12;
	public override int Bucket => 1;
	public override int AmmoMultiplier => 5;
	public override bool Automatic => false;
	public override bool UseSilencer => true;
	public override float PrimaryRate => 10f;
	public override float ReloadTime => 2.68f;
	public override CType Crosshair => CType.Pistol;
	public override string Icon => "ui/weapons/weapon_usp.png";
	public override string ShootSound => "css_usp.fire";
	public override string SilencerShootSound => "css_usp.fire.silenced";
	public override float Spread => 0.15f;
	public override float Force => 5f;
	public override float Damage => 35f;
	public override float BulletSize => 4f;
	public override float OnSilencerDuration => 3f;
	public override float OffSilencerDuration => 3f;
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
		anim.SetParam( "holdtype", 1 ); // TODO this is shit
		anim.SetParam( "holdtype_handedness", 0 );
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
