using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class UndoUIEntry : Panel
{
	public Panel Icon { get; internal set; }
	public Label Text { get; internal set; }

	public RealTimeSince TimeSinceBorn = 0;

	public UndoUIEntry()
	{
		Icon = Add.Panel( "icon" );
		Text = Add.Label( "" );
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
