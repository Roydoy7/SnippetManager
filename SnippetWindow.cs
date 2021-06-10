﻿using Microsoft.VisualStudio.Shell;
using SnippetManager.ViewModels;
using System;
using System.Runtime.InteropServices;

namespace SnippetManager
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("557667fc-aa6b-429a-ab8f-4cc237b7e2d9")]
    public class SnippetWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnippetWindow"/> class.
        /// </summary>
        public SnippetWindow() : base(null)
        {
            this.Caption = "SnippletWindow";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            var vm = new SnippetWindowControlViewModel();
            this.Content = vm.View;
        }
    }
}