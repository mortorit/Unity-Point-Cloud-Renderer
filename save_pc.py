import numpy as np
import csv
import os
import random
import sys

# ----------------------------------------------------------------------
# Main function: selects a file (random or by index), loads it,
# writes to CSV, and returns the index used.
# ----------------------------------------------------------------------
def select_and_save(samples_dir, output_path, index=None):
    """
    If 'index' is given, use the file at that index (clamped within 0..len(files)-1).
    If 'index' is None, pick a random file from 'samples/'.
    Writes out 'point_cloud.csv' and returns the index used.
    """
    # 1. List all npy files in the 'samples' folder
    files = os.listdir(samples_dir)
    files = [f for f in files if f.endswith(".npy")]
    if not files:
        raise ValueError("No .npy files found in 'samples' folder.")

    # 2. Decide which file to use
    if index is not None:
        # clamp index in case it's out of range
        index = max(0, min(index, len(files)-1))
    else:
        index = random.randint(0, len(files)-1)
    
    chosen_file = files[index]
    print(f"Using file '{chosen_file}' at index {index}")

    # 3. Load the data
    point_cloud = np.load(os.path.join(samples_dir, chosen_file), allow_pickle=True).item()

    # 4. Write to CSV
    xyz = point_cloud['xyz']  # shape: (N, 3)
    rgb = point_cloud['rgb']  # shape: (N, 3) or (N, 4)

    with open(output_path, "w", newline="") as f:
        writer = csv.writer(f)
        writer.writerow(["x", "y", "z", "r", "g", "b"])
        for (x, y, z), (r, g, b) in zip(xyz, rgb):
            writer.writerow([x, y, z, r, g, b])

    # 5. Return the index used for possible logging / debugging
    return index

# ----------------------------------------------------------------------
# If called from the command line, parse sys.argv for an optional index.
# ----------------------------------------------------------------------
def main():
    """
    Command line usage:
      python my_script.py <samples_dir> <output_path> [index]

    If 'index' is omitted, we pick a random file.
    """
    # Check we have at least 3 arguments (script name counts as 1)
    # i.e. we need 2 actual parameters: samples_dir and output_path
    if len(sys.argv) < 3:
        print("Usage: python my_script.py <samples_dir> <output_path> [index]")
        sys.exit(1)

    # Parse the mandatory arguments
    samples_dir = sys.argv[1]
    output_path = sys.argv[2]

    # Parse the optional index
    user_index = None
    if len(sys.argv) > 3:
        try:
            user_index = int(sys.argv[3])
        except ValueError:
            user_index = None  # If it's not a valid int, default to random

    # Call your function with these arguments
    idx_used = select_and_save(samples_dir, output_path, user_index)
    
    # Print the final index so it can be captured by the caller (like Unity)
    print(idx_used)

if __name__ == "__main__":
    main()