﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BvsDesktopLinux.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BvsDesktopLinux.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Currency.
        /// </summary>
        public static string NoteCurrency {
            get {
                return ResourceManager.GetString("NoteCurrency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Denomination.
        /// </summary>
        public static string NoteDenomination {
            get {
                return ResourceManager.GetString("NoteDenomination", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No..
        /// </summary>
        public static string NoteId {
            get {
                return ResourceManager.GetString("NoteId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bill Report.
        /// </summary>
        public static string TitleBillReport {
            get {
                return ResourceManager.GetString("TitleBillReport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remove Selected Notes.
        /// </summary>
        public static string TooltipDeleteNotes {
            get {
                return ResourceManager.GetString("TooltipDeleteNotes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Export Data to CSV file.
        /// </summary>
        public static string TooltipExportCSV {
            get {
                return ResourceManager.GetString("TooltipExportCSV", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Print Report as PDF.
        /// </summary>
        public static string TooltipPrintReport {
            get {
                return ResourceManager.GetString("TooltipPrintReport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Load Counts from Local Database.
        /// </summary>
        public static string TooltipRestoreCounts {
            get {
                return ResourceManager.GetString("TooltipRestoreCounts", resourceCulture);
            }
        }
    }
}
