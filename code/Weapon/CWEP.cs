using Sandbox;
using Sandbox.CWEP;
using System.Collections.Generic;

[Spawnable]
[Library( "weapon_cwep", Title = "Combination Weapon" )]
[Hammer.EditorModel("weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl")]
public partial class CWEPW : Weapon
{
	public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
	public override string WorldModelPath => "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl";

	public override int ClipSize => -1;
	public override float ReloadTime => 0.5f;
	public override int Bucket => 3;
	public override CType Crosshair => CType.ShotGun;
	public override string Icon => "ui/weapons/weapon_shotgun.png";

	protected virtual Vector3 LightOffset => Vector3.Forward * 10;

	public CWEPFireMode LastCurrentFireMode = null;
	public CWEPFireMode CurrentFireMode = null;
	public int CurrentFireModeInt = 0;
	private List<string> FireModes = new();

	private SpotLightEntity worldLight;
	private SpotLightEntity viewLight;

	[Net, Local, Predicted]
	private bool LightEnabled { get; set; } = true;

	TimeSince timeSinceLightToggled;

	public override void Spawn()
	{
		base.Spawn();

		worldLight = CreateLight();
		worldLight.SetParent( this, "muzzle", new Transform( LightOffset ) );
		worldLight.EnableHideInFirstPerson = true;
		worldLight.Enabled = false;
	}

	public override void CreateViewModel()
	{
		base.CreateViewModel();

		viewLight = CreateLight();
		viewLight.SetParent( ViewModelEntity, "muzzle", new Transform( LightOffset ) );
		viewLight.EnableViewmodelRendering = true;
		viewLight.Enabled = LightEnabled;
	}

	private SpotLightEntity CreateLight()
	{
		var light = new SpotLightEntity
		{
			Enabled = true,
			DynamicShadows = true,
			Range = 512,
			Falloff = 1.0f,
			LinearAttenuation = 0.0f,
			QuadraticAttenuation = 1.0f,
			Brightness = 2,
			Color = Color.White,
			InnerConeAngle = 20,
			OuterConeAngle = 40,
			FogStength = 1.0f,
			Owner = Owner,
			LightCookie = Texture.Load("materials/effects/lightcookie.vtex")
		};

		return light;
	}

	//[Event.Hotload]
	//public void OnHotloaded()
 //   {
 //       LoadAllFireMode();
 //   }

	private void LoadAllFireMode()
    {
		if (FireModes.Count > 0)
			FireModes = new();

		foreach (var en in TypeLibrary.GetDescriptions<CWEPFireMode>())
		{
			if (en.Title == "CWEPFireMode")
				continue;

			FireModes.Add(en.Name);
		}
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		if ( IsServer )
		{
			Activate();
		}

		LoadAllFireMode();
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		if ( IsServer )
		{
			if ( dropped )
			{
				Activate();
			}
			else
			{
				Deactivate();
			}
		}
	}

	public override void Simulate( Client owner )
	{
		if ( owner == null ) return;

		base.Simulate( owner );

		UpdateCurrentFireMode( owner );
		CurrentFireMode?.Simulate( owner );

		if ( timeSinceLightToggled > 0.1f && Input.Pressed( InputButton.Flashlight ) )
		{
			LightEnabled = !LightEnabled;

			PlaySound( LightEnabled ? "flashlight-on" : "flashlight-off" );

			if ( worldLight.IsValid() )
			{
				worldLight.Enabled = LightEnabled;
			}

			if ( viewLight.IsValid() )
			{
				viewLight.Enabled = LightEnabled;
			}

			timeSinceLightToggled = 0;
		}
	}

	private void UpdateCurrentFireMode( Client owner )
	{
		if ( CurrentFireMode == null )
		{
			if ( FireModes.Count > 0 )
			{
				if ( FireModes.IndexOf( "cwep_default" ) != -1 )
				{
					CurrentFireMode = TypeLibrary.Create<CWEPFireMode>( FireModes[FireModes.IndexOf( "cwep_default" )], false );
					CurrentFireModeInt = FireModes.IndexOf( "cwep_default" );
				}
				else
				{
					CurrentFireMode = TypeLibrary.Create<CWEPFireMode>( FireModes[0], false );
					CurrentFireModeInt = 0;
				}
			}
		}
		else
		{
			CurrentFireMode.Parent = this;
			CurrentFireMode.Owner = owner.Pawn as Player;

			if ( IsClient )
			{
				if ( CurrentFireMode.Crosshair == CType.None || ( LastCurrentFireMode != null && LastCurrentFireMode.Crosshair == CurrentFireMode.Crosshair ) ) return;
				if ( LastCurrentFireMode != null)
					CrosshairPanel.SetClass( LastCurrentFireMode.Crosshair.ToString(), false );

				CrosshairPanel.SetClass( CurrentFireMode.Crosshair.ToString(), true );
			}
		}
	}

