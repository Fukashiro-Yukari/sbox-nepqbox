using Sandbox;

[Library( "weapon_fists", Title = "Fists", Spawnable = false )]
partial class Fists : WeaponMelee
{
	public override string ViewModelPath => "models/first_person/first_person_arms.vmdl";

	public override int Bucket => 0;
	public override float PrimarySpeed => 0.5f;
	public override float SecondarySpeed => 0.5f;
	public override float PrimaryDamage => 25f;
	public override float PrimaryForce => 100f;
	public override float SecondaryDamage => 25f;
	public override float SecondaryForce => 100f;
	public override float PrimaryMeleeDistance => 80f;
	public override float SecondaryMeleeDistance => 80f;
	public override float ImpactSize => 20f;
	public override CType Crosshair => CType.None;
	public override string Icon => "ui/weapons/weapon_fists.png";
	public override string PrimaryAnimationHit => "attack";
	public override string PrimaryAnimationMiss => "attack";
	public override string SecondaryAnimationHit => "attack";
	public override string SecondaryAnimationMiss => "attack";
	public override bool CanUseSecondary => true;

	public override void OnCarryDrop( Entity dropper )
	{
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 5 );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );

		if ( Owner.IsValid() )
		{
			ViewModelEntity?.SetAnimParameter( "b_grounded", Owner.GroundEntity.IsValid() );
			ViewModelEntity?.SetAnimParameter( "aim_pitch", Owner.EyeRotation.Pitch() );
		}
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		ViewModelEntity = new ViewModel
		{
			Position = Position,
			Owner = Owner,
			EnableViewmodelRendering = true,
			EnableSwingAndBob = false,
		};

		ViewModelEntity.SetModel( ViewModelPath );
		ViewModelEntity.SetAnimGraph( "models/first_person/first_person_arms_punching.vanmgrph" );
	}

	[ClientRpc]
	public override void OnMeleeMiss( float length, float speed, float size, float rotation, string animation, bool leftHand )
	{
		ViewModelEntity?.SetAnimParameter( "attack_has_hit", false );

		base.OnMeleeMiss( length, speed, size, rotation, animation, leftHand );

		ViewModelEntity?.SetAnimParameter( "holdtype_attack", leftHand ? 2 : 1 );
	}

	[ClientRpc]
	public override void OnMeleeHit( float length, float speed, float size, float rotation, string animation, bool leftHand )
	{
		ViewModelEntity?.SetAnimParameter( "attack_has_hit", true );

		base.OnMeleeHit( length, speed, size, rotation, animation, leftHand );

		ViewModelEntity?.SetAnimParameter( "holdtype_attack", leftHand ? 2 : 1 );
	}
}
