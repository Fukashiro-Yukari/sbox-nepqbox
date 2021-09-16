using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

public partial class WeaponSniper : Weapon
{
	public virtual string LensTexture => "/materials/swb/scopes/swb_lens_hunter.png"; // Path to the lens texture
	public virtual string ScopeTexture => "/materials/swb/scopes/swb_scope_hunter.png"; // Path to the scope texture
	public virtual string ZoomInSound => "swb_sniper.zoom_in"; // Sound to play when zooming in
	public virtual string ZoomOutSound => "swb_sniper.zoom_in"; // Sound to play when zooming out
	public virtual List<float> ZoomLevels => new() { 20f };
	public virtual List<float> MouseSensitivity => new() { 0.2f };
	public virtual float ScopedSpread => 0f;

	[Net, Predicted]
	public int ZoomLevel { get; set; } = -1;

	private Panel SniperScopePanel;
	private float lerpZoomAmount = 0;

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		ZoomLevel = -1;
		lerpZoomAmount = 0;
		SniperScopePanel?.Delete();
	}

	public override bool CanSecondaryAttack()
	{
		return false;
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

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

		if ( !string.IsNullOrEmpty( ShootSound ) )
			PlaySound( ShootSound );

		var spread = Spread;

		if ( ZoomLevel > -1 )
			spread = ScopedSpread;
		//
		// Shoot the bullets
		//
		if ( NumBullets > 1 )
			ShootBullets( NumBullets, spread, Force, Damage, BulletSize );
		else
			ShootBullet( spread, Force, Damage, BulletSize );
	}

	public virtual void OnScopedStart()
	{
		if ( IsLocalPawn )
		{
			ViewModelEntity.RenderColor = ViewModelEntity.RenderColor.WithAlpha( 0f );

			if ( !string.IsNullOrEmpty( ZoomInSound ) )
				PlaySound( ZoomInSound );
		}
	}

	public virtual void OnScopedEnd()
	{
		lerpZoomAmount = 0;

		if ( IsLocalPawn )
		{
			ViewModelEntity.RenderColor = ViewModelEntity.RenderColor.WithAlpha( 1f );

			if ( !string.IsNullOrEmpty( ZoomOutSound ) )
				PlaySound( ZoomOutSound );
		}
	}

	public override void Reload()
	{
		base.Reload();

		if ( ClipSize < 0 || AmmoClip >= ClipSize && ClipSize > -1 )
			return;

		if ( AmmoCount <= 0 ) return;
		if ( ZoomLevel <= -1 ) return;

		OnScopedEnd();
	}

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );

		if ( Input.Pressed( InputButton.Attack2 ) && !IsReloading )
		{
			ZoomLevel++;

			if ( ZoomLevel > ZoomLevels.Count - 1 )
				ZoomLevel = -1;

			if ( ZoomLevel > -1 )
			{
				OnScopedStart();
			}
			else
			{
				OnScopedEnd();
			}
		}

		if ( IsReloading )
			ZoomLevel = -1;
	}

	public override void CreateHudElements()
	{
		base.CreateHudElements();

		if ( Local.Hud == null ) return;

		SniperScopePanel = new SniperScope( LensTexture, ScopeTexture );
		SniperScopePanel.Parent = Local.Hud;
	}

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		if ( ZoomLevel > -1 )
		{
			if ( lerpZoomAmount == 0 )
				lerpZoomAmount = camSetup.FieldOfView;

			lerpZoomAmount = lerpZoomAmount.LerpTo( ZoomLevels[ZoomLevel], 10f * RealTime.Delta );
			camSetup.FieldOfView = lerpZoomAmount;
		}
	}

	public override void BuildInput( InputBuilder owner )
	{
		if ( ZoomLevel > -1 )
		{
			var mouseSensitivitylvl = ZoomLevel;

			if ( ZoomLevel > MouseSensitivity.Count - 1 )
				mouseSensitivitylvl = MouseSensitivity.Count - 1;

			owner.ViewAngles = Angles.Lerp( owner.OriginalViewAngles, owner.ViewAngles, MouseSensitivity[mouseSensitivitylvl] );
		}
	}
}