	public override bool CanReload()
	{
		return base.CanReload() && Input.Pressed( InputButton.Reload );
	}

	public override bool CanPrimaryAttack()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.Attack1 ) || CurrentFireMode == null ) return false;
		if ( !Input.Pressed( InputButton.Attack1 ) && !CurrentFireMode.PrimaryAutomatic ) return false;

		var rate = CurrentFireMode.PrimaryRate;
		if ( rate <= 0 ) return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	public override bool CanSecondaryAttack()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.Attack2 ) || CurrentFireMode == null ) return false;
		if ( !Input.Pressed( InputButton.Attack2 ) && !CurrentFireMode.SecondaryAutomatic ) return false;

		var rate = CurrentFireMode.SecondaryRate;
		if ( rate <= 0 ) return true;

		return TimeSinceSecondaryAttack > (1 / rate);
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( CurrentFireMode != null && !CurrentFireMode.PrimaryCanUse ) return;

		(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

		if ( CurrentFireMode != null && CurrentFireMode.PrimaryShootEffects )
		{
			ShootEffects();
		}

		PlaySound( CurrentFireMode?.PrimarySoundName );
		CurrentFireMode?.AttackPrimary();
	}

	public override void AttackSecondary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( CurrentFireMode != null && !CurrentFireMode.SecondaryCanUse ) return;

		(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

		if ( CurrentFireMode != null && CurrentFireMode.SecondaryShootEffects )
		{
			DoubleShootEffects();
		}

		PlaySound( CurrentFireMode?.SecondarySoundName );
		CurrentFireMode?.AttackSecondary();
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	[ClientRpc]
	protected virtual void DoubleShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimParameter( "fire_double", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	[ClientRpc]
	protected virtual void ChangeFireModeEffects()
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimParameter( "fire_double", true );
	}

	public override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		FinishReload();
	}

	public override void Reload()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

		ChangeFireModeEffects();

		PlaySound( "flashlight-off" );

		if ( FireModes.Count > 1 )
		{
			CurrentFireModeInt++;

			if ( CurrentFireModeInt > FireModes.Count - 1 )
			{
				CurrentFireModeInt = 0;
			}

			LastCurrentFireMode = CurrentFireMode;
			CurrentFireMode?.BeforeChangeFireMode();
			CurrentFireMode = TypeLibrary.Create<CWEPFireMode>( FireModes[CurrentFireModeInt], false );
			CurrentFireMode?.AfterChangeFireMode();
		}
	}

	[ClientRpc]
	protected virtual void FinishReload()
	{
		ViewModelEntity?.SetAnimParameter( "reload_finished", true );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 3 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}

	private void Activate()
	{
		if ( worldLight.IsValid() )
		{
			worldLight.Enabled = LightEnabled;
		}
	}

	private void Deactivate()
	{
		if ( worldLight.IsValid() )
		{
			worldLight.Enabled = false;
		}
	}
}

namespace Sandbox.CWEP
{
	public partial class CWEPFireMode : BaseNetworkable
	{
		public CWEPW Parent { get; set; }
		public Player Owner { get; set; }
		public string ModeName { get; set; }
		public virtual bool PrimaryShootEffects => true;
		public virtual bool SecondaryShootEffects => true;
		public virtual bool PrimaryAutomatic => true;
		public virtual bool SecondaryAutomatic => true;
		public virtual bool PrimaryCanUse => true;
		public virtual bool SecondaryCanUse => false;
		public virtual string PrimarySoundName => "rust_pumpshotgun.shoot";
		public virtual string SecondarySoundName => "rust_pumpshotgun.shoot";
		public virtual float PrimaryRate => 15.0f;
		public virtual float SecondaryRate => 15.0f;
		public virtual CType Crosshair => CType.Common;

		public virtual void AttackPrimary()
		{

		}

		public virtual void AttackSecondary()
		{

		}

		public virtual void Simulate( Client cl )
		{

		}

		public virtual void BeforeChangeFireMode()
		{

		}

		public virtual void AfterChangeFireMode()
		{

		}
	}
}
