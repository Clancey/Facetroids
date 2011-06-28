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
			// Create our menu entries.
			gameTypeMenu = new MenuEntry (string.Empty);
			//languageMenuEntry = new MenuEntry(string.Empty);
			//frobnicateMenuEntry = new MenuEntry(string.Empty);
			//elfMenuEntry = new MenuEntry(string.Empty);
			
			SetMenuEntryText ();
			
			MenuEntry backMenuEntry = new MenuEntry ("Back");
			
			// Hook up menu event handlers.
			gameTypeMenu.Selected += UngulateMenuEntrySelected;
			//languageMenuEntry.Selected += LanguageMenuEntrySelected;
			//frobnicateMenuEntry.Selected += FrobnicateMenuEntrySelected;
			//elfMenuEntry.Selected += ElfMenuEntrySelected;
			backMenuEntry.Selected += OnCancel;
			
			// Add entries to the menu.
			MenuEntries.Add (gameTypeMenu);
			//MenuEntries.Add(languageMenuEntry);
			//MenuEntries.Add(frobnicateMenuEntry);
			//MenuEntries.Add(elfMenuEntry);
			MenuEntries.Add (backMenuEntry);
		}


		/// <summary>
		/// Fills in the latest values for the options screen menu text.
		/// </summary>
		void SetMenuEntryText ()
		{
			gameTypeMenu.Text = "Game Type: " + Settings.GameType;
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
