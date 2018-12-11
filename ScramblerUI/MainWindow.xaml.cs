using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using FMLib.Disc;
using FMLib.Randomizer;
using FMLib.Utility;
using Microsoft.Win32;

namespace FMScrambler
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool _isPasteEvent = false;
        private string _prevSeedText;
        private readonly Random _rnd = new Random();
 

        public MainWindow()
        {
            InitializeComponent();
        }


        // Randomizing via Game Image
        private async void btn_loadiso_Click(object sender, RoutedEventArgs e)
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
                await Task.Run(() => chunker.ExtractBin(dlg.FileName));

                pgr_back.Visibility = Visibility.Hidden;
                MessageBox.Show("Extracting game data complete.", "Extracting data",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Static.UsedIso = true;

                btn_loadiso.IsEnabled = false;
                btn_loadiso1.IsEnabled = false;
                btn_perform.IsEnabled = true;
            }      
        }

        private void btn_perform_Click(object sender, RoutedEventArgs e)
        {
            sync_Scramble();
        }

        private void sync_Scramble()
        {
            int cardCount = Static.GlitchFusions ? 1400 : 722;
            Static.SetCardCount(cardCount);

            DataScrambler fileHandler = new DataScrambler(int.Parse(txt_seed.Text));

            Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    fileHandler.PerformScrambling((int) txt_minAtk.Value, (int) txt_maxAtk.Value,
                        (int) txt_minDef.Value, (int) txt_maxDef.Value);
                });

            MessageBox.Show("Done scrambling, you may proceed with patching your game ISO now. A logfile was created in the tools directory as well.",
                "Done scrambling.", MessageBoxButton.OK, MessageBoxImage.Information);
            btn_patchiso.IsEnabled = true;
            btn_perform.IsEnabled = false;

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            txt_seed.Text = _rnd.Next(10000, 214748364).ToString();
            txt_seed.Focus();
        }

        private async void btn_patchiso_Click(object sender, RoutedEventArgs e)
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
                    int patchResult = await Task.Run(() => patcher.PatchImage());

                    if (patchResult == 1)
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
                int patchImgResult = await Task.Run(() => patcher.PatchImage());
                if (patchImgResult == 1)
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
            txt_seed.Text = _rnd.Next(10000, 214748364).ToString();
            txt_seed.Focus();
        }

        private void txt_seed_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.V == e.Key && Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (((TextBox) sender).Name)
                {
                    case "txt_seed":
                        _prevSeedText = txt_seed.Text;
                        break;
                    case "txt_minAtk":
                        _prevSeedText = txt_minAtk.Value.ToString();
                        break;
                    case "txt_maxAtk":
                        _prevSeedText = txt_maxAtk.Value.ToString();
                        break;
                    case "txt_minDef":
                        _prevSeedText = txt_minDef.Value.ToString();
                        break;
                    case "txt_maxDef":
                        _prevSeedText = txt_maxDef.Value.ToString();
                        break;
                }

                _isPasteEvent = true;
            }
        }

        private void txt_seed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isPasteEvent)
            {
                switch (((TextBox)sender).Name)
                {
                    case "txt_seed":
                        txt_seed.Text = _prevSeedText;
                        break;
                    case "txt_minAtk":
                        _prevSeedText = txt_minAtk.Value.ToString();
                        break;
                    case "txt_maxAtk":
                        _prevSeedText = txt_maxAtk.Value.ToString();
                        break;
                    case "txt_minDef":
                        _prevSeedText = txt_minDef.Value.ToString();
                        break;
                    case "txt_maxDef":
                        _prevSeedText = txt_maxDef.Value.ToString();
                        break;
                }

                _isPasteEvent = false;
            }
            if (txt_seed.Text.StartsWith("0"))
            {
                switch (((TextBox)sender).Name)
                {
                    case "txt_seed":
                        txt_seed.Text = $"1{txt_seed.Text.Substring(1)}"; ;
                        break;
                    case "txt_minAtk":
                        txt_minAtk.Value = Int32.Parse($"1{txt_minAtk.Value.ToString().Substring(1)}"); ;
                        break;
                    case "txt_maxAtk":
                        txt_maxAtk.Value = Int32.Parse($"1{txt_maxAtk.Value.ToString().Substring(1)}"); ;
                        break;
                    case "txt_minDef":
                        txt_minDef.Value = Int32.Parse($"1{txt_minDef.Value.ToString().Substring(1)}"); ;
                        break;
                    case "txt_maxDef":
                        txt_maxDef.Value = Int32.Parse($"1{txt_maxDef.Value.ToString().Substring(1)}"); ;
                        break;
                }
            }
        }
    
        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            Title = $"YGO! FM Fusion Scrambler Tool - {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} {Meta.VersionInfo}";
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

        private void grp_atkdef_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"{txt_minAtk.Value} - {txt_maxAtk.Value} || {txt_minDef.Value} - {txt_maxDef.Value}");
        }
    }
}
