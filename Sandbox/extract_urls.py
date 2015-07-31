from pathlib import Path
import mmap
import csv

def urls_from_tsv(path):
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

p = Path('../Data/DataSets')
relevant = ['Accommodation', 'Restaurant', 'Retail', 'Other']
for cat in relevant:
    folder = p / cat
    with open("../Data/Other/Links/" + cat + ".txt", "w")as f:
        for fname in folder.iterdir():
            for url in iter(urls_from_tsv(str(fname))):
                print(url, file=f)
