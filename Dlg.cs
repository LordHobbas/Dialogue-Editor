using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;

namespace Dialogue_Editor
{
	class Dlg
	{
#pragma warning disable 0649 //Just disables the "Field XYZ is never assigned to, and will always have its default value XX" warning
		public class DlgCharacter
		{
			public static int Count = 0;
			public static DlgCharacter Selected = null;

			public string name = "New Character";
			public Dictionary<string, DlgLine> dlgLines = new Dictionary<string, DlgLine>();
			public int lineIndex = 0;

			public DlgCharacter() { }
			private DlgCharacter(DlgCharacter other)
			{
				this.name = other.name;
				this.dlgLines = other.dlgLines;
				this.lineIndex = other.lineIndex;
			}

			public string ToXml()
			{
				return "xyox";
			}

			public override string ToString()
			{
				return name;
			}
		}

		public class DlgLine
		{
			public static int Count = 0;
			public static DlgLine Selected = null;

			public string text;
			public string id;
			public DlgCharacter character;
			public List<DlgAction> actions = new List<DlgAction>(); //Default actions, these are automatically added to interactions.

			public List<Interaction> interactions = new List<Interaction>(); //Rarely used, includes all interactions for this line

			public DlgLine() { }
			private DlgLine(DlgLine other)
			{
				this.text = other.text;
				this.id = other.id;
				this.character = other.character;
				this.actions = other.actions;
			}

			//public LineForm lineForm;
			//public ResponseForm responseForm;
			//public LineActionForm lineActionForm;
			//public RequirementsForm requirementForm;
			//public FollowingLineForm followingLineForm;
			//public LineForm lineForm;

			//public void ShowResponse(MainForm mf)
			//{
			//	Console.WriteLine(responseForm);
			//	if (responseForm == null)
			//	{
			//		responseForm = new ResponseForm(mf, this);
			//		responseForm.Show();
			//	}
			//	else
			//	{
			//		responseForm.Focus();
			//	}
			//}

			//public void ShowLineAction(MainForm mf)
			//{
			//	if (lineActionForm == null)
			//	{
			//		lineActionForm = new LineActionForm(mf, this);
			//		lineActionForm.Show();
			//	}
			//	else
			//	{
			//		lineActionForm.Focus();
			//	}
			//}

			//public void ShowRequirementForm(MainForm mf)
			//{
			//	if (requirementForm == null)
			//	{
			//		requirementForm = new RequirementsForm(mf, this);
			//		requirementForm.Show();
			//	}
			//	else
			//	{
			//		requirementForm.Focus();
			//	}
			//}

			//public void ShowFollowingLineForm(MainForm mf)
			//{
			//	if (followingLineForm == null)
			//	{
			//		followingLineForm = new FollowingLineForm(mf, this);
			//		followingLineForm.Show();
			//	}
			//	else
			//	{
			//		followingLineForm.Focus();
			//	}
			//}

			//public void ShowLineForm(MainForm mf)
			//{
			//	if (lineForm == null)
			//	{
			//		lineForm = new LineForm(mf, this);
			//		lineForm.Show();
			//	}
			//	else
			//	{
			//		lineForm.Focus();
			//	}
			//}

			public string ToXml()
			{
				return "xlinex";
			}

			public override string ToString()
			{
				string displayText = text;
				if (displayText.Length > 20)
				{
					displayText = displayText.Remove(17) + "...";
				}

				return id + ": " + displayText;
			}

			//public void closeForms()
			//{
			//	if (responseForm != null)
			//	{
			//		responseForm.DialogResult = .Forms.DialogResult.Ignore;
			//		responseForm.Close();
			//	}

			//	if (lineActionForm != null)
			//	{
			//		lineActionForm.DialogResult = .Forms.DialogResult.Ignore;
			//		lineActionForm.Close();
			//	}

			//	if (requirementForm != null)
			//	{
			//		requirementForm.DialogResult = .Forms.DialogResult.Ignore;
			//		requirementForm.Close();
			//	}

			//	if (followingLineForm != null)
			//	{
			//		followingLineForm.DialogResult = .Forms.DialogResult.Ignore;
			//		followingLineForm.Close();
			//	}

			//	if (lineForm != null)
			//	{
			//		lineForm.DialogResult = .Forms.DialogResult.Ignore;
			//		lineForm.Close();
			//	}
			//}

			public DlgLine Copy()
			{
				return new DlgLine(this);
			}
		}

		public class DlgAction
		{
			public static int Count = 0;
			public static DlgAction Selected = null;

			public string name;
			public string description;
			public List<DlgActionParameter> parameters = new List<DlgActionParameter>();
			public List<DlgRequirement> requirements = new List<DlgRequirement>(); //Default? Can add one like this for Line too. Keep for now

			public enum CallType : byte
			{
				OnExit,
				OnEnter
			}

			public CallType callType = CallType.OnExit;

			//public RequirementsForm requirementForm;
			//public ActionForm actionForm;

			public DlgAction() { }

