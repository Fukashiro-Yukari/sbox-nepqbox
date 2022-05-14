using Sandbox;

public class ResizerUndo : UndoRemove
{
	Entity entity;
	float scale;

	public ResizerUndo( Entity ent, float s )
	{
		entity = ent.Root;
		scale = s;
	}

	public override bool Delete()
	{
		if ( !entity.IsValid() || entity.IsWorld ) return false;
		if ( entity.PhysicsGroup == null ) return false;

		entity.Scale = scale;
		entity.PhysicsGroup.RebuildMass();
		entity.PhysicsGroup.Sleeping = false;

		foreach ( var child in entity.Children )
		{
			if ( !child.IsValid() )
				continue;

			if ( child.PhysicsGroup == null )
				continue;

			child.PhysicsGroup.RebuildMass();
			child.PhysicsGroup.Sleeping = false;
		}

		return true;
	}

	public override void Replace( object obj )
	{
		if ( !(obj is Entity e) ) return;

		entity = e;
	}

	public override bool ObjectEquals( object obj )
	{
		return entity.Equals( obj );
	}

	public override bool IsValid()
	{
		return entity.IsValid();
	}
}
