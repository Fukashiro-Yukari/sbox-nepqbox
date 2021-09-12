using Sandbox;

public class WeldUndo : UndoRemove
{
	Prop prop;

	public WeldUndo( Prop p )
	{
		prop = p;
	}

	public override bool Delete()
	{
		if ( !prop.IsValid() ) return false;

		prop.Unweld( true );

		return true;
	}

	public override void Replace( object obj )
	{
	}

	public override bool ObjectEquals( object obj )
	{
		return false;
	}

	public override bool IsValid()
	{
		return prop.IsValid();
	}
}
