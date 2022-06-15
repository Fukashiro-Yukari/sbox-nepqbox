using Sandbox;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Weapon : Carriable, IUse
{
	public virtual string SilencerWorldModelPath => "";
	public virtual string DroppedModelPath => "";
	public virtual string ReloadSound => "";
	public virtual string ShootSound => "";
	public virtual string SilencerShootSound => "";
	public virtual string BulletEjectParticle => "particles/pistol_ejectbrass.vpcf";
	public virtual string MuzzleFlashParticle => "particles/pistol_muzzleflash.vpcf";
	public virtual string BurstAnimation => null;
	public virtual int ClipSize => 16;
	public virtual int ClipTake => 1;
	public virtual int AmmoMultiplier => 5;
	public virtual int NumBursts => 3;
	public virtual int NumBullets => 1;
	public virtual bool RealReload => true;
	public virtual bool Automatic => true;
	public virtual bool UseSilencer => false;
	public virtual bool UseBursts => false;
	public virtual bool CanDischarge => false;
	public virtual bool UnlimitedAmmo => true;
	public virtual CType Crosshair => CType.Common;
	public virtual float PrimaryRate => 5.0f;
	public virtual float SecondaryRate => 15.0f;
	public virtual float ReloadTime => 3.0f;
	public virtual float Spread => 0f;
	public virtual float Force => 0f;
	public virtual float Damage => 0f;
	public virtual float BulletSize => 3f;
	public virtual float OnSilencerDuration => 2f;
	public virtual float OffSilencerDuration => 2f;
	public virtual float DischargeRecoil => 200f;
	public virtual float BurstsRate => 20f;
	public virtual float FOV => 65;
	public virtual ScreenShake ScreenShake => null;
	public virtual Func<Vector3, Vector3, Vector3, float, float, float, Entity> CreateEntity => null;

	[Net, Predicted]
	public int AmmoClip { get; set; }

	[Net, Predicted]
	public int AmmoCount { get; set; }

	public PickupTrigger PickupTrigger { get; protected set; }

	[Net, Predicted]
	public TimeSince TimeSincePrimaryAttack { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceSecondaryAttack { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceReload { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; set; }

	[Net]
	public TimeSince TimeSinceBurstsAttack { get; set; }

	[Net, Predicted]
	public bool IsReloading { get; set; }

	[Net]
	public bool Silencer { get; set; }

	public bool DoBursts { get; set; }
	public bool BurstsMode { get; set; }

	private bool SilencerDelay;
	private int BurstIndex { get; set; }

	public TimeSince TimeSinceDischarge { get; set; }

	protected TimeSince CrosshairLastShoot { get; set; }
	protected TimeSince CrosshairLastReload { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
		SetInteractsAs( CollisionLayer.Debris ); // so player movement doesn't walk into it

		PickupTrigger = new PickupTrigger
		{
			Parent = this,
			Position = Position,
			EnableTouch = true,
			EnableSelfCollisions = false
		};

		PickupTrigger.PhysicsBody.AutoSleep = false;
		AmmoClip = ClipSize;
		
		if ( ClipSize <= -1 )
		{
			if ( UnlimitedAmmo )
				AmmoCount = AmmoClip;
			else
				AmmoCount = AmmoMultiplier;
		}
		else
		{
			if ( UnlimitedAmmo )
				AmmoCount = AmmoClip;
			else
				AmmoCount = AmmoClip * AmmoMultiplier;
		}

		if ( !string.IsNullOrEmpty( DroppedModelPath ) )
			SetModel( DroppedModelPath );
		else if ( !string.IsNullOrEmpty( WorldModelPath ) )
			SetModel( WorldModelPath );
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		if ( !string.IsNullOrEmpty( DroppedModelPath ) && !string.IsNullOrEmpty( WorldModelPath ) )
			SetModel( WorldModelPath );

		TimeSinceDeployed = 0;
		IsReloading = false;
		DoBursts = false;

		SetSilencedEffects( Silencer );
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		if ( dropped && IsValid )
		{
			var oldVelocity = Velocity;

			if ( !string.IsNullOrEmpty( DroppedModelPath ) )
				SetModel( DroppedModelPath );

			if ( IsServer )
				PhysicsGroup?.ApplyImpulse( oldVelocity, true );
		}

		if ( SilencerDelay )
			SilencerDelay = false;

		DoBursts = false;
	}

	public virtual void Reload()
	{
		if ( IsReloading || ClipSize < 0 )
			return;

		if ( AmmoClip >= (RealReload ? ClipSize + 1 : ClipSize) && ClipSize > -1 )
			return;

		if ( AmmoCount <= 0 ) return;

		TimeSinceReload = 0;
		IsReloading = true;

		if ( SilencerDelay )
			SilencerDelay = false;

		DoBursts = false;

		(Owner as AnimatedEntity)?.SetAnimParameter( "b_reload", true );

		if ( !string.IsNullOrEmpty( ReloadSound ) )
			PlaySound( ReloadSound );

		StartReloadEffects();
		EmptyEffects( false );
	}

	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed < 0.6f )
			return;

		if ( !IsReloading )
		{
			if ( CanReload() )
			{
				Reload();
			}

			//
			// Reload could have changed our owner
			//
			if ( !Owner.IsValid() )
				return;

			if ( CanPrimaryAttack() )
			{
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}

			//
			// AttackPrimary could have changed our owner
			//
			if ( !Owner.IsValid() )
				return;

			if ( CanSecondaryAttack() )
			{
				TimeSinceSecondaryAttack = 0;
				AttackSecondary();
			}
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}

		if ( UseSilencer && !IsReloading && Input.Pressed( InputButton.SecondaryAttack ) )
		{
			if ( SilencerDelay ) return;

			SilencerDelay = true;

			ToggleSilencerEffects( !Silencer );

			_ = SilencerEquip();
		}

		if ( UseBursts && Input.Pressed( InputButton.SecondaryAttack ) )
		{
			BurstsMode = !BurstsMode;

			if ( BurstsMode )
				Hint.Add( To.Single( Owner ), "Switched to burst-fire mode" );
			else
			{
				if ( Automatic )
					Hint.Add( To.Single( Owner ), "Switched to automatic" );
				else
					Hint.Add( To.Single( Owner ), "Switched to semi-automatic" );
			}
		}

		if ( DoBursts && CanBurstsAttack() )
		{
			BurstIndex++;
			TimeSinceBurstsAttack = 0;

			using ( Prediction.Off() )
			{
				DoAttack();
			}

			if ( BurstIndex >= NumBursts )
			{
				BurstIndex = 0;
				DoBursts = false;
			}
		}
	}

	async Task SilencerEquip()
	{
		await GameTask.DelaySeconds( !Silencer ? OnSilencerDuration : OffSilencerDuration );

		if ( !SilencerDelay ) return;

		Silencer = !Silencer;
		SilencerDelay = false;

		SetSilencedEffects( Silencer );

		if ( Silencer )
		{
			if ( !string.IsNullOrEmpty( SilencerWorldModelPath ) )
				SetModel( SilencerWorldModelPath );
		}
		else
		{
			if ( !string.IsNullOrEmpty( WorldModelPath ) )
				SetModel( WorldModelPath );
		}
	}

	public virtual bool CanReload()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.Reload ) ) return false;

		return true;
	}

	public virtual bool CanPrimaryAttack()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.PrimaryAttack ) ) return false;
		if ( !Automatic && !Input.Pressed( InputButton.PrimaryAttack ) ) return false;

		var rate = PrimaryRate;
		if ( rate <= 0 ) return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	public virtual bool CanNPCPrimaryAttack()
	{
		var rate = PrimaryRate;

		if ( !Automatic )
			rate /= 3;

		if ( rate <= 0 ) return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	public virtual bool CanSecondaryAttack()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.SecondaryAttack ) ) return false;

		var rate = SecondaryRate;
		if ( rate <= 0 ) return true;

		return TimeSinceSecondaryAttack > (1 / rate);
	}

	public virtual bool CanBurstsAttack()
	{
		if ( !Owner.IsValid() || !BurstsMode ) return false;

		var rate = BurstsRate;
		if ( rate <= 0 ) return true;

		return TimeSinceBurstsAttack > (1 / rate);
	}

	public void DoAttack()
	{
		if ( !TakeAmmo( ClipTake ) )
		{
			DryFire();

			return;
		}

		(Owner as AnimatedEntity).SetAnimParameter( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();

		if ( Silencer )
		{
			if ( !string.IsNullOrEmpty( SilencerShootSound ) )
				PlaySound( SilencerShootSound );
		}
		else
		{
			if ( !string.IsNullOrEmpty( ShootSound ) )
				PlaySound( ShootSound );
		}

		//
		// Shoot the bullets
		//
		if ( NumBullets > 1 )
			ShootBullets( NumBullets, Spread, Force, Damage, BulletSize );
		else
			ShootBullet( Spread, Force, Damage, BulletSize );

		EmptyEffects( true );
	}

	public void DoNPCAttack()
	{
		if ( !TakeAmmo( ClipTake ) )
		{
			Reload();

			return;
		}

		(Owner as AnimatedEntity).SetAnimParameter( "b_attack", true );

		NPCShootEffects();

		if ( Silencer )
		{
			if ( !string.IsNullOrEmpty( SilencerShootSound ) )
				PlaySound( SilencerShootSound );
		}
		else
		{
			if ( !string.IsNullOrEmpty( ShootSound ) )
				PlaySound( ShootSound );
		}

		//
		// Shoot the bullets
		//
		if ( NumBullets > 1 )
			ShootBullets( NumBullets, Spread, Force, Damage, BulletSize );
		else
			ShootBullet( Spread, Force, Damage, BulletSize );
	}

	public virtual void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( SilencerDelay ) return;
		if ( BurstsMode )
		{
			AttackBursts();

			return;
		}

		DoAttack();
	}

	public virtual void NPCAttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		DoAttack();
	}

	public virtual void AttackBursts()
	{
		if ( !CanBurstsAttack() ) return;

		TimeSincePrimaryAttack = -0.5f;
		TimeSinceSecondaryAttack = -0.5f;
		BurstIndex = 0;
		DoBursts = true;
	}

	public virtual void AttackSecondary()
	{
	}

	public virtual void OnReloadFinish()
	{
		IsReloading = false;

		if ( AmmoClip <= 0 || !RealReload )
		{
			var ammo = TakeAmmo2( ClipSize - AmmoClip );
			if ( ammo == 0 )
				return;

			AmmoClip += ammo;
		}
		else
		{
			var ammo = TakeAmmo2( ClipSize + 1 - AmmoClip );
			if ( ammo == 0 )
				return;

			AmmoClip += ammo;
		}
	}

	[ClientRpc]
	public virtual void StartReloadEffects()
	{
		ViewModelEntity?.SetAnimParameter( "reload", true );
	}

	[ClientRpc]
	public virtual void ToggleSilencerEffects( bool on )
	{
		ViewModelEntity?.SetAnimParameter( on ? "add_silencer" : "remove_silencer", true );
	}

	[ClientRpc]
	public virtual void SetSilencedEffects( bool on )
	{
		ViewModelEntity?.SetAnimParameter( "silenced", on );
	}

	public bool TakeAmmo( int amount )
	{
		if ( ClipSize < 0 ) return true;
		if ( AmmoClip < amount )
			return false;

		AmmoClip -= amount;
		return true;
	}

	public int TakeAmmo2(int amount)
    {
		if ( UnlimitedAmmo ) return amount;

		var available = AmmoCount;
		amount = Math.Min(available, amount);

		AmmoCount = available - amount;
		return amount;
	}

	[ClientRpc]
	public virtual void DryFire()
	{
		PlaySound( "dm.dryfire" );
	}

	public override bool OnUse( Entity user )
	{
		if ( Owner != null )
			return false;

		if ( !user.IsValid() )
			return false;

		user.StartTouch( this );

		return false;
	}

	public override bool IsUsable( Entity user )
	{
		var player = user as Player;
		if ( Owner != null ) return false;

		if ( player.Inventory is Inventory inventory )
		{
			return inventory.CanAdd( this );
		}

		return true;
	}

	public void Remove()
	{
		Delete();
	}

	public void CrosshairShoot()
	{
		CrosshairLastShoot = 0;
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		if ( !Silencer )
		{
			if ( !string.IsNullOrEmpty( MuzzleFlashParticle ) && EffectEntity.GetAttachment( "muzzle" ) != null )
				Particles.Create( MuzzleFlashParticle, EffectEntity, "muzzle" );

			if ( !string.IsNullOrEmpty( BulletEjectParticle ) && EffectEntity.GetAttachment( "ejection_point" ) != null )
				Particles.Create( BulletEjectParticle, EffectEntity, "ejection_point" );
		}

		if ( IsLocalPawn )
		{
			if ( ScreenShake != null )
				ScreenUtil.Shake( ScreenShake.Length, ScreenShake.Delay, ScreenShake.Size, ScreenShake.Rotation );
		}

		if ( BurstsMode && !string.IsNullOrEmpty( BurstAnimation ) )
			ViewModelEntity?.SetAnimParameter( BurstAnimation, true );
		else
			ViewModelEntity?.SetAnimParameter( "fire", true );

		CrosshairShoot();
	}

	[ClientRpc]
	protected virtual void NPCShootEffects()
	{
		Host.AssertClient();

		if ( !Silencer )
		{
			if ( !string.IsNullOrEmpty( MuzzleFlashParticle ) && EffectEntity.GetAttachment( "muzzle" ) != null )
				Particles.Create( MuzzleFlashParticle, EffectEntity, "muzzle" );

			if ( !string.IsNullOrEmpty( BulletEjectParticle ) && EffectEntity.GetAttachment( "ejection_point" ) != null )
				Particles.Create( BulletEjectParticle, EffectEntity, "ejection_point" );
		}
	}

	[ClientRpc]
	protected virtual void EmptyEffects( bool isempty )
	{
		if ( isempty )
		{
			if ( AmmoClip > 0 ) return;

			ViewModelEntity?.SetAnimParameter( "empty", true );
		}
		else
		{
			ViewModelEntity?.SetAnimParameter( "empty", false );
		}
	}

	[ClientRpc]
	protected virtual void BulletTracer( Vector3 to )
	{
		var tr = EffectEntity.GetAttachment( "muzzle" );

		if ( tr == null ) return;

		var ps = Particles.Create( "particles/swb/tracer/tracer_large.vpcf", to );
		ps.SetPosition( 1, tr.GetValueOrDefault().Position );
		ps.SetPosition( 2, to );
	}

	/// <summary>
	/// Shoot a single bullet
	/// </summary>
	public virtual void ShootBullet( Vector3 pos, Vector3 dir, float spread, float force, float damage, float bulletSize )
	{
		var forward = dir;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
		forward = forward.Normal;

		if ( CreateEntity != null )
		{
			_ = CreateEntity.Invoke( pos, dir, forward, spread, force, damage );

			return;
		}

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( pos, pos + forward * 5000, bulletSize ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			//var random = new Random();
			//var randVal = random.Next( 0, 2 );

			//if ( randVal == 0 )

			BulletTracer( tr.EndPosition );

			//
			// We turn predictiuon off for this, so any exploding effects don't get culled etc
			//
			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}
	}

	/// <summary>
	/// Does a trace from start to end, does bullet impact effects. Coded as an IEnumerable so you can return multiple
	/// hits, like if you're going through layers or ricocet'ing or something.
	/// </summary>
	public virtual IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
	{
		bool InWater = Map.Physics.IsPointWater( start );

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				.HitLayer( CollisionLayer.Water, !InWater )
				.HitLayer( CollisionLayer.Debris )
				.Ignore( Owner )
				.Ignore( this )
				.Size( radius )
				.Run();

		if ( tr.Hit )
			yield return tr;

		//
		// Another trace, bullet going through thin material, penetrating water surface?
		//
	}

	/// <summary>
	/// Shoot a single bullet from owners view point
	/// </summary>
	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize )
	{
		ShootBullet( Owner.EyePosition, Owner.EyeRotation.Forward, spread, force, damage, bulletSize );
	}

	/// <summary>
	/// Shoot a multiple bullets from owners view point
	/// </summary>
	public virtual void ShootBullets( int numBullets, float spread, float force, float damage, float bulletSize )
	{
		var pos = Owner.EyePosition;
		var dir = Owner.EyeRotation.Forward;

		for ( int i = 0; i < numBullets; i++ )
		{
			ShootBullet( pos, dir, spread, force / numBullets, damage, bulletSize );
		}
	}

	public override void RenderHud( in Vector2 screensize )
	{
		var center = screensize * 0.5f;

		if ( IsReloading || (AmmoClip == 0 && ClipSize > 1) )
			CrosshairLastReload = 0;

		RenderCrosshair( center, CrosshairLastShoot.Relative, CrosshairLastReload.Relative );
	}

	TimeSince timeSinceZoomed;

	public virtual void RenderCrosshair( in Vector2 center, float lastAttack, float lastReload )
	{
		var draw = Render.Draw2D;

		switch ( Crosshair )
		{
			case CType.Common:
				{
					var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.3f, 0.0f ) );
					var color = Color.Lerp( Color.Red, Color.White, lastReload.LerpInverse( 0.0f, 0.4f ) );

					draw.BlendMode = BlendMode.Lighten;
					draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

					var length = 3.0f + shootEase * 5.0f;

					draw.Ring( center, length, length - 3.0f );

					break;
				}
			case CType.ShotGun:
				{
					var color = Color.Lerp( Color.Red, Color.White, lastReload.LerpInverse( 0.0f, 0.4f ) );
					draw.BlendMode = BlendMode.Lighten;
					draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

					// center
					{
						var shootEase = 1 + Easing.BounceIn( lastAttack.LerpInverse( 0.3f, 0.0f ) );
						draw.Ring( center, 15 * shootEase, 14 * shootEase );
					}

					// outer lines
					{
						var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.4f, 0.0f ) );
						var length = 30.0f;
						var gap = 30.0f + shootEase * 50.0f;
						var thickness = 4.0f;
						var extraAngle = 30 * shootEase;

						draw.CircleEx( center + Vector2.Right * gap, length, length - thickness, 32, 220, 320 );
						draw.CircleEx( center - Vector2.Right * gap, length, length - thickness, 32, 40, 140 );

						draw.Color = draw.Color.WithAlpha( 0.1f );
						draw.CircleEx( center + Vector2.Right * gap * 2.6f, length, length - thickness * 0.5f, 32, 220, 320 );
						draw.CircleEx( center - Vector2.Right * gap * 2.6f, length, length - thickness * 0.5f, 32, 40, 140 );
					}

					break;
				}
			case CType.Pistol:
				{
					var shootEase = Easing.EaseIn( lastAttack.LerpInverse( 0.2f, 0.0f ) );
					var color = Color.Lerp( Color.Red, Color.White, lastReload.LerpInverse( 0.0f, 0.4f ) );

					draw.BlendMode = BlendMode.Lighten;
					draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

					var length = 8.0f - shootEase * 2.0f;
					var gap = 10.0f + shootEase * 30.0f;
					var thickness = 2.0f;

					draw.Line( thickness, center + Vector2.Left * gap, center + Vector2.Left * (length + gap) );
					draw.Line( thickness, center - Vector2.Left * gap, center - Vector2.Left * (length + gap) );

					draw.Line( thickness, center + Vector2.Up * gap, center + Vector2.Up * (length + gap) );
					draw.Line( thickness, center - Vector2.Up * gap, center - Vector2.Up * (length + gap) );

					break;
				}
			case CType.SMG:
				{
					var color = Color.Lerp( Color.Red, Color.White, lastReload.LerpInverse( 0.0f, 0.4f ) );
					draw.BlendMode = BlendMode.Lighten;
					draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

					// center circle
					{
						var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.1f, 0.0f ) );
						var length = 2.0f + shootEase * 2.0f;
						draw.Circle( center, length );
					}


					draw.Color = draw.Color.WithAlpha( draw.Color.a * 0.2f );

					// outer lines
					{
						var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.2f, 0.0f ) );
						var length = 3.0f + shootEase * 2.0f;
						var gap = 30.0f + shootEase * 50.0f;
						var thickness = 2.0f;

						draw.Line( thickness, center + Vector2.Up * gap + Vector2.Left * length, center + Vector2.Up * gap - Vector2.Left * length );
						draw.Line( thickness, center - Vector2.Up * gap + Vector2.Left * length, center - Vector2.Up * gap - Vector2.Left * length );

						draw.Line( thickness, center + Vector2.Left * gap + Vector2.Up * length, center + Vector2.Left * gap - Vector2.Up * length );
						draw.Line( thickness, center - Vector2.Left * gap + Vector2.Up * length, center - Vector2.Left * gap - Vector2.Up * length );
					}

					break;
				}
			case CType.Rifle:
				{
					var color = Color.Lerp( Color.Red, Color.White, lastReload.LerpInverse( 0.0f, 0.4f ) );
					draw.BlendMode = BlendMode.Lighten;
					draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

					// center circle
					{
						var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.1f, 0.0f ) );
						var length = 2.0f + shootEase * 2.0f;
						draw.Circle( center, length );
					}


					draw.Color = draw.Color.WithAlpha( draw.Color.a * 0.2f );

					// outer lines
					{
						var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.2f, 0.0f ) );
						var length = 3.0f + shootEase * 2.0f;
						var gap = 15.0f + shootEase * 50.0f;
						var thickness = 2.0f;

						draw.Line( thickness, center + Vector2.Up * gap + Vector2.Left * length, center + Vector2.Up * gap - Vector2.Left * length );
						draw.Line( thickness, center - Vector2.Up * gap + Vector2.Left * length, center - Vector2.Up * gap - Vector2.Left * length );

						draw.Line( thickness, center + Vector2.Left * gap + Vector2.Up * length, center + Vector2.Left * gap - Vector2.Up * length );
						draw.Line( thickness, center - Vector2.Left * gap + Vector2.Up * length, center - Vector2.Left * gap - Vector2.Up * length );
					}

					break;
				}
			case CType.Crossbow:
				{
					if ( this is not WeaponSniper sniper )
						break;

					if ( sniper.ZoomLevel > -1 )
						timeSinceZoomed = 0;

					var zoomFactor = timeSinceZoomed.Relative.LerpInverse( 0.4f, 0 );

					var color = Color.Lerp( Color.Red, Color.White, lastReload.LerpInverse( 0.0f, 0.4f ) );
					draw.BlendMode = BlendMode.Lighten;
					draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

					// outer lines
					{
						var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.4f, 0.0f ) );
						var length = 10.0f;
						var gap = 40.0f + shootEase * 50.0f;

						gap -= zoomFactor * 20.0f;


						draw.Line( 0, center + Vector2.Up * gap, length, center + Vector2.Up * (gap + length) );
						draw.Line( 0, center - Vector2.Up * gap, length, center - Vector2.Up * (gap + length) );

						draw.Color = draw.Color.WithAlpha( draw.Color.a * zoomFactor );

						for ( int i = 0; i < 4; i++ )
						{
							gap += 40.0f;

							draw.Line( 0, center - Vector2.Left * gap, length, center - Vector2.Left * (gap + length) );
							draw.Line( 0, center + Vector2.Left * gap, length, center + Vector2.Left * (gap + length) );

							draw.Color = draw.Color.WithAlpha( draw.Color.a * 0.5f );
						}
					}

					break;
				}
		}
	}

	private void Discharge()
	{
		if ( TimeSinceDischarge < 0.5f )
			return;

		TimeSinceDischarge = 0;

		var muzzle = GetAttachment( "muzzle" ) ?? default;
		var pos = muzzle.Position;
		var rot = muzzle.Rotation;

		TakeAmmo( 1 );
		ShootEffects();
		PlaySound( ShootSound );

		for ( int i = 0; i < NumBullets; i++ )
			ShootBullet( pos, rot.Forward, Spread, Force, Damage, BulletSize );

		ApplyAbsoluteImpulse( rot.Backward * DischargeRecoil );
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		if ( CanDischarge && eventData.Speed > 500.0f )
		{
			if ( AmmoClip > 0 )
				Discharge();
		}
	}
}

public enum CType
{
	None,
	Common,
	ShotGun,
	Pistol,
	SMG,
	Rifle,
	Crossbow
}
