using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

class InventoryIcon : Panel
{
	public Entity Weapon;
	public Image Icon;
	public Label Label;

	public InventoryIcon(Entity weapon)
	{
		Weapon = weapon;
		Label = Add.Label("", "item-name");

		AddChild(out Icon, "icon");

		Weapon wep = weapon as Weapon;
		Carriable car = weapon as Carriable;

		if (wep != null)
			Icon.SetTexture(wep.Icon);
		else if (car != null)
			Icon.SetTexture(car.Icon);
	}

	internal void TickSelection()
	{
		Label.SetText(Weapon.ClassInfo.Title);

		SetClass("active", Local.Pawn.ActiveChild == Weapon);
	}

	public override void Tick()
	{
		base.Tick();

		if (!Weapon.IsValid() || Weapon.Owner != Local.Pawn)
            Delete();
    }
}
