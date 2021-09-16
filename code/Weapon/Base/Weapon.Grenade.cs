using Sandbox;

public partial class WeaponGrenade : Weapon
{
	public override int ClipSize => -1;
	public override int AmmoMultiplier => 5;
	public override int Bucket => 3;
	public virtual float Delay => 1f;
	public virtual float PrimaryDistance => 0f;
	public virtual float SecondaryDistance => 0f;

	public override bool CanPrimaryAttack() => true;
	public override bool CanSecondaryAttack() => true;

	float Distance;
	TimeSince StartThrow;
	bool Pin;

	public override void AttackPrimary()
	{
		if ( Pin ) return;

		Distance = PrimaryDistance;

		PullPin();
	}

	public override void AttackSecondary()
	{
		if ( Pin ) return;
		
		Distance = SecondaryDistance;

		PullPin();
	}

	public virtual void PullPin()
	{
		if ( Pin ) return;

		ThrowEffects();

		Pin = true;
	}

	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed < 0.6f )
			return;

		if ( Pin )
		{
			if ( !Input.Down(InputButton.Attack1) && !Input.Down( InputButton.Attack2 ) )
			{
				StartThrow = 0f;
				Pin = false;

				(Owner as AnimEntity).SetAnimBool( "b_attack", true );
				ThrowEffects();
			}
			else
			{

			}
		}
		else if ( StartThrow > 0.1 )
		{
			Throw();
		}
	}

	[ClientRpc]
	protected virtual void ThrowEffects()
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public virtual void Throw()
	{

	}
}
