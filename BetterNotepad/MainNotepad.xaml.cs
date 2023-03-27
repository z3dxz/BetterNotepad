using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Documents;

namespace BetterNotepad
{
    
    public partial class MainNotepad : Window
    {

        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        

        public string filepath = "";
        public string whenOpen = "";

        public MainNotepad()
        {
            InitializeComponent();
#pragma warning disable CS8622
            StateChanged += MainWindowStateChangeRaised;
#pragma warning restore CS8622

            IntPtr Handle = new WindowInteropHelper(this).EnsureHandle();



            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
            {
                _ = DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
            }


            NewFile();

            Console.WriteLine("Notepad in Console Mode");

            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i <= args.Length - 1; i++)
            {
                if (args[i].EndsWith("BetterNotepad.dll") == false && args[i].EndsWith("BetterNotepad.exe") == false)
                {
                    string path = args[i];
                    Console.WriteLine("Attempting to open " + path + "...");
                    try
                    {
                        OpenFile(path);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to open");
                        MessageBox.Show("Failed to open this file: " + filepath, "Failed");
                    }
                }
            }

        }


        public static int WordCount(string s)
        {
            return s.Split(new char[] { ' ' },
              StringSplitOptions.RemoveEmptyEntries).Length;
        }


        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            if (MainText.Text == whenOpen)
            {
                SystemCommands.CloseWindow(this);
            }
            else
            {
                
                SaveDialogWindow w = new();

                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                void clickno(object sender, RoutedEventArgs e)
                {

                    SystemCommands.CloseWindow(this);

                    SystemCommands.CloseWindow(w);

                }

                void clickyes(object sender, RoutedEventArgs e)
                {
                    SaveFile(false);

                    saveprompt1.Visibility = Visibility.Hidden;
                    w.ySave.Click -= clickyes;
                    w.nSave.Click -= clickno;
                    SystemCommands.CloseWindow(w);
                }

                void clickcancel(object sender, RoutedEventArgs e)
                {
                    w.ySave.Click -= clickyes;
                    w.nSave.Click -= clickno;
                    w.ByeByeButtons.Click -= clickcancel;
                    SystemCommands.CloseWindow(w);
                }

                w.ySave.Click += clickyes;

                w.nSave.Click += clickno;

                w.ByeByeButtons.Click += clickcancel;
                w.Show();
                


            }


        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainBorder.BorderThickness = new Thickness(1);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        void NewFile()
        {
            if (MainText.Text == whenOpen)
            {

                MainText.Text = "";
                filepath = "";
                LogoTextThing.Text = "Notepad";
                FileLabel.Text = "";
                whenOpen = "";
                updateLines();
            }
            else
            {

                SaveDialogWindow w = new();

                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                void clickno(object sender, RoutedEventArgs e)
                {

                    SystemCommands.CloseWindow(w);

                    saveprompt1.Visibility = Visibility.Hidden;
                    w.ySave.Click -= clickyes;
                    w.nSave.Click -= clickno;

                    MainText.Text = "";
                    filepath = "";
                    LogoTextThing.Text = "Notepad";
                    FileLabel.Text = "";
                    whenOpen = "";

                    updateLines();
                }

                void clickyes(object sender, RoutedEventArgs e)
                {
                    SaveFile(false);

                    saveprompt1.Visibility = Visibility.Hidden;
                    w.ySave.Click -= clickyes;
                    w.nSave.Click -= clickno;
                    SystemCommands.CloseWindow(w);
                }

                void clickcancel(object sender, RoutedEventArgs e)
                {
                    saveprompt1.Visibility = Visibility.Hidden;
                    w.ySave.Click -= clickyes;
                    w.nSave.Click -= clickno;
                    w.ByeByeButtons.Click -= clickcancel;
                    SystemCommands.CloseWindow(w);
                }

                w.ySave.Click += clickyes;

                w.nSave.Click += clickno;

                w.ByeByeButtons.Click += clickcancel;
                w.Show();
                

            }


        }

        private void NewF(object sender, RoutedEventArgs e)
        {
            NewFile();
        }


