using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.API;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using TAFactory.Utilities;
using BYTE = System.Byte;
using WORD = System.Int16;
using DWORD = System.Int32;

namespace TAFactory.IconPack
{
    #region Enumuration
    //[Flags]
    public enum IconFlags : int
    {
        Icon              = 0x000000100,     // get icon
        LinkOverlay       = 0x000008000,     // put a link overlay on icon
        Selected          = 0x000010000,     // show icon in selected state
        LargeIcon         = 0x000000000,     // get large icon
        SmallIcon         = 0x000000001,     // get small icon
        OpenIcon          = 0x000000002,     // get open icon
        ShellIconSize     = 0x000000004,     // get shell size icon
    }
    #endregion

    /// <summary>
    /// Contains helper function to help dealing with System.Drawing.Icon.
    /// </summary>
    public static class IconHelper
    {
        #region Public Methods
        /// <summary>
        /// Returns TAFactory.IconPack.IconInfo object that holds the information about the icon.
        /// </summary>
        /// <param name="icon">System.Drawing.Icon to get the information about.</param>
        /// <returns>TAFactory.IconPack.IconInfo object that holds the information about the icon.</returns>
        public static IconInfo GetIconInfo(Icon icon)
        {
            return new IconInfo(icon);
        }
        /// <summary>
        /// Returns TAFactory.IconPack.IconInfo object that holds the information about the icon.
        /// </summary>
        /// <param name="icon">The icon file path.</param>
        /// <returns>TAFactory.IconPack.IconInfo object that holds the information about the icon.</returns>
        public static IconInfo GetIconInfo(string fileName)
        {
            return new IconInfo(fileName);
        }
        
        /// <summary>
        /// Extracts an icon from a givin icon file or an executable module (.dll or an .exe file).
        /// </summary>
        /// <param name="fileName">The path of the icon file or the executable module.</param>
        /// <param name="iconIndex">The index of the icon in the executable module.</param>
        /// <returns>A System.Drawing.Icon extracted from the file at the specified index in case of an executable module.</returns>
        public static Icon ExtractIcon(string fileName, int iconIndex)
        {
            Icon icon = null;
            //Try to load the file as icon file.
            try { icon = new Icon(Environment.ExpandEnvironmentVariables(fileName)); }
            catch { }

            if (icon != null) //The file was an icon file, return the icon.
                return icon;

            //Load the file as an executable module.
            using (IconExtractor extractor = new IconExtractor(fileName))
            {
                return extractor.GetIconAt(iconIndex);
            }
        }
        /// <summary>
        /// Extracts all the icons from a givin icon file or an executable module (.dll or an .exe file).
        /// </summary>
        /// <param name="fileName">The path of the icon file or the executable module.</param>
        /// <returns>
        /// A list of System.Drawing.Icon found in the file.
        /// If the file was an icon file, it will return a list containing a single icon.
        /// </returns>
        public static List<Icon> ExtractAllIcons(string fileName)
        {
            Icon icon = null;
            List<Icon> list = new List<Icon>();
            //Try to load the file as icon file.
            try { icon = new Icon(Environment.ExpandEnvironmentVariables(fileName)); }
            catch { }

            if (icon != null) //The file was an icon file.
            {
                list.Add(icon);
                return list;
            }

            //Load the file as an executable module.
            using (IconExtractor extractor = new IconExtractor(fileName))
            {
                for (int i = 0; i < extractor.IconCount; i++)
                {
                    list.Add(extractor.GetIconAt(i));
                }
            }
            return list;
        }
        
        /// <summary>
        /// Splits the group icon into a list of icons (the single icon file can contain a set of icons).
        /// </summary>
        /// <param name="icon">The System.Drawing.Icon need to be splitted.</param>
        /// <returns>List of System.Drawing.Icon.</returns>
        public static List<Icon> SplitGroupIcon(Icon icon)
        {
            IconInfo info = new IconInfo(icon);
            return info.Images;
        }

        /// <summary>
        /// Gets the System.Drawing.Icon that best fits the current display device.
        /// </summary>
        /// <param name="icon">System.Drawing.Icon to be searched.</param>
        /// <returns>System.Drawing.Icon that best fit the current display device.</returns>
        public static Icon GetBestFitIcon(Icon icon)
        {
            IconInfo info = new IconInfo(icon);
            int index = info.GetBestFitIconIndex();
            return info.Images[index];
        }
        /// <summary>
        /// Gets the System.Drawing.Icon that best fits the current display device.
        /// </summary>
        /// <param name="icon">System.Drawing.Icon to be searched.</param>
        /// <param name="desiredSize">Specifies the desired size of the icon.</param>
        /// <returns>System.Drawing.Icon that best fit the current display device.</returns>
        public static Icon GetBestFitIcon(Icon icon, Size desiredSize)
        {
            IconInfo info = new IconInfo(icon);
            int index = info.GetBestFitIconIndex(desiredSize);
            return info.Images[index];
        }
        /// <summary>
        /// Gets the System.Drawing.Icon that best fits the current display device.
        /// </summary>
        /// <param name="icon">System.Drawing.Icon to be searched.</param>
        /// <param name="desiredSize">Specifies the desired size of the icon.</param>
        /// <param name="isMonochrome">Specifies whether to get the monochrome icon or the colored one.</param>
        /// <returns>System.Drawing.Icon that best fit the current display device.</returns>
        public static Icon GetBestFitIcon(Icon icon, Size desiredSize, bool isMonochrome)
        {
            IconInfo info = new IconInfo(icon);
            int index = info.GetBestFitIconIndex(desiredSize, isMonochrome);
            return info.Images[index];
        }

