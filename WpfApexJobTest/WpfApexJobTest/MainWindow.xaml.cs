using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApexJobTest.Class;

namespace WpfApexJobTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DirectoryInfo dirInfo = new DirectoryInfo(@"C:\Temp\");
        string foldername = "TEST";
        string filename = "Document.xml";
        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists(@"C:/Temp/" + foldername))
            {
                Directory.CreateDirectory(@"C:/Temp/" + foldername);
                Directory.CreateDirectory(@"C:/Temp/" + foldername);
                File.Create(@"C:/Temp/TEST/" + filename);
            }
        }
        private void FillData1(DirectoryInfo info)
        {

            textBoxFname.Text = info.Name;
            textBoxFSize.Text = GetDirectorySize(info.FullName).ToString();
            textBoxNoFiles.Text = GetDirectoryFileNumber(info.FullName).ToString();
            textBoxFileName.Text = "";
            textBoxFileSize.Text = "";
            textBoxFileLastCreationTime.Text = "";
            textBoxFileLastAccsessTime.Text = "";
            textBoxFileLastModifiedTIme.Text = "";

        }

        private void FillData2(FileInfo info)
        {
            textBoxFileName.Text = info.Name;

            textBoxFileSize.Text = info.Length.ToString();
            DateTime creationTime = info.CreationTime;
            DateTime lastAccessTime = info.LastAccessTime;
            DateTime lastWriteTime = info.LastWriteTime;
            textBoxFileLastCreationTime.Text = creationTime.ToString("MM/dd/yyyy");
            textBoxFileLastAccsessTime.Text = lastAccessTime.ToString("MM/dd/yyyy");
            textBoxFileLastModifiedTIme.Text = lastAccessTime.ToString("MM/dd/yyyy");

            textBoxFname.Text = "";
            textBoxFSize.Text = "";
            textBoxNoFiles.Text = "";
        }

        static long GetDirectorySize(string p)
        {
            // 1.
            // Get array of all file names.
            string[] a = Directory.GetFiles(p, "*.*");

            // 2.
            // Calculate total bytes of all files in a loop.
            long b = 0;
            foreach (string name in a)
            {
                // 3.
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            // 4.
            // Return total size
            return b;
        }

        static int GetDirectoryFileNumber(string p)
        {
            // 1.
            // Get array of all file names.
            string[] a = Directory.GetFiles(p, "*.*");
            return a.Length;

        }
        public void LoadDirectories()
        {
            treeViewLista.Items.Clear();
            var drives = dirInfo.GetDirectories();
            foreach (var drive in drives)
            {
                this.treeViewLista.Items.Add(this.GetFolder(drive));
            }
        }
     
        private TreeViewItem GetFolder(DirectoryInfo directory)
        {
            var item = new TreeViewItem
            {
                Header = directory.Name,
                DataContext = directory,
                Tag = directory
            };
            this.AddDummy(item);
            item.Expanded += new RoutedEventHandler(Item_Expanded);
            return item;
        }

        private TreeViewItem GetFile(FileInfo file)
        {
            var item = new TreeViewItem
            {
                Header = file.Name,
                DataContext = file,
                Tag = file
            };
            return item;
        }
        private void AddDummy(TreeViewItem item)
        {
            item.Items.Add(new DummyTreeViewItem());
        }

        private bool HasDummy(TreeViewItem item)
        {
            return item.HasItems && (item.Items.OfType<TreeViewItem>().ToList().FindAll(tvi => tvi is DummyTreeViewItem).Count > 0);
        }

        private void RemoveDummy(TreeViewItem item)
        {
            var dummies = item.Items.OfType<TreeViewItem>().ToList().FindAll(tvi => tvi is DummyTreeViewItem);
            foreach (var dummy in dummies)
            {
                item.Items.Remove(dummy);
            }
        }

        private void ExploreDirectories(TreeViewItem item)
        {
            var directoryInfo = (DirectoryInfo)null;
            if (item.Tag is DriveInfo)
            {
                directoryInfo = ((DriveInfo)item.Tag).RootDirectory;
            }
            else if (item.Tag is DirectoryInfo)
            {
                directoryInfo = (DirectoryInfo)item.Tag;
            }
            else if (item.Tag is FileInfo)
            {
                directoryInfo = ((FileInfo)item.Tag).Directory;
            }
            if (object.ReferenceEquals(directoryInfo, null)) return;
            foreach (var directory in directoryInfo.GetDirectories())
            {
                var isHidden = (directory.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                var isSystem = (directory.Attributes & FileAttributes.System) == FileAttributes.System;
                if (!isHidden && !isSystem)
                {
                    item.Items.Add(this.GetFolder(directory));
                }
            }
        }

        private void ExploreFiles(TreeViewItem item)
        {
            var directoryInfo = (DirectoryInfo)null;
            if (item.Tag is DriveInfo)
            {
                directoryInfo = ((DriveInfo)item.Tag).RootDirectory;
            }
            else if (item.Tag is DirectoryInfo)
            {
                directoryInfo = (DirectoryInfo)item.Tag;
            }
            else if (item.Tag is FileInfo)
            {
                directoryInfo = ((FileInfo)item.Tag).Directory;
            }
            if (object.ReferenceEquals(directoryInfo, null)) return;
            foreach (var file in directoryInfo.GetFiles())
            {
                var isHidden = (file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                var isSystem = (file.Attributes & FileAttributes.System) == FileAttributes.System;
                if (!isHidden && !isSystem)
                {
                    item.Items.Add(this.GetFile(file));
                }
            }
        }
        void Item_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (this.HasDummy(item))
            {
                this.Cursor = Cursors.Wait;
                this.RemoveDummy(item);
                this.ExploreDirectories(item);
                this.ExploreFiles(item);
                this.Cursor = Cursors.Arrow;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDirectories();
        }

        private void treeViewLista_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem)
            {
                TreeViewItem tvi = e.NewValue as TreeViewItem;
                if (tvi.Tag is DirectoryInfo)

                {
                    FillData1((DirectoryInfo)tvi.Tag);
                }
                else if (tvi.Tag is FileInfo)
                {
                    FillData2((FileInfo)tvi.Tag);
                }

            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.InitialDirectory = @"C:\Temp\";
            sv.FileName = "Document";
            sv.Filter = "XML Files (*.xml)|*.xml";
            sv.Title = "Save Xml file";

            if (sv.ShowDialog() == true)
            {
                string fileName = sv.FileName;
                SaveXML.SaveXml(sv.FileName, fileName);             
                LoadDirectories();
            }
        }
    }
}

   
    

