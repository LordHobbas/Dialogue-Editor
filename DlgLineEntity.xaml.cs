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
	/// Interaction logic for DlgObjectBox.xaml
	/// </summary>
	public partial class DlgLineEntity : UserControl
	{
		//#pragma warning disable 0649 //Just disables the "Field XYZ is never assigned to, and will always have its default value XX" warning
		public static DlgLineEntity Selected;

		internal Dlg.Interaction interaction;

		internal Rectangle[] dropArray = new Rectangle[8];
		internal Rectangle[] connectArray = new Rectangle[8];
		//#pragma warning restore 0649

		internal DlgLineEntity(Dlg.DlgLine DlgLine)
		{
			InitializeComponent();

			interaction = new Dlg.Interaction();
			this.interaction.dlgLine = DlgLine;

			this.textBlockId.Text = DlgLine.id;
			this.textBlockValue.Text = DlgLine.text;

			connectArray[0] = NorthConnect;
			connectArray[1] = WestConnect;
			connectArray[2] = EastConnect;
			connectArray[3] = SouthConnect;
			connectArray[4] = NortEastConnect;
			connectArray[5] = NorthWestConnect;
			connectArray[6] = SouthWestConnect;
			connectArray[7] = SouthEastConnect;

			dropArray[0] = NorthDrop;
			dropArray[1] = WestDrop;
			dropArray[2] = EastDrop;
			dropArray[3] = SouthDrop;
			dropArray[4] = NorthWestDrop;
			dropArray[5] = NorthEastDrop;
			dropArray[6] = SouthWestDrop;
			dropArray[7] = SouthEastDrop;

			for(int i=0; i<8; i++)
			{
				connectArray[i].Tag = this;
				dropArray[i].Tag = this;
			}
		}

		internal void Refresh()
		{
			this.textBlockId.Text = interaction.dlgLine.id;
			this.textBlockValue.Text = interaction.dlgLine.text;
		}

		internal Brush Fill
		{
			get
			{
				return MainRectangle.Fill;
			}
			set
			{
				MainRectangle.Fill = value;
			}
		}

		internal Brush Stroke
		{
			get
			{
				return MainRectangle.Stroke;
			}
			set
			{
				MainRectangle.Stroke = value;
			}
		}

		internal void HideConnects()
		{
			foreach(Rectangle r in connectArray)
			{
				r.Visibility = Visibility.Hidden;
			}
		}

		internal void ShowConnects()
		{
			foreach (Rectangle r in connectArray)
			{
				r.Visibility = Visibility.Visible;
			}
		}

		internal Rectangle DropHover(Point p)
		{
			if (PointInRect(p, NorthDrop) )
			{
				return NorthDrop;
			}
			else if (PointInRect(p, SouthDrop))
			{
				return SouthDrop;
			}
			else if (PointInRect(p, EastDrop))
			{
				return EastDrop;
			}
			else if (PointInRect(p, WestDrop))
			{
				return WestDrop;
			}
			return null;
		}

		internal Point lineConnectPos(Rectangle r)
		{
			switch (r.Name)
			{
				case "NorthDrop":
					return new Point(this.ActualWidth/2, 0);
				case "SouthDrop":
					return new Point(this.ActualWidth/2, this.ActualHeight);
				case "EastDrop":
					return new Point(this.ActualWidth, this.ActualHeight/2);
				case "WestDrop":
					return new Point(0, this.ActualHeight/2);
				case "NorthEastDrop":
					return new Point(this.ActualWidth-3, 3);
				case "NorthWestDrop":
					return new Point(3, 3);
				case "SouthEastDrop":
					return new Point(this.ActualWidth-3, this.ActualHeight-3);
				case "SouthWestDrop":
					return new Point(3, this.ActualHeight-3);
				default:
					break;
			}

			return new Point();
		}

		private bool PointInRect(Point p, Rectangle r)
		{
			Point TopLeft = r.TransformToAncestor(this).Transform(new Point(0, 0)); //Get the relative point.
			Point BottomRight = new Point();

			if(r.HorizontalAlignment == HorizontalAlignment.Left | r.HorizontalAlignment == HorizontalAlignment.Right)
			{
				BottomRight.X = TopLeft.X + r.Width;
			}
			else
			{
				BottomRight.X = TopLeft.X + (this.Width - r.Margin.Left - r.Margin.Right);
			}

			if (r.VerticalAlignment == VerticalAlignment.Top | r.VerticalAlignment == VerticalAlignment.Bottom)
			{
				BottomRight.Y = TopLeft.Y + r.Height;
			}
			else
			{
				BottomRight.Y = TopLeft.Y + (this.Height - r.Margin.Top - r.Margin.Bottom);
			}

			if(p.X >= TopLeft.X && p.Y >= TopLeft.Y)
			{
				if (p.X <= BottomRight.X && p.Y <= BottomRight.Y)
				{
					return true;
				}
			}
			return false;
		}
	}
}
