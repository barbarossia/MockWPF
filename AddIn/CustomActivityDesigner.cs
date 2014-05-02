using System;
using System.Activities.Presentation;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AddIn
{
    public class CustomActivityDesigner
    {
        private ActivityDesigner root;
        private Grid workflowView;
        //private Canvas canvas;
        private WorkflowDesigner wd;
        public CustomActivityDesigner(WorkflowDesigner wd)
        {
            this.wd = wd;
            

        }


        public void SetupCompositeActivityDesinger()
        {
            //this.root.IsEnabled = false;
            //DesignerView designerView = (DesignerView)SelectPrintContentHelper.SearchDependencyObject(wd.View, typeof(DesignerView));
            AdornerDecorator decorator = (AdornerDecorator)SelectPrintContentHelper.SearchDependencyObject(wd.View, typeof(AdornerDecorator));
            workflowView = (Grid)SelectPrintContentHelper.SearchDependencyObject(decorator, typeof(Grid));
            var canvas = CreateCanvas(workflowView);
            CreateTags(canvas);
        }

        private Canvas CreateCanvas(Grid grid)
        {
            Canvas canvas = new Canvas();
            canvas.Height = grid.Height;
            canvas.Width = grid.Width;
            //canvas.Background = Brushes.Green;
            //canvas.Opacity = 0.3;
            workflowView.Children.Add(canvas);
            return canvas;
        }

        private void CreateTags(Canvas canvas)
        {
            var root = (ActivityDesigner)SelectPrintContentHelper.SearchDependencyObject(wd.View, typeof(ActivityDesigner));
            CreateTagsCore(root, canvas);
            var desingers = SelectPrintContentHelper.SearchDependencyObjects(root, typeof(ActivityDesigner));
            foreach(var desinger in desingers)
            {
                CreateTagsCore((ActivityDesigner)desinger, canvas);
            }
        }

        private void CreateTagsCore(ActivityDesigner designer, Canvas canvas)
        {
            Point offset = SelectPrintContentHelper.GetRelativeOffset(designer, workflowView);
            Rectangle tag = new Rectangle();
            tag.Tag = designer;
            tag.Width = 10;
            tag.Height = 10;
            tag.MouseLeftButtonDown += tag_MouseLeftButtonDown;
            tag.Fill = Brushes.Blue;

            canvas.Children.Add(tag);
            Canvas.SetTop(tag, offset.Y);
            Canvas.SetLeft(tag, offset.X);
        }

        void tag_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Rectangle tag = (Rectangle)sender;
            tag.Fill = Brushes.Green;
        }

    }
}
