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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dialogue_Editor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// object.TransformToAncestor(relative to).Transform(new Point(0, 0)); //Get the relative point for the startPoint

		#region initial
		private Dictionary<string, TreeViewItem> treeViewDlgSource;
		internal Dictionary<string, Dlg.DlgCharacter> characterDictionary; //Key is name of character
		internal Dictionary<string, Dlg.DlgLine> lineDictionary; //Key is id of Line
		internal Dictionary<string, Dlg.DlgAction> actionDictionary; //Key is name of action

		internal UIElement focusedElement = null;

		private DialogueHandler DH;

		public MainWindow()
		{
			InitializeComponent();
			this.MouseUp += Main_PreviewMouseUp;

			treeViewDlgSource = new Dictionary<string, TreeViewItem>();
			treeViewDlg.ItemsSource = treeViewDlgSource.Values;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			characterDictionary = new Dictionary<string, Dlg.DlgCharacter>();
			lineDictionary = new Dictionary<string, Dlg.DlgLine>();
			actionDictionary = new Dictionary<string, Dlg.DlgAction>();

			DH = new DialogueHandler(this);
			DH.dialogues.Add(new Dlg.Dialogue());
			listBoxDialogues.ItemsSource = DH.dialogues;
		}

		#endregion

		#region Global
		//###################### CONTROLS ######################
		private void Main_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (DH.mouseMode != DialogueHandler.MouseMode.None)
			{
				DH.mouseMode = DialogueHandler.MouseMode.None;
				if(Mouse.Captured != null) Mouse.Captured.ReleaseMouseCapture();
				tbMouseMode.Text = "None";
			}
		}

		private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9,-]+"); //regex that matches disallowed text
			e.Handled = regex.IsMatch(e.Text);
		}

		private void treeViewDlg_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if(treeViewDlg.SelectedItem != null)
			{
				focusedElement = treeViewDlg;
			}
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Delete:
					if (focusedElement == mapCanvas)
					{
						DH.RemoveSelection();
					}
					else if (focusedElement == treeViewDlg)
					{
						//Remove DlgLine or Character
					}
					break;
				default:
					break;
			}
		}

		private void buttonSaveInfoPane_Click(object sender, RoutedEventArgs e)
		{
			//bool saved = true;
			Dlg.Dialogue.Selected.dimensions.Height = Double.Parse(MapInfoTbX.Text);
			Dlg.Dialogue.Selected.dimensions.Width = Double.Parse(MapInfoTbY.Text);

			loadMap();
		}

		//###################### METHODS ######################
		private void reloadTreeView()
		{
			treeViewDlgSource.Clear();

			foreach (Dlg.DlgCharacter c in characterDictionary.Values)
			{
				TreeViewItem cItem = new TreeViewItem();
				cItem.Tag = c;
				cItem.Header = "☺ " + c.ToString();

				foreach (Dlg.DlgLine l in c.dlgLines.Values)
				{
					TreeViewItem lItem = new TreeViewItem();
					lItem.Tag = l;
					lItem.Header = l.ToString();
					cItem.Items.Add(lItem);
				}
				cItem.IsExpanded = true;

				treeViewDlgSource.Add(c.name, cItem);
			}
			treeViewDlg.ItemsSource = treeViewDlgSource.Values;
		}
		#endregion

		#region Dialogue Map
		//###################### CONTROLS ######################
		private void listBoxDialogues_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			loadMap();
		}

		private void mapCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if(DH.mouseMode == DialogueHandler.MouseMode.Pan)
			{
				Point newPos = Mouse.GetPosition(mapCanvas);
				newPos.X -= DH.mouseStartPosition.X;
				newPos.Y -= DH.mouseStartPosition.Y;

				scrollViewerDlg.ScrollToHorizontalOffset(scrollViewerDlg.HorizontalOffset - newPos.X);
				scrollViewerDlg.ScrollToVerticalOffset(scrollViewerDlg.VerticalOffset - newPos.Y);
			}
		}

		private void treeViewDlg_MouseMove(object sender, MouseEventArgs e)
		{
			if (DH.mouseMode == DialogueHandler.MouseMode.Drag)
			{
				Point position = e.GetPosition(null);
				if (Math.Abs(position.X - DH.mouseStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - DH.mouseStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					DH.mouseMode = DialogueHandler.MouseMode.Drag;
					tbMouseMode.Text = "DragObject";
					DataObject data = new DataObject(((TreeViewItem)treeViewDlg.SelectedItem).Tag);
					DragDropEffects de = DragDrop.DoDragDrop(this.treeViewDlg, data, DragDropEffects.Move);
					DH.mouseMode = DialogueHandler.MouseMode.None;
					tbMouseMode.Text = "None";
				}
			}
		}

		private void DlgLineTreeViewItem_MouseDown(object sender, MouseEventArgs e)
		{
			Dlg.DlgLine selectedLine = ((TreeViewItem)sender).Tag as Dlg.DlgLine;
			if (DH.mouseMode == DialogueHandler.MouseMode.None)
			{
				DH.mouseStartPosition = Mouse.GetPosition(null);
				DH.mouseMode = DialogueHandler.MouseMode.Drag;
				tbMouseMode.Text = "DragObject";
			}
		}

		//###################### METHODS ######################
		private void loadMap()
		{
			if (listBoxDialogues.SelectedIndex != -1)
			{
				if(Dlg.Dialogue.Selected != null)
				{
					foreach (Dlg.Interaction interaction in Dlg.Dialogue.Selected.InteractionList)
					{
						mapCanvas.Children.Remove(interaction.dlgLineEntity);
						foreach(Dlg.InteractionConnection nextInteraction in interaction.interactionTo)
						{
							mapCanvas.Children.Remove(nextInteraction.arrow.arrowPoly);
						}
					}
				}

				Dlg.Dialogue.Selected = listBoxDialogues.SelectedItem as Dlg.Dialogue;
				DialogueWarning.Visibility = Visibility.Hidden;
				scrollViewerDlg.IsEnabled = true;
				scrollViewerDlg.Opacity = 1d;
				tbLoadedDialogue.Text = Dlg.Dialogue.Selected.name;

				MapInfoTbX.Text = Dlg.Dialogue.Selected.dimensions.Width.ToString();
				MapInfoTbY.Text = Dlg.Dialogue.Selected.dimensions.Height.ToString();

				mapCanvas.Width = Dlg.Dialogue.Selected.dimensions.Width;
				mapCanvas.Height = Dlg.Dialogue.Selected.dimensions.Height;

				listBoxDialogueInteractions.Items.Clear();
				foreach (Dlg.Interaction interaction in Dlg.Dialogue.Selected.InteractionList)
				{
					listBoxDialogueInteractions.Items.Add(interaction);
					mapCanvas.Children.Add(interaction.dlgLineEntity);
					foreach(Dlg.InteractionConnection ic in interaction.interactionTo)
					{
						mapCanvas.Children.Add(ic.arrow.arrowPoly);
					}
				}
			}
			else
			{
				Dlg.Dialogue.Selected = null;
				DialogueWarning.Visibility = Visibility.Hidden;
				scrollViewerDlg.IsEnabled = false;
				scrollViewerDlg.Opacity = 0.5d;
				tbLoadedDialogue.Text = "None";
			}
		}

		#endregion

		#region Line
		//###################### CONTROLS ######################
		private void menuNewLine_Click(object sender, RoutedEventArgs e)
		{
			Dlg.DlgLine newLine = createNewLine();
		}

		//###################### METHODS ######################
		private Dlg.DlgLine createNewLine()
		{
			WindowNewLine wnl = new WindowNewLine(this);
			if (wnl.ShowDialog() == true)
			{
				lineDictionary.Add(wnl.newLine.id, wnl.newLine);

				TreeViewItem lItem = new TreeViewItem();
				lItem.Header = wnl.newLine.ToString();
				lItem.Tag = wnl.newLine;
				lItem.PreviewMouseDown += DlgLineTreeViewItem_MouseDown; //Måste vara preview; vanlig anropas aldrig
				treeViewDlgSource[wnl.newLine.character.name].Items.Add(lItem);
				treeViewDlg.Items.Refresh();

				return wnl.newLine;
			}
			return null;
		}

		#endregion

		#region Character
		//###################### CONTROLS ######################
		private void menuNewCharacter_Click(object sender, RoutedEventArgs e)
		{
			newCharacter();
		}

		//###################### METHODS ######################
		private void newCharacter()
		{
			Dlg.DlgCharacter newCharacter = new Dlg.DlgCharacter();
			if (characterDictionary.Keys.Contains(newCharacter.name))
			{
				int i = 1;
				while (characterDictionary.Keys.Contains(newCharacter.name + i.ToString()))
				{
					i++;
				}
				newCharacter.name += i.ToString();
			}
			characterDictionary.Add(newCharacter.name, newCharacter);

			TreeViewItem characterNode = new TreeViewItem();
			characterNode.Header = "☺ " + newCharacter.ToString();
			characterNode.Tag = newCharacter;

			treeViewDlgSource.Add(newCharacter.name, characterNode);
			treeViewDlg.Items.Refresh();

			//comboBoxCharacter.Items.Add(newCharacter.name);

			//Loop through all lineforms

			//toolStripStatusLabel1.Text = "Character Created";
		}
		#endregion

		private void menuNewDialogue_Click(object sender, RoutedEventArgs e)
		{
			Dlg.Dialogue newDlg = new Dlg.Dialogue();
			newDlg.name = "New Dialogue";
			newDlg.dimensions.Width = 500;
			newDlg.dimensions.Height = 500;

			DH.dialogues.Add(newDlg);
			listBoxDialogues.Items.Refresh();
		}
	}
}
