using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public partial class PickupFeed : Panel
{
	public static PickupFeed Current;

	public PickupFeed()
	{
		Current = this;

		StyleSheet.Load( "/UI/PickupFeed.scss" );
	}

	[ClientRpc]
	public static void OnPickup( Weapon wep )
	{
		Current?.AddPickupFeed( wep );
	}

	[ClientRpc]
	public static void OnPickupAmmo( Weapon wep, int ammo )
	{
		Current?.AddPickupFeedAmmo( wep, ammo );
	}

	public virtual Panel AddPickupFeed( Weapon wep )
	{
		var e = Current.AddChild<PickupFeedEntry>();

		e.Icon.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, wep.Icon );
		e.Text.Text = $"+ {wep.ClassInfo.Title}";

		return e;
	}

	public virtual Panel AddPickupFeedAmmo( Weapon wep, int ammo )
	{
		var e = Current.AddChild<PickupFeedEntry>();

		if ( wep.IsValid() )
			e.Icon.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, wep.Icon );
		
		e.Text.Text = $"+ {ammo} Ammo";

		return e;
	}
}
