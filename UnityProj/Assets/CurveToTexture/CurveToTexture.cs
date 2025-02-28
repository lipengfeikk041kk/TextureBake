﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CurveToTexture : EditorWindow
{
    [MenuItem("MyaTools/CurveToTexture Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CurveToTexture window = (CurveToTexture)EditorWindow.GetWindow(typeof(CurveToTexture));
        window.Show();
    }
    public bool square = true;
    public SIZE _sizeH = SIZE.x128;
    public SIZE _sizeV = SIZE.x128;
    public enum SIZE
    {
        x1 = 1,
        x64 = 64,
        x128 = 128,
        x256 = 256,
        x512 = 512,
    }

    AnimationCurve ac = AnimationCurve.Linear(0, 0, 1, 1);

    Texture2D cureveTex;
    public bool AutoRefresh = false;

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        square = EditorGUILayout.Toggle("Square Size?", square);
        if (square)
        {
            _sizeH = (SIZE)EditorGUILayout.EnumPopup("Size:", _sizeH);
            _sizeV = _sizeH;
        }
        else
        {
            _sizeH = (SIZE)EditorGUILayout.EnumPopup("Horizontal Size:", _sizeH);
            _sizeV = (SIZE)EditorGUILayout.EnumPopup("Vertical Size:", _sizeV);
        }
        ac = EditorGUILayout.CurveField(ac);
        EditorGUILayout.BeginHorizontal();
        {
            AutoRefresh = GUILayout.Toggle(AutoRefresh, "AutoRefresh");

            if (GUILayout.Button("Refresh") && !AutoRefresh)
            {
                RefreshCureveTex();
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (AutoRefresh)
            {
                RefreshCureveTex();
            }
        }

        EditorGUILayout.EndHorizontal();

        DrawPreview();
        if (GUILayout.Button("Save"))
        {
            SaveCureveTex();
        }
    }
    void DrawPreview()
    {
        GUILayout.Label("preview:", EditorStyles.boldLabel);
        if (cureveTex != null)
        {
            EditorGUILayout.BeginHorizontal("PreBackground");
            GUILayout.FlexibleSpace();
            Rect rect = GUILayoutUtility.GetRect(50, 150, 50, 150);
            EditorGUI.DrawPreviewTexture(rect, cureveTex);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.ObjectField(cureveTex, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        }

    }
    void SaveCureveTex()
    {
        if (cureveTex != null)
        {
            byte[] dataBytes = cureveTex.EncodeToPNG();
            //string savePath = Application.dataPath + "/SampleCircle.png";

            string folderPath = PlayerPrefs.GetString("EPME_LastParticleCheckPath");
            string savePath = EditorUtility.SaveFilePanelInProject("Save png", folderPath + "cureveTex", "png",
        "Please enter a file name to save the texture to");

            if (savePath != "")
            {
                FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);
                fileStream.Write(dataBytes, 0, dataBytes.Length);
                fileStream.Close();
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
        }
    }
    void RefreshCureveTex()
    {
        int h = (int)_sizeH;
        int v = (int)_sizeV;
        cureveTex = new Texture2D(h, v);

        for (int x = 0; x < h; x++)
        {
            for (int y = 0; y < v; y++)
            {
                float curveValue = ac.Evaluate((float)x / (float)h);
                cureveTex.SetPixel(x, y, new Color(curveValue, curveValue, curveValue));
            }
        }
        cureveTex.Apply();
    }
}
