using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class AdminPanel : Panel
{
	public AdminPanel()
	{
		StyleSheet.Load( "ui/right/Admin.scss" );

		Add.Button( "Admin Cleanup", () => ConsoleSystem.Run( "admin_cleanup" ) );
	}
}
