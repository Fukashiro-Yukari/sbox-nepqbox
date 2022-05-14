
using Sandbox;
using Sandbox.Tools;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

[Library]
public partial class SpawnMenu : Panel
{
	public static SpawnMenu Instance;
	readonly Panel toollist;
	readonly Panel untilist;
	private Panel toolPanel;
	private Panel untiPanel;

	public SpawnMenu()
	{
		Instance = this;

		StyleSheet.Load( "/UI/SpawnMenu.scss" );

		var left = Add.Panel( "left" );
		{
			var tabs = left.AddChild<ButtonGroup>();
			tabs.AddClass( "tabs" );

			var body = left.Add.Panel( "body" );

			{
				var props = body.AddChild<SpawnList>();
				tabs.SelectedButton = tabs.AddButtonActive( "Props", ( b ) => props.SetClass( "active", b ) );

				var ents = body.AddChild<EntityList>();
				tabs.AddButtonActive( "Entities", ( b ) => ents.SetClass( "active", b ) );

				var weps = body.AddChild<WeaponList>();
				tabs.AddButtonActive( "Weapons", ( b ) => weps.SetClass( "active", b ) );

				var npcs = body.AddChild<NPCList>();
				tabs.AddButtonActive( "NPCs", ( b ) => npcs.SetClass( "active", b ) );

				var models = body.AddChild<CloudModelList>();
				tabs.AddButtonActive( "s&works", ( b ) => models.SetClass( "active", b ) );
			}
		}

		var right = Add.Panel( "right" );
		{
			var tabs = right.Add.ButtonGroup( "tabs" );
			{
				tabs.SelectedButton = tabs.Add.Button( "Tools" );
				tabs.Add.Button( "Utility" );
			}
			var body = right.Add.Panel( "body" );
			{
				toollist = body.Add.Panel( "page toollist visible" );
				{
					RebuildToolList();
				}
				untilist = body.Add.Panel( "page toollist" );
				{
					RebuildUtilityList();
				}

				body.Add.Panel( "inspector" );
			}

			tabs.AddEventListener( "startactive", () =>
			{
				var id = tabs.GetChildIndex( tabs.SelectedButton );

				foreach ( var c in body?.Children )
				{
					var i = c.Parent.GetChildIndex( c );
					c.SetClass( "visible", i == id );
				}
			} );
		}
	}

	void RebuildToolList()
	{
		toollist.DeleteChildren( true );

		foreach ( var entry in Library.GetAllAttributes<BaseTool>() )
		{
			if ( entry.Title == "BaseTool" )
				continue;

			var button = toollist.Add.Button( entry.Title );
			button.SetClass( "active", entry.Name == ConsoleSystem.GetValue( "tool_current" ) );

			button.AddEventListener( "onclick", () =>
			{
				ConsoleSystem.Run( "tool_current", entry.Name );
				ConsoleSystem.Run( "inventory_current", "weapon_tool" );

				foreach ( var child in toollist.Children )
					child.SetClass( "active", child == button );
			} );
		}

		toolPanel = toollist.Add.Panel( "toolpanel" );
	}

	Button CreateUtilityButton<T>( Panel untilist, string text ) where T : Panel, new()
	{
		var button = untilist.Add.Button( text );

		button.AddEventListener( "onclick", () =>
		{
			foreach ( var child in untilist.Children )
				child.SetClass( "active", child == button );

			untiPanel?.DeleteChildren();
			untiPanel?.AddChild<T>();
		} );

		return button;
	}

	void RebuildUtilityList()
	{
		untilist.DeleteChildren( true );

		untiPanel = untilist.Add.Panel( "toolpanel" );
		untiPanel.AddChild<ClientPanel>();

		CreateUtilityButton<ClientPanel>( untilist, "Client" ).SetClass( "active", true );
		CreateUtilityButton<AdminPanel>( untilist, "Admin" );
	}

	public override void Tick()
	{
		base.Tick();

		Parent.SetClass( "spawnmenuopen", Input.Down( InputButton.Menu ) );

		UpdateActiveTool();
	}

	void UpdateActiveTool()
	{
		var toolCurrent = ConsoleSystem.GetValue( "tool_current" );
		var tool = string.IsNullOrWhiteSpace( toolCurrent ) ? null : Library.GetAttribute( toolCurrent );

		foreach ( var child in toollist.Children )
		{
			if ( child is Button button )
			{
				child.SetClass( "active", tool != null && button.Text == tool.Title );
			}
		}
	}

	public override void OnHotloaded()
	{
		base.OnHotloaded();

		RebuildToolList();
		RebuildUtilityList();
	}
}
