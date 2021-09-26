using Sandbox;
using System;

[Library( "weapon_knife", Title = "Knife", Spawnable = true )]
[Hammer.EditorModel( "weapons/css_knife/css_w_knife.vmdl" )]
partial class Knife : WeaponMelee
{
	public override string ViewModelPath => "weapons/css_knife/css_v_knife.vmdl";
	public override string WorldModelPath => "weapons/css_knife/css_w_knife.vmdl";

	public override int Bucket => 2;
	public override float PrimarySpeed => 0.6f;
	public override float SecondarySpeed => 1.29f;
	public override float PrimaryDamage => 35f;
	public override float PrimaryForce => 25f;
	public override float SecondaryDamage => 65f;
	public override float SecondaryForce => 50f;
	public override float PrimaryMeleeDistance => 55f;
	public override float SecondaryMeleeDistance => 35f;
	public override float PrimaryBackDamage => 35f;
	public override float SecondaryBackDamage => 150f;
	public override float ImpactSize => 10f;
	public override float FOV => 75;
	public override CType Crosshair => CType.None;
	public override string Icon => "ui/weapons/weapon_knife.png";
	public override string PrimaryAnimationHit => "swing";
	public override string PrimaryAnimationMiss => "swing_miss";
	public override string SecondaryAnimationHit => "stab";
	public override string SecondaryAnimationMiss => "stab_miss";
	public override string PrimaryAttackSound => "css_knife.hit";
	public override string SecondaryAttackSound => "css_knife.hit";
	public override string HitWorldSound => "css_knife.hitwall";
	public override string MissSound => "css_knife.slash";
	public override string BackAttackSound => "css_knife.stab";
	public override bool CanUseSecondary => true;

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 4 ); // TODO this is shit
		anim.SetParam( "holdtype_attack", 2.0f );
		anim.SetParam( "holdtype_handedness", 1 );
		anim.SetParam( "holdtype_pose", 0f );
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
