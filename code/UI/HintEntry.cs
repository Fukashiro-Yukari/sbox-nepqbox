using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class HintEntry : Panel
{
	public Label HintText { get; internal set; }
	public Panel Icon { get; internal set; }

	public RealTimeSince TimeSinceBorn = 0;

	public HintEntry()
	{
		Icon = Add.Panel( "icon" );
		HintText = Add.Label( "", "text" );
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
