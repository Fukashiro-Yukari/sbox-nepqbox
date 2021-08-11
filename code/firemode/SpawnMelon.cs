namespace Sandbox.CWEP
{
	[Library( "cwep_spawnmelon", Title = "Spawn Melon", Description = "Fun !" )]
	public partial class SpawnMelon : CWEPFireMode
	{
		public override bool PrimaryAutomatic => true;
		public override CType Crosshair => CType.SMG;

		public override void AttackPrimary()
		{
			if ( Parent.IsServer )
				using ( Prediction.Off() )
				{
					var ent = new Prop();
					ent.SetModel( "models/sbox_props/watermelon/watermelon" );
					ent.Position = Owner.EyePos + Owner.EyeRot.Forward * 80;
					ent.Rotation = Owner.EyeRot;
					ent.Owner = Owner;
					ent.Velocity = Owner.EyeRot.Forward * 50000;
					ent.DeleteAsync( 5 );
				}
		}
	}
}
