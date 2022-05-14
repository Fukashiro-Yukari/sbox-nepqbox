using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Tests;
using System.Threading.Tasks;

[Library]
public partial class CloudModelList : Panel
{
	VirtualScrollPanel Canvas;

	public CloudModelList()
	{
		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemWidth = 100;
		Canvas.Layout.ItemHeight = 100;

		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var file = (Package)data;
			var panel = cell.Add.Panel( "icon" );
			panel.AddEventListener( "onclick", () => ConsoleSystem.Run( "spawn", file.FullIdent ) );
			panel.Style.BackgroundImage = Texture.Load( file.Thumb );
		};

		_ = UpdateItems();
	}

	public async Task UpdateItems()
	{
		var q = new Package.Query();
		q.Type = Package.Type.Model;

		var found = await q.RunAsync( default );

		Canvas.SetItems( found );
	}

}
