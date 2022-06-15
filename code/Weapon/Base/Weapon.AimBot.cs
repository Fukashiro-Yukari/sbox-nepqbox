using Sandbox;
using System.Linq;

public partial class WeaponAimBot : Weapon
{
	ModelEntity Target;

	public ModelEntity FindTarget()
	{
		var rply = All.OfType<Player>().Where( x => x != Owner && x.IsValid() && x.LifeState == LifeState.Alive ).ToArray();

		if ( rply.Length > 0 )
			return rply[Rand.Int( 0, rply.Length - 1 )];

		var rnpc = All.OfType<NPC>().Where( x => x.IsValid() ).ToArray();

		if ( rnpc.Length > 0 )
			return rnpc[Rand.Int( 0, rnpc.Length - 1 )];

		return null;
	}

	public override void ShootBullet( Vector3 pos, Vector3 dir, float spread, float force, float damage, float bulletSize )
	{
		if ( !Target.IsValid() )
		{
			base.ShootBullet( pos, dir, spread, force, damage, bulletSize );

			return;
		}

		var eyes = Target.GetAttachment( "eyes" );

		if ( eyes == null )
		{
			base.ShootBullet( pos, dir, spread, force, damage, bulletSize );

			return;
		}

		var endpos = eyes.Value.Position;

		foreach ( var tr in TraceBullet( pos, endpos, bulletSize ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			BulletTracer( tr.EndPosition );

			//
			// We turn predictiuon off for this, so any exploding effects don't get culled etc
			//
			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, dir * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}
	}

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );

		if ( !Target.IsValid() || Target.LifeState != LifeState.Alive )
			Target = FindTarget();
	}
}
