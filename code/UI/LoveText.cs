using System;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class LoveTextPanel : WorldPanel
{
	public LoveTextPanel()
	{
		StyleSheet.Load( "/UI/LoveText.scss" );

		var BG = Add.Panel( "BG" );

		var random = new Random();
		var text = "S&box destroyed my game mode again";

		if ( random.NextSingle() < 0.3 )
			text = "NO NO NO NO NO NO NO NO NO NO NO NO";

		BG.Add.Label( text, "Love" );
	}
}
