from pathlib import Path
import mmap
import csv
import re

def reduce(original_file, cleaned_file, expr):
    prog = re.compile(expr)
    print("walking file", path)
    with open(path, "r", encoding='latin1') as tsv:
        mm = mmap.mmap(tsv.fileno(), 0, access=mmap.ACCESS_READ)
        pos = 0
        tabpos = mm.find(b'\t')
        while tabpos != -1:
            yield mm[pos:tabpos].decode('ascii')
            pos = mm.find(b'\n', tabpos)
            if pos == -1:
                break
            else:
                pos += 1
                tabpos = mm.find(b'\t', pos)
        mm.close()

data_path = Path('../Data')
original_data = data_path / "DataSets"
cleaned_data = data_path / "CleanDataSets"

website_category = 'Retail'
website_name = 'claires'
expression = r'''claires.com/us/products/'''

original_file = original_data / website_category / website_name
cleaned_file = original_data / website_category / website_name

reduce(original_file, cleaned_file, expression)
