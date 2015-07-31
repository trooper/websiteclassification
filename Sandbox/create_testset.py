from pathlib import Path
import mmap
import csv
import random
import shutil
import os

data_path = Path('../Data')
original_data = data_path / "DataSets"
training_data = data_path / "TrainingSet"

if not training_data.exists():
    os.mkdir(str(training_data))

relevant = ['Accommodation', 'Restaurant', 'Retail', 'Other']
for cat in relevant:
    folder = original_data / cat
    dest_folder = training_data / cat
    if not dest_folder.exists():
        os.mkdir(str(dest_folder))
    for fname in folder.iterdir():
        if random.random() < 0.2:
            fname = Path(fname).name
            print("Moving", fname)
            shutil.move(str(folder / fname), str(dest_folder / fname))
        
