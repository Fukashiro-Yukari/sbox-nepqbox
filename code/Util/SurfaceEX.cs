namespace Sandbox
{
	public partial class SurfaceEXRpc
	{
		[ClientRpc]
		public static void CreateClientParticle( string particleName, Vector3 pos, Vector3 forward )
		{
			var ps = Particles.Create( particleName, pos );
			ps.SetForward( 0, forward );
		}
	}

	public static class SurfaceEX
	{
		public static void DoBulletImpactServer( this Surface self, TraceResult tr )
		{
			//
			// No effects on resimulate
			//
			if ( !Prediction.FirstTime )
				return;

			//
			// Drop a decal
			//
			if ( Host.IsServer )
			{
				var decalPath = Rand.FromArray( self.ImpactEffects.BulletDecal );
				if ( decalPath != null )
				{
					if ( DecalDefinition.ByPath.TryGetValue( decalPath, out var decal ) )
					{
						decal.PlaceUsingTrace( tr );
					}
				}
			}

			//
			// Make an impact sound
			//
			if ( !string.IsNullOrWhiteSpace( self.Sounds.Bullet ) )
			{
				Sound.FromWorld( self.Sounds.Bullet, tr.EndPos );
			}

			//
			// Get us a particle effect
			//
			string particleName = Rand.FromArray( self.ImpactEffects.Bullet );
			if ( particleName == null ) particleName = Rand.FromArray( self.ImpactEffects.Regular );

			if ( particleName != null && Host.IsServer )
			{
				SurfaceEXRpc.CreateClientParticle( particleName, tr.EndPos, tr.Normal );
			}
		}
	}
}
