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
		Label.SetText( Weapon.ClassInfo.Title );

		SetClass( "active", Local.Pawn.ActiveChild == Weapon );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !Weapon.IsValid() || Weapon.Owner != Local.Pawn )
			Delete();
	}
}
