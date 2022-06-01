using Sandbox;

[Spawnable]
[Library( "weapon_flashlight", Title = "Flashlight" )]
[Hammer.EditorModel("weapons/rust_pistol/rust_pistol.vmdl")]
partial class Flashlight : WeaponMelee
{
	public override string ViewModelPath => "weapons/rust_flashlight/v_rust_flashlight.vmdl";
	public override string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";
	public override int Bucket => 0;
	public override float SecondarySpeed => 0.5f;
	public override float SecondaryDamage => 25f;
	public override float SecondaryForce => 100f;
	public override float SecondaryMeleeDistance => 80f;
	public override float ImpactSize => 20f;
	public override CType Crosshair => CType.None;
	public override string Icon => "ui/weapons/weapon_pistol.png";
	public override string SecondaryAnimationHit => "attack_hit";
	public override string SecondaryAnimationMiss => "attack";
	public override string SecondaryAttackSound => "rust_flashlight.attack";
	public override string HitWorldSound => "rust_flashlight.attack";
	public override string MissSound => "rust_flashlight.attack";
	public override bool CanUseSecondary => true;
	public override ScreenShake SecondaryScreenShakeHit => new ScreenShake
	{
		Length = 1.0f,
		Delay = 1.0f,
		Rotation = 3.0f
	};

	protected virtual Vector3 LightOffset => Vector3.Forward * 10;

	private SpotLightEntity worldLight;
	private SpotLightEntity viewLight;

	[Net, Local, Predicted]
	private bool LightEnabled { get; set; } = true;

	TimeSince timeSinceLightToggled;

	public override void Spawn()
	{
		base.Spawn();

		worldLight = CreateLight();
		worldLight.SetParent( this, "slide", new Transform( LightOffset ) );
		worldLight.EnableHideInFirstPerson = true;
		worldLight.Enabled = false;
	}

	public override void CreateViewModel()
	{
		base.CreateViewModel();

		viewLight = CreateLight();
		viewLight.SetParent( ViewModelEntity, "light", new Transform( LightOffset ) );
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

	public override void Simulate( Client cl )
	{
		if ( cl == null )
			return;

		base.Simulate( cl );

		bool toggle = Input.Pressed( InputButton.Flashlight ) || Input.Pressed( InputButton.Attack1 );

		if ( timeSinceLightToggled > 0.1f && toggle )
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

	public override bool CanPrimaryAttack()
	{
		return false;
	}

	public override bool CanReload()
	{
		return false;
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

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		if ( IsServer )
		{
			Activate();
		}
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
}
