using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Snippets
{
    public partial class Startup : Window
    {
        #region Fields and Properties
        private Border _activeCard = new Border();
        private List<Border> _cards = new List<Border>();
        private List<Border> _dialogs = new List<Border>();
        private List<Snippet> _snippets = new List<Snippet>();

        private DirectoryInfo _appPath;

        public Border ActiveCard { get => _activeCard; set => _activeCard = value; }
        public List<Border> Cards { get => _cards; set => _cards = value; }
        public List<Border> Dialogs { get => _dialogs; set => _dialogs = value; }
        public List<Snippet> Snippets { get => _snippets; set => _snippets = value; }
                
        public DirectoryInfo AppPath
        {
            get
            {
                if (_appPath == null)
                {
                    _appPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                }
                return _appPath;
            }
            set => _appPath = value;
        }

        public string SnippetDefinitionsFile { get => $"{AppPath}{Path.DirectorySeparatorChar}{ConfigurationManager.AppSettings["SnippetFolder"]}{Path.DirectorySeparatorChar}snippet.definitions"; }


        #endregion

        public Startup()
        {
            InitializeComponent();
            LoadCards();
            LoadDialogs();
            LoadSnippets();
            LoadProject();
            ShowCard(uiSnippetsCard);
            uiFooterSearchText.Focus();
        }

        #region Class Methods
        private void LoadCards()
        {
            Cards.Clear();
            Cards.Add(uiSnippetsCard);
        }

        private void ShowCard(Border card)
        {
            HideDialogs();
            HideCards();
            ActiveCard = card;
            card.Visibility = Visibility.Visible;
            PostShowCard();
        }

        private void HideCards()
        {
            foreach (var card in Cards)
            {
                card.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadDialogs()
        {
            Dialogs.Clear();
            Dialogs.Add(uiFinished);
        }

        private void ShowDialog(Border dialog)
        {
            HideDialogs();
            HideCards();
            dialog.Visibility = Visibility.Visible;
        }

        private void PostShowCard()
        {
            switch (ActiveCard.Name)
            {
                case "uiSnippetsCard":
                    uiFooterSearch.IsEnabled = true;
                    uiFooterSearchText.IsEnabled = true;
                    break;
                default:
                    uiFooterSearch.IsEnabled = false;
                    uiFooterSearchText.IsEnabled = false;
                    break;
            }
        }

        private void HideDialogs()
        {
            uiContent.Effect = null;
            uiContent.IsEnabled = true;
            foreach (var dialog in Dialogs)
            {
                dialog.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadSnippets()
        {
            Snippets = Snippet.ReadCollection(new FileInfo(SnippetDefinitionsFile));
            RefreshSnippets();
        }

        private void BuildNewGrid(int rows, int columns, Grid grid)
        {
            ClearGridDefinitions(grid);
            AddGridDefinitions(rows, columns, grid);

            for (int row = 0; row < rows; row++)
            {
                AddRowToGrid(row, grid);
            }

            var nextRow = 0;
            for (int column = 0; column < columns; column++)
            {
                AddColumnToGrid(column, nextRow, grid);
                nextRow++;
            }
        }

        private void ClearGridDefinitions(Grid grid)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }

        private void AddGridDefinitions(int rows, int columns, Grid grid)
        {
            var rowCount = columns > rows ? columns : rows;
            for (int row = 0; row < rowCount; row++)
            {
                var height = row % 2 == 0 ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
                grid.RowDefinitions.Add(new RowDefinition { Height = height });
            }

            for (int column = 0; column < 4; column++)
            {
                var width = column % 2 == 0 ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
            }
        }

        private void AddRowToGrid(int row, Grid grid)
        {
            var rowLabel = new Label { Content = $"Row {row + 1}", Style = App.Current.FindResource("ContentLabel") as Style };
            rowLabel.SetValue(Grid.RowProperty, row);
            rowLabel.SetValue(Grid.ColumnProperty, 0);
            grid.Children.Add(rowLabel);

            var rowCheckBox = new CheckBox { Content = $"Auto Height", IsChecked = true };
            rowCheckBox.SetValue(Grid.RowProperty, row);
            rowCheckBox.SetValue(Grid.ColumnProperty, 1);
            grid.Children.Add(rowCheckBox);
        }

        private void AddColumnToGrid(int column, int row, Grid grid)
        {
            var columnLabel = new Label { Content = $"Column {column + 1}", Style = App.Current.FindResource("ContentLabel") as Style };
            columnLabel.SetValue(Grid.RowProperty, row);
            columnLabel.SetValue(Grid.ColumnProperty, 2);
            grid.Children.Add(columnLabel);

            var columnCheckBox = new CheckBox { Content = $"Auto Width", IsChecked = true };
            columnCheckBox.SetValue(Grid.RowProperty, row);
            columnCheckBox.SetValue(Grid.ColumnProperty, 3);
            grid.Children.Add(columnCheckBox);
        }

        private void FakePause(int wait = 0)
        {
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(wait > 0 ? wait : 500);
            dispatcherTimer.Tick += ConfirmationVisualTimer_Tick;
            dispatcherTimer.Start();
        }

        private void LoadProject() { }
        #endregion

        #region Event Handlers
        private void uiPrimaryMenu_Snippets_Click(object sender, RoutedEventArgs e)
        {
            RefreshSnippets();
            ShowCard(uiSnippetsCard);
        }

        private void RefreshSnippets()
        {
            uiSnippetList.Children.Clear();
            var category = "";
            foreach (var snippet in Snippets.OrderByDescending(x => x.Category).ThenBy(x => x.Name).ToList())
            {
                if (string.IsNullOrWhiteSpace(category) || category != snippet.Category)
                {
                    category = snippet.Category;
                    var categoryLabel = new Label { Content = category, Style = App.Current.FindResource("GroupLabel") as Style };
                    uiSnippetList.Children.Add(categoryLabel);
                }
                var button = new Button { Content = $"{snippet.Name} - {snippet.Description}", Tag = snippet, Style = App.Current.FindResource("ButtonList") as Style };
                button.Click += SnippetButton_Click;
                uiSnippetList.Children.Add(button);
            }
            uiScrollGrid.IsEnabled = Snippets.Where(x => x.Name == "Border Grid").Any();
        }

        private void SnippetButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDialog(uiFinished);
            var snippet = (sender as Button).Tag as Snippet;
            Clipboard.SetText(snippet.GetContent(Path.GetDirectoryName(SnippetDefinitionsFile)));
            FakePause();
        }

        private void ConfirmationVisualTimer_Tick(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer;
            timer.Stop();
            ShowCard(ActiveCard);
        }

        private void uiPrimaryMenu_Classes_Click(object sender, RoutedEventArgs e) { }

        private void uiPrimaryMenu_Settings_Click(object sender, RoutedEventArgs e) { }

        private void uiFooterButton_Click(object sender, RoutedEventArgs e) { }

        private void uiOK_Click(object sender, RoutedEventArgs e)
        {
            ShowCard(ActiveCard);
        }

        private void uiCreateProject_Click(object sender, RoutedEventArgs e)
        {
            #region Create Directories
            //Directory.CreateDirectory(solutionPath.FullName);
            //Directory.CreateDirectory(projectPath.FullName);
            //Directory.CreateDirectory(targetThemePath.FullName);
            //Directory.CreateDirectory(targetFontPath.FullName);
            #endregion

            #region Load Template Files
            //var windowXAMLTemplate = new FileInfo($"{appTemplatePath}{Path.DirectorySeparatorChar}LlamaSoft.NewProject.Window.xaml.snippet");
            //var windowXAMLCodeTemplate = new FileInfo($"{appTemplatePath}{Path.DirectorySeparatorChar}LlamaSoft.NewProject.Window.xaml.cs.snippet");
            //var appXAMLTemplate = new FileInfo($"{appTemplatePath}{Path.DirectorySeparatorChar}LlamaSoft.NewProject.App.xaml.snippet");
            //var appXAMLCodeTemplate = new FileInfo($"{appTemplatePath}{Path.DirectorySeparatorChar}LlamaSoft.NewProject.App.xaml.cs.snippet");
            #endregion

            #region Declare Generated Files
            //var windowXAMLFile = new FileInfo($"{solutionPath}{Path.DirectorySeparatorChar}{uiWindowName.Text}.xaml");
            //var windowXAMLCodeFile = new FileInfo($"{solutionPath}{Path.DirectorySeparatorChar}{uiWindowName.Text}.xaml.cs");
            //var appXAMLFile = new FileInfo($"{solutionPath}{Path.DirectorySeparatorChar}App.xaml");
            //var appXAMLCodeFile = new FileInfo($"{solutionPath}{Path.DirectorySeparatorChar}App.xaml.cs");
            #endregion                       

            #region Replace Tokens in Generated Files
            //var windowXAMLContent = ReplaceTokens(windowXAMLTemplate.FullName);
            //var windowXAMLCodeContent = ReplaceTokens(windowXAMLCodeTemplate.FullName);
            //var appXAMLContent = ReplaceTokens(appXAMLTemplate.FullName);
            //var appXAMLCodeContent = ReplaceTokens(appXAMLCodeTemplate.FullName);
            #endregion

            #region Save Generated Files
            //File.WriteAllLines(windowXAMLFile.FullName, windowXAMLContent);
            //File.WriteAllLines(windowXAMLCodeFile.FullName, windowXAMLCodeContent);
            //File.WriteAllLines(appXAMLFile.FullName, appXAMLContent);
            //File.WriteAllLines(appXAMLCodeFile.FullName, appXAMLCodeContent);
            #endregion

            #region Move Resource Directories
            //CopyFiles(new DirectoryInfo($"{AppPath}{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}{ThemeFolder}"), targetThemePath);
            //CopyFiles(new DirectoryInfo($"{AppPath}{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}{FontFolder}"), targetFontPath);
            #endregion

            #region csproj Generation
            //UpdateProjectFile(appTemplatePath, solutionPath);
            #endregion

            FakePause();
        }

        private void uiFooterSearch_Click(object sender, RoutedEventArgs e)
        {
            var searchFor = uiFooterSearchText.Text.ToLower();
            var children = uiSnippetList.Children;
            foreach (var child in children)
            {
                if (child.GetType().Name != "Button") { continue; }
                var button = child as Button;
                button.Background = Brushes.White;
            }
                    
            Button firstButton = null;            
            foreach (var snippet in Snippets)
            {
                var snippetFile = snippet.SnippetFileName;
                if (snippet.Name.ToLower().Contains(searchFor) 
                    || snippet.Description.ToLower().Contains(searchFor)
                    || snippet.Category.ToLower().Contains(searchFor)
                    || snippet.GetContent(Path.GetDirectoryName(SnippetDefinitionsFile)).ToLower().Contains(searchFor)) 
                {
                    for (int i = 0; i < uiSnippetList.Children.Count; i++)
                    {
                        if (uiSnippetList.Children[i].GetType().Name != "Button") { continue; }
                        var button = uiSnippetList.Children[i] as Button;
                        var selected = button.Tag as Snippet;
                        if (selected.Name != snippet.Name) { continue; }
                        if (firstButton == null) { firstButton = button; }
                        button.Background = Brushes.LightBlue;
                    }
                }
                if(firstButton != null) { firstButton.BringIntoView(); }
            }
        }
        #endregion

        private void CancelNewSnippet()
        {
            uiName.Text = "";
            uiFileName.Text = "";
            uiDescription.Text = "";
            uiText.Text = "";
            uiCategory.SelectedIndex = 0;
        }

        private void uiCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelNewSnippet();
        }

        private void uiSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(uiName.Text) &&
                   !string.IsNullOrWhiteSpace(uiFileName.Text) &&
                   !string.IsNullOrWhiteSpace(uiDescription.Text) &&
                   !string.IsNullOrWhiteSpace(uiText.Text))
            {
                var newSnippet = new Snippet
                {
                    Name = uiName.Text,
                    Description = uiDescription.Text,
                    Text = uiText.Text,
                    Category = uiCategory.Text,
                    SnippetFileName = uiFileName.Text.Replace(" ", string.Empty).ToLower()
                };
                foreach(var bad in Path.GetInvalidFileNameChars())
                {
                    newSnippet.SnippetFileName = newSnippet.SnippetFileName.Replace(bad.ToString(), string.Empty);
                }
                
                newSnippet.SnippetFileName = newSnippet.BuildSnippetFileName();
                newSnippet.Save(SnippetDefinitionsFile);
                Snippets.Add(newSnippet);
                Snippet.SaveList(Snippets, new FileInfo(SnippetDefinitionsFile));
                CancelNewSnippet();
                RefreshSnippets();
                ShowCard(uiSnippetsCard);
            }
        }

        private void uiScrollGrid_Click(object sender, RoutedEventArgs e)
        {
            if(Snippets == null || Snippets.Count() == 0) { RefreshSnippets(); }

            ShowDialog(uiFinished);
            var snippet = Snippets.Where(x => x.Name == "Border Grid").FirstOrDefault();
            if(snippet == null) { return; }
            Clipboard.SetText(snippet.GetContent(Path.GetDirectoryName(SnippetDefinitionsFile)));
            FakePause();
        }
    }
}