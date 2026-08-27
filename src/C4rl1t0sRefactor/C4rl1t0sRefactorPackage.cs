using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace C4rl1t0sRefactor
{
    /// <summary>
    /// VSPackage that registers the "C4rl1t0sRefactor" command and its Ctrl+Shift+R binding.
    /// The package is loaded on demand the first time the command is invoked.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("C4rl1t0sRefactor", "ReSharper-style 'Refactor This' pop-up on Ctrl+Shift+R.", "1.0.1")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    public sealed class C4rl1t0sRefactorPackage : AsyncPackage
    {
        public const string PackageGuidString = "b7e5f0c2-3a4d-4e6f-9a1b-2c3d4e5f6a7b";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await C4rl1t0sRefactorCommand.InitializeAsync(this);
        }
    }
}
