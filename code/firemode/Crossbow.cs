namespace Sandbox.CWEP
{
	[Library( "cwep_crossbow", Title = "Crossbow", Description = "Automatic crossbow" )]
	public partial class Crossbow : CWEPFireMode
	{
		public override string PrimarySoundName => "rust_smg.shoot";

		public override void AttackPrimary()
		{
			if ( Parent.IsServer )
				using ( Prediction.Off() )
				{
					var bolt = new CrossbowBolt();
					bolt.Position = Owner.EyePos;
					bolt.Rotation = Owner.EyeRot;
					bolt.Owner = Owner;
					bolt.Velocity = Owner.EyeRot.Forward * 100;
				}
		}
	}
}
