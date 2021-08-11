using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

public partial class HitIndicator : Panel
{
	public static HitIndicator Current;

	public HitIndicator()
	{
		Current = this;
	}

	public override void Tick()
	{
		base.Tick();
	}

	public void OnHit( Vector3 pos, float amount, bool isdeath )
	{
		new HitPoint( amount, pos, this, isdeath );
	}

	public class HitPoint : Panel
	{
		public HitPoint( float amount, Vector3 pos, Panel parent, bool isdeath )
		{
			Parent = parent;

			if ( isdeath )
				SetClass( "death", true );

			_ = Lifetime();
		}

		async Task Lifetime()
		{
			await Task.Delay( 200 );
			Delete();
		}
	}
}


