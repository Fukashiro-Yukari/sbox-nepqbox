using System;
using System.Collections.Generic;

namespace Sandbox
{
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
