namespace Sandbox.CWEP
{
	[Library( "cwep_spawnball", Title = "Spawn Ball", Description = "Ball" )]
	public partial class SpawnBall : CWEPFireMode
	{
		public override bool PrimaryAutomatic => false;
		public override bool SecondaryCanUse => true;
		public override CType Crosshair => CType.Pistol;

		public override void AttackPrimary()
		{
			if ( Parent.IsServer )
				using ( Prediction.Off() )
				{
					var ent = new UnlimitedBouncyBallEntity();
					ent.Position = Owner.EyePosition + Owner.EyeRotation.Forward * 80;
					ent.Rotation = Owner.EyeRotation;
					ent.Owner = Owner;
					ent.Velocity = Owner.EyeRotation.Forward * 50000;
					ent.DeleteAsync( 5 );
				}
		}

		public override void AttackSecondary()
		{
			AttackPrimary();
		}
	}
}