        /// <summary>
        /// Extracts an icon (that best fits the current display device) from a givin icon file or an executable module (.dll or an .exe file).
        /// </summary>
        /// <param name="fileName">The path of the icon file or the executable module.</param>
        /// <param name="iconIndex">The index of the icon in the executable module.</param>
        /// <returns>A System.Drawing.Icon (that best fits the current display device) extracted from the file at the specified index in case of an executable module.</returns>
        public static Icon ExtractBestFitIcon(string fileName, int iconIndex)
        {
            Icon icon = ExtractIcon(fileName, iconIndex);
            return GetBestFitIcon(icon);
        }
        /// <summary>
        /// Extracts an icon (that best fits the current display device) from a givin icon file or an executable module (.dll or an .exe file).
        /// </summary>
        /// <param name="fileName">The path of the icon file or the executable module.</param>
        /// <param name="iconIndex">The index of the icon in the executable module.</param>
        /// <param name="desiredSize">Specifies the desired size of the icon.</param>
        /// <returns>A System.Drawing.Icon (that best fits the current display device) extracted from the file at the specified index in case of an executable module.</returns>
        public static Icon ExtractBestFitIcon(string fileName, int iconIndex, Size desiredSize)
        {
            Icon icon = ExtractIcon(fileName, iconIndex);
            return GetBestFitIcon(icon, desiredSize);
        }
        /// <summary>
        /// Extracts an icon (that best fits the current display device) from a givin icon file or an executable module (.dll or an .exe file).
        /// </summary>
        /// <param name="fileName">The path of the icon file or the executable module.</param>
        /// <param name="iconIndex">The index of the icon in the executable module.</param>
        /// <param name="desiredSize">Specifies the desired size of the icon.</param>
        /// <param name="isMonochrome">Specifies whether to get the monochrome icon or the colored one.</param>
        /// <returns>A System.Drawing.Icon (that best fits the current display device) extracted from the file at the specified index in case of an executable module.</returns>
        public static Icon ExtractBestFitIcon(string fileName, int iconIndex, Size desiredSize, bool isMonochrome)
        {
            Icon icon = ExtractIcon(fileName, iconIndex);
            return GetBestFitIcon(icon, desiredSize, isMonochrome);
        }

        /// <summary>
        /// Gets icon associated with the givin file.
        /// </summary>
        /// <param name="fileName">The file path (both absolute and relative paths are valid).</param>
        /// <param name="flags">Specifies which icon to be retrieved (Larg, Small, Selected, Link Overlay and Shell Size).</param>
        /// <returns>A System.Drawing.Icon associated with the givin file.</returns>
        public static Icon GetAssociatedIcon(string fileName, IconFlags flags)
        {
            flags |= IconFlags.Icon;
            SHFILEINFO fileInfo = new SHFILEINFO();
            IntPtr result = Win32.SHGetFileInfo(fileName, 0, ref fileInfo, (uint)Marshal.SizeOf(fileInfo), (SHGetFileInfoFlags) flags);

            if (fileInfo.hIcon == IntPtr.Zero)
                return null;

            return Icon.FromHandle(fileInfo.hIcon);
        }
        /// <summary>
        /// Gets large icon associated with the givin file.
        /// </summary>
        /// <param name="fileName">The file path (both absolute and relative paths are valid).</param>
        /// <returns>A System.Drawing.Icon associated with the givin file.</returns>
        public static Icon GetAssociatedLargeIcon(string fileName)
        {
            return GetAssociatedIcon(fileName, IconFlags.LargeIcon);
        }
        /// <summary>
        /// Gets small icon associated with the givin file.
        /// </summary>
        /// <param name="fileName">The file path (both absolute and relative paths are valid).</param>
        /// <returns>A System.Drawing.Icon associated with the givin file.</returns>
        public static Icon GetAssociatedSmallIcon(string fileName)
        {
            return GetAssociatedIcon(fileName, IconFlags.SmallIcon);
        }

        /// <summary>
        /// Merges a list of icons into one single icon.
        /// </summary>
        /// <param name="icons">The icons to be merged.</param>
        /// <returns>System.Drawing.Icon that contains all the images of the givin icons.</returns>
        public static Icon Merge(params Icon[] icons)
        {
            List<IconInfo> list = new List<IconInfo>(icons.Length);
            int numImages = 0;
            foreach (Icon icon in icons)
            {
                if (icon != null)
                {
                    IconInfo info = new IconInfo(icon);
                    list.Add(info);
                    numImages += info.Images.Count;
                }
            }
            if (list.Count == 0)
            {
                throw new ArgumentNullException("icons", "The icons list should contain at least one icon.");
            }

            //Write the icon to a stream.
            MemoryStream outputStream = new MemoryStream();
            int imageIndex = 0;
            int imageOffset = IconInfo.SizeOfIconDir + numImages * IconInfo.SizeOfIconDirEntry;
            for (int i = 0; i < list.Count; i++)
            {
                IconInfo iconInfo = list[i];
                //The firs image, we should write the icon header.
                if (i == 0)
                {
                    //Get the IconDir and update image count with the new count.
                    IconDir dir = iconInfo.IconDir;
                    dir.Count = (short)numImages;

                    //Write the IconDir header.
                    outputStream.Seek(0, SeekOrigin.Begin);
                    Utility.WriteStructure<IconDir>(outputStream, dir);
                }
                //For each image in the current icon, we should write the IconDirEntry and the image raw data.
                for (int j = 0; j < iconInfo.Images.Count; j++)
                {
                    //Get the IconDirEntry and update the ImageOffset to the new offset.
                    IconDirEntry entry = iconInfo.IconDirEntries[j];
                    entry.ImageOffset = imageOffset;

                    //Write the IconDirEntry to the stream.
                    outputStream.Seek(IconInfo.SizeOfIconDir + imageIndex * IconInfo.SizeOfIconDirEntry, SeekOrigin.Begin);
                    Utility.WriteStructure<IconDirEntry>(outputStream, entry);

                    //Write the image raw data.
                    outputStream.Seek(imageOffset, SeekOrigin.Begin);
                    outputStream.Write(iconInfo.RawData[j], 0, entry.BytesInRes);

                    //Update the imageIndex and the imageOffset
                    imageIndex++;
                    imageOffset += entry.BytesInRes;
                }
            }

            //Create the icon from the stream.
            outputStream.Seek(0, SeekOrigin.Begin);
            Icon resultIcon = new Icon(outputStream);
            outputStream.Close();

            return resultIcon;
        }
        #endregion
    }
}

