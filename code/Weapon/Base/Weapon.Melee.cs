using Sandbox;
using System.Linq;

public partial class WeaponMelee : Weapon
{
	public override int ClipSize => -1;
	public override int AmmoMultiplier => -1;
	public virtual float PrimaryDamage => 25f;
	public virtual float PrimaryForce => 100;
	public virtual float SecondaryDamage => 25f;
	public virtual float SecondaryForce => 100;
	public virtual float PrimarySpeed => 1f;
	public virtual float SecondarySpeed => 1f;
	public virtual float PrimaryMeleeDistance => 100f;
	public virtual float SecondaryMeleeDistance => 80f;
	public virtual float PrimaryBackDamage => 35f;
	public virtual float SecondaryBackDamage => 150f;
	public virtual float ImpactSize => 10f;
	public virtual string PrimaryAnimationHit => "";
	public virtual string PrimaryAnimationMiss => "";
	public virtual string SecondaryAnimationHit => "";
	public virtual string SecondaryAnimationMiss => "";
	public virtual string PrimaryAttackSound => "";
	public virtual string SecondaryAttackSound => "";
	public virtual string HitWorldSound => "";
	public virtual string MissSound => "";
	public virtual string BackAttackSound => "";
	public virtual bool CanUseSecondary => false;
	public virtual ScreenShake PrimaryScreenShakeHit => null;
	public virtual ScreenShake PrimaryScreenShakeMiss => null;
	public virtual ScreenShake SecondaryScreenShakeHit => null;
	public virtual ScreenShake SecondaryScreenShakeMiss => null;

	[Net, Predicted]
	public bool IsBack { get; set; }

	public virtual bool MeleeAttack( float damage, float force, string swingSound, string animationHit, string animationMiss, ScreenShake screenShakeHit, ScreenShake screenShakeMiss, float meleeDistance, float backDamage, bool leftHand )
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity).SetAnimParameter( "b_attack", true );

		var forward = Owner.EyeRotation.Forward;
		forward = forward.Normal;

		bool hit = false;

		// I think the method is broken, if you melee miss, the method will not output anything.
		var bullets = TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * meleeDistance, ImpactSize );

		//Log.Info( "try" );
		//Log.Info( bullets.Count() );

		foreach ( var tr in bullets )
		{
			//Log.Info( "test" );
			//Log.Info( tr.Entity );

			if ( !tr.Entity.IsValid() )
			{
				PlaySound( MissSound );

				if ( screenShakeMiss == null )
				{
					screenShakeMiss = new ScreenShake
					{
						Length = -1f,
						Delay = -1f,
						Size = -1f,
						Rotation = -1f
					};
				}

				//Log.Info( "test" );

				OnMeleeMiss( screenShakeMiss.Length, screenShakeMiss.Delay, screenShakeMiss.Size, screenShakeMiss.Rotation, animationMiss, leftHand );

				continue;
			}

			if ( IsServer )
				tr.Surface.DoBulletImpact( tr );

			hit = true;
			var isFlesh = tr.Entity is Player || tr.Entity is NPC;
			var newRot = Rotation.From( 0, Rotation.Angles().yaw, 0 );
			var trenewRot = Rotation.From( 0, tr.Entity.Rotation.Angles().yaw, 0 );
			var dif = newRot.Distance( trenewRot );

			IsBack = isFlesh && dif >= 100 && dif <= 145 && !string.IsNullOrEmpty( BackAttackSound );

			if ( IsBack )
			{
				PlaySound( BackAttackSound );
			}
			else
			{
				PlaySound( isFlesh ? swingSound : HitWorldSound );
			}

			if ( screenShakeHit == null )
			{
				screenShakeHit = new ScreenShake
				{
					Length = -1f,
					Delay = -1f,
					Size = -1f,
					Rotation = -1f
				};
			}

			OnMeleeHit( screenShakeHit.Length, screenShakeHit.Delay, screenShakeHit.Size, screenShakeHit.Rotation, animationHit, leftHand );

			if ( !IsServer ) continue;

			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * force, IsBack ? backDamage : damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}

		return hit;
	}

	public virtual bool CanMelee( TimeSince timeSinceAttack, float attackSpeed, InputButton attack )
	{
		if ( !Owner.IsValid() || !Input.Down( attack ) ) return false;

		var speed = attackSpeed;
		if ( speed <= 0 ) return true;

		if ( !Automatic && !Input.Pressed( attack ) ) return false;

		return timeSinceAttack > attackSpeed;
	}

	public override bool CanPrimaryAttack()
	{
		return CanMelee( TimeSincePrimaryAttack, PrimarySpeed, InputButton.Attack1 );
	}

	public override bool CanSecondaryAttack()
	{
		if ( !CanUseSecondary ) return false;

		return CanMelee( TimeSinceSecondaryAttack, SecondarySpeed, InputButton.Attack2 );
	}

	public override bool CanReload()
	{
		return false;
	}

	public override void AttackPrimary()
	{
		_ = MeleeAttack( PrimaryDamage, PrimaryForce, PrimaryAttackSound, PrimaryAnimationHit, PrimaryAnimationMiss, PrimaryScreenShakeHit, PrimaryScreenShakeMiss, PrimaryMeleeDistance, PrimaryBackDamage, true );
	}

	public override void AttackSecondary()
	{
		_ = MeleeAttack( SecondaryDamage, SecondaryForce, SecondaryAttackSound, SecondaryAnimationHit, SecondaryAnimationMiss, SecondaryScreenShakeHit, SecondaryScreenShakeMiss, SecondaryMeleeDistance, SecondaryBackDamage, false );
	}

	[ClientRpc]
	public virtual void OnMeleeMiss( float length, float delay, float size, float rotation, string animation, bool leftHand )
	{
		Host.AssertClient();

		if ( IsLocalPawn )
		{
			if ( length != -1 )
				ScreenUtil.Shake( length, delay, size, rotation );
		}

		ViewModelEntity?.SetAnimParameter( animation, true );
	}

	[ClientRpc]
	public virtual void OnMeleeHit( float length, float delay, float size, float rotation, string animation, bool leftHand )
	{
		Host.AssertClient();

		if ( IsLocalPawn )
		{
			if ( length != -1 )
				ScreenUtil.Shake( length, delay, size, rotation );
		}

		ViewModelEntity?.SetAnimParameter( animation, true );
		CrosshairPanel?.CreateEvent( "fire" );
	}
}
