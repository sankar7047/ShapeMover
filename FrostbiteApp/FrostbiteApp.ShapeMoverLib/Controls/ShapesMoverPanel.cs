using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FrostbiteApp.ShapeMoverLib.Base;
using System.Collections.Generic;
using System.Linq;
using FrostbiteApp.ShapeMoverLib.Models;
using FrostbiteApp.ShapeMoverLib.Enums;
using Color = System.Windows.Media.Color;
using Size = System.Windows.Size;
using Point = System.Windows.Point;

namespace FrostbiteApp.ShapeMoverLib.Controls
{
    public class ShapesMoverPanel : Panel
    {
        #region Fields

        readonly Random random = new();
        readonly Stack<UndoRedoTransaction> undoStack = new();
        readonly Stack<UndoRedoTransaction> redoStack = new();
        Point mouseDownOnChild;
        Point mouseDown;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Attached property to hold the value of X
        /// </summary>
        public static readonly DependencyProperty XProperty =
        DependencyProperty.RegisterAttached(nameof(X), typeof(double), typeof(ShapesMoverPanel), new PropertyMetadata(0.0));

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// /// <summary>
        /// Attached property to hold the value of Y
        /// </summary>
        /// </summary>
        public static readonly DependencyProperty YProperty =
        DependencyProperty.RegisterAttached(nameof(Y), typeof(double), typeof(ShapesMoverPanel), new PropertyMetadata(0.0));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        #endregion

        #region Properties


        /// <summary>
        /// Command to add shape to the panel
        /// </summary>
        public ICommand AddShapeCommand
        {
            get => new Command(OnAddShapeCommandExecute);
        }

        /// <summary>
        /// Command to undo the last action
        /// </summary>
        public ICommand UndoCommand
        {
            get => new Command(OnUndoCommandExecute, OnCanUndoCommandExecute);
        }

        /// <summary>
        /// Command to redp the last action
        /// </summary>
        public ICommand RedoCommand
        {
            get => new Command(OnRedoCommandExecute, OnCanRedoCommandExecute);
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Each element calculates the desired size
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in base.InternalChildren)
            {
                // Desired size for the element is measured
                element.Measure(availableSize);
            }
            return availableSize;
        }

        /// <summary>
        /// Each element arranged with a random x,y positions
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElementCollection elements = base.InternalChildren;
            foreach (Shape shape in elements)
            {
                Point point;
                var dataContext = shape.DataContext as ShapeModel;

                // Checks the child has x,y coordinates
                var x = dataContext.X != 0 ? dataContext.X : (double)shape.GetValue(XProperty);
                var y = dataContext.Y != 0 ? dataContext.Y : (double)shape.GetValue(YProperty);

                if (x != 0 && y != 0) // Use the existing coordinates
                    point = new Point(x, y);
                else // Creates a random x,y coordinates
                    point = new Point(random.Next(0, (int)(finalSize.Width - shape.DesiredSize.Width)), random.Next(0, (int)(finalSize.Height - shape.DesiredSize.Height)));

                ArrangeChild(shape, point);
            }

