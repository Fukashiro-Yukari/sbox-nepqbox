using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class ClientPanel : Panel
{
	public ClientPanel()
	{
		StyleSheet.Load( "ui/right/Client.scss" );

		Add.Button( "Undo", () => ConsoleSystem.Run( "undo" ) );
		Add.Button( "Cleanup", () => ConsoleSystem.Run( "cleanup" ) );
	}
}
