using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class CurrentWeaponName : Panel
{
	public Label Label;

	public CurrentWeaponName()
	{
		Label = Add.Label( "", "value" );
	}

	public override void Tick()
	{
		SetClass( "changepos", true );

		var player = Local.Pawn;

		if ( player == null ) return;

		var cac = player.ActiveChild;

		if ( cac == null ) return;

		Label.Text = cac.ClassInfo.Title;

		var wep = player.ActiveChild as Weapon;

		if ( wep == null ) return;

		SetClass( "changepos", wep.ClipSize < 0 );
	}
}
