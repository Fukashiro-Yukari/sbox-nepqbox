using Sandbox;
using Sandbox.UI;
using System;

public partial class Hint : Panel
{
	public static Hint Current;

	public Hint()
	{
		Current = this;
	}

	public static new void Add( string text )
	{
		if ( Host.IsClient ) return;

		AddHintMessage( text );
	}

	public static new void Add( To to, string text )
	{
		if ( Host.IsClient ) return;

		AddHintMessage( to, text );
	}

	[ClientRpc]
	public static void AddHintMessage( string text )
	{
		Current?.AddHint( text );
	}

	public virtual Panel AddHint( string text )
	{
		var e = Current.AddChild<HintEntry>();

		e.HintText.Text = text;
		e.Icon.SetClass( "close", true );

		return e;
	}
}