        private void SpellTB(object sender, RoutedEventArgs e)
        {
			
            if (!SpellCheck.GetIsEnabled(MainText))
            {
                SpellCheck.SetIsEnabled(MainText, true);
                spellb.Foreground = new SolidColorBrush(Colors.LightBlue);
            }
            else
            {
                SpellCheck.SetIsEnabled(MainText, false);
                spellb.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void OpenFile(string fname)
        {
            string bytes1 = File.ReadAllText(fname);

            filepath = fname;
            MainText.Text = bytes1;
            updateLines();
            FileLabel.Text = fname;
            LogoTextThing.Text = System.IO.Path.GetFileNameWithoutExtension(fname);
            whenOpen = bytes1;
        }

        private void OpenF(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new();
            if (openFileDialog.ShowDialog() == true)
            {
                long length = new System.IO.FileInfo(openFileDialog.FileName).Length;
                if (length > 200000)
                {

                    string messageBoxText = "The size of this file exceeds the recommended maximium and may take a while to open. Would you like to open it anyway?";
                    string caption = "Notepad Warning";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

                    if(result == MessageBoxResult.Yes)
                    {
                        OpenFile(openFileDialog.FileName);
                    }

                }
                else
                {
                    OpenFile(openFileDialog.FileName);
                }

            }

        }

        private async void SaveFile(bool sAs)
        {
            if (filepath != null && filepath != "" && sAs == false)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(MainText.Text);



                try
                {
                    File.WriteAllBytes(filepath, bytes);
                    saveBB.Text = "File saved successfully!";
                    saveBB.Foreground = new SolidColorBrush(Colors.White);
                }
                catch
                {
                    saveBB.Text = "Failed to save!";
                    saveBB.Foreground = new SolidColorBrush(Colors.Red);
                }

                OpenFile(filepath);

                filesaved.Margin = new Thickness(0, 0, 0, -50);
                await Task.Delay(100);
                filesaved.Margin = new Thickness(0, 0, 0, 40);
                await Task.Delay(2000);
                filesaved.Margin = new Thickness(0, 0, 0, -50);
            }
            else
            {
                Microsoft.Win32.SaveFileDialog dlg = new();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".txt"; // Default file extension
                dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;

                    byte[] bytes = Encoding.UTF8.GetBytes(MainText.Text);
                    try
                    {
                        File.WriteAllBytes(filename, bytes);
                        saveBB.Text = "File saved successfully!";
                        saveBB.Foreground = new SolidColorBrush(Colors.White);
                    }
                    catch
                    {
                        saveBB.Text = "Failed";
                        saveBB.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    filepath = filename;
                    FileLabel.Text = filepath;
                    whenOpen = MainText.Text;
                    LogoTextThing.Text = System.IO.Path.GetFileNameWithoutExtension(filename);

                    filesaved.Margin = new Thickness(0, 0, 0, -50);
                    await Task.Delay(100);
                    filesaved.Margin = new Thickness(0, 0, 0, 20);
                    await Task.Delay(2000);
                    filesaved.Margin = new Thickness(0, 0, 0, -50);
                }
            }
        }
        private void SaveF(object sender, RoutedEventArgs e)
        {

            SaveFile(false);
        }

        private void SaveFAs(object sender, RoutedEventArgs e)
        {

            SaveFile(true);
        }
        private void SmallF(object sender, RoutedEventArgs e)
        {
            MainText.FontSize = Math.Clamp(MainText.FontSize - 5, 1, 500);

            f1ontSize1.Text = MainText.FontSize.ToString();
        }
        private void BigF(object sender, RoutedEventArgs e)
        {
            MainText.FontSize = Math.Clamp(MainText.FontSize + 5, 1, 500);

            f1ontSize1.Text = MainText.FontSize.ToString();
        }

        private void FontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainText != null)
            {
                try
                {
                    float xxx = float.Parse(f1ontSize1.Text, CultureInfo.InvariantCulture.NumberFormat);
                    MainText.FontSize = Math.Clamp(xxx, 0, 500);
                }
                catch
                {
                    MainText.FontSize = 1;
                }

            }

        }



        private new void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                int zerooo = 0;

                Console.WriteLine(5 / zerooo);
            }


            else if (e.Delta < 0)
            {

            }

        }


        private void Window_Activated(object sender, EventArgs e)
        {
            MainText.Focus();
        }

        private void Undobb(object sender, RoutedEventArgs e)
        {
            MainText.Undo();
        }

        private void Redobb(object sender, EventArgs e)
        {
            MainText.Redo();
        }

        private void PrintB(object sender, RoutedEventArgs e)
        {
            PrintDialog dlg = new()
            {
                PageRangeSelection = PageRangeSelection.AllPages,
                UserPageRangeEnabled = false
            };
            if (dlg.ShowDialog() == true)
            {
                FlowDocument doc = new();
                // For normal A4&/letter pages, the defaults will print in two columns
                doc.ColumnWidth = dlg.PrintableAreaWidth;
                doc.Blocks.Add(new Paragraph(new Run(MainText.Text)));
                dlg.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Notepad Export");
            }
        }
        private void CutB(object sender, RoutedEventArgs e)
        {
            MainText.Cut();
        }

        private void CopyB(object sender, RoutedEventArgs e)
        {
            MainText.Copy();
        }
        private void PasteB(object sender, RoutedEventArgs e)
        {
            MainText.Paste();
        }

        private void WrapB(object sender, RoutedEventArgs e)
        {
            if (MainText.TextWrapping == TextWrapping.NoWrap)
            {
                MainText.TextWrapping = TextWrapping.Wrap;
                wraptext.Foreground = new SolidColorBrush(Colors.LightBlue);
            }
            else
            {
                MainText.TextWrapping = TextWrapping.NoWrap;
                wraptext.Foreground = new SolidColorBrush(Colors.White);
            }

        }

        private void FindB(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The 'Find and replace' feature will be added in version 5", "Notice", MessageBoxButton.OK);
            //FindReplace win3 = new FindReplace();


            //win3.ShowDialog();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        bool paperCLIP = false;

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {

                paperCLIP = true;
            }
            if (e.Key == Key.S && paperCLIP)
            {
                SaveFile(false);

                paperCLIP = false;
            }
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            paperCLIP = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainText.Text == whenOpen)
            {


            }
            else
            {

            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void FontB_Click(object sender, RoutedEventArgs e)
        {


            System.Windows.Forms.FontDialog fd = new();
            var result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Debug.WriteLine(fd.Font);

                MainText.FontFamily = new FontFamily(fd.Font.Name);
                MainText.FontWeight = fd.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                MainText.FontStyle = fd.Font.Italic ? FontStyles.Italic : FontStyles.Normal;

                TextDecorationCollection tdc = new();
                if (fd.Font.Underline) tdc.Add(TextDecorations.Underline);
                if (fd.Font.Strikeout) tdc.Add(TextDecorations.Strikethrough);
                MainText.TextDecorations = tdc;
            }

        }

        private void ColorB_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new();
            var result = cd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                MainText.Foreground = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            


        }

        private void searchBack_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            string temp = MainText.Text;
            MainText.Text = "";

            MainText.Text = temp;
            while (index < MainText.Text.LastIndexOf(findTxt.Text))
            {
                MainText.Focus();
                MainText.Select(index, findTxt.Text.Length);
                MainText.SelectionBrush = Brushes.Red;
                index = MainText.Text.IndexOf(findTxt.Text, index) + 1;
            }
            updateLines();
        }

        private void searchForward_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LowerSelectF(object sender, RoutedEventArgs e)
        {
            string selectedTxt = MainText.SelectedText;
            MainText.SelectedText = selectedTxt.ToLower();
        }
        private void UpperSelectF(object sender, RoutedEventArgs e)
        {
            string selectedTxt = MainText.SelectedText;
            MainText.SelectedText = selectedTxt.ToUpper();
        }

		private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                MainText.FontSize = Math.Clamp(MainText.FontSize + e.Delta / 50, 1, 500);

                f1ontSize1.Text = MainText.FontSize.ToString();

            }
        }

        void updateLines()
        {
            int numLines = MainText.Text.Split('\n').Length;

            wordbar.Text = MainText.Text.Length + " Characters | " + CountWords(MainText.Text) + " Words | " + numLines + " Lines";

        }

		private void MainText_KeyDown(object sender, KeyEventArgs e)
		{
            updateLines();
		}

		public static int CountWords(string test)
        {
            int count = 0;
            bool wasInWord = false;
            bool inWord = false;

            for (int i = 0; i < test.Length; i++)
            {
                if (inWord)
                {
                    wasInWord = true;
                }

                if (Char.IsWhiteSpace(test[i]))
                {
                    if (wasInWord)
                    {
                        count++;
                        wasInWord = false;
                    }
                    inWord = false;
                }
                else
                {
                    inWord = true;
                }
            }

            // Check to see if we got out with seeing a word
            if (wasInWord)
            {
                count++;
            }

            return count;
        }
    }
}