			public DlgAction(DlgAction other)
			{
				name = other.name;
				description = other.description;
				callType = other.callType;

				foreach (DlgActionParameter p in other.parameters)
				{
					parameters.Add(p.Copy());
				}
			}

			//public void ShowRequirementForm(MainForm mf)
			//{
			//	if (requirementForm == null)
			//	{
			//		requirementForm = new RequirementsForm(mf, this);
			//		requirementForm.Show();
			//	}
			//	else
			//	{
			//		requirementForm.Focus();
			//	}
			//}

			//public void ShowActionForm(MainForm mf)
			//{
			//	if (requirementForm == null)
			//	{
			//		actionForm = new ActionForm(mf, this);
			//		actionForm.Show();
			//	}
			//	else
			//	{
			//		actionForm.Focus();
			//	}
			//}

			public string ToXml()
			{
				return "xactionx";
			}

			public override string ToString()
			{
				return name;
			}

			public DlgAction Copy()
			{
				return new DlgAction(this);
			}


		}

		public class DlgActionParameter
		{
			public static int Count = 0;
			public static DlgActionParameter Selected = null;

			public string name;
			public string description;
			public string value;
			public DlgAction action;

			//public ParameterForm parameterForm;

			public DlgActionParameter() { }

			private DlgActionParameter(DlgActionParameter other)
			{
				name = other.name;
				description = other.description;
				value = other.value;
			}

			//public void ShowParameterForm(MainForm mf)
			//{
			//	if (parameterForm == null)
			//	{
			//		parameterForm = new ParameterForm(mf, this);
			//		parameterForm.Show();
			//	}
			//	else
			//	{
			//		parameterForm.Focus();
			//	}
			//}

			public string ToXml()
			{
				return "xparameterx";
			}

			public override string ToString()
			{
				return name;
			}

			public DlgActionParameter Copy()
			{
				return new DlgActionParameter(this);
			}
		}

		public class DlgRequirement
		{
			public static int Count = 0;
			public static DlgRequirement Selected = null;

			public string variable = "DEFAULT NAME";
			public string value = "1";
			public string access = "Global";
			public string condition = "is";
			public bool isVisible = false;

			public DlgRequirement() { }
			private DlgRequirement(DlgRequirement other)
			{
				variable = other.variable;
				value = other.value;
				access = other.access;
				condition = other.condition;
			}

			public virtual DlgRequirement Copy()
			{
				return new DlgRequirement(this);
			}

			public override string ToString()
			{
				return variable + " " + condition + " " + value;
			}
		}

		internal class Dialogue
		{
			public static int Count = 0;
			public static Dialogue Selected = null;

			internal string name = "Test";
			internal List<Interaction> InteractionList = new List<Interaction>();
			internal Size dimensions = new Size(500, 500);

			internal Dialogue()
			{

			}

			private Dialogue(Dialogue other)
			{
				InteractionList = new List<Interaction>();
				foreach (Interaction i in other.InteractionList)
				{
					this.InteractionList.Add(i);
				}
			}

			public override string ToString()
			{
				return name;
			}
		}

		//The class that the dlgVisual will represent. Holds all the information nessecary.
		internal class Interaction
		{
			public static int Count = 0;
			public static Interaction Selected = null;

			internal DlgLine dlgLine;
			internal DlgLineEntity dlgLineEntity;
			internal List<InteractionConnection> interactionTo = new List<InteractionConnection>();
			internal List<InteractionConnection> interactionFrom = new List<InteractionConnection>();
			internal Dialogue dialogue;
			internal List<DlgAction> actionList = new List<DlgAction>();
			internal List<DlgRequirement> requirements = new List<DlgRequirement>();
			internal enum InteractionType { FollowedByNpcLine, HasResponses, EndsDialogue }; //Needed?
			internal InteractionType interactionType = InteractionType.FollowedByNpcLine;    //Needed?

			internal Interaction() { Count++; Console.WriteLine("Interaction count: " + Count.ToString()); }
			private Interaction(Interaction other)
			{

			}
			~Interaction()
			{
				Count--;
				Console.WriteLine("Interaction count: " + Count.ToString());
			}
		}

		//The line between each DlgVisual
		internal class InteractionConnection
		{
			public static int Count = 0;
			public static InteractionConnection Selected = null;

			internal Interaction startInteraction;
			internal Interaction endInteraction;
			internal System.Windows.Shapes.Rectangle startRect;
			internal System.Windows.Shapes.Rectangle endRect;
			internal Arrow arrow;

			internal InteractionConnection() { }
		}

		internal class Arrow
		{
			internal System.Windows.Shapes.Polygon headPoly;
			internal System.Windows.Shapes.Polygon pommelPoly;
			internal System.Windows.Shapes.Polygon arrowPoly;
			internal InteractionConnection interactionConnection;
			private Point[] pommel = new Point[8];
			private Point[] head = new Point[5];