namespace TAFactory.IconPack
{
	/// <summary>
	/// Get icon resources (RT_GROUP_ICON and RT_ICON) from an executable module (either a .dll or an .exe file).
	/// </summary>
	public class IconExtractor : IDisposable
	{
		#region Public Propreties
		private string _fileName;
		/// <summary>
		/// A fully quallified name of the executable module.
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			private set { _fileName = value; }
		}

		private IntPtr _moduleHandle;
		/// <summary>
		/// Gets the module handle.
		/// </summary>
		public IntPtr ModuleHandle
		{
			get { return _moduleHandle; }
			private set { _moduleHandle = value; }
		}

		private List<ResourceName> _iconNamesList;
		/// <summary>
		/// Gets a list of icons resource names RT_GROUP_ICON;
		/// </summary>
		public List<ResourceName> IconNamesList
		{
			get { return _iconNamesList; }
			private set { _iconNamesList = value; }
		}

		/// <summary>
		/// Gets number of RT_GROUP_ICON found in the executable module.
		/// </summary>
		public int IconCount
		{
			get { return this.IconNamesList.Count; }
		}
		#endregion

		#region Private Properties
		private Dictionary<int, Icon> _iconCache;
		/// <summary>
		/// Gets or sets the RT_GROUP_ICON cache.
		/// </summary>
		private Dictionary<int, Icon> IconCache
		{
			get { return _iconCache; }
			set { _iconCache = value; }
		}
		#endregion

