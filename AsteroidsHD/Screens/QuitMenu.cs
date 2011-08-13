using System;
namespace AsteroidsHD
{
	class QuitMenu: MenuScreen 
	{
		
		MenuEntry continueMenu;
		MenuEntry quiteMenu;
		
		public QuitMenu () : base("Are you sure you want to quit?")
		{
			continueMenu = new MenuEntry ("Continue");
			quiteMenu = new MenuEntry("Quit");
			
			continueMenu.Selected  += OnCancel;;
			
			quiteMenu.Selected += delegate {
				Util.BackgroundScreen.AutoMove = true;
				LoadingScreen.Load(ScreenManager, false, null, Util.BackgroundScreen, new MainMenuScreen());
			};
			
			
            MenuEntries.Add(continueMenu);
            MenuEntries.Add(quiteMenu);
		}
	}
}

