using System;
using System.Collections.Generic;

namespace Sandbox
{
	public abstract class UndoRemove
	{
		public abstract bool Delete();
		public abstract bool Replace( object obj );
		public abstract bool ObjectEquals( object obj );
		public abstract bool IsValid();
	}

	public struct UndoTable
	{
		string Name;
		List<UndoRemove> Entities;
		Client Owner;

		public UndoTable( string name )
		{
			if ( name == null )
				throw new ArgumentNullException( "UndoTable name cannot be empty" );

			Name = name;
			Entities = new();
			Owner = null;
		}

		private UndoRemove CreateUndoRemove( object obj )
		{
			var ent = obj as Entity;
			var phyjoint = obj as IPhysicsJoint;
			var particles = obj as Particles;

			if ( ent != null )
			{
				return new EntityUndo( ent );
			}
			else if ( phyjoint != null )
			{
				return new PhysicsJointUndo( phyjoint );
			}
			else if ( particles != null )
			{
				return new ParticlesUndo( particles );
			}

			return null;
		}

		public void Tick()
		{
			if ( Entities.Count > 0 )
			{
				for ( int i = 0; i < Entities.Count; i++ )
				{
					var undo = Entities[i];

					if ( !undo.IsValid() )
					{
						var slot = Entities.IndexOf( undo );

						Entities.RemoveAt( slot );
					}
				}
			}
		}

		public void AddEntity( object obj )
		{
			if ( obj == null ) return;

			Entities.Add( CreateUndoRemove( obj ) );
		}

		public void Add( UndoRemove undo )
		{
			Entities.Add( undo );
		}

		public void Delete()
		{
			foreach ( var undoremove in Entities )
			{
				undoremove.Delete();
			}
		}

		public bool ReplaceEntity( object from, object to )
		{
			var isreplace = false;

			foreach (var undo in Entities )
			{
				if ( undo.ObjectEquals( from ) )
				{
					isreplace = true;

					undo.Replace( to );
				}
			}

			return isreplace;
		}

		public bool IsValid()
		{
			return Entities.Count > 0;
		}

		public void SetClient( Client client )
		{
			Owner = client;
		}

		public Client GetClient()
		{
			return Owner;
		}

		public string GetName()
		{
			return Name;
		}
	}

	public class Undo
    {
		private static Dictionary<Client, List<UndoTable>> PlayerEntitys = new();

		UndoTable temptable;

		public Undo( string name )
		{
			temptable = new( name );
		}

		public Undo AddEntity( object ent )
		{
			temptable.AddEntity( ent );

			return this;
		}

		public Undo Add( UndoRemove undo )
		{
			temptable.Add( undo );

			return this;
		}

		public Undo SetClient( Client cl )
		{
			temptable.SetClient( cl );

			return this;
		}

		public void Finish()
		{
			var cl = temptable.GetClient();
			if ( cl == null ) return;

			AddPlayer( cl );

			var list = PlayerEntitys[cl];

			list.Add( temptable );
		}

		private static void AddPlayer( Client cl )
		{
			if ( PlayerEntitys.ContainsKey( cl ) ) return;

			PlayerEntitys.Add( cl, new() );
		}

		public static void AddEntity( string name, Client cl, object ent )
		{
			if ( cl == null ) return;

			new Undo( name ).SetClient( cl ).AddEntity( ent ).Finish();
		}

		public static string DoUndo( Client cl )
		{
			if ( cl == null ) return null;

			AddPlayer( cl );

			var list = PlayerEntitys[cl];
			if ( list == null || list.Count < 1 ) return null;

			var undo = list[list.Count - 1];

			undo.Delete();

			list.RemoveAt( list.Count - 1 );

			return undo.GetName();
		}

		public static void ReplaceEntity( object from, object to )
		{
			foreach (var playerentitys in PlayerEntitys.Values )
			{
				foreach (var undotable in playerentitys )
				{
					var done = undotable.ReplaceEntity( from, to );

					if ( done )
						break;
				}
			}
		}

		[Event.Tick.Server]
		public static void Tick()
		{
			foreach ( var playerentitys in PlayerEntitys.Values )
			{
				for ( int i = 0; i < playerentitys.Count; i++ )
				{
					var undotable = playerentitys[i];

					undotable.Tick();

					if ( !undotable.IsValid() )
					{
						var slot = playerentitys.IndexOf( undotable );

						playerentitys.RemoveAt( slot );
					}
				}
			}
		}
	}
}
