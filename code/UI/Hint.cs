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

	public virtual Panel AddHint( string text )
	{
		var e = Current.AddChild<HintEntry>();

		e.HintText.Text = text;
		e.Icon.SetClass( "close", true );

		return e;
	}
}
