#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
#if IPHONE
using Microsoft.Xna.Framework;
#else
using Microsoft.Xna.Framework;
using System;
#endif
#endregion

namespace AsteroidsHD
{
	/// <summary>
	/// The options screen is brought up over the top of the main menu
	/// screen, and gives the user a chance to configure the game
	/// in various hopefully useful ways.
	/// </summary>
	class OptionsMenuScreen : MenuScreen
	{
		#region Fields

		MenuEntry gameTypeMenu;
		MenuEntry controlMenu;
		SliderMenuEntry slider;
		MenuEntry backMenuEntry;
		//MenuEntry languageMenuEntry;
		//MenuEntry frobnicateMenuEntry;
		//MenuEntry elfMenuEntry;

		#endregion

		#region Initialization


		/// <summary>
		/// Constructor.
		/// </summary>
		public OptionsMenuScreen () : base("Options")
		{
			Console.WriteLine("options created");
			// Create our menu entries.
			
		}
		public override void LoadContent ()
		{
			Console.WriteLine("loading content");
			base.LoadContent ();
			Console.WriteLine("base content loaded");
			gameTypeMenu = new MenuEntry (string.Empty);
			controlMenu = new MenuEntry(string.Empty);
			
			Console.WriteLine("loading slider:" + ScreenManager == null);

			slider = new SliderMenuEntry("Sensitivity",Settings.Sensativity,ScreenManager);
			
			Console.WriteLine("slider loaded");
			backMenuEntry = new MenuEntry ("Back");
			
			Console.WriteLine("back done");
			SetMenuEntryText ();
			
			
			// Hook up menu event handlers.
			gameTypeMenu.Selected += UngulateMenuEntrySelected;
			controlMenu.Selected += delegate {
				Settings.UseAccel = !Settings.UseAccel;
				controlMenu.Text = "Controls: " + (Settings.UseAccel ? "Accelerometer" : "Gamepad");
			};
			
			slider.ValueChanged += (value)=>
			{
				Settings.Sensativity = value;
			};
			
			backMenuEntry.Selected+= OnCancel;;
			
			Console.WriteLine("adding menu entries");
			// Add entries to the menu.
			MenuEntries.Add (gameTypeMenu);
			MenuEntries.Add(controlMenu);
			MenuEntries.Add(slider);
			MenuEntries.Add (backMenuEntry);
			
			Console.WriteLine("loading content completed");
		}
		
		public override void UnloadContent ()
		{
			base.UnloadContent ();
			slider.Unload();
		}

		/// <summary>
		/// Fills in the latest values for the options screen menu text.
		/// </summary>
		void SetMenuEntryText ()
		{
			Console.WriteLine("Setting text");
			gameTypeMenu.Text = "Game Type: " + Settings.GameType;
			controlMenu.Text = "Controls: " + (Settings.UseAccel ? "Accelerometer" : "Gamepad");
			//languageMenuEntry.Text = "Language: " + languages[currentLanguage];
			//frobnicateMenuEntry.Text = "Frobnicate: " + (frobnicate ? "on" : "off");
			//elfMenuEntry.Text = "elf: " + elf;
		}


		#endregion

		#region Handle Input


		/// <summary>
		/// Event handler for when the Ungulate menu entry is selected.
		/// </summary>
		void UngulateMenuEntrySelected (object sender, PlayerIndexEventArgs e)
		{
			
			Settings.GameType++;
			
			if (Settings.GameType > GameType.Modern)
				Settings.GameType = GameType.Facebook;
			
			SetMenuEntryText ();
		}


		/// <summary>
		/// Event handler for when the Language menu entry is selected.
		/// </summary>
		void LanguageMenuEntrySelected (object sender, PlayerIndexEventArgs e)
		{
			//currentLanguage = (currentLanguage + 1) % languages.Length;
			
			SetMenuEntryText ();
		}


		/// <summary>
		/// Event handler for when the Frobnicate menu entry is selected.
		/// </summary>
		void FrobnicateMenuEntrySelected (object sender, PlayerIndexEventArgs e)
		{
			//frobnicate = !frobnicate;
			
			SetMenuEntryText ();
		}


		/// <summary>
		/// Event handler for when the Elf menu entry is selected.
		/// </summary>
		void ElfMenuEntrySelected (object sender, PlayerIndexEventArgs e)
		{
			//elf++;
			
			SetMenuEntryText ();
		}
		
		
		#endregion
	}
}
