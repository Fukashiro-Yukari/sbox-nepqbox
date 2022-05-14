using System;
using System.Collections.Generic;

namespace Sandbox
{
	public struct UndoTable
	{
		string Name;
		string Text;
		List<UndoRemove> Entities;
		Client Owner;

		public UndoTable( string name )
		{
			if ( string.IsNullOrEmpty( name ) )
				throw new ArgumentNullException( "UndoTable name cannot be empty" );

			Name = name;
			Text = Name;
			Entities = new();
			Owner = null;
		}

		private UndoRemove CreateUndoRemove( object obj )
		{
			if ( obj is Entity ent )
			{
				return new EntityUndo( ent );
			}
			else if ( obj is PhysicsJoint phyjoint )
			{
				return new PhysicsJointUndo( phyjoint );
			}
			else if ( obj is Particles particles )
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

			foreach ( var undo in Entities )
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

		public void SetText( string text )
		{
			if ( string.IsNullOrEmpty( text ) ) return;

			Text = text;
		}

		public string GetText()
		{
			return Text;
		}
	}
}
