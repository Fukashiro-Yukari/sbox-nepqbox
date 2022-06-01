using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

class InventoryIcon : Panel
{
	public Carriable Weapon;
	public Image Icon;
	public Label Label;

	public InventoryIcon( Carriable weapon )
	{
		Weapon = weapon;
		Label = Add.Label( "", "item-name" );

		AddChild( out Icon, "icon" );

		Icon.SetTexture( weapon.Icon );
	}

	internal void TickSelection()
	{
		var di = DisplayInfo.For( Weapon );

		Label.SetText( di.Name );

		var ply = Local.Pawn as Player;

		SetClass( "active", ply.ActiveChild == Weapon );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !Weapon.IsValid() || Weapon.Owner != Local.Pawn )
			Delete();
	}
}
