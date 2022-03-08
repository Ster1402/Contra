using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Contra.utils
{
    public class Settings
    {
        //Params
        #region Properties

        //Account
        public List<string> playerNames;

        //Audio
        public bool backgroundMusicEnabled { get; set; }
        public bool SFXMusicEnabled { get; set; }
        public int volumeBackgroundMusic { get; set; }
        public int volumeSFXMusic { get; set; }

        //Commands
        public Dictionary<string, string> playerKeys { get; set; }

        #endregion

        #region Constructor
        public Settings()
        {
            playerNames = new List<string>() { "  Overlord" };
            playerKeys = new Dictionary<string, string>();

            backgroundMusicEnabled = true;
            SFXMusicEnabled = true;
            volumeBackgroundMusic = volumeSFXMusic = 100;

            playerKeys.Add("KeyLeft", Keys.Left.ToString());
            playerKeys.Add("KeyUp", Keys.Up.ToString());
            playerKeys.Add("KeyRight", Keys.Right.ToString());
            playerKeys.Add("KeyDown", Keys.Down.ToString());
            playerKeys.Add("KeyShoot", Keys.Space.ToString());

        }

        public Settings(Settings settings)
        {
            //Volume
            SFXMusicEnabled = settings.SFXMusicEnabled;
            backgroundMusicEnabled = settings.backgroundMusicEnabled;

            volumeBackgroundMusic = settings.volumeBackgroundMusic;
            volumeSFXMusic = settings.volumeSFXMusic;

            playerNames = new List<string>();
            playerNames.AddRange(settings.playerNames);

            playerKeys = new Dictionary<string, string>();

            playerKeys["KeyLeft"] = settings.playerKeys["KeyLeft"];
            playerKeys["KeyUp"] = settings.playerKeys["KeyUp"];
            playerKeys["KeyRight"] = settings.playerKeys["KeyRight"];
            playerKeys["KeyDown"] = settings.playerKeys["KeyDown"];
            playerKeys["KeyShoot"] = settings.playerKeys["KeyShoot"];

        }

        #endregion

        public void loadPreviousSettings()
        {

        }

        public void refresh(GameWindows windows)
        {
            //Account
            playerNames = windows.playerNames.Items.Cast<string>().ToList();
            windows.gameMap.playerName.Text = windows.playerNames.Text.Trim();
            windows.gameMap.Invalidate();

            //windows.gameMap.playerName.Text = windows.player.name =

            //Audio
            if (windows.checkBackgroundMusic.Checked)
            {
                volumeBackgroundMusic = windows.trackBarBackgroundMusic.Value * 10;
            }
            else
            {
                volumeBackgroundMusic = 0;
            }

            if (windows.checkSFXMusic.Checked)
            {
                volumeSFXMusic = windows.trackBarSFXMusic.Value * 10;
            }
            else
            {
                volumeSFXMusic = 0;
            }

            SFXMusicEnabled = windows.checkSFXMusic.Checked;
            backgroundMusicEnabled = windows.checkBackgroundMusic.Checked;

            windows.soundBackground.settings.volume = volumeBackgroundMusic;
            windows.soundHover.settings.volume = volumeSFXMusic;

            //Commands
            playerKeys["KeyLeft"] = windows.commandsSelectLeft.Text.Trim();
            playerKeys["KeyUp"] = windows.commandsSelectUp.Text.Trim();
            playerKeys["KeyRight"] = windows.commandsSelectRight.Text.Trim();
            playerKeys["KeyDown"] = windows.commandsSelectDown.Text.Trim();
            playerKeys["KeyShoot"] = windows.commandsSelectShoot.Text.Trim();
        }

        public void update(GameWindows windows)
        {
            windows.soundBackground.settings.volume = volumeBackgroundMusic;
            windows.soundHover.settings.volume = volumeSFXMusic;

            //Commands
            windows.commandsSelectLeft.Text = playerKeys["KeyLeft"];
            windows.commandsSelectUp.Text = playerKeys["KeyUp"];
            windows.commandsSelectRight.Text = playerKeys["KeyRight"];
            windows.commandsSelectDown.Text = playerKeys["KeyDown"];
            windows.commandsSelectShoot.Text = playerKeys["KeyShoot"];
        }

        public void reset(GameWindows windows)
        {
            //Audio
            backgroundMusicEnabled = SFXMusicEnabled = true;
            volumeBackgroundMusic = volumeSFXMusic = 100;
            windows.checkBackgroundMusic.Checked = true;
            windows.checkBackgroundMusic.CheckState = CheckState.Checked;
            windows.trackBarBackgroundMusic.Enabled = true;
            windows.trackBarBackgroundMusic.Value = 10;

            windows.checkSFXMusic.Checked = true;
            windows.checkSFXMusic.CheckState = CheckState.Checked;
            windows.trackBarSFXMusic.Enabled = true;
            windows.trackBarSFXMusic.Value = 10;

            //Commands
            playerKeys["KeyLeft"] = Keys.Left.ToString();
            playerKeys["KeyUp"] = Keys.Up.ToString();
            playerKeys["KeyRight"] = Keys.Right.ToString();
            playerKeys["KeyDown"] = Keys.Down.ToString();
            playerKeys["KeyShoot"] = Keys.Space.ToString();

            windows.gameMap.playerName.Text = windows.playerNames.Text.Trim();
            windows.soundBackground.settings.volume = volumeBackgroundMusic;
            windows.soundHover.settings.volume = volumeSFXMusic;

            windows.commandsSelectLeft.Text = Keys.Left.ToString();
            windows.commandsSelectUp.Text = Keys.Up.ToString();
            windows.commandsSelectRight.Text = Keys.Right.ToString();
            windows.commandsSelectDown.Text = Keys.Down.ToString();
            windows.commandsSelectShoot.Text = Keys.Space.ToString();

        }

    }

}
