using Sandbox;

[Spawnable]
[Library( "weapon_crowbarsbox", Title = "Crowbar S&box Style" )]
[EditorModel( "models/dm_crowbar.vmdl" )]
partial class CrowBarSbox : WeaponMelee
{
	public override string ViewModelPath => "models/v_dm_crowbar.vmdl";
	public override string WorldModelPath => "models/dm_crowbar.vmdl";

	public override int Bucket => 0;
	public override float PrimarySpeed => 0.5f;
	public override float PrimaryDamage => 25f;
	public override float PrimaryForce => 32f;
	public override float PrimaryMeleeDistance => 70f;
	public override float ImpactSize => 15f;
	public override CType Crosshair => CType.None;
	public override string Icon => "ui/weapons/weapon_crowbar.png";
	public override string PrimaryAnimationHit => "attack";
	public override string PrimaryAnimationMiss => "attack";
	public override string PrimaryAttackSound => "dm.crowbar_attack";
	public override string HitWorldSound => "dm.crowbar_attack";
	public override string MissSound => "dm.crowbar_attack";

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 5 );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );

		if ( Owner.IsValid() && ViewModelEntity.IsValid() )
		{
			ViewModelEntity.SetAnimParameter( "b_grounded", Owner.GroundEntity.IsValid() );
			ViewModelEntity.SetAnimParameter( "aim_pitch", Owner.EyeRotation.Pitch() );
		}
	}

	[ClientRpc]
	public override void OnMeleeMiss( float length, float speed, float size, float rotation, string animation, bool leftHand )
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimParameter( "attack_has_hit", false );

		base.OnMeleeMiss( length, speed, size, rotation, animation, leftHand );

		ViewModelEntity?.SetAnimParameter( "holdtype_attack", 1 );
	}

	[ClientRpc]
	public override void OnMeleeHit( float length, float speed, float size, float rotation, string animation, bool leftHand )
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimParameter( "attack_has_hit", true );

		base.OnMeleeHit( length, speed, size, rotation, animation, leftHand );

		ViewModelEntity?.SetAnimParameter( "holdtype_attack", 1 );
	}
}
