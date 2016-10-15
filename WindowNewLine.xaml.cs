using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Dialogue_Editor
{
	/// <summary>
	/// Interaction logic for WindowNewLine.xaml
	/// </summary>
	public partial class WindowNewLine : Window
	{
		private MainWindow mw;
		internal Dlg.DlgLine newLine;

		public WindowNewLine(MainWindow mw)
		{
			InitializeComponent();
			this.mw = mw;
		}

		private void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			foreach(Dlg.DlgCharacter c in mw.characterDictionary.Values)
			{
				cbCharacter.Items.Add(c.name);
			}
		}

		private void buttonNew_Click(object sender, RoutedEventArgs e)
		{
			if(cbCharacter.SelectedIndex != -1)
			{
				newLine = new Dlg.DlgLine();
				newLine.character = mw.characterDictionary[(string)cbCharacter.SelectedItem];
				newLine.text = textBoxLine.Text;
				newLine.id = newLine.character.name + "_" + newLine.character.lineIndex.ToString();
				newLine.character.lineIndex++;
				newLine.character.dlgLines.Add(newLine.id, newLine);
				this.DialogResult = true;
			}
			else
			{
				MessageBox.Show("Please select a character before creating a new Line");
			}
		}

		private void buttonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}
