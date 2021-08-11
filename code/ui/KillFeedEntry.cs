
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

public partial class KillFeedEntry : Panel
{
	public Label Left { get; internal set; }
	public Label Right { get; internal set; }
	public Label Method { get; internal set; }
	public Panel Icon { get; internal set; }

	public RealTimeSince TimeSinceBorn = 0;

	public KillFeedEntry()
	{
		Left = Add.Label( "", "left" );
		Method = Add.Label( "", "method" );
		Icon = Add.Panel( "icon" );
		Right = Add.Label( "", "right" );
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
