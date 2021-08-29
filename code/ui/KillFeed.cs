
using Sandbox;
using Sandbox.UI;

public partial class KillFeed : Sandbox.UI.KillFeed
{
	public override Panel AddEntry( ulong lsteamid, string left, ulong rsteamid, string right, string method )
	{
		Log.Info( $"{left} killed {right} using {method}" );

		var e = Current.AddChild<KillFeedEntry>();

		if (method != null && method.StartsWith("weapon_") || method == "crossbow_bolt")
		{
			e.AddClass( method );
			e.Icon.SetClass( "close", false );
		}
		else
		{
			e.Method.Text = method;
			e.Icon.SetClass( "close", true );
		}

		e.Left.Text = left;
		e.Left.SetClass( "me", lsteamid == (Local.SteamId) );

		e.Right.Text = right;
		e.Right.SetClass( "me", rsteamid == (Local.SteamId) );

		return e;
	}

	public virtual Panel AddEntry(string left, ulong rsteamid, string right, string method)
	{
		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass("me", false);

		if (method != null && method.StartsWith("weapon_") || method == "crossbow_bolt")
		{
			e.AddClass(method);
			e.Icon.SetClass("close", false);
		}
		else
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

		if (method != null && method.StartsWith("weapon_") || method == "crossbow_bolt")
		{
			e.AddClass(method);
			e.Icon.SetClass("close", false);
		}
		else
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

		if (method != null && method.StartsWith("weapon_") || method == "crossbow_bolt")
		{
			e.AddClass(method);
			e.Icon.SetClass("close", false);
		}
		else
		{
			e.Method.Text = method;
			e.Icon.SetClass("close", true);
		}

		e.Right.Text = right;
		e.Right.SetClass("me", false);

		return e;
	}
}
