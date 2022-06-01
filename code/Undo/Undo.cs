using System;
using System.Collections.Generic;

namespace Sandbox
{
	public partial class Undo
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

		public void Finish( string text = null )
		{
			temptable.SetText( text );

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

		public static void DoUndo( Client cl )
		{
			if ( cl == null ) return;

			AddPlayer( cl );

			var list = PlayerEntitys[cl];
			if ( list == null || list.Count < 1 ) return;

			var undo = list[list.Count - 1];

			undo.Delete();

			list.RemoveAt( list.Count - 1 );

			Event.Run( "OnUndo", undo.GetText(), cl );

			AddUndoText( To.Single( cl ), undo.GetText() );
		}

		public static void DoUndoAll( Client cl )
		{
			if ( cl == null ) return;

			AddPlayer( cl );

			var list = PlayerEntitys[cl];
			if ( list == null || list.Count < 1 ) return;

			foreach ( var undo in list )
			{
				undo.Delete();
			}

			list.Clear();

			Event.Run( "OnUndo", "Cleaned up everything!", cl );

			AddCustomUndoText( To.Single( cl ), "Cleaned up everything!" );
		}

		public static void DoUndoAllAdmin( Client cl )
		{
			foreach ( var list in PlayerEntitys.Values )
			{
				foreach ( var undo in list )
				{
					undo.Delete();
				}

				list.Clear();
			}

			Event.Run( "OnUndo", "Cleaned up everything!", cl );

			AddCustomUndoText( To.Single( cl ), "Cleaned up everything!" );
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

		[ClientRpc]
		public static void AddUndoText( string text )
		{
			Event.Run( "OnUndo", $"Undone {text}" );

			UndoUI.Current.AddUndoText( text );
		}

		[ClientRpc]
		public static void AddUndoText( Entity ent )
		{
			var di = DisplayInfo.For( ent );

			Event.Run( "OnUndo", $"Undone {di.Name}" );

			UndoUI.Current.AddUndoText( di.Name );
		}

		[ClientRpc]
		public static void AddCustomUndoText( string text )
		{
			Event.Run( "OnUndo", text );

			UndoUI.Current.AddCustomUndoText( text );
		}

		[ConCmd.Server( "undo" )]
		public static void UndoCmd()
		{
			var owner = ConsoleSystem.Caller.Pawn;

			if ( owner == null )
				return;

			DoUndo( ConsoleSystem.Caller );
		}

		[ConCmd.Server( "cleanup" )]
		public static void CleanupCmd()
		{
			var owner = ConsoleSystem.Caller.Pawn;

			if ( owner == null )
				return;

			DoUndoAll( ConsoleSystem.Caller );
		}

		[ConCmd.Server( "admin_cleanup" )]
		public static void AdminCleanupCmd()
		{
			var owner = ConsoleSystem.Caller.Pawn;

			if ( owner == null )
				return;

			DoUndoAllAdmin( ConsoleSystem.Caller );
		}
	}
}
