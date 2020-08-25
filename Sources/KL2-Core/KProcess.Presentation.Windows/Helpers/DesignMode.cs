// -----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Helper permettant de déterminer si le contexte courant est en mode Design.
    /// </summary>
    public static class DesignMode
    {

        private static bool? _isInDesignMode;

        /// <summary>
        /// Indique si le contexte courant est en mode design.
        /// </summary>
        public static bool IsInDesignMode
        {

#if SILVERLIGHT
            get 
            { 
                if (!_isInDesignMode.HasValue)
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
                return _isInDesignMode.Value; 
            }
#else
            get 
            {
                if (!_isInDesignMode.HasValue)
                    _isInDesignMode = DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
                return _isInDesignMode.Value; 
            }
#endif
        }

    }
}
