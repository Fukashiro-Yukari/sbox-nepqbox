using Sandbox;

public class EntityUndo : UndoRemove
{
	Entity entity;

	public EntityUndo(Entity ent )
	{
		entity = ent;
	}

	public override bool Delete()
	{
		if ( !entity.IsValid() || entity.IsWorld ) return false;

		entity.Delete();

		return true;
	}

	public override bool Replace( object obj )
	{
		var ent = obj as Entity;

		if ( ent == null ) return false;

		entity = ent;

		return true;
	}

	public override bool ObjectEquals( object obj )
	{
		return entity.Equals(obj);
	}

	public override bool IsValid()
	{
		return entity.IsValid();
	}
}