		#region Constructor/Destructor
		/// <summary>
		/// Initializes a new IconExtractor and loads the executable module into the address space of the calling process.
		/// The executable module can be a .dll or an .exe file.
		/// The specified module can cause other modules to be mapped into the address space.
		/// </summary>
		/// <param name="fileName">The name of the executable module (either a .dll or an .exe file). The file name can contain environment variables (like %SystemRoot%).</param>
		public IconExtractor(string fileName)
		{
			LoadLibrary(fileName);
		}
		/// <summary>
		/// Destructs the IconExtractor object.
		/// </summary>
		~IconExtractor()
		{
			Dispose();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets a System.Drawing.Icon that represents RT_GROUP_ICON at the givin index.
		/// </summary>
		/// <param name="index">The index of the RT_GROUP_ICON in the executable module.</param>
		/// <returns>Returns System.Drawing.Icon.</returns>
		public Icon GetIconAt(int index)
		{
			if ( index < 0 || index >= this.IconCount )
			{
				if ( this.IconCount > 0 )
					throw new ArgumentOutOfRangeException("index", index, "Index should be in the range (0-" + this.IconCount.ToString() + ").");
				else
					throw new ArgumentOutOfRangeException("index", index, "No icons in the list.");
			}

			if ( !this.IconCache.ContainsKey(index) )
				this.IconCache[index] = GetIconFromLib(index);

			return this.IconCache[index];
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This function maps a specified executable module into the address space of the calling process.
		/// The executable module can be a .dll or an .exe file.
		/// The specified module can cause other modules to be mapped into the address space.
		/// </summary>
		/// <param name="fileName">The name of the executable module (either a .dll or an .exe file). The file name can contain environment variables (like %SystemRoot%).</param>
		private void LoadLibrary(string fileName)
		{
			if ( string.IsNullOrEmpty(fileName) )
				throw new ArgumentNullException("fileName");

			this.FileName = Environment.ExpandEnvironmentVariables(fileName);
			//Load the executable module into memory using LoadLibraryEx API.
			this.ModuleHandle = Win32.LoadLibraryEx(Environment.ExpandEnvironmentVariables(this.FileName), IntPtr.Zero, LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE);
			if ( this.ModuleHandle == IntPtr.Zero )
			{
				int errorNum = Marshal.GetLastWin32Error();
				switch ( (GetLastErrorResult)errorNum )
				{
					case GetLastErrorResult.ERROR_FILE_NOT_FOUND:
						throw new FileNotFoundException("File not found.", this.FileName);
					case GetLastErrorResult.ERROR_BAD_EXE_FORMAT:
						throw new ArgumentException("The file '" + this.FileName + "' is not a valid win32 executable or dll.");
					default:
						throw new Win32Exception(errorNum);
				}
			}

			this.IconNamesList = new List<ResourceName>();
			this.IconCache = new Dictionary<int, Icon>();

			//Enumurate the resource names of RT_GROUP_ICON by calling EnumResourcesCallBack function for each resource of that type.
			Win32.EnumResourceNames(this.ModuleHandle, ResourceTypes.RT_GROUP_ICON, EnumResourcesCallBack, IntPtr.Zero);
		}
		/// <summary>
		/// The callback function that is being called for each resource (RT_GROUP_ICON, RT_ICON) in the executable module.
		/// The function stores the resource name of type RT_GROUP_ICON into the GroupIconsList and 
		/// stores the resource name of type RT_ICON into the IconsList.
		/// </summary>
		/// <param name="hModule">The module handle.</param>
		/// <param name="lpszType">Specifies the type of the resource being enumurated (RT_GROUP_ICON, RT_ICON).</param>
		/// <param name="lpszName">Specifies the name of the resource being enumurated. For more ifnormation, see the Remarks section.</param>
		/// <param name="lParam">Specifies the application defined parameter passed to the EnumResourceNames function.</param>
		/// <returns>This callback function return true to continue enumuration.</returns>
		/// <remarks>
		/// If the high bit of lpszName is not set (=0), lpszName specifies the integer identifier of the givin resource.
		/// Otherwise, it is a pointer to a null terminated string.
		/// If the first character of the string is a pound sign (#), the remaining characters represent a decimal number that specifies the integer identifier of the resource. For example, the string "#258" represents the identifier 258.
		/// #define IS_INTRESOURCE(_r) ((((ULONG_PTR)(_r)) >> 16) == 0)
		/// </remarks>
		private bool EnumResourcesCallBack(IntPtr hModule, ResourceTypes lpszType, IntPtr lpszName, IntPtr lParam)
		{
			switch ( lpszType )
			{
				case ResourceTypes.RT_GROUP_ICON:
					this.IconNamesList.Add(new ResourceName(lpszName));
					break;
				default:
					break;
			}

			return true;
		}
		/// <summary>
		/// Gets a System.Drawing.Icon that represents RT_GROUP_ICON at the givin index from the executable module.
		/// </summary>
		/// <param name="index">The index of the RT_GROUP_ICON in the executable module.</param>
		/// <returns>Returns System.Drawing.Icon.</returns>
		private Icon GetIconFromLib(int index)
		{
			byte[] resourceData = GetResourceData(this.ModuleHandle, this.IconNamesList[index], ResourceTypes.RT_GROUP_ICON);
			//Convert the resouce into an .ico file image.
			using ( MemoryStream inputStream = new MemoryStream(resourceData) )
			using ( MemoryStream destStream = new MemoryStream() )
			{
				//Read the GroupIconDir header.
				GroupIconDir grpDir = Utility.ReadStructure<GroupIconDir>(inputStream);

				int numEntries = grpDir.Count;
				int iconImageOffset = IconInfo.SizeOfIconDir + numEntries * IconInfo.SizeOfIconDirEntry;

				//Write the IconDir header.
				Utility.WriteStructure<IconDir>(destStream, grpDir.ToIconDir());
				for ( int i = 0; i < numEntries; i++ )
				{
					//Read the GroupIconDirEntry.
					GroupIconDirEntry grpEntry = Utility.ReadStructure<GroupIconDirEntry>(inputStream);

					//Write the IconDirEntry.
					destStream.Seek(IconInfo.SizeOfIconDir + i * IconInfo.SizeOfIconDirEntry, SeekOrigin.Begin);
					Utility.WriteStructure<IconDirEntry>(destStream, grpEntry.ToIconDirEntry(iconImageOffset));

					//Get the icon image raw data and write it to the stream.
					byte[] imgBuf = GetResourceData(this.ModuleHandle, grpEntry.ID, ResourceTypes.RT_ICON);
					destStream.Seek(iconImageOffset, SeekOrigin.Begin);
					destStream.Write(imgBuf, 0, imgBuf.Length);

					//Append the iconImageOffset.
					iconImageOffset += imgBuf.Length;
				}
				destStream.Seek(0, SeekOrigin.Begin);
				return new Icon(destStream);
			}
		}
		/// <summary>
		/// Extracts the raw data of the resource from the module.
		/// </summary>
		/// <param name="hModule">The module handle.</param>
		/// <param name="resrouceName">The name of the resource.</param>
		/// <param name="resourceType">The type of the resource.</param>
		/// <returns>The resource raw data.</returns>
		private static byte[] GetResourceData(IntPtr hModule, ResourceName resourceName, ResourceTypes resourceType)
		{
			//Find the resource in the module.
			IntPtr hResInfo = IntPtr.Zero;
			try { hResInfo = Win32.FindResource(hModule, resourceName.Value, resourceType); }
			finally { resourceName.Free(); }
			if ( hResInfo == IntPtr.Zero )
			{
				throw new Win32Exception();
			}
			//Load the resource.
			IntPtr hResData = Win32.LoadResource(hModule, hResInfo);
			if ( hResData == IntPtr.Zero )
			{
				throw new Win32Exception();
			}
			//Lock the resource to read data.
			IntPtr hGlobal = Win32.LockResource(hResData);
			if ( hGlobal == IntPtr.Zero )
			{
				throw new Win32Exception();
			}
			//Get the resource size.
			int resSize = Win32.SizeofResource(hModule, hResInfo);
			if ( resSize == 0 )
			{
				throw new Win32Exception();
			}
			//Allocate the requested size.
			byte[] buf = new byte[resSize];
			//Copy the resource data into our buffer.
			Marshal.Copy(hGlobal, buf, 0, buf.Length);

			return buf;
		}
		/// <summary>
		/// Extracts the raw data of the resource from the module.
		/// </summary>
		/// <param name="hModule">The module handle.</param>
		/// <param name="resrouceName">The identifier of the resource.</param>
		/// <param name="resourceType">The type of the resource.</param>
		/// <returns>The resource raw data.</returns>
		private static byte[] GetResourceData(IntPtr hModule, int resourceId, ResourceTypes resourceType)
		{
			//Find the resource in the module.
			IntPtr hResInfo = Win32.FindResource(hModule, (IntPtr)resourceId, resourceType);
			if ( hResInfo == IntPtr.Zero )
			{
				throw new Win32Exception();
			}
			//Load the resource.
			IntPtr hResData = Win32.LoadResource(hModule, hResInfo);
			if ( hResData == IntPtr.Zero )
			{
				throw new Win32Exception();
			}
			//Lock the resource to read data.
			IntPtr hGlobal = Win32.LockResource(hResData);
			if ( hGlobal == IntPtr.Zero )
			{
				throw new Win32Exception();
			}
			//Get the resource size.
			int resSize = Win32.SizeofResource(hModule, hResInfo);
			if ( resSize == 0 )
			{
				throw new Win32Exception();
			}
			//Allocate the requested size.
			byte[] buf = new byte[resSize];
			//Copy the resource data into our buffer.
			Marshal.Copy(hGlobal, buf, 0, buf.Length);

			return buf;
		}
		#endregion

		#region IDisposable Members
		/// <summary>
		/// Releases the resources of that object.
		/// </summary>
		public void Dispose()
		{
			if ( this.ModuleHandle != IntPtr.Zero )
			{
				try { Win32.FreeLibrary(this.ModuleHandle); }
				catch { }
				this.ModuleHandle = IntPtr.Zero;
			}
			if ( this.IconNamesList != null )
				this.IconNamesList.Clear();
		}
		#endregion
	}
}

namespace TAFactory.IconPack
{
	/// <summary>
	/// Presents an Icon Directory.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 6)]
	public struct IconDir
	{
		public WORD Reserved;   // Reserved (must be 0)
		public WORD Type;       // Resource Type (1 for icons)
		public WORD Count;      // How many images?

