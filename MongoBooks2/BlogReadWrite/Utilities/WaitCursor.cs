// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitCursor.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The wait cursor utility.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogReadWrite.Utilities
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The wait cursor utility.
    /// </summary>
    public class WaitCursor : IDisposable
    {
        /// <summary>
        /// The previous cursor to return to when finished waiting.
        /// </summary>
        private readonly Cursor _previousCursor;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitCursor" /> class.
        /// </summary>
        public WaitCursor()
        {
            _previousCursor = Mouse.OverrideCursor;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose the resources and reset the cursor.
        /// </summary>
        public void Dispose()
        {
            Mouse.OverrideCursor = _previousCursor;
        }

        #endregion
    }
}
