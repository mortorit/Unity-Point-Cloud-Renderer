# Unity-Point-Cloud-Renderer

This repository allows to:

1. **Select and load a `.npy` point cloud** (optionally by index or at random) using Python.  
2. **Export** that point cloud to a CSV file.  
3. **Import** the CSV into Unity to generate colored spheres representing each point in a 3D scene.

---

## Features

- **Random or Indexed Sample Selection**  
  The Python script can either pick a random `.npy` file from a specified folder or use a provided index.  
- **Customizable Paths**  
  Pass in the paths for your samples directory, CSV output path, and (optionally) the index as command-line arguments.  
- **Unity Integration**  
  A Unity C# script (`PointCloudLoader.cs`) calls the Python script, then reads the CSV to generate sphere GameObjects.  
- **Colored Points**  
  Each point in the CSV includes (x, y, z, r, g, b), and the Unity script applies those colors to individual spheres.

---

## Requirements

1. **Python 3.x**  
   - Ensure Python is installed on the same machine where Unity runs (desktop platforms only).  
   - Python **must** be accessible via command line (on your PATH) or by specifying the full path in `PointCloudLoader.cs`.

2. **NumPy**  
   - Used in Python to load the `.npy` file.  
   - Install with `pip install numpy` if not already installed.

3. **Unity** (2020+ recommended)  
   - For Windows, macOS, or Linux standalone platforms (it **won’t** work on WebGL, iOS, Android, or consoles because they cannot spawn external processes).

---

