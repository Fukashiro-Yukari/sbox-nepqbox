using Sandbox;

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
	public virtual float MeleeDistance => 90f;
	public virtual float ImpactSize => 10f;
	public virtual string PrimaryAnimationHit => "";
	public virtual string PrimaryAnimationMiss => "";
	public virtual string SecondaryAnimationHit => "";
	public virtual string SecondaryAnimationMiss => "";
	public virtual string PrimaryAttackSound => "";
	public virtual string SecondaryAttackSound => "";
	public virtual string HitWorldSound => "";
	public virtual string MissSound => "";
	public virtual bool CanUseSecondary => false;
	public virtual ScreenShake PrimaryScreenShakeHit => null;
	public virtual ScreenShake PrimaryScreenShakeMiss => null;
	public virtual ScreenShake SecondaryScreenShakeHit => null;
	public virtual ScreenShake SecondaryScreenShakeMiss => null;

	bool isFlesh;

	public virtual bool MeleeAttack( float damage, float force, string swingSound, string animationHit, string animationMiss, ScreenShake screenShakeHit, ScreenShake screenShakeMiss )
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity).SetAnimBool( "b_attack", true );

		var forward = Owner.EyeRot.Forward;
		forward = forward.Normal;

		bool hit = false;

		foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * MeleeDistance, ImpactSize ) )
		{
			if ( !tr.Entity.IsValid() )
			{
				PlaySound( MissSound );

				if ( screenShakeMiss == null )
				{
					screenShakeMiss = new ScreenShake
					{
						Length = -1f,
						Speed = -1f,
						Size = -1f,
						Rotation = -1f
					};
				}

				OnMeleeMiss( screenShakeMiss.Length, screenShakeMiss.Speed, screenShakeMiss.Size, screenShakeMiss.Rotation, animationMiss );

				continue;
			}

			tr.Surface.DoBulletImpact( tr );

			hit = true;
			isFlesh = tr.Entity is Player || tr.Entity is Npc;

			PlaySound( isFlesh ? swingSound : HitWorldSound );

			if ( screenShakeHit == null )
			{
				screenShakeHit = new ScreenShake
				{
					Length = -1f,
					Speed = -1f,
					Size = -1f,
					Rotation = -1f
				};
			}

			OnMeleeHit( screenShakeHit.Length, screenShakeHit.Speed, screenShakeHit.Size, screenShakeHit.Rotation, animationHit );

			if ( !IsServer ) continue;

			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * force, damage )
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
		_ = MeleeAttack( PrimaryDamage, PrimaryForce, PrimaryAttackSound, PrimaryAnimationHit, PrimaryAnimationMiss, PrimaryScreenShakeHit, PrimaryScreenShakeMiss );
	}

	public override void AttackSecondary()
	{
		_ = MeleeAttack( SecondaryDamage, SecondaryForce, SecondaryAttackSound, SecondaryAnimationHit, SecondaryAnimationMiss, SecondaryScreenShakeHit, SecondaryScreenShakeMiss );
	}

	[ClientRpc]
	public virtual void OnMeleeMiss( float length, float speed, float size, float rotation, string animation )
	{
		Host.AssertClient();

		if ( IsLocalPawn )
		{
			if ( length != -1 )
				_ = new Sandbox.ScreenShake.Perlin( length, speed, size, rotation );
		}

		ViewModelEntity?.SetAnimBool( animation, true );
	}

	[ClientRpc]
	public virtual void OnMeleeHit( float length, float speed, float size, float rotation, string animation )
	{
		Host.AssertClient();

		if ( IsLocalPawn )
		{
			if ( length != -1 )
				_ = new Sandbox.ScreenShake.Perlin( length, speed, size, rotation );
		}

		ViewModelEntity?.SetAnimBool( animation, true );
		CrosshairPanel?.CreateEvent( "fire" );
	}
}
