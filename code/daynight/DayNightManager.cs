﻿using Sandbox;
using System;

namespace Sandbox.DayNight
{
	public static partial class DayNightManager
	{
		public delegate void SectionChanged( TimeSection section );
		public static event SectionChanged OnSectionChanged;

		public static TimeSection Section { get; private set; }

		//[ConVar.Replicated( "sv_day_night_cycle_time_of_day" )]
		public static float TimeOfDay { get; set; } = 0f;

		[ConVar.Replicated( "sv_day_night_cycle_speed" )]
		public static float Speed { get; set; } = 0.05f;

		public static TimeSection ToSection( float time )
		{
			if ( time > 5f && time <= 9f )
				return TimeSection.Dawn;

			if ( time > 9f && time <= 18f )
				return TimeSection.Day;

			if ( time > 18f && time <= 21f )
				return TimeSection.Dusk;

			return TimeSection.Night;
		}

		[Event.Tick.Server]
		private static void Tick()
		{
			TimeOfDay += Speed * Time.Delta;

			if ( TimeOfDay >= 24f )
			{
				TimeOfDay = 0f;
			}

			var currentSection = ToSection( TimeOfDay );

			if ( currentSection != Section )
			{
				Section = currentSection;
				OnSectionChanged?.Invoke( currentSection );
				ChangeSectionForClient( To.Everyone, currentSection );
			}
		}

		[ClientRpc]
		public static void ChangeSectionForClient( TimeSection section )
		{
			Host.AssertClient();
			OnSectionChanged?.Invoke( section );
		}
	}
}
