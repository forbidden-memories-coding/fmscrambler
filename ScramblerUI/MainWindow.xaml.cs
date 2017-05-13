using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FMScrambler.helper;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace FMScrambler
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void btn_loadiso_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog {Title = "Location of SLUS_014.11", Filter = "SLUS_014.11 | SLUS_014.11"};

 
            if (dlg.ShowDialog() == true)
            {
                Static.SLUSPath = dlg.FileName;
                lbl_path.Content = Path.GetDirectoryName(dlg.FileName);

                if (!File.Exists(Path.GetDirectoryName(dlg.FileName) + "\\DATA\\WA_MRG.MRG"))
                {
                    dlg.Title = "Location of WA_MRG.MRG";
                    dlg.Filter = "WA_MRG | WA_MRG.MRG";

                    if (dlg.ShowDialog() == true)
                    {
                        Static.WAPath = dlg.FileName;
                        btn_perform.IsEnabled = true;
                    }
                }
                else
                {
                    Static.WAPath = Path.GetDirectoryName(dlg.FileName) + "\\DATA\\WA_MRG.MRG";
                    btn_perform.IsEnabled = true;
                }
                Console.WriteLine("Load SL "+Static.SLUSPath);
                Console.WriteLine("Load WA " + Static.WAPath);
            }

 
        }

        public void setActionLabel(string text)
        {
            lbl_status.Content = text;
        }

        private void btn_perform_Click(object sender, RoutedEventArgs e)
        {
            sync_Scramble();

        }

        private void sync_Scramble()
        {
            var cardCount = (Static.glitchFusions) ? 1400 : 722;
            Static.setCardCount(cardCount);
            FileHandler fileHandler = new FileHandler();

            MessageBox.Show("Done scrambling, you may proceed with patching your game ISO now. A logfile was created in the tools directory as well.",
                       "Done scrambling.", MessageBoxButton.OK, MessageBoxImage.Information);
            fileHandler.LoadSlus(lbl_path.Content.ToString());
            fileHandler.ScrambleFusions(int.Parse(txt_seed.Text));

            btn_patchiso.IsEnabled = true;
            btn_perform.IsEnabled = false;

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txt_seed_Loaded(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();        
            txt_seed.Text = rnd.Next(10000, 214748364).ToString();
        }

        private void ch_glitch_Checked(object sender, RoutedEventArgs e)
        {
            Static.glitchFusions = true;
        }

        private void ch_glitch_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.glitchFusions = false;
        }

        private void ch_illegal_Unchecked(object sender, RoutedEventArgs e)
        {
            Static.highID = false;
        }

        private void ch_illegal_Checked(object sender, RoutedEventArgs e)
        {
            Static.highID = true;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            txt_seed.Text = rnd.Next(10000, 214748364).ToString();
        }

        private void btn_patchiso_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Did you make a backup copy of your Image file before patching? If not, do so before pressing OK.", "Backup Info",
                MessageBoxButton.OK, MessageBoxImage.Question);

            OpenFileDialog dlg = new OpenFileDialog { Title = "Forbidden Memories Image" };

            if (dlg.ShowDialog() == true)
            {
                Console.WriteLine(dlg.FileName);
                ImagePatcher patcher = new ImagePatcher(dlg.FileName);
                if (patcher.PatchImage() == 1)
                    MessageBox.Show("Image successfully patched! Have fun playing!", "Done patching.",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Error patching Image. Not Forbidden Memories or wrong version.", "Error patching.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
