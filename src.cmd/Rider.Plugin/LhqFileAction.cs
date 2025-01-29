using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ProjectModel;
using JetBrains.UI.Controls.TreeListView;
using JetBrains.Util;

[Action("LHQRiderPlugin.LhqFileAction", "Process LHQ File", Id = 54321)]
public class LhqFileAction : IExecutableAction
{
    //https://github.com/JetBrains/resharper-rider-plugin
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
        // Get the selected files from the data context
        var selectedFiles = context.GetData(TreeListViewWithDataContext.DataConstants.SelectedItems);

        if (selectedFiles != null && selectedFiles.Count == 1)
        {
            var file = selectedFiles[0] as IProjectFile;
            if (file != null && file.Location.ExtensionNoDot.Equals("lhq", StringComparison.OrdinalIgnoreCase))
            {
                //presentation.Enabled = true;
                presentation.Visible = true;
                return true;
            }
        }

        //presentation.Enabled = false;
        presentation.Visible = false;
        return false;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        // Get the selected files from the data context
        var selectedFiles = context.GetData(TreeListViewWithDataContext.DataConstants.SelectedItems);

        if (selectedFiles != null && selectedFiles.Count == 1)
        {
            var file = selectedFiles[0] as IProjectFile;
            if (file != null && file.Location.ExtensionNoDot.Equals("lhq", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.ShowInfo($"Processing LHQ file: {file.Location.Name}");
                // Add your custom logic here
            }
        }
    }
}