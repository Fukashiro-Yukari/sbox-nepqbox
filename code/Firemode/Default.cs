namespace Sandbox.CWEP
{
	[Library( "cwep_default", Title = "Default", Description = "Default fire mode, press R to change fire mode" )]
	public partial class Default : CWEPFireMode
	{
		public override bool PrimaryAutomatic => false;
		public override CType Crosshair => CType.ShotGun;

		public override void AttackPrimary()
		{
			Parent.ShootBullets( 10, 0.1f, 10.0f, 9.0f, 3.0f );
		}
	}
}
