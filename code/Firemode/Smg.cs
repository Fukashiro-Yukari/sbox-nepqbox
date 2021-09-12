namespace Sandbox.CWEP
{
	[Library( "cwep_smg", Title = "SMG", Description = "Submachine gun" )]
	public partial class SMGFireMode : CWEPFireMode // An item with the same key has already been added. Key: SMG (I don¡¯t understand why I can¡¯t set this class name to SMG)
	{
		public override string PrimarySoundName => "rust_smg.shoot";
		public override CType Crosshair => CType.SMG;

		public override void AttackPrimary()
		{
			Parent.ShootBullet( 0.1f, 1.5f, 5.0f, 3.0f );
		}
	}
}
