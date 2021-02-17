using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ccMushroom
{
	class IconTest
	{
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern int GetDIBColorTable(
		  IntPtr dc, int index, int entries,
		  [In, Out] RgbQuad[] colors);
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern int GetDIBits(
		  IntPtr dc, IntPtr bmp, int startScan, int scanLineCount,
		  [In, Out] byte[] data, IntPtr info, ColorTableType usage);
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetIconInfo(
		  IntPtr icon, out IconInfo info);
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetDC(IntPtr window);
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool ReleaseDC(IntPtr window, IntPtr dc);
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern IntPtr SelectObject(
		  IntPtr hdc, IntPtr obj);
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr handle);
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteDC(IntPtr hdc);

		private enum ColorTableType
		{
			Rgb = 0,
			Palette = 1,
		}
		private struct IconInfo
		{
			public bool IsIcon;
			public int HotSpotX;
			public int HotSoptY;
			public IntPtr MaskHbitmap;
			public IntPtr ColorHbitmap;
		}
		private struct RgbQuad
		{
			public byte Blue;
			public byte Green;
			public byte Red;
			public byte Reserved;
		}
		private struct BitmapInfoHeader
		{
			public int Size;
			public int Width;
			public int Height;
			public short Planes;
			public short BitCount;
			public int Compression;
			public int SizeImage;
			public int XPelsPerMeter;
			public int YPelsPerMeter;
			public int ColorTableUsed;
			public int ColorImportant;
		}
		private class BitmapInfo
		{
			public BitmapInfoHeader Header;
			public RgbQuad[] ColorTable;
			public byte[] ImageData;
			public override string ToString()
			{
				string header
				  = string.Format(
					  "Size:({0},{1}) BitCount:{2} SizeImage:{3}",
					  this.Header.Width, this.Header.Height,
					  this.Header.BitCount, this.Header.SizeImage);
				StringBuilder table = new StringBuilder();
				foreach ( RgbQuad color in this.ColorTable )
				{
					table.AppendFormat("Color [{0},{1},{2}], ",
									   color.Red, color.Green, color.Blue);
				}
				string data = BitConverter.ToString(this.ImageData);
				return string.Format(
					"Header:[{0}]\r\nColorTable:[{1}]\r\nImageData:[{2}]",
					header, table, data.Replace("-", " "));
			}
		}
		/*static void Main()
		{
			GetIconImage(new Icon("icooo4bpp16x16.ico").Handle);
		}*/
		public static void GetIconImage(IntPtr iconHandle)
		{
			IconInfo iconInfo;
			GetIconInfo(iconHandle, out iconInfo);
			IntPtr maskbmp = iconInfo.MaskHbitmap;
			IntPtr colorbmp = iconInfo.ColorHbitmap;
			BitmapInfo info;
			info = GetDIBits(iconInfo.MaskHbitmap);
			Console.WriteLine(info);
			Console.WriteLine(GetDIBits(iconInfo.ColorHbitmap));
			DeleteObject(iconInfo.MaskHbitmap);
			DeleteObject(iconInfo.ColorHbitmap);
		}
		private static BitmapInfo GetDIBits(IntPtr hBmp)
		{
			IntPtr wdc = GetDC(IntPtr.Zero);
			IntPtr dc = CreateCompatibleDC(wdc);
			IntPtr old = SelectObject(dc, hBmp);
			BitmapInfo bmpInfo = new BitmapInfo();
			int headerSize = Marshal.SizeOf(typeof(BitmapInfoHeader));
			IntPtr info = Marshal.AllocCoTaskMem(headerSize + 4 * 256);
			try
			{
				unsafe
				{
					BitmapInfoHeader* header
						= (BitmapInfoHeader*)info.ToPointer();
					header->Size = headerSize;
					header->BitCount = 0;
					int result = GetDIBits(dc, hBmp, 0, 0, null,
										   info, ColorTableType.Rgb);
					int colorCount = header->ColorTableUsed;
					bmpInfo.ColorTable = new RgbQuad[colorCount];
					if ( colorCount > 0 )
					{
						GetDIBColorTable(dc, 0, colorCount,
										 bmpInfo.ColorTable);
					}
					if ( result != 0 )
					{
						byte[] data = new byte[header->SizeImage];
						GetDIBits(dc, hBmp, 0, header->Height, data,
								  info, ColorTableType.Rgb);
						bmpInfo.ImageData = data;
					}
					bmpInfo.Header = *header;
					return bmpInfo;
				}
			}
			finally
			{
				Marshal.FreeCoTaskMem(info);
				SelectObject(dc, old);
				DeleteDC(dc);
				ReleaseDC(IntPtr.Zero, wdc);
			}
		}
	}
}
