using System.Collections.Generic;

namespace C4rl1t0sRefactor
{
    /// <summary>One entry in the C4rl1t0sRefactor pop-up.</summary>
    public sealed class RefactorDefinition
    {
        public int Number { get; set; }

        public string Label { get; set; }

        /// <summary>
        /// One or more DTE command canonical names, tried in order. The first one that
        /// exists and is available at the caret is executed.
        /// </summary>
        public string[] Commands { get; set; }

        /// <summary>Set at run time; drives the greyed-out look for entries that don't apply.</summary>
        public bool IsAvailable { get; set; } = true;
    }

    /// <summary>
    /// The catalog of refactorings offered by the menu. It mirrors ReSharper's
    /// "Refactor This" list, mapped onto Visual Studio's built-in commands.
    /// Edit this list to add, remove or reorder entries.
    /// </summary>
    internal static class RefactorCatalog
    {
        // Opens the Quick Actions / light-bulb menu at the caret. This is where VS keeps
        // refactorings that have no dedicated command: Move type to file, Move to namespace,
        // Inline, Introduce variable/field/constant, Convert, Generate, etc.
        private const string QuickActions = "EditorContextMenus.CodeWindow.QuickActionsForPosition";

        public static List<RefactorDefinition> Build()
        {
            return new List<RefactorDefinition>
            {
                new RefactorDefinition { Number = 1,  Label = "Rename... (renombrar)",                             Commands = new[] { "Refactor.Rename" } },
                new RefactorDefinition { Number = 2,  Label = "Extract Method... (extraer metodo)",                Commands = new[] { "Refactor.ExtractMethod" } },
                new RefactorDefinition { Number = 3,  Label = "Extract Interface... (extraer interfaz)",           Commands = new[] { "Refactor.ExtractInterface" } },
                new RefactorDefinition { Number = 4,  Label = "Extract Base Class... (extraer clase base)",        Commands = new[] { "Refactor.ExtractBaseClass", QuickActions } },
                new RefactorDefinition { Number = 5,  Label = "Encapsulate Field... (encapsular campo)",           Commands = new[] { "Refactor.EncapsulateField" } },
                new RefactorDefinition { Number = 6,  Label = "Change Signature... (cambiar firma)",               Commands = new[] { "Refactor.ChangeSignature", QuickActions } },
                new RefactorDefinition { Number = 7,  Label = "Remove Parameters... (quitar parametros)",          Commands = new[] { "Refactor.RemoveParameters", "Refactor.ChangeSignature" } },
                new RefactorDefinition { Number = 8,  Label = "Reorder Parameters... (reordenar parametros)",      Commands = new[] { "Refactor.ReorderParameters", "Refactor.ChangeSignature" } },
                new RefactorDefinition { Number = 9,  Label = "Promote Local to Parameter (introducir parametro)", Commands = new[] { "Refactor.PromoteLocalVariableToParameter", QuickActions } },
                new RefactorDefinition { Number = 10, Label = "Move Type to File / Move to Namespace...",          Commands = new[] { QuickActions } },
                new RefactorDefinition { Number = 11, Label = "Inline / Introduce Variable / Introduce Field...",  Commands = new[] { QuickActions } },
                new RefactorDefinition { Number = 12, Label = "Quick Actions & Refactorings (Ctrl+.)",             Commands = new[] { QuickActions } },
            };
        }
    }
}
