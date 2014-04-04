using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace saga.util
{
	public class WindowHandleSearch
	{
		protected delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		protected static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowProc lpEnumFunc, IntPtr lParam);

		protected delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);
		[DllImport("user32.Dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		protected static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

		[DllImport("user32.dll")]
		protected static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
		[DllImport("user32.dll")]
		private static extern bool SetWindowText(IntPtr hWnd, String lpString);
		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		protected static extern IntPtr FindWindow(String lpszClass, String lpszWindow);

		// ハンドル保持用リスト
		protected List<IntPtr> hWndList;
		// ウィンドウハンドル
		protected IntPtr hWnd;
		// ウィンドウ名
		protected string windowName;

		/*
		 * コンストラクタ
		 * @param windowName 検索ウィンドウ名
		 */
		public WindowHandleSearch(string windowName)
		{
			this.windowName = windowName;

			this.hWndList = new List<IntPtr>();
			Update();
		}
		/*
		 * 子ウィンドウハンドルリストを取得
		 * @return 子ウィンドウハンドルリスト
		 */
		public List<IntPtr> GetList()
		{
			return hWndList;
		}
		/*
		 * 子ウィンドウハンドルリストのindex目を取得
		 * @param index インデックス
		 * @throw IndexOutOfRangeException インデックスの範囲外です
		 * @return ウィンドウハンドル -1:親ウィンドウハンドル
		 */
		public IntPtr GetList(int index)
		{
			if (index == -1)
			{
				return hWnd;
			}
			return hWndList[index];
		}
		// 子ハンドルのリスト更新
		public void Update()
		{
			hWndList.Clear();
			this.hWnd = GetWindowHandle(windowName);
			if (hWnd.Equals((IntPtr)0))
			{
				string str = "\"" + windowName + "\"は起動していません";
				throw new ApplicationException(str);
			}
			GCHandle listHandle = GCHandle.Alloc(hWndList);
			try
			{
				EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
				EnumChildWindows(hWnd, childProc, GCHandle.ToIntPtr(listHandle));
			}
			finally
			{
				if (listHandle.IsAllocated)
				{
					listHandle.Free();
				}
			}
		}

		// ウィンドウ列挙
		protected static bool EnumWindow(IntPtr handle, IntPtr pointer)
		{
			GCHandle gch = GCHandle.FromIntPtr(pointer);
			List<IntPtr> list = gch.Target as List<IntPtr>;
			if (list == null)
			{
				throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
			}
			list.Add(handle);
			//  You can modify this to check to see if you want to cancel the operation, then return a null here
			return true;
		}

		/*
		 * 引数のウィンドウハンドルを取得
		 * @param titleStr ウィンドウ名
		 * @throws ApplicationException Voiceroidが起動していません
		 * @return ウィンドウハンドル
		 */
		protected IntPtr GetWindowHandle(String titleStr)
		{
			Process[] ps = Process.GetProcesses();

			foreach (Process pitem in ps)
			{
				if ((pitem.MainWindowHandle != IntPtr.Zero) && (pitem.MainWindowTitle.Equals(titleStr)))
				{
					return pitem.MainWindowHandle;
				}
			}
			IntPtr hWnd = FindWindow(null, windowName);
			if (hWnd != null)
			{
				return hWnd;
			}
			return (IntPtr)0;
		}

	}
}
