using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;  // For Process, ProcessStartInfo
using Debug = UnityEngine.Debug; // Avoid ambiguity with System.Diagnostics.Debug

public class PointCloudLoader : MonoBehaviour {
    [Header("Python Settings")]
    [Tooltip("Path to your Python interpreter (or 'python' if on PATH).")]
    public string pythonExePath = "python";

    [Tooltip("Path to your Python script that defines 'select_and_save(samples_dir, output_path, index=None)'.")]
    public string pythonScriptPath = "C:/path/to/my_script.py";
    
    [Header("Data Settings")]
    [Tooltip("Folder with .npy samples (passed as 'samples_dir' to Python).")]
    public string samplesDir = "C:/path/to/samples";

    [Tooltip("CSV output path (passed as 'output_path' to Python).")]
    public string outputCsvPath = "C:/path/to/point_cloud.csv";

    [Tooltip("If true, we pass no index => random sample. If false, we pass sampleIndex.")]
    public bool randomSelection = true;

    [Tooltip("If randomSelection = false, pass this index to Python.")]
    public int sampleIndex = 0;

    [Header("Final CSV Import Settings in Unity")]
    [Tooltip("When we read the CSV in Unity, we look for this filename (or path).")]
    public string csvFileName = "point_cloud.csv";   // e.g. in Assets/Data/point_cloud.csv

    [Header("Sphere Settings")]
    public float sphereRadius = 0.04f;
    public Material pointMaterial;

    // We'll store positions & colors here
    private List<Vector3> positions = new List<Vector3>();
    private List<Color> colors = new List<Color>();

    void Start() {
        // 1) Call Python script first (blocking call)
        CallPythonScriptWithArgs();

        // 2) Then load the CSV that Python just generated
        LoadCSV();

        // 3) Generate the sphere point cloud
        GeneratePointCloud();
    }

    /// <summary>
    /// Calls the Python script, passing:
    ///   1) samples_dir
    ///   2) output_path
    ///   3) optional index (if randomSelection = false)
    /// </summary>
    private void CallPythonScriptWithArgs() {
        // Build the command-line arguments
        // We'll always pass the 'samplesDir' and 'outputCsvPath'.
        // If randomSelection = false, also pass the index.

        // Wrap them in quotes to handle spaces in file paths
        string args = $"\"{pythonScriptPath}\" \"{samplesDir}\" \"{outputCsvPath}\"";

        if (!randomSelection) {
            // Pass sampleIndex as a 3rd argument
            args += $" {sampleIndex}";
        }

        Debug.Log("Launching Python with args: " + args);

        ProcessStartInfo startInfo = new ProcessStartInfo {
            FileName = pythonExePath,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try {
            using (Process process = Process.Start(startInfo)) {
                // Read output (blocking)
                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(stderr)) {
                    Debug.LogError("Python error:\n" + stderr);
                }
                else {
                    Debug.Log("Python output:\n" + stdout);
                }
            }
        }
        catch (System.Exception e) {
            Debug.LogError("Failed to run Python script. " + e);
        }
    }

    /// <summary>
    /// Loads the CSV from disk into 'positions' and 'colors'.
    /// Note: This example assumes you put the CSV in 'Assets/Data/point_cloud.csv',
    /// or some known location. Adjust to your needs.
    /// </summary>
    void LoadCSV() {
        // If your CSV is always at outputCsvPath, you could load from that directly.
        // But if you're copying or referencing it inside Assets/Data, use that path here.

        string filePath = Path.Combine(Application.dataPath, "Data", csvFileName);

        if (!File.Exists(filePath)) {
            Debug.LogError("CSV file not found at: " + filePath);
            return;
        }

        using (StreamReader sr = new StreamReader(filePath)) {
            bool isHeader = true;
            while (!sr.EndOfStream) {
                string line = sr.ReadLine();

                // Skip the header line if it exists
                if (isHeader) {
                    isHeader = false;
                    continue;
                }

                var values = line.Split(',');
                if (values.Length < 6) continue;

                // Use invariant culture for decimal points
                float x = float.Parse(values[0], CultureInfo.InvariantCulture);
                float y = float.Parse(values[1], CultureInfo.InvariantCulture);
                float z = float.Parse(values[2], CultureInfo.InvariantCulture);
                float r = float.Parse(values[3], CultureInfo.InvariantCulture);
                float g = float.Parse(values[4], CultureInfo.InvariantCulture);
                float b = float.Parse(values[5], CultureInfo.InvariantCulture);

                positions.Add(new Vector3(x, y, z));
                colors.Add(new Color(r, g, b));
            }
        }
    }

    /// <summary>
    /// Instantiates one sphere per point using 'positions' and 'colors'.
    /// </summary>
    void GeneratePointCloud() {
        for (int i = 0; i < positions.Count; i++) {
            Vector3 pos = positions[i];
            Color col = colors[i];

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = pos;
            sphere.transform.localScale = Vector3.one * sphereRadius;

            var renderer = sphere.GetComponent<Renderer>();
            if (pointMaterial != null) {
                // Create a new material instance so each sphere can have its own color
                renderer.material = new Material(pointMaterial);
            }
            else {
                // Fallback to Standard shader if no material assigned
                renderer.material = new Material(Shader.Find("Standard"));
            }

            renderer.material.color = col;
        }
    }
}
