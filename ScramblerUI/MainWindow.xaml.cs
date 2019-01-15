using System;
using System.Globalization;
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

                btn_patchiso.IsEnabled = false;
                btn_perform.IsEnabled = true;
            }      
        }

        private void btn_perform_Click(object sender, RoutedEventArgs e)
        {
            SyncScramble();
        }

        private void SyncScramble()
        {
            int cardCount = Static.GlitchFusions ? 1400 : 722;
            Static.SetCardCount(cardCount);

            DataScrambler fileHandler = new DataScrambler(int.Parse(txt_seed.Text));

            Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    fileHandler.PerformScrambling((int) txt_minAtk.Value, (int) txt_maxAtk.Value,
                        (int) txt_minDef.Value, (int) txt_maxDef.Value, (int) txt_minCost.Value,
                        (int) txt_maxCost.Value, (int) txt_minDropRate.Value, (int) txt_maxDropRate.Value);
                });

            MessageBox.Show("Done scrambling, you may proceed with patching your game ISO now." + (Static.Spoiler ? " Spoiler files were generated as well" : ""),
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

        private void btn_patchiso_Click(object sender, RoutedEventArgs e)
        {
            if (!Static.UsedIso)
            {
                MessageBox.Show("Did you make a backup copy of your Image file before patching? If not, do so before pressing OK.", "Backup Info",
                    MessageBoxButton.OK, MessageBoxImage.Question);

                OpenFileDialog dlg = new OpenFileDialog { Title = "Forbidden Memories Image" };

                if (dlg.ShowDialog() == true)
                {
                    DoPatch(dlg.FileName);
                }
            }
            else
            {
                DoPatch(Static.IsoPath);
            }
            pgr_back.Visibility = Visibility.Hidden;
        }

        private async void DoPatch(string path)
        {
#if DEBUG
            Console.WriteLine(path);
#endif
            pgr_back.Visibility = Visibility.Visible;
            ImagePatcher patcher = new ImagePatcher(path);
            Static.IsoPath = path;
            int patchResult = await Task.Run(() => patcher.PatchImage());

            if (patchResult == 1)
            {
                MessageBox.Show("Image successfully patched! Have fun playing! Location of Randomized Image: " + Static.IsoPath, "Done patching.",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Process.Start(Directory.GetParent(Static.IsoPath).FullName);

                //Allow scrambling again
                btn_perform.IsEnabled = true;
                btn_patchiso.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Error patching Image. Not Forbidden Memories or wrong version.", "Error patching.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txt_seed_Initialized(object sender, EventArgs e)
        {
            txt_seed.Text = _rnd.Next(10000, 214748364).ToString();
            _prevSeedText = txt_seed.Text;
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
            LabelUpdateSeed();
        }

        private void LabelUpdateSeed()
        {
            if (lbl_isoExample != null)
            {
                var content = (string)lbl_isoExample.Content;
                var offset = content.IndexOf('[') + 1;
                content = content.Remove(offset, content.IndexOf(']') - offset);
                content = content.Insert(offset, txt_seed.Text);
                lbl_isoExample.Content = content;
            }
        }
    
        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            Title = $"YGO! FM Fusion Scrambler Tool - {Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} {Meta.VersionInfo}";
            lbl_isoExample.Content = $"fmscrambler[{txt_seed.Text}].bin";

            InitCharTable();
        }

        private void InitCharTable()
        {
            var table_path = @"./CharacterTable.txt";

            if (!File.Exists(table_path))
            {
                MessageBox.Show("CharacterTable.txt not found! Provide a path for it!", "Unable to find CharacterTable.txt", MessageBoxButton.OK, MessageBoxImage.Error);
                OpenFileDialog ofd = new OpenFileDialog { Title = "CharacterTable file", Filter = "CharacterTable.txt|CharacterTable.txt" };
                if (ofd.ShowDialog() == true)
                {
                    table_path = ofd.FileName;
                }
                else
                {
                    Close();
                    return;
                }
            }

            StringReader strReader = new StringReader(File.ReadAllText(table_path));

            string input;

            while ((input = strReader.ReadLine()) != null)
            {
                Match match = Regex.Match(input, "^([A-Fa-f0-9]{2})\\=(.*)$");

                if (!match.Success)
                {
                    continue;
                }

                char k1 = Convert.ToChar(match.Groups[2].ToString());
                byte k2 = (byte)int.Parse(match.Groups[1].ToString(), NumberStyles.HexNumber);

                Static.Dict.Add(k2, k1);

                if (!Static.RDict.ContainsKey(k1))
                {
                    Static.RDict.Add(k1, k2);
                }
            }
            //There should be 85 entries otherwise file got corrupted, misread or user manually provided a bad file
            if (Static.Dict.Values.Count != 85)
            {
                MessageBox.Show("Provided CharacterTable.txt is incorrect or incomplete!", "Error reading CharacterTable.txt", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void btn_loadiso1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Title = "Location of SLUS_014.11", Filter = "SLUS_014.11|SLUS_014.11" };

            if (dlg.ShowDialog() == true)
            {
                Static.SlusPath = dlg.FileName;
                lbl_path.Content = Path.GetDirectoryName(dlg.FileName);

                if (!File.Exists(Path.GetDirectoryName(dlg.FileName) + "\\DATA\\WA_MRG.MRG"))
                {
                    dlg.Title = "Location of WA_MRG.MRG";
                    dlg.Filter = "WA_MRG|WA_MRG.MRG";

                    if (dlg.ShowDialog() == true)
                    {
                        Static.WaPath = dlg.FileName;
                        btn_patchiso.IsEnabled = false;
                        btn_perform.IsEnabled = true;
                        Static.UsedIso = false;
                    }
                }
                else
                {
                    Static.WaPath = Path.GetDirectoryName(dlg.FileName) + "\\DATA\\WA_MRG.MRG";
                    btn_patchiso.IsEnabled = false;
                    btn_perform.IsEnabled = true;
                    Static.UsedIso = false;
                }
            }
        }

        private void grp_atkdef_MouseUp(object sender, MouseButtonEventArgs e)
        {
#if DEBUG
            Console.WriteLine($"{txt_minAtk.Value} - {txt_maxAtk.Value} || {txt_minDef.Value} - {txt_maxDef.Value}");
#endif
        }
    }
}
