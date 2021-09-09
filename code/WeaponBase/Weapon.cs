using Sandbox;
using System;
using System.Threading.Tasks;

public partial class Weapon : BaseWeapon, IUse
{
	public virtual string WorldModelPath => "";
	public virtual string SilencerWorldModelPath => "";
	public virtual int ClipSize => 16;
	public virtual int ClipTake => 1;
	public virtual int AmmoMultiplier => 3;
	public virtual int Bucket => 1;
	public virtual int BucketWeight => 100;
	public virtual float ReloadTime => 3.0f;
	public virtual string ReloadSound => "";
	public virtual string ShootSound => "";
	public virtual string SilencerShootSound => "";
	public virtual string Icon => "";
	public virtual string BulletEjectParticle => "particles/pistol_ejectbrass.vpcf";
	public virtual string MuzzleFlashParticle => "particles/pistol_muzzleflash.vpcf";
	public virtual bool RealReload => true;
	public virtual bool Automatic => true;
	public virtual bool UseSilencer => false;
	public virtual bool CanDischarge => false;
	public virtual bool UnlimitedAmmo => true;
	public virtual CType Crosshair => CType.Common;
	public virtual int NumBullets => 1;
	public virtual float Spread => 0f;
	public virtual float Force => 0f;
	public virtual float Damage => 0f;
	public virtual float BulletSize => 3f;
	public virtual float OnSilencerDuration => 2f;
	public virtual float OffSilencerDuration => 2f;
	public virtual float DischargeRecoil => 200f;
	public virtual ScreenShake ScreenShake => null;
	public virtual Func<Vector3, Vector3, Vector3, float, float, float, Entity> CreateEntity => null;

	[Net, Predicted]
	public int AmmoClip { get; set; }

	[Net, Predicted]
	public int AmmoCount { get; set; }

	public PickupTrigger PickupTrigger { get; protected set; }

	[Net, Predicted]
	public TimeSince TimeSinceReload { get; set; }

	[Net, Predicted]
	public bool IsReloading { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; set; }

	[Net, Predicted]
	private bool Silencer { get; set; }

	private bool SilencerDelay;

	public TimeSince TimeSinceDischarge { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		PickupTrigger = new PickupTrigger
		{
			Parent = this,
			Position = Position,
			EnableTouch = true,
			EnableSelfCollisions = false
		};

		PickupTrigger.PhysicsBody.EnableAutoSleeping = false;
		AmmoClip = ClipSize;
		
		if ( ClipSize <= -1 )
		{
			AmmoCount = AmmoMultiplier;
		}
		else
		{
			if ( UnlimitedAmmo )
				AmmoCount = AmmoClip;
			else
				AmmoCount = AmmoClip * AmmoMultiplier;
		}

		if ( !string.IsNullOrEmpty( WorldModelPath ) )
			SetModel( WorldModelPath );
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		TimeSinceDeployed = 0;
		IsReloading = false;

		SetSilencedEffects( Silencer );
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		if ( SilencerDelay )
			SilencerDelay = false;
	}

	public override void Reload()
	{
		if ( IsReloading || ClipSize < 0 )
			return;

		if ( AmmoClip >= ClipSize && ClipSize > -1 )
			return;

		if ( AmmoCount <= 0 ) return;

		TimeSinceReload = 0;
		IsReloading = true;

		(Owner as AnimEntity)?.SetAnimBool( "b_reload", true );

		if ( !string.IsNullOrEmpty( ReloadSound ) )
			PlaySound( ReloadSound );

		StartReloadEffects();
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && (Input.Pressed( InputButton.Attack1 ) || Automatic);
	}

	//public override bool CanSecondaryAttack()
	//{
	//	return base.CanSecondaryAttack() && Input.Pressed( InputButton.Attack2 );
	//}

	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed < 0.6f )
			return;

		if ( !IsReloading )
		{
			base.Simulate( owner );
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( SilencerDelay ) return;
		if ( !TakeAmmo( ClipTake ) )
		{
			//DryFire();
			Reload();

			return;
		}

		(Owner as AnimEntity).SetAnimBool( "b_attack", true );

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
	}

	public override void AttackSecondary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( UseSilencer )
		{
			if ( SilencerDelay ) return;

			SilencerDelay = true;

			ToggleSilencerEffects( !Silencer );

			_ = SilencerEquip();
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
		ViewModelEntity?.SetAnimBool( "reload", true );

		// TODO - player third person model reload
	}

	[ClientRpc]
	public virtual void ToggleSilencerEffects( bool on )
	{
		ViewModelEntity?.SetAnimBool( on ? "add_silencer" : "remove_silencer", true );
	}

	[ClientRpc]
	public virtual void SetSilencedEffects( bool on )
	{
		ViewModelEntity?.SetAnimBool( "silenced", on );
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
		// CLICK
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
			EnableViewmodelRendering = true
		};

		ViewModelEntity.SetModel( ViewModelPath );
	}

	public override void CreateHudElements()
	{
		if ( Local.Hud == null || Crosshair == CType.None ) return;

		CrosshairPanel = new Crosshair();
		CrosshairPanel.Parent = Local.Hud;

		if ( Crosshair == CType.Common ) return;

		CrosshairPanel.AddClass( Crosshair.ToString() );
	}

	public bool OnUse( Entity user )
	{
		if ( Owner != null )
			return false;

		if ( !user.IsValid() )
			return false;

		user.StartTouch( this );

		return false;
	}

	public virtual bool IsUsable( Entity user )
	{
		if ( Owner != null ) return false;

		if ( user.Inventory is Inventory inventory )
		{
			return inventory.CanAdd( this );
		}

		return true;
	}

	public void Remove()
	{
		PhysicsGroup?.Wake();
		Delete();
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		if ( !Silencer )
		{
			if ( !string.IsNullOrEmpty( MuzzleFlashParticle ) )
				Particles.Create( MuzzleFlashParticle, EffectEntity, "muzzle" );

			if ( !string.IsNullOrEmpty( BulletEjectParticle ) )
				Particles.Create( BulletEjectParticle, EffectEntity, "ejection_point" );
		}

		if ( IsLocalPawn )
		{
			if ( ScreenShake != null )
				_ = new Sandbox.ScreenShake.Perlin( ScreenShake.Length, ScreenShake.Speed, ScreenShake.Size, ScreenShake.Rotation );
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
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

			//
			// We turn predictiuon off for this, so any exploding effects don't get culled etc
			//
			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}
	}

	/// <summary>
	/// Shoot a single bullet from owners view point
	/// </summary>
	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize )
	{
		ShootBullet( Owner.EyePos, Owner.EyeRot.Forward, spread, force, damage, bulletSize );
	}

	/// <summary>
	/// Shoot a multiple bullets from owners view point
	/// </summary>
	public virtual void ShootBullets( int numBullets, float spread, float force, float damage, float bulletSize )
	{
		var pos = Owner.EyePos;
		var dir = Owner.EyeRot.Forward;

		for ( int i = 0; i < numBullets; i++ )
		{
			ShootBullet( pos, dir, spread, force / numBullets, damage, bulletSize );
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
	Rifle
}

public class ScreenShake
{
	public float Length { get; set; } = 1.0f;
	public float Speed { get; set; } = 1.0f;
	public float Size { get; set; } = 1.0f;
	public float Rotation { get; set; } = 0.6f;
}
