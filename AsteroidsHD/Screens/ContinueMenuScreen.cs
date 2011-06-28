using System;
namespace AsteroidsHD
{
	class ContinueMenuScreen : MenuScreen
	{
		
		MenuEntry continueMenu;
		MenuEntry quiteMenu;
        public event EventHandler<PlayerIndexEventArgs> Continue;
		public ContinueMenuScreen () : base("Do you want to play again?")
		{
			continueMenu = new MenuEntry ("Play Again");
			quiteMenu = new MenuEntry("Main Menu");
			
			continueMenu.Selected  += delegate {
				ExitScreen();
				if(Continue != null)
					Continue(this, new PlayerIndexEventArgs(ControllingPlayer.Value));
			};;
			
			quiteMenu.Selected += delegate {
				LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
			};
			
			
            MenuEntries.Add(continueMenu);
            MenuEntries.Add(quiteMenu);
		}
	}
}

