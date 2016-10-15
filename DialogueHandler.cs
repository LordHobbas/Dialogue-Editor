using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Dialogue_Editor
{
	internal class DialogueHandler
	{
		#region initial
		internal enum MouseMode
		{
			None,
			Move,
			Drag,
			Connect,
			Pan,
			Selecting
		}

		internal MouseMode mouseMode = MouseMode.None;
		internal Point mouseStartPosition = new Point(0, 0);
		internal List<Dlg.Dialogue> dialogues = new List<Dlg.Dialogue>();
		internal MainWindow mw;

		private Canvas mapCanvas;

		internal DialogueHandler(MainWindow mw)
		{
			this.mw = mw;

			mapCanvas = mw.mapCanvas;
			mapCanvas.MouseMove += mapCanvas_MouseMove;
			mapCanvas.MouseDown += mapCanvas_MouseDown;
			mapCanvas.Drop += mainCanvas_Drop;
			mapCanvas.DragOver += MainCanvas_DragOver;
		}
		#endregion

		#region Events
		internal void DlgLineEntity_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (mouseMode == MouseMode.None)
			{
				//First deselct the previous selectedDLE
				deselect();

				DlgLineEntity.Selected = sender as DlgLineEntity;
				Dlg.Interaction.Selected = DlgLineEntity.Selected.interaction;
				DlgLineEntity.Selected.Stroke = System.Windows.Media.Brushes.Yellow;

				mouseStartPosition = Mouse.GetPosition(DlgLineEntity.Selected);
				mouseMode = MouseMode.Move;
				mw.tbMouseMode.Text = "MoveObject";
				mapCanvas.CaptureMouse();
			}
		}

		internal void DlgLineEntity_MouseEnter(object sender, MouseEventArgs e)
		{
			if (mouseMode == MouseMode.None)
			{
				DlgLineEntity convertedSender = sender as DlgLineEntity;
				convertedSender.ShowConnects();
			}
		}

		internal void DlgLineEntity_MouseLeave(object sender, MouseEventArgs e)
		{
			DlgLineEntity convertedSender = sender as DlgLineEntity;
			if (mouseMode == MouseMode.None)
			{
				convertedSender.HideConnects();
			}
		}

		private void mapCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (mouseMode == MouseMode.Move)
			{
				Point mPos = Mouse.GetPosition(mapCanvas);
				Point newPos = new Point(mPos.X - mouseStartPosition.X, mPos.Y - mouseStartPosition.Y);
				moveDlgLineEntity(DlgLineEntity.Selected, newPos);
			}
		}

		private void mapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			mw.focusedElement = mapCanvas;
			if (mouseMode == MouseMode.None && Mouse.RightButton == MouseButtonState.Pressed)
			{
				mouseMode = MouseMode.Pan;
				mw.tbMouseMode.Text = "Pan";
				mouseStartPosition = Mouse.GetPosition(mapCanvas);
				mapCanvas.CaptureMouse();
			}
			else if (mouseMode == MouseMode.None && Mouse.LeftButton == MouseButtonState.Pressed)
			{
				deselect();
			}
		}

		private void Arrow_MouseEnter(object sender, MouseEventArgs e)
		{
			if (Dlg.InteractionConnection.Selected != null && Dlg.InteractionConnection.Selected.arrow.arrowPoly == sender as Polygon) return;
			(sender as Polygon).Stroke = System.Windows.Media.Brushes.Yellow;
			(sender as Polygon).StrokeThickness = 1;
		}

		private void Arrow_MouseLeave(object sender, MouseEventArgs e)
		{
			if (Dlg.InteractionConnection.Selected != null && Dlg.InteractionConnection.Selected.arrow.arrowPoly == sender as Polygon) return;
			(sender as Polygon).Stroke = null;
		}

		private void Arrow_MouseDown(object sender, MouseButtonEventArgs e)
		{
			//First deselct all the other objects
			deselect();

			mouseMode = MouseMode.Selecting;
			mw.tbMouseMode.Text = "Selecting";
			Polygon p = sender as Polygon;
			Dlg.Arrow a = p.Tag as Dlg.Arrow;
			Dlg.InteractionConnection.Selected = a.interactionConnection;
			a.StrokeThickness = 2;
			a.Stroke = System.Windows.Media.Brushes.LimeGreen;
		}

		#endregion

		#region Drag & Drop
		//Creates a dlgVisualObject
		private void mainCanvas_Drop(object sender, DragEventArgs e)
		{
			//Sender is mainCanvas
			//if (e.Data.GetData(typeof(Dlg.Line)) != null)
			if (mouseMode == MouseMode.Drag)
			{
				Dlg.DlgLine droppedLine = e.Data.GetData(typeof(Dlg.DlgLine)) as Dlg.DlgLine;
				createDlgLineEntity(droppedLine, e.GetPosition(mapCanvas));
			}
			else if (mouseMode == MouseMode.Connect)
			{
				mapCanvas.Children.Remove(Dlg.InteractionConnection.Selected.arrow.arrowPoly);
				Dlg.InteractionConnection.Selected.arrow = null;

				mouseMode = MouseMode.None;
				mw.tbMouseMode.Text = "None";
			}
		}

		private void DlgLineEntityDrops_DragLeave(object sender, DragEventArgs e)
		{
			Rectangle convertedSender = (Rectangle)(sender);
			convertedSender.Opacity = 0;
		}

		//Enters DragDrop
		private void DlgLineEntityConnect_PreviewMouseDown(object sender, MouseEventArgs e)
		{
			Rectangle convertedConnecter = sender as Rectangle;
			if (convertedConnecter == null)
				return;
			mouseMode = MouseMode.Connect;
			mw.tbMouseMode.Text = "Connect";
			DlgLineEntity.Selected = convertedConnecter.Tag as DlgLineEntity;
			DlgLineEntity.Selected.HideConnects();

			Dlg.InteractionConnection.Selected = new Dlg.InteractionConnection();
			Dlg.InteractionConnection.Selected.startRect = convertedConnecter;
			Dlg.InteractionConnection.Selected.startInteraction = DlgLineEntity.Selected.interaction;

			Point relativePoint = GetRelativeLocation(convertedConnecter, mapCanvas);
			relativePoint.X += convertedConnecter.Width / 2;
			relativePoint.Y += convertedConnecter.Height / 2;
			Dlg.InteractionConnection.Selected.arrow = new Dlg.Arrow(relativePoint, mapCanvas);

			//currentConnection.arrow.StrokeThickness = 4;
			//currentConnection.arrow.Stroke = System.Windows.Media.Brushes.Black;
			Dlg.InteractionConnection.Selected.arrow.Fill = System.Windows.Media.Brushes.Black;
			Dlg.InteractionConnection.Selected.arrow.IsHitTestVisible = false;
			Dlg.InteractionConnection.Selected.arrow.Opacity = 0.6;

			DragDrop.DoDragDrop(convertedConnecter, new DataObject(DlgLineEntity.Selected), DragDropEffects.Move);

			Dlg.InteractionConnection.Selected = null;

			//Hide all drops and do more
			//Disable HitTestVisible for all interactionConnections
		}

		private void DlgLineEntityDrops_DragEnter(object sender, DragEventArgs e)
		{
			if (mouseMode != MouseMode.Connect) return;
			Rectangle convertedSender = (Rectangle)(sender);
			convertedSender.Opacity = 0.5;
		}

		private void DlgLineEntityDrops_Drop(object sender, DragEventArgs e)
		{
			if (mouseMode == MouseMode.Connect)
			{
				mouseMode = MouseMode.None;
				mw.tbMouseMode.Text = "None";

				Rectangle cr = sender as Rectangle;

				createConnection(cr);
			}
		}

		private void MainCanvas_DragOver(object sender, DragEventArgs e)
		{
			if (mouseMode == MouseMode.Connect)
			{
				Point mousePos = e.GetPosition(mapCanvas);
				//double angle = Math.Atan2(mousePos.X - currentConnection.arrow.X1, mousePos.Y - currentConnection.arrow.Y1);

				Dlg.InteractionConnection.Selected.arrow.endPoint = mousePos;
			}
		}

		#endregion

		#region Custom Methods

		private void createConnection(Rectangle connectTo)
		{
			Dlg.InteractionConnection.Selected.endRect = connectTo;
			Dlg.InteractionConnection.Selected.endInteraction = (connectTo.Tag as DlgLineEntity).interaction;

			Point dlgvPos = GetRelativeLocation(Dlg.InteractionConnection.Selected.endInteraction.dlgLineEntity, mapCanvas);
			Point crPos = Dlg.InteractionConnection.Selected.endInteraction.dlgLineEntity.lineConnectPos(connectTo);
			Dlg.InteractionConnection.Selected.arrow.endPoint = new Point(dlgvPos.X + crPos.X, dlgvPos.Y + crPos.Y);
			Dlg.InteractionConnection.Selected.arrow.IsHitTestVisible = true;
			Dlg.InteractionConnection.Selected.arrow.StrokeThickness = 2;
			Dlg.InteractionConnection.Selected.arrow.Opacity = 1;
			Dlg.InteractionConnection.Selected.arrow.interactionConnection = Dlg.InteractionConnection.Selected;

			connectTo.Opacity = 0;

			Dlg.InteractionConnection.Selected.startInteraction.interactionTo.Add(Dlg.InteractionConnection.Selected);
			Dlg.InteractionConnection.Selected.endInteraction.interactionFrom.Add(Dlg.InteractionConnection.Selected);

			Dlg.InteractionConnection.Selected.arrow.arrowPoly.MouseEnter += Arrow_MouseEnter;
			Dlg.InteractionConnection.Selected.arrow.arrowPoly.MouseLeave += Arrow_MouseLeave;
			Dlg.InteractionConnection.Selected.arrow.arrowPoly.MouseDown += Arrow_MouseDown;
		}

		private void deleteConnection()
		{

		}

		private void createDlgLineEntity(Dlg.DlgLine dlgLine, Point p)
		{
			//Create DlgLineEntity
			DlgLineEntity newDlgLineEntity = new DlgLineEntity(dlgLine);
			newDlgLineEntity.PreviewMouseDown += DlgLineEntity_PreviewMouseDown;
			newDlgLineEntity.MouseEnter += DlgLineEntity_MouseEnter;
			newDlgLineEntity.MouseLeave += DlgLineEntity_MouseLeave;

			//Create interaction
			Dlg.Interaction newInteraction = newDlgLineEntity.interaction;
			newInteraction.dlgLine = dlgLine;
			newInteraction.dlgLineEntity = newDlgLineEntity;
			newInteraction.dialogue = Dlg.Dialogue.Selected;

			Dlg.Dialogue.Selected.InteractionList.Add(newInteraction);
			mapCanvas.Children.Add(newDlgLineEntity);
			//newDlgLineEntity.Margin = new Thickness(p.X, p.Y, 0, 0);
			moveDlgLineEntity(newDlgLineEntity, p);

			#region drops and connects
			for (int i = 0; i < 8; i++)
			{
				newDlgLineEntity.dropArray[i].DragEnter += DlgLineEntityDrops_DragEnter;
				newDlgLineEntity.dropArray[i].DragLeave += DlgLineEntityDrops_DragLeave;
				newDlgLineEntity.dropArray[i].Drop += DlgLineEntityDrops_Drop;

				newDlgLineEntity.connectArray[i].PreviewMouseDown += DlgLineEntityConnect_PreviewMouseDown;
			}
			#endregion
		}

		private void moveDlgLineEntity(DlgLineEntity dle, Point moveTo)
		{
			Point newPos = SnapToGrid(moveTo, new Point(0, 0), new Point(mapCanvas.ActualWidth - dle.ActualWidth, mapCanvas.ActualHeight - dle.ActualHeight));
			dle.Margin = new Thickness(newPos.X, newPos.Y, 0, 0);

			foreach (Dlg.InteractionConnection ic in dle.interaction.interactionFrom)
			{
				//Changing End position
				Point rectPos = dle.lineConnectPos(ic.endRect);
				ic.arrow.endPoint = new Point(newPos.X + rectPos.X, newPos.Y + rectPos.Y);
			}

			foreach (Dlg.InteractionConnection ic in dle.interaction.interactionTo)
			{
				//Changing start position
				Point relPos = GetRelativeLocation(ic.startRect, mapCanvas);
				ic.arrow.startPoint = new Point(relPos.X + ic.startRect.ActualWidth / 2, relPos.Y + ic.startRect.ActualHeight / 2);
			}
		}

		private void deselect()
		{
			if (Dlg.InteractionConnection.Selected != null)
			{
				Dlg.InteractionConnection.Selected.arrow.Stroke = null;
				Dlg.InteractionConnection.Selected = null;
			}

			if (DlgLineEntity.Selected != null)
			{
				DlgLineEntity.Selected.Stroke = System.Windows.Media.Brushes.Black;
				DlgLineEntity.Selected = null;
			}
		}

		private void removeConnection(Dlg.InteractionConnection ic, bool ignoreInteractionTo = false, bool ignoreInteractionFrom = false)
		{
			Console.WriteLine("a");
			mapCanvas.Children.Remove(ic.arrow.arrowPoly); //remove visually
			if (!ignoreInteractionTo) ic.startInteraction.interactionTo.Remove(ic);
			if (!ignoreInteractionFrom) ic.endInteraction.interactionFrom.Remove(ic);
		}

		private void removeDlgLineEntity(DlgLineEntity dle)
		{
			Console.WriteLine("b");
			mapCanvas.Children.Remove(dle); //Remove visually
			Dlg.Interaction interaction = dle.interaction;
			interaction.dialogue.InteractionList.Remove(interaction); //Remove from Dialogue
			interaction.dlgLine.interactions.Remove(interaction); //Remove from Line

			//Remove interactionConnections
			foreach (Dlg.InteractionConnection ic in interaction.interactionFrom) { removeConnection(ic, false, true); }
			foreach (Dlg.InteractionConnection ic in interaction.interactionTo) { removeConnection(ic, true); }
		}

		internal Point SnapToGrid(Point position, Point lowestValue, Point highestValue)
		{
			position.X = Math.Max(lowestValue.X, position.X);
			position.Y = Math.Max(lowestValue.Y, position.Y);

			position.X = Math.Min(highestValue.X, position.X);
			position.Y = Math.Min(highestValue.Y, position.Y);

			double xSnap = position.X % 20;
			double ySnap = position.Y % 20;

			// If it's less than half the grid size, snap left/up 
			// (by subtracting the remainder), 
			// otherwise move it the remaining distance of the grid size right/down
			// (by adding the remaining distance to the next grid point).
			if (xSnap <= 20 / 2.0)
				xSnap *= -1;
			else
				xSnap = 20 - xSnap;
			if (ySnap <= 20 / 2.0)
				ySnap *= -1;
			else
				ySnap = 20 - ySnap;

			xSnap += position.X;
			ySnap += position.Y;

			return new Point(xSnap, ySnap);
		}

		internal void RemoveSelection()
		{
			//Is called from MainWindow.Window_KeyDown
			if (Dlg.InteractionConnection.Selected != null)
			{
				removeConnection(Dlg.InteractionConnection.Selected);
				Dlg.InteractionConnection.Selected = null;
			}
			else if (DlgLineEntity.Selected != null)
			{
				removeDlgLineEntity(DlgLineEntity.Selected);
				DlgLineEntity.Selected = null;
				Dlg.Interaction.Selected = null;
				Dlg.InteractionConnection.Selected = null;
			}
		}

		internal static Point GetRelativeLocation(UIElement target, UIElement relativeTo)
		{
			return target.TransformToAncestor(relativeTo).Transform(new Point(0, 0));
		}

		#endregion
	}
}
