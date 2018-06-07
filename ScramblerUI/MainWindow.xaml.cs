using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FMLib.Utility;
using FMScrambler.Utility;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace FMScrambler
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool isPasteEvent = false;
        private string prevSeedText;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_loadiso_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog {Title = "Location of Yu-Gi-Oh! Forbidden Memories NTSC CUE File", Filter = "*.cue | *.cue" };
     
            if (dlg.ShowDialog() == true)
            {
                //Static.IsoPath = dlg.FileName;
                lbl_path.Content = Path.GetDirectoryName(dlg.FileName);

                MessageBox.Show("Extracting game data can take a minute... please wait.", "Extracting data",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                pgr_back.Visibility = Visibility.Visible;
                BinChunk chunker = new BinChunk();
                chunker.ExtractBin(dlg.FileName);
                pgr_back.Visibility = Visibility.Hidden;
                MessageBox.Show("Extracting game data complete.", "Extracting data",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Static.UsedIso = true;

                btn_loadiso.IsEnabled = false;
                btn_loadiso1.IsEnabled = false;
                btn_perform.IsEnabled = true;
            }      
        }

        public void setActionLabel(string text)
        {
            //lbl_status.Content = text;
        }

        private void btn_perform_Click(object sender, RoutedEventArgs e)
        {
            sync_Scramble();
        }

        private void sync_Scramble()
        {
            int cardCount = Static.GlitchFusions ? 1400 : 722;
            Static.SetCardCount(cardCount);

            FileHandler fileHandler = new FileHandler();

            MessageBox.Show("Done scrambling, you may proceed with patching your game ISO now. A logfile was created in the tools directory as well.",
                       "Done scrambling.", MessageBoxButton.OK, MessageBoxImage.Information);

            fileHandler.LoadSlus(lbl_path.Content.ToString());
            fileHandler.ScrambleFusions(int.Parse(txt_seed.Text));

            btn_patchiso.IsEnabled = true;
            //btn_perform.IsEnabled = false;

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txt_seed_Loaded(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();        
            txt_seed.Text = rnd.Next(10000, 214748364).ToString();
        }

        private void ch_glitch_Checked(object sender, RoutedEventArgs e)
        {
            Static.GlitchFusions = true;
        }

        private void ch_glitch_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.GlitchFusions = false;
        }

        private void ch_illegal_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.HighId = false;
        }

        private void ch_illegal_Checked(object sender, RoutedEventArgs e)
        {
            Static.HighId = true;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            txt_seed.Text = rnd.Next(10000, 214748364).ToString();
        }

        private void btn_patchiso_Click(object sender, RoutedEventArgs e)
        {
            if (!Static.UsedIso)
            {
                MessageBox.Show("Did you make a backup copy of your Image file before patching? If not, do so before pressing OK.", "Backup Info",
                    MessageBoxButton.OK, MessageBoxImage.Question);

                OpenFileDialog dlg = new OpenFileDialog { Title = "Forbidden Memories Image" };

                if (dlg.ShowDialog() == true)
                {
                    #if DEBUG
                    Console.WriteLine(dlg.FileName);
                    #endif
                    pgr_back.Visibility = Visibility.Visible;
                    ImagePatcher patcher = new ImagePatcher(dlg.FileName);

                    if (patcher.PatchImage() == 1)
                    {
                        MessageBox.Show("Image successfully patched! Have fun playing!", "Done patching.",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error patching Image. Not Forbidden Memories or wrong version.", "Error patching.",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                pgr_back.Visibility = Visibility.Visible;
                ImagePatcher patcher = new ImagePatcher(Static.IsoPath);

                if (patcher.PatchImage() == 1)
                {
                    MessageBox.Show("Image successfully patched! Have fun playing! Location of Randomized Image: "+ Static.IsoPath, "Done patching.",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    Process.Start(Directory.GetCurrentDirectory());

                }
                else
                {
                    MessageBox.Show("Error patching Image. Not Forbidden Memories or wrong version.", "Error patching.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


            pgr_back.Visibility = Visibility.Hidden;
        }

        private void txt_seed_Initialized(object sender, EventArgs e)
        {
            Random rnd = new Random();
            txt_seed.Text = rnd.Next(10000, 214748364).ToString();
        }

        private void txt_seed_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.V == e.Key && Keyboard.Modifiers == ModifierKeys.Control)
            {
                prevSeedText = txt_seed.Text;
                isPasteEvent = true;
            }
        }

        private void txt_seed_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (isPasteEvent)
            {
                txt_seed.Text = prevSeedText;
                isPasteEvent = false;
            }
            if (txt_seed.Text.StartsWith("0"))
            {
                txt_seed.Text = $"1{txt_seed.Text.Substring(1)}";
            }
        }

        private void chk_atkdefenabled_Checked(object sender, RoutedEventArgs e)
        {
            Static.RandomAtkdef = true;
            grp_atkdef.IsEnabled = true;
        }

        private void chk_atkdefenabled_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.RandomAtkdef = false;
            grp_atkdef.IsEnabled = false;
        }

        private void chk_randomattributes_Checked(object sender, RoutedEventArgs e)
        {
            Static.RandomAttributes = true;
        }

        private void chk_randomattributes_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.RandomAttributes = false;
        }

        private void chk_randomguardianstars_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.RandomGuardianStars = false;
        }

        private void chk_randomguardianstars_Checked(object sender, RoutedEventArgs e)
        {
            Static.RandomGuardianStars = true;
        }

        private void chk_randomdrops_Checked(object sender, RoutedEventArgs e)
        {
            Static.RandomCardDrops = true;
        }

        private void chk_randomdrops_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.RandomCardDrops = false;
        }

        private void chk_randomtypes_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.RandomTypes = false;
        }

        private void chk_randomtypes_Checked(object sender, RoutedEventArgs e)
        {
            Static.RandomTypes = true;
        }

        private void chk_glitchgs_Checked(object sender, RoutedEventArgs e)
        {
            Static.GlitchGuardianStars = true;
        }

        private void chk_glitchgs_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.GlitchGuardianStars = false;

        }

        private void chk_randomdecks_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.RandomDecks = false;
        }

        private void chk_randomdecks_Checked(object sender, RoutedEventArgs e)
        {
            Static.RandomDecks = true;
        }

        private void chk_randomequips_Checked(object sender, RoutedEventArgs e)
        {
            Static.RandomEquips = true;
        }

        private void chk_randomequips_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.RandomEquips = false;
        }

        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            this.Title =
                $"YGO! FM Fusion Scrambler Tool - {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} {Meta.VersionInfo}";
        }

        private void btn_loadiso1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Title = "Location of SLUS_014.11", Filter = "SLUS_014.11 | SLUS_014.11" };

            if (dlg.ShowDialog() == true)
            {
                Static.SlusPath = dlg.FileName;
                lbl_path.Content = Path.GetDirectoryName(dlg.FileName);

                if (!File.Exists(Path.GetDirectoryName(dlg.FileName) + "\\DATA\\WA_MRG.MRG"))
                {
                    dlg.Title = "Location of WA_MRG.MRG";
                    dlg.Filter = "WA_MRG | WA_MRG.MRG";

                    if (dlg.ShowDialog() == true)
                    {
                        Static.WaPath = dlg.FileName;
                        btn_perform.IsEnabled = true;
                    }
                }
                else
                {
                    Static.WaPath = Path.GetDirectoryName(dlg.FileName) + "\\DATA\\WA_MRG.MRG";
                    btn_perform.IsEnabled = true;
                }
            }
        }
    }
}