            return finalSize;
        }

        /// <summary>
        /// Override for MouseDown initiates the drag of child
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            // Gets the position of mouse down with respect to the panel
            mouseDown = e.GetPosition(this);

            if (e.Source is Shape child)
            {
                mouseDownOnChild = e.GetPosition(child);
                var point = new Point(mouseDown.X - mouseDownOnChild.X, mouseDown.Y - mouseDownOnChild.Y);
                ArrangeChild(child, point);
                child.CaptureMouse();
            }
        }

        /// <summary>
        /// Override for MouseMove performs the drag of child
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed && mouseDown != default)
            {
                var mousePosition = e.GetPosition(this);
                if (e.Source is Shape child)
                {
                    // Points calculated based on the mouse points and child arranged with the points.
                    var point = new Point(mousePosition.X - mouseDownOnChild.X, mousePosition.Y - mouseDownOnChild.Y);
                    ArrangeChild(child, point);
                }
            }

        }

        /// <summary>
        /// Override for Mouse up release the drag and drops of child
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            var mouseUp = e.GetPosition(this);
            if (e.Source is Shape child)
            {
                var point = new Point(mouseUp.X - mouseDownOnChild.X, mouseUp.Y - mouseDownOnChild.Y);
                ArrangeChild(child, point);
                undoStack.Push(new UndoRedoTransaction
                {
                    Data = child.DataContext as ShapeModel,
                    Reason = UndoRedoReason.Move,
                    OldValue = new Point(mouseDown.X - mouseDownOnChild.X, mouseDown.Y - mouseDownOnChild.Y),
                    NewValue = point
                });
                redoStack.Clear();
                child.ReleaseMouseCapture();
                mouseDown = default;
                mouseDownOnChild = default;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a shape to the panel based on a given Shape Model or creates one.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isFromRedo"></param>
        public void AddShape(ShapeModel model = null, bool isFromRedo = false)
        {
            ShapeModel shapeModel;
            if (model == null)
            {
                shapeModel = new ShapeModel
                {
                    Height = 40,
                    Width = 40,
                    // Creates a random color to fill the shape
                    FillColor = new SolidColorBrush(Color.FromArgb(
                    (byte)random.Next(256),
                    (byte)random.Next(256),
                    (byte)random.Next(256),
                    (byte)random.Next(256)))
                };
            }
            else
                shapeModel = model;

            // Generates a shape based on the shape model
            var shape = GenerateShape(shapeModel);
            Children.Add(shape);

            if (!isFromRedo) // Add shape called from redo, then no need to add it to the undo stack.
            {
                undoStack.Push(new UndoRedoTransaction
                {
                    Data = shapeModel,
                    Reason = UndoRedoReason.Add
                });
                redoStack.Clear();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns a shape(Here it is Ellipse and can modify to generate any shapes)
        /// Shape Model will the DataContext for the Shape.
        /// </summary>
        /// <param name="shapeModel"></param>
        /// <returns></returns>
        private FrameworkElement GenerateShape(ShapeModel shapeModel)
        {
            var shape = new Ellipse()
            {
                Stroke = new SolidColorBrush(Colors.Gray),
                StrokeThickness = 1,
                DataContext = shapeModel
            };

            // Set bindings
            shape.SetBinding(HeightProperty, "Height");
            shape.SetBinding(WidthProperty, "Width");
            shape.SetBinding(Shape.FillProperty, "FillColor");

            return shape;
        }

        /// <summary>
        /// Arrange Child with the given points in the panel
        /// </summary>
        /// <param name="child"></param>
        /// <param name="point"></param>
        private void ArrangeChild(UIElement child, Point point)
        {
            if (child is Shape shape)
            {
                var size = new Size(shape.DesiredSize.Width, shape.DesiredSize.Height);
                shape.SetValue(XProperty, point.X);
                shape.SetValue(YProperty, point.Y);
                (shape.DataContext as ShapeModel).X = point.X;
                (shape.DataContext as ShapeModel).Y = point.Y;
                shape.Arrange(new Rect(point, size));
            }
        }

        /// <summary>
        /// Command invoke method for Add shape command
        /// </summary>
        /// <param name="param"></param>
        private void OnAddShapeCommandExecute(object param)
        {
            AddShape(param as ShapeModel);
        }

        /// <summary>
        /// Command invoke method for Undo command
        /// </summary>
        /// <param name="param"></param>
        private void OnUndoCommandExecute(object param)
        {
            var undoObj = undoStack.Peek();
            var child = Children.OfType<Shape>().FirstOrDefault(x => x.DataContext == undoObj.Data);
            if (child != null)
            {
                if (undoObj.Reason == UndoRedoReason.Add)
                {
                    // Undoing the last transaction for Add reason
                    Children.Remove(child);
                }
                else
                {
                    // Rearranging the child to old position for Move reason
                    ArrangeChild(child, undoObj.OldValue);
                }

                redoStack.Push(undoObj); // Pushing it to redo stack and pop it out from undo stack.
                undoStack.Pop();
            }

        }

        /// <summary>
        /// Returns whether undo can be done
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool OnCanUndoCommandExecute(object param)
        {
            return undoStack.Any();
        }

        /// <summary>
        /// Command invoke method for Redo command
        /// </summary>
        /// <param name="param"></param>
        private void OnRedoCommandExecute(object param)
        {
            var redoObj = redoStack.Peek();
            if (redoObj.Reason == UndoRedoReason.Add)
            {
                // Readd the shape to the panel for Add reason
                AddShape(redoObj.Data, true);
            }
            else
            {
                // Redoing the movement of the shape to the new position
                var child = Children.OfType<Shape>().FirstOrDefault(x => x.DataContext == redoObj.Data);
                if(child != null)
                    ArrangeChild(child, redoObj.NewValue);
            }

            undoStack.Push(redoObj); // Pushing the redo task to undo task and pop out from redo task.
            redoStack.Pop();
        }

        /// <summary>
        /// Returns whether redo can be done.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool OnCanRedoCommandExecute(object param)
        {
            return redoStack.Any();
        }

        #endregion

    }
}
