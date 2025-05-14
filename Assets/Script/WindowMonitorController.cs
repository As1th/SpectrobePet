using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowMonitorController : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll")]
    static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll")]
    static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    private List<RECT> monitorRects = new List<RECT>();
    private int currentMonitorIndex = 0;
    private IntPtr hwnd;

    void Start()
    {
        hwnd = GetActiveWindow();
        RefreshMonitorList();

        // Important: Force windowed mode, not fullscreen or exclusive fullscreen.
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    void OnMouseDown()
    {
       
            MoveToNextMonitor();
        
    }

    void RefreshMonitorList()
    {
        monitorRects.Clear();

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            new MonitorEnumDelegate((IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr data) =>
            {
                monitorRects.Add(rect);
                return true;
            }), IntPtr.Zero);
    }

    void MoveToNextMonitor()
    {
       // Debug.LogError("DOING");
        if (monitorRects.Count == 0) return;

        currentMonitorIndex = (currentMonitorIndex + 1) % monitorRects.Count;
        RECT targetRect = monitorRects[currentMonitorIndex];

        int width = Screen.width;
        int height = Screen.height;

        // Optional: match monitor size
        width = targetRect.right - targetRect.left;
        height = targetRect.bottom - targetRect.top;

        bool moved = MoveWindow(hwnd, targetRect.left, targetRect.top, width, height, true);
      //  Debug.LogError($"Moving to Monitor {currentMonitorIndex + 1}: {targetRect.left},{targetRect.top} - Success: {moved}");
    }
}
