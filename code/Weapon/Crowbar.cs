using Sandbox;

[Library( "weapon_crowbar", Title = "Crowbar", Spawnable = true )]
[Hammer.EditorModel( "weapons/hl2_crowbar/w_hl2_crowbar.vmdl" )]
partial class Crowbar : WeaponMelee
{
	public override string ViewModelPath => "weapons/hl2_crowbar/v_hl2_crowbar.vmdl";
	public override string WorldModelPath => "weapons/hl2_crowbar/w_hl2_crowbar.vmdl";

	public override int Bucket => 0;
	public override float PrimaryDamage => 25f;
	public override float PrimarySpeed => 0.4f;
	public override float PrimaryMeleeDistance => 80f;
	public override float ImpactSize => 5f;
	public override CType Crosshair => CType.None;
	public override string Icon => "ui/weapons/weapon_crowbar.png";
	public override string PrimaryAnimationHit => "attack_hit";
	public override string PrimaryAnimationMiss => "attack";
	public override string PrimaryAttackSound => "hl2_crowbar.hit";
	public override string HitWorldSound => "hl2_crowbar.hitworld";
	public override string MissSound => "hl2_crowbar.swing";
	public override ScreenShake PrimaryScreenShakeHit => new ScreenShake
	{
		Length = 1.0f,
		Speed = 1.0f,
		Size = 3.0f,
	};
	public override ScreenShake PrimaryScreenShakeMiss => new ScreenShake
	{
		Length = 1.0f,
		Speed = 1.0f,
		Size = 3.0f,
	};

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 4); // TODO this is shit
		anim.SetParam("holdtype_attack", 2.0f);
		anim.SetParam("holdtype_handedness", 1);
		anim.SetParam("holdtype_pose", 0f);
		anim.SetParam("aimat_weight", 1.0f);
	}
}
