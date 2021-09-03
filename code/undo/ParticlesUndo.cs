using Sandbox;

public class ParticlesUndo : UndoRemove
{
	Particles particles;

	public ParticlesUndo( Particles p )
	{
		particles = p;
	}

	public override bool Delete()
	{
		particles.Destroy( true );

		return true;
	}

	public override bool Replace( object obj )
	{
		var p = obj as Particles;

		if ( p == null ) return false;

		particles = p;

		return true;
	}

	public override bool ObjectEquals( object obj )
	{
		return particles.Equals( obj );
	}

	public override bool IsValid()
	{
		return true;
	}
}