			internal Arrow(Point startPoint, System.Windows.Controls.Canvas canvas)
			{
				_startPoint = startPoint;

				pommel[0] = (new Point(0, -0.75));   //0 startpoint - 0.5
				pommel[1] = (new Point(4.5, -5.25));   //1 Pommel
				pommel[2] = (new Point(9, -5.25));   //2 Pommel
				pommel[3] = (new Point(13.5, -2.5));   //3 Pommel
				head[0] = (new Point(-10, -2.5));   //4
				head[1] = (new Point(-10, -8.25));   //5 Head upper
				head[2] = (new Point(0, 0));        //6 Endpoint
				head[3] = (new Point(-10, 8.25));    //7 Head lower
				head[4] = (new Point(-10, 2.5));    //8
				pommel[4] = (new Point(13.5, 2.5));    //9 Pommel
				pommel[5] = (new Point(9, 5.25));    //10 Pommel
				pommel[6] = (new Point(4.5, 5.25));    //11 Pommel
				pommel[7] = (new Point(0, 0.75));    //12 startpoint + 0.5

				arrowPoly = new System.Windows.Shapes.Polygon();
				for (int i = 0; i < 4; i++) arrowPoly.Points.Add(new Point(pommel[i].X + startPoint.X, pommel[i].Y + startPoint.Y));
				for (int i = 0; i < 5; i++) arrowPoly.Points.Add(new Point(head[i].X + startPoint.X, head[i].Y + startPoint.Y));
				for (int i = 4; i < 8; i++) arrowPoly.Points.Add(new Point(pommel[i].X + startPoint.X, pommel[i].Y + startPoint.Y));

				headPoly = new System.Windows.Shapes.Polygon();
				foreach (Point p in head) { headPoly.Points.Add(p); }

				pommelPoly = new System.Windows.Shapes.Polygon();
				foreach (Point p in pommel) { pommelPoly.Points.Add(p); }

				canvas.Children.Add(arrowPoly);
				//canvas.Children.Add(head);
				//canvas.Children.Add(pommel);

				arrowPoly.Tag = this;
				headPoly.Tag = this;
				pommelPoly.Tag = this;
			}

			internal double StrokeThickness
			{
				get { return arrowPoly.StrokeThickness; }
				set { arrowPoly.StrokeThickness = value; headPoly.StrokeThickness = value; pommelPoly.StrokeThickness = value; }
			}
			internal Brush Stroke
			{
				get { return arrowPoly.Stroke; }
				set { arrowPoly.Stroke = value; headPoly.Stroke = value; pommelPoly.Stroke = value; }
			}
			internal bool IsHitTestVisible
			{
				get { return arrowPoly.IsHitTestVisible; }
				set { arrowPoly.IsHitTestVisible = value; headPoly.IsHitTestVisible = value; pommelPoly.IsHitTestVisible = value; }
			}
			internal double Opacity
			{
				get { return arrowPoly.Opacity; }
				set { arrowPoly.Opacity = value; headPoly.Opacity = value; pommelPoly.Opacity = value; }
			}
			internal Brush Fill
			{
				get { return arrowPoly.Fill; }
				set { arrowPoly.Fill = value; headPoly.Fill = value; pommelPoly.Fill = value; }
			}

			private Point _endPoint; internal Point endPoint
			{
				get { return _endPoint; }
				set
				{
					_endPoint = value;
					double angle = Math.Atan2(value.Y - _startPoint.Y, value.X - _startPoint.X);

					double sinA = Math.Sin(angle);
					double cosA = Math.Cos(angle);

					arrowPoly.Points.Clear();
					for (int i = 0; i < 4; i++) arrowPoly.Points.Add(rotatePoint(pommel[i], cosA, sinA, startPoint));
					for (int i = 0; i < 5; i++) arrowPoly.Points.Add(rotatePoint(head[i], cosA, sinA, _endPoint));
					for (int i = 4; i < 8; i++) arrowPoly.Points.Add(rotatePoint(pommel[i], cosA, sinA, startPoint));
				}
			}

			private Point _startPoint; internal Point startPoint
			{
				get { return _startPoint; }
				set
				{
					_startPoint = value;
					double angle = Math.Atan2(_endPoint.Y - _startPoint.Y, _endPoint.X - _startPoint.X);

					double sinA = Math.Sin(angle);
					double cosA = Math.Cos(angle);

					arrowPoly.Points.Clear();
					for (int i = 0; i < 4; i++) arrowPoly.Points.Add(rotatePoint(pommel[i], cosA, sinA, _startPoint));
					for (int i = 0; i < 5; i++) arrowPoly.Points.Add(rotatePoint(head[i], cosA, sinA, endPoint));
					for (int i = 4; i < 8; i++) arrowPoly.Points.Add(rotatePoint(pommel[i], cosA, sinA, _startPoint));
				}
			}

			private Point rotatePoint(Point op, double cosA, double sinA, Point cp)
			{
				return new Point(
					cosA * op.X - sinA * op.Y + cp.X,
					sinA * op.X + cosA * op.Y + cp.Y);
			}
		}
#pragma warning restore 0649
	}
}
