using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _rowCount = 5;
        private int _columnCount = 5;
        public int RowCount { get => _rowCount; set => _rowCount = value; }
        public int ColumnCount { get => _columnCount; set => _columnCount = value; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            BuildNewGrid(RowCount, ColumnCount, uiDefinitionsGrid);
        }

        #region Class Methods
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

            for(int column = 0; column < 4; column++)
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
        
        private void UpdateGridDefinitions()
        {
            var numberOfRows = 0;
            var validRowCount = int.TryParse(uiRowCount.Text, out numberOfRows);

            var numberOfColumns = 0;
            var validColumnCount = int.TryParse(uiColumnCount.Text, out numberOfColumns);

            if(validRowCount && validColumnCount)
            {
                BuildNewGrid(numberOfRows, numberOfColumns, uiDefinitionsGrid);
            }
        }
        #endregion

        #region Event Handlers
        private void uiRowCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateGridDefinitions();
        }

        private void uiColumnCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateGridDefinitions();
        }

        private void uiGenCode_Click(object sender, RoutedEventArgs e)
        {
            var xamlTemplate = File.ReadAllText(@"C:\Users\mcoupland\source\repos\IAS\Code\WPFGen\Templates\DefaultWindowXAML.txt");
            var generatedXAML = xamlTemplate.Replace("~NAMESPACE~", uiProjectName.Text);
            generatedXAML = generatedXAML.Replace("~WINDOW~", uiWindowName.Text);
            var generatedXAMLFile = new FileInfo($@"C:\WPFGen\Generated\{uiProjectName.Text}\{uiWindowName.Text}.xaml");
            Directory.CreateDirectory(generatedXAMLFile.DirectoryName);
            File.WriteAllText(generatedXAMLFile.FullName, generatedXAML);

            var codeTemplate = File.ReadAllText(@"C:\Users\mcoupland\source\repos\IAS\Code\WPFGen\Templates\DefaultWindowCode.txt");
            var generatedCode = codeTemplate.Replace("~NAMESPACE~", uiProjectName.Text);
            generatedCode = generatedCode.Replace("~WINDOW~", uiWindowName.Text);
            var generatedCodeFile = new FileInfo($@"C:\WPFGen\Generated\{uiProjectName.Text}\{uiWindowName.Text}.xaml.cs");
            Directory.CreateDirectory(generatedCodeFile.DirectoryName);
            File.WriteAllText(generatedCodeFile.FullName, generatedCode);
        }
        #endregion
    }
}