		/// <summary>
		/// Converts the current TAFactory.IconPack.IconDir into TAFactory.IconPack.GroupIconDir.
		/// </summary>
		/// <returns>TAFactory.IconPack.GroupIconDir</returns>
		public GroupIconDir ToGroupIconDir()
		{
			GroupIconDir grpDir = new GroupIconDir();
			grpDir.Reserved = this.Reserved;
			grpDir.Type = this.Type;
			grpDir.Count = this.Count;
			return grpDir;
		}
	}

	/// <summary>
	/// Presents an Icon Directory Entry.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	public struct IconDirEntry
	{
		public BYTE Width;          // Width, in pixels, of the image
		public BYTE Height;         // Height, in pixels, of the image
		public BYTE ColorCount;     // Number of colors in image (0 if >=8bpp)
		public BYTE Reserved;       // Reserved ( must be 0)
		public WORD Planes;         // Color Planes
		public WORD BitCount;       // Bits per pixel
		public DWORD BytesInRes;     // How many bytes in this resource?
		public DWORD ImageOffset;    // Where in the file is this image?

		/// <summary>
		/// Converts the current TAFactory.IconPack.IconDirEntry into TAFactory.IconPack.GroupIconDirEntry.
		/// </summary>
		/// <param name="id">The resource identifier.</param>
		/// <returns>TAFactory.IconPack.GroupIconDirEntry</returns>
		public GroupIconDirEntry ToGroupIconDirEntry(int id)
		{
			GroupIconDirEntry grpEntry = new GroupIconDirEntry();
			grpEntry.Width = this.Width;
			grpEntry.Height = this.Height;
			grpEntry.ColorCount = this.ColorCount;
			grpEntry.Reserved = this.Reserved;
			grpEntry.Planes = this.Planes;
			grpEntry.BitCount = this.BitCount;
			grpEntry.BytesInRes = this.BytesInRes;
			grpEntry.ID = (short)id;
			return grpEntry;
		}
	}

	/// <summary>
	/// Presents a Group Icon Directory.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 6)]
	public struct GroupIconDir
	{
		public WORD Reserved;   // Reserved (must be 0)
		public WORD Type;       // Resource Type (1 for icons)
		public WORD Count;      // How many images?

		/// <summary>
		/// Converts the current TAFactory.IconPack.GroupIconDir into TAFactory.IconPack.IconDir.
		/// </summary>
		/// <returns>TAFactory.IconPack.IconDir</returns>
		public IconDir ToIconDir()
		{
			IconDir dir = new IconDir();
			dir.Reserved = this.Reserved;
			dir.Type = this.Type;
			dir.Count = this.Count;
			return dir;
		}
	}

	/// <summary>
	/// Presents a Group Icon Directory Entry.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 14)]
	public struct GroupIconDirEntry
	{
		public BYTE Width;          // Width, in pixels, of the image
		public BYTE Height;         // Height, in pixels, of the image
		public BYTE ColorCount;     // Number of colors in image (0 if >=8bpp)
		public BYTE Reserved;       // Reserved ( must be 0)
		public WORD Planes;         // Color Planes
		public WORD BitCount;       // Bits per pixel
		public DWORD BytesInRes;     // How many bytes in this resource?
		public WORD ID;             // the ID

		/// <summary>
		/// Converts the current TAFactory.IconPack.GroupIconDirEntry into TAFactory.IconPack.IconDirEntry.
		/// </summary>
		/// <param name="id">The resource identifier.</param>
		/// <returns>TAFactory.IconPack.IconDirEntry</returns>
		public IconDirEntry ToIconDirEntry(int imageOffiset)
		{
			IconDirEntry entry = new IconDirEntry();
			entry.Width = this.Width;
			entry.Height = this.Height;
			entry.ColorCount = this.ColorCount;
			entry.Reserved = this.Reserved;
			entry.Planes = this.Planes;
			entry.BitCount = this.BitCount;
			entry.BytesInRes = this.BytesInRes;
			entry.ImageOffset = imageOffiset;
			return entry;
		}
	}
}

namespace TAFactory.IconPack
{
	/// <summary>
	/// Provides information about a givin icon.
	/// This class cannot be inherited.
	/// </summary>
	[Serializable]
	public class IconInfo
	{
		#region ReadOnly
		public static int SizeOfIconDir = Marshal.SizeOf(typeof(IconDir));
		public static int SizeOfIconDirEntry = Marshal.SizeOf(typeof(IconDirEntry));
		public static int SizeOfGroupIconDir = Marshal.SizeOf(typeof(GroupIconDir));
		public static int SizeOfGroupIconDirEntry = Marshal.SizeOf(typeof(GroupIconDirEntry));
		#endregion

		#region Properties
		private Icon _sourceIcon;
		/// <summary>
		/// Gets the source System.Drawing.Icon.
		/// </summary>
		public Icon SourceIcon
		{
			get { return _sourceIcon; }
			private set { _sourceIcon = value; }
		}

		private string _fileName = null;
		/// <summary>
		/// Gets the icon's file name. 
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			private set { _fileName = value; }
		}

		private List<Icon> _images;
		/// <summary>
		/// Gets a list System.Drawing.Icon that presents the icon contained images.
		/// </summary>
		public List<Icon> Images
		{
			get { return _images; }
			private set { _images = value; }
		}

		/// <summary>
		/// Get whether the icon contain more than one image or not.
		/// </summary>
		public bool IsMultiIcon
		{
			get { return (this.Images.Count > 1); }
		}

