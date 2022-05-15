using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class PickupFeedEntry : Panel
{
	public Panel Icon { get; internal set; }
	public Label Text { get; internal set; }

	public RealTimeSince TimeSinceBorn = 0;

	public PickupFeedEntry()
	{
		Icon = Add.Panel( "Icon" );
		Text = Add.Label( "", "Text" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( TimeSinceBorn > 6 )
		{
			Delete();
		}
	}
}
