#if UNITY_STANDALONE_WIN
using Ookii.Dialogs;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Text;
using System.IO;

namespace AnotherFileBrowser.Windows
{
    public class FileBrowser
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        public FileBrowser() { }

        /// <summary>
        /// FileDialog for picking a single file
        /// </summary>
        /// <param name="browserProperties">Special Properties of File Dialog</param>
        /// <param name="filepath">User picked path (Callback)</param>
        public void OpenFileBrowser(BrowserProperties browserProperties, Action<string> filepath)
        {
            var ofd = new VistaOpenFileDialog();
            ofd.Multiselect = false;
            ofd.Title = browserProperties.title == null ? "Select a File" : browserProperties.title;
            ofd.InitialDirectory = browserProperties.initialDir == null ? @"C:\" : browserProperties.initialDir;
            ofd.Filter = browserProperties.filter == null ? "All files (*.*)|*.*" : browserProperties.filter;
            ofd.FilterIndex = browserProperties.filterIndex + 1;
            ofd.RestoreDirectory = browserProperties.restoreDirectory;

            if (ofd.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK)
            {
                filepath(ofd.FileName);

            }
        }
        /*
         * //EV
        */
        public void OpenMultiFileBrowser(BrowserProperties browserProperties, Action<string[]> filepaths)
        {
            var ofd = new VistaOpenFileDialog();
            ofd.Title = browserProperties.title == null ? "Select a File" : browserProperties.title;
            ofd.InitialDirectory = browserProperties.initialDir == null ? @"C:\" : browserProperties.initialDir;
            ofd.Filter = browserProperties.filter == null ? "All files (*.*)|*.*" : browserProperties.filter;
            ofd.FilterIndex = browserProperties.filterIndex + 1;
            ofd.RestoreDirectory = browserProperties.restoreDirectory;
            ofd.Multiselect = true;

            if (ofd.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK)
            {
                filepaths(ofd.FileNames);
            }
        }
        /*
         * //EV
        */
        public void SaveFileBrowser(BrowserProperties browserProperties, string content, Action<string> assyncCallbackOne)
        {
            var sfd = new VistaSaveFileDialog();
            sfd.Title = browserProperties.title == null ? "Save a File" : browserProperties.title;
            sfd.InitialDirectory = browserProperties.initialDir == null ? @"C:\" : browserProperties.initialDir;
            sfd.Filter = browserProperties.filter == null ? "All files (*.*)|*.*" : browserProperties.filter;
            sfd.FilterIndex = browserProperties.filterIndex + 1;
            sfd.RestoreDirectory = browserProperties.restoreDirectory;


            if (sfd.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK)
            {
                Stream stream = null;
                try
                {
                    stream = sfd.OpenFile();
                    stream.Write(Encoding.ASCII.GetBytes(content), 0, content.Length);
                    stream.Close();

                }
                catch (System.Exception _ex)
                {
                    if (stream != null)
                        stream.Close();
                    Debug.LogError(_ex);
                    throw;
                }
                assyncCallbackOne(sfd.FileName);

            }
        }


    }

    public class BrowserProperties
    {
        public string title; //Title of the Dialog
        public string initialDir; //Where dialog will be opened initially
        public string filter; //aka File Extension for filtering files
        public int filterIndex; //Index of filter, if there is multiple filter. Default is 0. 
        public bool restoreDirectory = true; //Restore to last return directory


        public BrowserProperties() { }
        public BrowserProperties(string title) { this.title = title; }
    }

    public class WindowWrapper : IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }
}
#endif