		private int _bestFitIconIndex;
		/// <summary>
		/// Gets icon index that best fits to screen resolution.
		/// </summary>
		public int BestFitIconIndex
		{
			get { return _bestFitIconIndex; }
			private set { _bestFitIconIndex = value; }
		}

		private int _width;
		/// <summary>
		/// Gets icon width.
		/// </summary>
		public int Width
		{
			get { return _width; }
			private set { _width = value; }
		}

		private int _height;
		/// <summary>
		/// Gets icon height.
		/// </summary>
		public int Height
		{
			get { return _height; }
			private set { _height = value; }
		}

		private int _colorCount;
		/// <summary>
		/// Gets number of colors in icon (0 if >=8bpp).
		/// </summary>
		public int ColorCount
		{
			get { return _colorCount; }
			private set { _colorCount = value; }
		}

		private int _planes;
		/// <summary>
		/// Gets icon color planes.
		/// </summary>
		public int Planes
		{
			get { return _planes; }
			private set { _planes = value; }
		}

		private int _bitCount;
		/// <summary>
		/// Gets icon bits per pixel (0 if < 8bpp).
		/// </summary>
		public int BitCount
		{
			get { return _bitCount; }
			private set { _bitCount = value; }
		}

		/// <summary>
		/// Gets icon bits per pixel.
		/// </summary>
		public int ColorDepth
		{
			get
			{
				if ( this.BitCount != 0 )
					return this.BitCount;
				if ( this.ColorCount == 0 )
					return 0;
				return (int)Math.Log(this.ColorCount, 2);
			}
		}
		#endregion

		#region Icon Headers Properties
		private IconDir _iconDir;
		/// <summary>
		/// Gets the TAFactory.IconPack.IconDir of the icon.
		/// </summary>
		public IconDir IconDir
		{
			get { return _iconDir; }
			private set { _iconDir = value; }
		}

		private GroupIconDir _groupIconDir;
		/// <summary>
		/// Gets the TAFactory.IconPack.GroupIconDir of the icon.
		/// </summary>
		public GroupIconDir GroupIconDir
		{
			get { return _groupIconDir; }
			private set { _groupIconDir = value; }
		}

		private List<IconDirEntry> _iconDirEntries;
		/// <summary>
		/// Gets a list of TAFactory.IconPack.IconDirEntry of the icon.
		/// </summary>
		public List<IconDirEntry> IconDirEntries
		{
			get { return _iconDirEntries; }
			private set { _iconDirEntries = value; }
		}

		private List<GroupIconDirEntry> _groupIconDirEntries;
		/// <summary>
		/// Gets a list of TAFactory.IconPack.GroupIconDirEntry of the icon.
		/// </summary>
		public List<GroupIconDirEntry> GroupIconDirEntries
		{
			get { return _groupIconDirEntries; }
			private set { _groupIconDirEntries = value; }
		}

		private List<byte[]> _rawData;
		/// <summary>
		/// Gets a list of raw data for each icon image.
		/// </summary>
		public List<byte[]> RawData
		{
			get { return _rawData; }
			private set { _rawData = value; }
		}

