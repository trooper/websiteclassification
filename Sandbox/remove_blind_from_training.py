from os import listdir
from os.path import isfile, join
import os
import math
import shutil
import random

folders_in_blind = [x[0] for x in os.walk('.\\BlindSet\\')]
print(folders_in_blind)
for folder_in_blind in folders_in_blind:
	if folder_in_blind == '.' or folder_in_blind == '.\\BlindSet\\':
		continue
	print("-------------")
	print("Processing folder {}".format(folder_in_blind))
	count = 0
	files_in_blind = [f for f in listdir(folder_in_blind) if isfile(join(folder_in_blind, f))]
	for f in files_in_blind:
		training_file = '.\\DataSets\\' + folder_in_blind[folder_in_blind.rfind('\\') + 1:] + '\\' + f
		if os.path.isfile(training_file): 
			os.remove(training_file)
			#print("Will remove", training_file)
			count += 1
		else:
			print("For file '{}' in BlindSet there's no file in DataSet".format(training_file))
	print("Deleted {}".format(count))
