using System;
namespace AsteroidsHD
{
	class ContinueMenuScreen : MenuScreen
	{
		
		MenuEntry continueMenu;
		MenuEntry quiteMenu;
        public event EventHandler<PlayerIndexEventArgs> Continue;
		public ContinueMenuScreen (string extraText) : base("Do you want to play again?",extraText)
		{
			continueMenu = new MenuEntry ("Play Again");
			quiteMenu = new MenuEntry("Main Menu");
			
			continueMenu.Selected  += delegate {
				ExitScreen();
				if(Continue != null)
					Continue(this, new PlayerIndexEventArgs(ControllingPlayer.Value));
			};;
			
			quiteMenu.Selected += delegate {
				Util.BackgroundScreen.AutoMove = true;
				LoadingScreen.Load(ScreenManager, false, null, Util.BackgroundScreen, new MainMenuScreen());
			};
			
			
            MenuEntries.Add(continueMenu);
            MenuEntries.Add(quiteMenu);
		}
	}
}