		private byte[] _resourceRawData;
		/// <summary>
		/// Gets the icon raw data as a resource data.
		/// </summary>
		public byte[] ResourceRawData
		{
			get { return _resourceRawData; }
			set { _resourceRawData = value; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Intializes a new instance of TAFactory.IconPack.IconInfo which contains the information about the givin icon.
		/// </summary>
		/// <param name="icon">A System.Drawing.Icon object to retrieve the information about.</param>
		public IconInfo(Icon icon)
		{
			this.FileName = null;
			LoadIconInfo(icon);
		}

		/// <summary>
		/// Intializes a new instance of TAFactory.IconPack.IconInfo which contains the information about the icon in the givin file.
		/// </summary>
		/// <param name="fileName">A fully qualified name of the icon file, it can contain environment variables.</param>
		public IconInfo(string fileName)
		{
			this.FileName = FileName;
			LoadIconInfo(new Icon(fileName));
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets the index of the icon that best fits the current display device.
		/// </summary>
		/// <returns>The icon index.</returns>
		public int GetBestFitIconIndex()
		{
			int iconIndex = 0;
			IntPtr resBits = Marshal.AllocHGlobal(this.ResourceRawData.Length);
			Marshal.Copy(this.ResourceRawData, 0, resBits, this.ResourceRawData.Length);
			try { iconIndex = Win32.LookupIconIdFromDirectory(resBits, true); }
			finally { Marshal.FreeHGlobal(resBits); }

			return iconIndex;
		}
		/// <summary>
		/// Gets the index of the icon that best fits the current display device.
		/// </summary>
		/// <param name="desiredSize">Specifies the desired size of the icon.</param>
		/// <returns>The icon index.</returns>
		public int GetBestFitIconIndex(Size desiredSize)
		{
			return GetBestFitIconIndex(desiredSize, false);
		}
		/// <summary>
		/// Gets the index of the icon that best fits the current display device.
		/// </summary>
		/// <param name="desiredSize">Specifies the desired size of the icon.</param>
		/// <param name="isMonochrome">Specifies whether to get the monochrome icon or the colored one.</param>
		/// <returns>The icon index.</returns>
		public int GetBestFitIconIndex(Size desiredSize, bool isMonochrome)
		{
			int iconIndex = 0;
			LookupIconIdFromDirectoryExFlags flags = LookupIconIdFromDirectoryExFlags.LR_DEFAULTCOLOR;
			if ( isMonochrome )
				flags = LookupIconIdFromDirectoryExFlags.LR_MONOCHROME;
			IntPtr resBits = Marshal.AllocHGlobal(this.ResourceRawData.Length);
			Marshal.Copy(this.ResourceRawData, 0, resBits, this.ResourceRawData.Length);
			try { iconIndex = Win32.LookupIconIdFromDirectoryEx(resBits, true, desiredSize.Width, desiredSize.Height, flags); }
			finally { Marshal.FreeHGlobal(resBits); }

			return iconIndex;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Loads the icon information from the givin icon into class members.
		/// </summary>
		/// <param name="icon">A System.Drawing.Icon object to retrieve the information about.</param>
		private void LoadIconInfo(Icon icon)
		{
			if ( icon == null )
				throw new ArgumentNullException("icon");

			this.SourceIcon = icon;
			MemoryStream inputStream = new MemoryStream();
			this.SourceIcon.Save(inputStream);

			inputStream.Seek(0, SeekOrigin.Begin);
			IconDir dir = Utility.ReadStructure<IconDir>(inputStream);

			this.IconDir = dir;
			this.GroupIconDir = dir.ToGroupIconDir();

			this.Images = new List<Icon>(dir.Count);
			this.IconDirEntries = new List<IconDirEntry>(dir.Count);
			this.GroupIconDirEntries = new List<GroupIconDirEntry>(dir.Count);
			this.RawData = new List<byte[]>(dir.Count);

			IconDir newDir = dir;
			newDir.Count = 1;
			for ( int i = 0; i < dir.Count; i++ )
			{
				inputStream.Seek(SizeOfIconDir + i * SizeOfIconDirEntry, SeekOrigin.Begin);

				IconDirEntry entry = Utility.ReadStructure<IconDirEntry>(inputStream);

				this.IconDirEntries.Add(entry);
				this.GroupIconDirEntries.Add(entry.ToGroupIconDirEntry(i));

				byte[] content = new byte[entry.BytesInRes];
				inputStream.Seek(entry.ImageOffset, SeekOrigin.Begin);
				inputStream.Read(content, 0, content.Length);
				this.RawData.Add(content);

				IconDirEntry newEntry = entry;
				newEntry.ImageOffset = SizeOfIconDir + SizeOfIconDirEntry;

				MemoryStream outputStream = new MemoryStream();
				Utility.WriteStructure<IconDir>(outputStream, newDir);
				Utility.WriteStructure<IconDirEntry>(outputStream, newEntry);
				outputStream.Write(content, 0, content.Length);

				outputStream.Seek(0, SeekOrigin.Begin);
				Icon newIcon = new Icon(outputStream);
				outputStream.Close();

				this.Images.Add(newIcon);
				if ( dir.Count == 1 )
				{
					this.BestFitIconIndex = 0;

					this.Width = entry.Width;
					this.Height = entry.Height;
					this.ColorCount = entry.ColorCount;
					this.Planes = entry.Planes;
					this.BitCount = entry.BitCount;
				}
			}
			inputStream.Close();
			this.ResourceRawData = GetIconResourceData();

			if ( dir.Count > 1 )
			{
				this.BestFitIconIndex = GetBestFitIconIndex();

				this.Width = this.IconDirEntries[this.BestFitIconIndex].Width;
				this.Height = this.IconDirEntries[this.BestFitIconIndex].Height;
				this.ColorCount = this.IconDirEntries[this.BestFitIconIndex].ColorCount;
				this.Planes = this.IconDirEntries[this.BestFitIconIndex].Planes;
				this.BitCount = this.IconDirEntries[this.BestFitIconIndex].BitCount;
			}

		}
		/// <summary>
		/// Returns the icon's raw data as a resource data.
		/// </summary>
		/// <returns>The icon's raw as a resource data.</returns>
		private byte[] GetIconResourceData()
		{
			MemoryStream outputStream = new MemoryStream();
			Utility.WriteStructure<GroupIconDir>(outputStream, this.GroupIconDir);
			foreach ( GroupIconDirEntry entry in this.GroupIconDirEntries )
			{
				Utility.WriteStructure<GroupIconDirEntry>(outputStream, entry);
			}

			return outputStream.ToArray();
		}
		#endregion
	}
}

namespace TAFactory.IconPack
{
	/// <summary>
	/// Represents a resource name (either integer resource or string resource).
	/// </summary>
	public class ResourceName : IDisposable
	{
		#region Properties
		private int? _id;
		/// <summary>
		/// Gets the resource identifier, returns null if the resource is not an integer resource.
		/// </summary>
		public int? Id
		{
			get { return _id; }
			private set { _id = value; }
		}

		private string _name;
		/// <summary>
		/// Gets the resource name, returns null if the resource is not a string resource.
		/// </summary>
		public string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		private IntPtr _value;
		/// <summary>
		/// Gets a pointer to resource name that can be used in FindResource function.
		/// </summary>
		public IntPtr Value
		{
			get
			{
				if ( this.IsIntResource )
					return new IntPtr(this.Id.Value);

				if ( this._value == IntPtr.Zero )
					this._value = Marshal.StringToHGlobalAuto(this.Name);

				return _value;
			}
			private set { _value = value; }
		}

		/// <summary>
		/// Gets whether the resource is an integer resource.
		/// </summary>
		public bool IsIntResource
		{
			get { return (this.Id != null); }
		}
		#endregion

		#region Constructor/Destructor
		/// <summary>
		/// Initializes a new TAFactory.IconPack.ResourceName object.
		/// </summary>
		/// <param name="lpszName">Specifies the resource name. For more ifnormation, see the Remarks section.</param>
		/// <remarks>
		/// If the high bit of lpszName is not set (=0), lpszName specifies the integer identifier of the givin resource.
		/// Otherwise, it is a pointer to a null terminated string.
		/// If the first character of the string is a pound sign (#), the remaining characters represent a decimal number that specifies the integer identifier of the resource. For example, the string "#258" represents the identifier 258.
		/// #define IS_INTRESOURCE(_r) ((((ULONG_PTR)(_r)) >> 16) == 0).
		/// </remarks>
		public ResourceName(IntPtr lpName)
		{
			if ( ((uint)lpName >> 16) == 0 )  //Integer resource
			{
				this.Id = lpName.ToInt32();
				this.Name = null;
			}
			else
			{
				this.Id = null;
				this.Name = Marshal.PtrToStringAuto(lpName);
			}
		}
		/// <summary>
		/// Destructs the ResourceName object.
		/// </summary>
		~ResourceName()
		{
			Dispose();
		}
		#endregion

		#region Public Functions
		/// <summary>
		/// Returns a System.String that represents the current TAFactory.IconPack.ResourceName.
		/// </summary>
		/// <returns>Returns a System.String that represents the current TAFactory.IconPack.ResourceName.</returns>
		public override string ToString()
		{
			if ( this.IsIntResource )
				return "#" + this.Id.ToString();

			return this.Name;
		}
		/// <summary>
		/// Releases the pointer to the resource name.
		/// </summary>
		public void Free()
		{
			if ( this._value != IntPtr.Zero )
			{
				try { Marshal.FreeHGlobal(this._value); }
				catch { }
				this._value = IntPtr.Zero;
			}
		}
		#endregion

		#region IDisposable Members
		/// <summary>
		/// Release the pointer to the resource name.
		/// </summary>
		public void Dispose()
		{
			Free();
		}
		#endregion
	}
}

namespace TAFactory.Utilities
{
	/// <summary>
	/// Holds a set of utilities.
	/// </summary>
	public static class Utility
	{
		#region Stream Utilities
		/// <summary>
		/// Reads a structure of type T from the input stream.
		/// </summary>
		/// <typeparam name="T">The structure type to be read.</typeparam>
		/// <param name="inputStream">The input stream to read from.</param>
		/// <returns>A structure of type T that was read from the stream.</returns>
		public static T ReadStructure<T>(Stream inputStream) where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));
			byte[] buffer = new byte[size];
			inputStream.Read(buffer, 0, size);
			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.Copy(buffer, 0, ptr, size);
			object ret = Marshal.PtrToStructure(ptr, typeof(T));
			Marshal.FreeHGlobal(ptr);

			return (T)ret;
		}
		/// <summary>
		/// Writes as structure of type T to the output stream.
		/// </summary>
		/// <typeparam name="T">The structure type to be written.</typeparam>
		/// <param name="outputStream">The output stream to write to.</param>
		/// <param name="structure">The structure to be written.</param>
		public static void WriteStructure<T>(Stream outputStream, T structure) where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));
			byte[] buffer = new byte[size];
			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(structure, ptr, true);
			Marshal.Copy(ptr, buffer, 0, size);
			Marshal.FreeHGlobal(ptr);
			outputStream.Write(buffer, 0, size);
		}
		#endregion
	}
}

