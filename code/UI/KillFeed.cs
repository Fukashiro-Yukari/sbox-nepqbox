
using Sandbox;
using Sandbox.UI;
using System;

public partial class KillFeed : Sandbox.UI.KillFeed
{
	private bool GetIcon(string method, KillFeedEntry e)
    {
		try
		{
			if (method != null && method.StartsWith("weapon_") || method == "crossbow_bolt")
			{
				var killWeapon = Library.Create<Entity>(method);

				Weapon wep = killWeapon as Weapon;
				Carriable car = killWeapon as Carriable;
				CrossbowBolt cb = killWeapon as CrossbowBolt;

				if (wep != null)
				{
					if (!string.IsNullOrEmpty(wep.Icon))
					{
						var panelBackground = new PanelBackground();
						panelBackground.Texture = Texture.Load(wep.Icon);
						e.Icon.Style.Background = panelBackground;
						e.Icon.SetClass("close", false);
						killWeapon.Delete();

						return true;
					}
				}
				else if (car != null)
				{
					if (!string.IsNullOrEmpty(car.Icon))
					{
						var panelBackground = new PanelBackground();
						panelBackground.Texture = Texture.Load(car.Icon);
						e.Icon.Style.Background = panelBackground;
						e.Icon.SetClass("close", false);
						killWeapon.Delete();

						return true;
					}
				}
				else if (cb != null)
				{
					if (!string.IsNullOrEmpty(cb.Icon))
					{
						var panelBackground = new PanelBackground();
						panelBackground.Texture = Texture.Load(cb.Icon);
						e.Icon.Style.Background = panelBackground;
						e.Icon.SetClass("close", false);
						killWeapon.Delete();

						return true;
					}
				}
			}
		}
		catch (Exception) { }

		return false;
    }

	public override Panel AddEntry( ulong lsteamid, string left, ulong rsteamid, string right, string method )
	{
		Log.Info( $"{left} killed {right} using {method}" );

		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass( "me", lsteamid == (Local.SteamId) );

		if (!GetIcon(method, e))
		{
			e.Method.Text = method;
			e.Icon.SetClass("close", true);
		}

		e.Right.Text = right;
		e.Right.SetClass( "me", rsteamid == (Local.SteamId) );

		return e;
	}

	public virtual Panel AddEntry(string left, ulong rsteamid, string right, string method)
	{
		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass("me", false);

		if (!GetIcon(method, e))
		{
			e.Method.Text = method;
			e.Icon.SetClass("close", true);
		}

		e.Right.Text = right;
		e.Right.SetClass("me", rsteamid == (Local.Client?.SteamId));

		return e;
	}

	public virtual Panel AddEntry(ulong lsteamid, string left, string right, string method)
	{
		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass("me", lsteamid == (Local.Client?.SteamId));

		if (!GetIcon(method, e))
		{
			e.Method.Text = method;
			e.Icon.SetClass("close", true);
		}

		e.Right.Text = right;
		e.Right.SetClass("me", false);

		return e;
	}

	public virtual Panel AddEntry(string left, string right, string method)
	{
		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass("me", false);

		if (!GetIcon(method, e))
		{
			e.Method.Text = method;
			e.Icon.SetClass("close", true);
		}

		e.Right.Text = right;
		e.Right.SetClass("me", false);

		return e;
	}
}
