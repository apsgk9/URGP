using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugRuntimeConsole : MonoBehaviour
{
    /// <summary>
    /// A struct to hold all the info about a debug message.
    /// </summary>
    private struct DebugMessage
    {
        /// <summary> The condition text. </summary>
        public string Condition { get; set; }
        /// <summary> The stack trace. </summary>
        public string StackTrace { get; set; }
        /// <summary> The type of the log. </summary>
        public LogType Type { get; set; }

        public DebugMessage(string condition, string stackTrace, LogType type)
        {
            Condition = condition;
            StackTrace = stackTrace;
            Type = type;
        }
    }

    [SerializeField]
    [Tooltip("Determines if the console should be enabled.")]
    private bool m_EnableConsole = false;
    [SerializeField]
    [Tooltip("Determines if timestamps should be added to the log message.")]
    private bool m_ShowTimestamps = true;
    [SerializeField]
    [Tooltip("Determines if the game object should be kept through scenes.")]
    private bool m_DontDestroyOnLoad = true;
    [SerializeField]
    [Tooltip("Determines if the logs can be exported into a text file.")]
    private bool m_CanExportLogs = true;
    [SerializeField]
    [Tooltip("An optional IMGUI skin.")]
    private GUISkin m_GUISkin;

    [Header("Color Settings")]
    [SerializeField]
    [Tooltip("The color for the basic log message.")]
    private Color m_LogColor = Color.white;
    [SerializeField]
    [Tooltip("The color for warning messages.")]
    private Color m_WarningColor = Color.yellow;
    [SerializeField]
    [Tooltip("The color for error messages.")]
    private Color m_ErrorColor = Color.red;
    [SerializeField]
    [Tooltip("The color for exception messages.")]
    private Color m_ExceptionColor = Color.red;
    [SerializeField]
    [Tooltip("The color for assert messages.")]
    private Color m_AssertColor = Color.red;

    // The entry to be expanded. -1 means no entry.
    private int m_ExpandedEntry = -1;

    // Determines if the console should be shown.
    private bool m_Show;

    // The Vector2 for the scroll bar.
    private Vector2 m_Scroll;

    // All the messages.
    private List<DebugMessage> m_Messages = new List<DebugMessage>();
    PlayerInputActions PlayerInput;
    private bool m_ToggleKey;

    private void Awake()
    {
        
        PlayerInput=new PlayerInputActions();
    }
    private void Down(InputAction.CallbackContext obj)
    {
        m_ToggleKey=true;
    }

    void Start()
    {
        // If Dont Destroy On Load is marked, mark the game object as such.
        if (m_DontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);

            // If it can find another debug console, destroy the object current game object.
            DebugRuntimeConsole otherConsole = FindObjectOfType<DebugRuntimeConsole>();
            if (otherConsole != null && otherConsole.transform != this.transform)
                Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        
        PlayerInput.Enable();
        PlayerInput.PlayerControls.DebugConsole.started +=Down;
        // Subscribe to the log message received event.
        Application.logMessageReceived += OnGetLogMessage;
    }

    private void OnDisable()
    {
        PlayerInput.Disable();
        PlayerInput.PlayerControls.DebugConsole.started -=Down;
        // Unsubscribe to the log message received event.
        Application.logMessageReceived -= OnGetLogMessage;
    }

    private void OnGetLogMessage(string condition, string stackTrace, LogType type)
    {
        // If Show Timestamps is checked, add a timestamp with the current hour, minute and second.
        if (m_ShowTimestamps)
        {
            string originalCondition = condition;
            condition = "[" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second + "] " + originalCondition;
        }

        // Trim the condition and stack trace.
        condition = condition.Trim();
        stackTrace = stackTrace.Trim();

        // Add the message.
        m_Messages.Add(new DebugMessage(condition, stackTrace, type));
    }

    private void Update()
    {
        // Show when toggle key is pressed and console is enabled.
        if (m_ToggleKey && m_EnableConsole)
        {
            m_Show = !m_Show;
            m_ToggleKey=false;
        }
    }

    void OnGUI()
    {
        // Only run if show and enable console is enabled.
        if (m_Show && m_EnableConsole)
        {
            // If there's a skin, apply it.
            if (m_GUISkin != null)
                GUI.skin = m_GUISkin;

            // Begin an area.
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, 300), GUI.skin.box);
            // Begin the scroll area.
            m_Scroll = GUILayout.BeginScrollView(m_Scroll);

            // Loop through every message.
            for (int i = 0; i < m_Messages.Count; i++)
            {
                // Create a new label style and base it on the original label.
                GUIStyle labelStyle = GUI.skin.label;
                // Create a entry color field.
                Color entryColor = Color.magenta;
                // Get the entry color based on the message type.
                switch (m_Messages[i].Type)
                {
                    case LogType.Error:
                        entryColor = m_ErrorColor;
                        break;
                    case LogType.Assert:
                        entryColor = m_AssertColor;
                        break;
                    case LogType.Warning:
                        entryColor = m_WarningColor;
                        break;
                    case LogType.Log:
                        entryColor = m_LogColor;
                        break;
                    case LogType.Exception:
                        entryColor = m_ExceptionColor;
                        break;
                }
                // Set the text color to the entry color.
                labelStyle.normal.textColor = entryColor;

                // Create a button that looks like a label.
                if (GUILayout.Button(m_Messages[i].Condition, labelStyle))
                {
                    // When clicked, if the selected entry is the same as this message, close the entry.
                    // Else set the selected entry to thie message entry.
                    if (m_ExpandedEntry == i)
                        m_ExpandedEntry = -1;
                    else
                        m_ExpandedEntry = i;
                }

                // If the expanded entry is this message, show the stack trace.
                if (m_ExpandedEntry == i)
                {
                    // Create a new box style based on the GUI skin box.
                    GUIStyle boxStyle = GUI.skin.box;
                    // Set the text alignment.
                    boxStyle.alignment = TextAnchor.UpperLeft;
                    // Set the text color to the same color as the entry color.
                    boxStyle.normal.textColor = entryColor;
                    // Show it as a box.
                    GUILayout.Box(m_Messages[i].StackTrace, boxStyle);
                }
            }

            // End the scroll view.
            GUILayout.EndScrollView();

            // If Can Export Logs is checked, show the export button.
            if (m_CanExportLogs)
            {
                // When click, do the export logs function.
                if (GUILayout.Button("Export Logs"))
                {
                    ExportLogs();
                }
            }

            // And lastly end the area.
            GUILayout.EndArea();
        }
    }

    /// <summary>
    /// Exports all logs to Application.dataPath/logs
    /// </summary>
    public void ExportLogs()
    {
        // If Can Export Logs is unchecked, stop here.
        if (!m_CanExportLogs)
            return;

        // Create a new string builder.
        StringBuilder sb = new StringBuilder();
        // Loop through all the messages.
        for (int i = 0; i < m_Messages.Count; i++)
        {
            // Add the message type to the line.
            switch (m_Messages[i].Type)
            {
                case LogType.Error:
                    sb.Append("[ERROR] ");
                    break;
                case LogType.Assert:
                    sb.Append("[ASSERT] ");
                    break;
                case LogType.Warning:
                    sb.Append("[WARNING] ");
                    break;
                case LogType.Log:
                    sb.Append("[LOG] ");
                    break;
                case LogType.Exception:
                    sb.Append("[EXCEPTION] ");
                    break;
                default:
                    sb.Append("[UNKNOWN TYPE] ");
                    break;
            }

            // Append the condition.
            sb.Append(m_Messages[i].Condition);
            // Start a new line.
            sb.AppendLine();
            // Add the stack trace.
            sb.AppendLine(m_Messages[i].StackTrace);
            // Add a new empty line.
            sb.AppendLine();
        }

        // Make sure the output directory exists.
        if (!Directory.Exists(Application.dataPath + "/output_logs/"))
            Directory.CreateDirectory(Application.dataPath + "/output_logs/");

        // Create the file name and then the file.
        System.DateTime now = System.DateTime.Now;
        string fileName = string.Format("{0}-{1}-{2} {3}-{4}-{5}.txt", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        File.WriteAllText(Application.dataPath + "/output_logs/" + fileName, sb.ToString());
    }
}