namespace Microsoft.API
{
	[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Auto)]
	public delegate bool EnumResNameProc(IntPtr hModule, ResourceTypes lpszType, IntPtr lpszName, IntPtr lParam);

	#region Enumurations
	[Flags]
	public enum LoadLibraryExFlags : int
	{
		DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
		LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
		LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
	}
	public enum GetLastErrorResult : int
	{
		ERROR_SUCCESS = 0,
		ERROR_FILE_NOT_FOUND = 2,
		ERROR_BAD_EXE_FORMAT = 193,
		ERROR_RESOURCE_TYPE_NOT_FOUND = 1813
	}
	public enum ResourceTypes : int
	{
		RT_ICON = 3,
		RT_GROUP_ICON = 14
	}
	public enum LookupIconIdFromDirectoryExFlags : int
	{
		LR_DEFAULTCOLOR = 0,
		LR_MONOCHROME = 1
	}
	public enum LoadImageTypes : int
	{
		IMAGE_BITMAP = 0,
		IMAGE_ICON = 1,
		IMAGE_CURSOR = 2
	}
	[Flags]
	public enum SHGetFileInfoFlags : int
	{
		Icon = 0x000000100,     // get icon
		DisplayName = 0x000000200,     // get display name
		TypeName = 0x000000400,     // get type name
		Attributes = 0x000000800,     // get attributes
		IconLocation = 0x000001000,     // get icon location
		ExeType = 0x000002000,     // return exe type
		SysIconIndex = 0x000004000,     // get system icon index
		LinkOverlay = 0x000008000,     // put a link overlay on icon
		Selected = 0x000010000,     // show icon in selected state
		AttrSpecified = 0x000020000,     // get only specified attributes
		LargeIcon = 0x000000000,     // get large icon
		SmallIcon = 0x000000001,     // get small icon
		OpenIcon = 0x000000002,     // get open icon
		ShellIconSize = 0x000000004,     // get shell size icon
		PIDL = 0x000000008,     // pszPath is a pidl
		UseFileAttributes = 0x000000010      // use passed dwFileAttribute
	}
	#endregion

	#region Structures
	[StructLayout(LayoutKind.Sequential)]
	public struct SHFILEINFO
	{
		public IntPtr hIcon;
		public IntPtr iIcon;
		public uint dwAttributes;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	};
	#endregion

	public static class Win32
	{
		#region Constants
		public const int MAX_PATH = 260;
		#endregion

		#region Helper Functions
		public static bool IsIntResource(IntPtr lpszName)
		{
			return (((uint)lpszName >> 16) == 0);
		}
		#endregion

		#region API Functions
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryExFlags dwFlags);

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		public static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool EnumResourceNames(IntPtr hModule, ResourceTypes lpszType, EnumResNameProc lpEnumFunc, IntPtr lParam);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, ResourceTypes lpType);

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr LockResource(IntPtr hResData);

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		public static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);

		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		public static extern int LookupIconIdFromDirectory(IntPtr presbits, bool fIcon);

		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		public static extern int LookupIconIdFromDirectoryEx(IntPtr presbits, bool fIcon, int cxDesired, int cyDesired, LookupIconIdFromDirectoryExFlags Flags);

		[DllImport("user32.dll", EntryPoint = "LoadImageW", SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr LoadImage(IntPtr hInstance, IntPtr lpszName, LoadImageTypes imageType, int cxDesired, int cyDesired, uint fuLoad);

		[DllImport("shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, SHGetFileInfoFlags uFlags);
		#endregion
	}
}
