using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace C4rl1t0sRefactor
{
    /// <summary>
    /// Handles the Ctrl+Shift+R command: shows the pop-up menu, then forwards the
    /// chosen entry to the matching built-in Visual Studio refactoring command.
    /// </summary>
    internal sealed class C4rl1t0sRefactorCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("c8f6a1d3-4b5e-4f70-8b2c-3d4e5f6a7b8c");

        private readonly AsyncPackage _package;

        public static C4rl1t0sRefactorCommand Instance { get; private set; }

        private C4rl1t0sRefactorCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            if (commandService == null) throw new ArgumentNullException(nameof(commandService));

            var id = new CommandID(CommandSet, CommandId);
            commandService.AddCommand(new MenuCommand(Execute, id));
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new C4rl1t0sRefactorCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE2 dte = null;
            try
            {
                dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
                if (dte == null) return;

                var items = RefactorCatalog.Build();
                foreach (var item in items)
                    item.IsAvailable = item.Commands.Any(c => IsCommandAvailable(dte, c));

                // Modal dialog: when it closes, focus returns to the editor, so the
                // refactoring command acts on the original caret position.
                var window = new RefactorMenuWindow(items);
                if (window.ShowModal() == true && window.Selected != null)
                    RunRefactoring(dte, window.Selected);
            }
            catch (Exception ex)
            {
                TrySetStatus(dte, "C4rl1t0sRefactor failed: " + ex.Message);
            }
        }

        private static bool IsCommandAvailable(DTE2 dte, string canonicalName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var command = dte.Commands.Item(canonicalName, 0);
                return command != null && command.IsAvailable;
            }
            catch
            {
                return false;
            }
        }

        private static void RunRefactoring(DTE2 dte, RefactorDefinition definition)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (var name in definition.Commands)
            {
                try
                {
                    var command = dte.Commands.Item(name, 0);
                    if (command == null || !command.IsAvailable) continue;

                    dte.ExecuteCommand(name);
                    return;
                }
                catch
                {
                    // Command missing in this VS version or not applicable here: try the next fallback.
                }
            }

            TrySetStatus(dte, $"C4rl1t0sRefactor: \"{definition.Label}\" is not available at the current caret position.");
        }

        private static void TrySetStatus(DTE2 dte, string message)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (dte != null) dte.StatusBar.Text = message;
            }
            catch
            {
                // Never let a status-bar failure surface to the user.
            }
        }
    }
}
