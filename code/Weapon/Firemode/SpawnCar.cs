namespace Sandbox.CWEP
{
	[Library( "cwep_spawncar", Title = "Spawn Car", Description = "Flying car" )]
	public partial class SpawnCar : CWEPFireMode
	{
		public override bool PrimaryAutomatic => false;
		public override bool SecondaryAutomatic => false;
		public override bool SecondaryCanUse => true;
		public override CType Crosshair => CType.Pistol;

		private void Spawn(bool super = false)
		{
			if ( Parent.IsServer )
				using ( Prediction.Off() )
				{
					var ent = new CarEntity();
					ent.Position = Owner.EyePos + Owner.EyeRot.Forward * 80;
					ent.Rotation = Owner.EyeRot;
					ent.Owner = Owner;
					ent.Velocity = Owner.EyeRot.Forward * (super ? 50000 : 1000);
					ent.DeleteAsync( 5 );

					// BUG: Unable to use weapons
					//if (ent.IsValid() && incar )
					//{
					//ent.ForcedDriver( Owner );

					//if ( Owner is SandboxPlayer player && player.Vehicle == null)
					//{
					//	player.Vehicle = ent;
					//	player.VehicleController = new CarController();
					//	player.VehicleAnimator = new CarAnimator();
					//	player.VehicleCamera = new CarCamera();
					//	player.Parent = ent;
					//	player.LocalPosition = Vector3.Up * 10;
					//	player.LocalRotation = Rotation.Identity;
					//	player.LocalScale = 1;
					//	player.PhysicsBody.Enabled = false;

					//	ent.driver = player;
					//}
					//}
				}
		}

		public override void AttackPrimary()
		{
			Spawn();
		}

		public override void AttackSecondary()
		{
			Spawn(true);
		}
	}
}
